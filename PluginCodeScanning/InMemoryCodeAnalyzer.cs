using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;

namespace FieldReferenceFinder.PluginCodeScanning
{
    internal sealed class InMemoryCodeAnalyzer
    {
        // Scan one assembly at a time. No disk cache, source export, or runtime loading.
        public void Analyze(byte[] bytes, Guid assemblyId, string assemblyName,
            string fieldName, ScanReport report, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException("A field logical name is required.", nameof(fieldName));

            var fieldPattern = new Regex(@"(?<![a-zA-Z0-9_])" + Regex.Escape(fieldName) +
                @"(?![a-zA-Z0-9_])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            using (var stream = new MemoryStream(bytes, false))
            using (var module = new PEFile(assemblyName, stream))
            {
                var settings = new DecompilerSettings { ThrowOnAssemblyResolveErrors = false };
                var decompiler = new CSharpDecompiler(module, new MemoryOnlyAssemblyResolver(), settings)
                {
                    CancellationToken = cancellationToken
                };
                var metadata = module.Metadata;
                int failedMethods = 0;
                foreach (var typeHandle in metadata.TypeDefinitions)
                {
                    var type = metadata.GetTypeDefinition(typeHandle);
                    var typeName = GetTypeName(metadata, typeHandle);
                    foreach (var methodHandle in type.GetMethods())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var method = metadata.GetMethodDefinition(methodHandle);
                        var methodName = metadata.GetString(method.Name);
                        if (method.RelativeVirtualAddress == 0) continue;

                        // Generated entity accessors can mention every field even when unused.
                        // Early-bound call-site analysis is outside this first scanner's scope.
                        if (methodName.StartsWith("get_", StringComparison.Ordinal) ||
                            methodName.StartsWith("set_", StringComparison.Ordinal)) continue;
                        try
                        {
                            var syntax = decompiler.Decompile(methodHandle);
                            var literals = syntax.Descendants.OfType<PrimitiveExpression>()
                                .Where(node => node.Value is string &&
                                    node.Ancestors.OfType<BlockStatement>().Any() &&
                                    fieldPattern.IsMatch((string)node.Value));
                            foreach (var literal in literals)
                            {
                                var statement = literal.Ancestors.OfType<Statement>().FirstOrDefault();
                                var snippet = Regex.Replace((statement ?? (AstNode)literal).ToString(), @"\s+", " ").Trim();
                                if (snippet.Length > 1200) snippet = snippet.Substring(0, 1200) + "...";
                                var member = typeName + "." + methodName +
                                    " [0x" + MetadataTokens.GetToken(methodHandle).ToString("X8") + "]";
                                if (report.Matches.Any(match => match.AssemblyId == assemblyId &&
                                    match.Member == member && match.Snippet == snippet)) continue;
                                report.Matches.Add(new CodeMatch
                                {
                                    AssemblyId = assemblyId,
                                    AssemblyName = assemblyName,
                                    Member = member,
                                    Snippet = snippet
                                });
                            }
                        }
                        catch (OperationCanceledException) { throw; }
                        catch (Exception)
                        {
                            // Preserve matches from other methods, but never imply complete coverage.
                            failedMethods++;
                        }
                    }
                }
                if (failedMethods > 0)
                {
                    report.Issues.Add(new ScanIssue
                    {
                        AssemblyId = assemblyId,
                        AssemblyName = assemblyName,
                        Reason = failedMethods + " method(s) could not be decompiled; results are partial."
                    });
                }
            }
        }

        private static string GetTypeName(MetadataReader metadata, TypeDefinitionHandle handle)
        {
            var type = metadata.GetTypeDefinition(handle);
            var name = metadata.GetString(type.Name);
            var parent = type.GetDeclaringType();
            if (!parent.IsNil) return GetTypeName(metadata, parent) + "+" + name;
            var ns = metadata.GetString(type.Namespace);
            return string.IsNullOrEmpty(ns) ? name : ns + "." + name;
        }
    }
}
