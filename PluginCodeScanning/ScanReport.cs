using System;
using System.Collections.Generic;

namespace FieldReferenceFinder.PluginCodeScanning
{
    internal sealed class CodeMatch
    {
        public Guid AssemblyId { get; set; }
        public string AssemblyName { get; set; }
        public string Member { get; set; }
        public string Snippet { get; set; }
    }

    internal sealed class ScanIssue
    {
        public Guid AssemblyId { get; set; }
        public string AssemblyName { get; set; }
        public string Reason { get; set; }
    }

    internal sealed class ScanReport
    {
        public List<CodeMatch> Matches { get; } = new List<CodeMatch>();
        public List<ScanIssue> Issues { get; } = new List<ScanIssue>();
        public int AssembliesExamined { get; set; }
        public int AssembliesCompleted { get; set; }
    }
}
