using System.Threading.Tasks;
using ICSharpCode.Decompiler.Metadata;

namespace FieldReferenceFinder.PluginCodeScanning
{
    // Deliberately no disk/GAC probing, network access, Assembly.Load or temporary files.
    // Unresolved dependencies are tolerated by the decompiler and reduce analysis fidelity.
    internal sealed class MemoryOnlyAssemblyResolver : IAssemblyResolver
    {
        public PEFile Resolve(IAssemblyReference reference) => null;
        public PEFile ResolveModule(PEFile mainModule, string moduleName) => null;
        public Task<PEFile> ResolveAsync(IAssemblyReference reference) =>
            Task.FromResult<PEFile>(null);
        public Task<PEFile> ResolveModuleAsync(PEFile mainModule, string moduleName) =>
            Task.FromResult<PEFile>(null);
    }
}
