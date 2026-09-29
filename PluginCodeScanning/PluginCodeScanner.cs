using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace FieldReferenceFinder.PluginCodeScanning
{
    internal sealed class PluginCodeScanner
    {
        private const int MaxAssemblyBytes = 32 * 1024 * 1024;
        private readonly IOrganizationService service;

        public PluginCodeScanner(IOrganizationService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public ScanReport Scan(string fieldName, string exclusionPatterns, Action<string> progress = null)
        {
            var report = new ScanReport();
            var analyzer = new InMemoryCodeAnalyzer();
            try
            {
                // All assemblies: plug-ins registered on another table can query this field.
                progress?.Invoke("Loading custom plug-in assemblies...");
                foreach (var assembly in GetAssemblies())
                {
                    report.AssembliesExamined++;
                    var name = assembly.GetAttributeValue<string>("name");
                    if (string.IsNullOrWhiteSpace(name)) name = assembly.Id.ToString();
                    progress?.Invoke($"Analyzing assembly {report.AssembliesExamined}: {name}...");
                    if (MatchesExclusion(name, exclusionPatterns))
                    {
                        progress?.Invoke($"Skipped {name}: excluded by settings.");
                        continue;
                    }
                    try
                    {
                        var source = assembly.GetAttributeValue<OptionSetValue>("sourcetype")?.Value;
                        if (source != 0)
                        {
                            AddIssue(report, assembly.Id, name,
                                "Not scanned: assembly is not database-stored (package/file/system source).");
                            progress?.Invoke($"Skipped {name}: assembly content is not database-stored.");
                            continue;
                        }
                        progress?.Invoke($"Retrieving {name}...");
                        var record = service.Retrieve("pluginassembly", assembly.Id, new ColumnSet("content"));
                        var content = record.GetAttributeValue<string>("content");
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            AddIssue(report, assembly.Id, name, "Not scanned: no assembly content was returned.");
                            progress?.Invoke($"Skipped {name}: no assembly content was returned.");
                            continue;
                        }
                        if (content.Length > ((long)MaxAssemblyBytes + 2) / 3 * 4)
                        {
                            AddIssue(report, assembly.Id, name, "Not scanned: assembly exceeds the 32 MiB analysis limit.");
                            progress?.Invoke($"Skipped {name}: assembly exceeds the 32 MiB limit.");
                            continue;
                        }
                        var bytes = Convert.FromBase64String(content);
                        progress?.Invoke($"Decompiling {name} in memory...");
                        using (var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                        {
                            int issuesBefore = report.Issues.Count;
                            analyzer.Analyze(bytes, assembly.Id, name, fieldName, report, timeout.Token);
                            if (report.Issues.Count == issuesBefore) report.AssembliesCompleted++;
                        }
                        progress?.Invoke($"Completed {name}: {report.Matches.Count} candidate match(es) so far.");
                    }
                    catch (OperationCanceledException)
                    {
                        AddIssue(report, assembly.Id, name, "Partial/not scanned: exceeded the 30-second decompilation limit.");
                        progress?.Invoke($"Timed out while analyzing {name}.");
                    }
                    catch (Exception ex)
                    {
                        // Do not retain binary payloads or decompiled source in diagnostic logs.
                        AddIssue(report, assembly.Id, name, "Partial/not scanned: retrieval or analysis failed (" +
                            ex.GetType().Name + ").");
                        progress?.Invoke($"Failed {name}: {ex.GetType().Name}.");
                    }
                }
            }
            catch (Exception ex)
            {
                AddIssue(report, Guid.Empty, "Assembly inventory",
                    "Partial/not scanned: could not enumerate assemblies (" + ex.GetType().Name + ").");
                progress?.Invoke($"Assembly inventory failed: {ex.GetType().Name}.");
            }
            progress?.Invoke($"Plug-in code scan complete: {report.AssembliesCompleted}/{report.AssembliesExamined} assemblies completed.");
            return report;
        }

        public ScanReport Scan(string fieldName, Action<string> progress = null)
        {
            return Scan(fieldName, SettingsDefaults(), progress);
        }

        private static string SettingsDefaults()
        {
            return "ActivityAnalysisPlugins.*\nActivityFeeds.*\nCRM.*\nEmailEngagementPlugins.*\n" +
                "Microsoft.*\nMicrosoft.CCaaS\nMicrosoft.CDS.*\nMicrosoft.Dataverse.*\nMicrosoft.Dynamics.*\nMicrosoft.PowerFx.*\n" +
                "Microsoft.Portal.*\nMicrosoft.PowerPages.*\nMicrosoft.Xrm.*\nMicrosoftPowerAppsModernShellPlatformApiPlugins\n" +
                "MicrosoftPowerAppsCardsPlugins\nMicrosoftPowerAppsAppFrameworkPlatformFeaturePlugins\nScheduleCommon.*";
        }

        private static bool MatchesExclusion(string assemblyName, string patterns)
        {
            if (string.IsNullOrWhiteSpace(patterns)) return false;
            foreach (var rawPattern in patterns.Split(new[] { '\r', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var pattern = rawPattern.Trim();
                if (pattern.Length == 0) continue;
                var prefix = pattern.EndsWith(".*", StringComparison.Ordinal)
                    ? pattern.Substring(0, pattern.Length - 1)
                    : pattern;
                if (pattern.EndsWith("*", StringComparison.Ordinal))
                {
                    if (assemblyName.StartsWith(prefix.TrimEnd('*'), StringComparison.OrdinalIgnoreCase)) return true;
                }
                else if (string.Equals(assemblyName, pattern, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<Entity> GetAssemblies()
        {
            var query = new QueryExpression("pluginassembly")
            {
                ColumnSet = new ColumnSet("name", "sourcetype", "customizationlevel"),
                PageInfo = new PagingInfo { PageNumber = 1, Count = 250 }
            };
            // Dataverse uses customizationlevel = 1 for custom/customized components.
            // Use byte because this Dataverse attribute is represented as a tinyint.
            query.Criteria.AddCondition("customizationlevel", ConditionOperator.Equal, (byte)1);
            query.AddOrder("pluginassemblyid", OrderType.Ascending);
            while (true)
            {
                var page = service.RetrieveMultiple(query);
                foreach (var assembly in page.Entities) yield return assembly;
                if (!page.MoreRecords) yield break;
                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = page.PagingCookie;
            }
        }

        private static void AddIssue(ScanReport report, Guid id, string name, string reason)
        {
            report.Issues.Add(new ScanIssue { AssemblyId = id, AssemblyName = name, Reason = reason });
        }
    }
}
