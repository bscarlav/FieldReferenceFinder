# Plug-in code scanning

Enable **Plug-in Code (in memory)** to scan database-stored assemblies in the connected
organization. The option is off by default because decompilation may take time.

The scanner retrieves assembly content through the SDK, decodes it into a byte array,
opens a MemoryStream, and decompiles method bodies using ILSpy. No downloaded assembly,
temporary DLL, decompiled source file, or on-disk scan cache is created. It never loads
retrieved code for execution. Dependency resolution does not probe the filesystem or
network. Only matching snippets and coverage diagnostics survive in the results grid;
an explicit CSV export includes those displayed snippets.

Each assembly is retrieved once per search. Analysis is bounded to 32 MiB per assembly
and 30 seconds of decompilation per assembly. Bytes and decompiler streams are released
after each assembly; there is no persistent cache.

Results are **candidates**, not confirmed table dependencies. Matching searches literal
strings in method bodies, including exact tokens in query text. It does not prove the
entity/table or whether the method executes. Unused property accessors and attribute
declarations are excluded to avoid reporting every generated early-bound field.

Limitations: early-bound property call sites, dynamically constructed field names,
resource/configuration files, external dependencies, and package/file-store assemblies
are not analyzed in this first version. Missing metadata can reduce decompilation fidelity.
Skipped assemblies, timeouts, and method failures are reported as scan issues; an empty
result does not prove that a field is unused. Only assemblies with
Dataverse customizationlevel = 1 are considered, including custom assemblies
registered on other tables. Microsoft/system assemblies are excluded before
their content is retrieved.

## Deployment

Restore packages.config dependencies, then build Release and copy only
bin/Release/Plugins/FieldReferenceFinder.dll into XrmToolBox's Plugins folder.

The build uses ILMerge to internalize the scanner dependencies into that DLL.
XrmToolBox and Dataverse SDK assemblies remain external and supplied by the host.
The DLL in bin/Release is the unmerged intermediate; deploy the one in Plugins.
No scanner dependency subfolder is required. Previously generated loose scanner
libraries are unused by this merged artifact.

The merged application is installed on disk; downloaded Dataverse assemblies and
decompiled text remain in memory. ILMerge runs only at build time, never during a scan.

## Verification

Run dotnet build tests/PluginCodeScanning.Tests.csproj, then execute
tests/bin/Debug/net48/PluginCodeScanning.Tests.exe.
The test harness reads its own compiled fixture bytes; production scanning uses only
SDK-provided bytes. Tests cover matches, false positives, malformed input, cancellation,
paged inventory, skipped assemblies and preservation of partial results.

For deployment verification, build tests/MergedSmoke/MergedSmoke.csproj and run its
net48 executable with three arguments: the merged DLL path, the fixture test EXE path,
and bin/Release (host dependency directory). This separate process verifies that
decompilation succeeds without loading any external scanner dependency assemblies.
