using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

internal static class Program
{
    private static readonly string[] PrivateDependencies = {
        "ICSharpCode.Decompiler", "System.Collections.Immutable", "System.Reflection.Metadata",
        "System.Memory", "System.Buffers", "System.Runtime.CompilerServices.Unsafe"
    };

    public static int Main(string[] args)
    {
        if (args.Length != 3) throw new ArgumentException("Expected merged DLL, fixture EXE and host dependency folder.");
        // This process has no compile-time scanner dependencies. Permit only host libraries.
        AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
        {
            var name = new AssemblyName(e.Name).Name;
            if (PrivateDependencies.Contains(name))
                throw new Exception("Merged DLL requested external scanner dependency: " + name);
            var path = Path.Combine(args[2], name + ".dll");
            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        };
        var assembly = Assembly.LoadFile(Path.GetFullPath(args[0]));
        var references = assembly.GetReferencedAssemblies().Select(a => a.Name).ToArray();
        if (references.Any(PrivateDependencies.Contains))
            throw new Exception("Scanner dependencies remain external.");

        var reportType = assembly.GetType("FieldReferenceFinder.PluginCodeScanning.ScanReport", true);
        var analyzerType = assembly.GetType("FieldReferenceFinder.PluginCodeScanning.InMemoryCodeAnalyzer", true);
        var report = Activator.CreateInstance(reportType, true);
        var analyzer = Activator.CreateInstance(analyzerType, true);
        analyzerType.GetMethod("Analyze").Invoke(analyzer, new object[] {
            File.ReadAllBytes(args[1]), Guid.NewGuid(), "Fixture", "ownerid", report, CancellationToken.None
        });
        var matches = (IEnumerable)reportType.GetProperty("Matches").GetValue(report);
        bool direct = false;
        foreach (var match in matches)
        {
            var member = (string)match.GetType().GetProperty("Member").GetValue(match);
            if (member.StartsWith("Fixture.Direct")) direct = true;
        }
        if (!direct) throw new Exception("Merged analyzer did not find expected fixture reference.");
        if (((IEnumerable)reportType.GetProperty("Issues").GetValue(report)).Cast<object>().Any())
            throw new Exception("Merged decompilation reported failures.");
        if (AppDomain.CurrentDomain.GetAssemblies().Any(a => PrivateDependencies.Contains(a.GetName().Name)))
            throw new Exception("A private scanner dependency was loaded externally.");
        Console.WriteLine("PASS: merged analyzer runs with no external scanner DLLs.");
        return 0;
    }
}
