using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Serilog;

namespace EventBuilder.Cecil
{
    public class PathSearchAssemblyResolver : IAssemblyResolver
    {
        private readonly string[] _targetAssemblyDirs;

        public PathSearchAssemblyResolver(string[] targetAssemblyDirs)
        {
            _targetAssemblyDirs = targetAssemblyDirs;
        }

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {
            var dllName = fullName.Split(',')[0] + ".dll";

            var fullPath = _targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(x => File.Exists(x));
            if (fullPath == null) {
                dllName = fullName.Split(',')[0] + ".winmd";
                fullPath = _targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(x => File.Exists(x));
            }

            // NB: This hacks WinRT's weird mscorlib to just use the regular one
            // We forget why this was needed, maybe it's not needed anymore?
            if (fullName.Contains("mscorlib") && fullName.Contains("255")) {
                fullPath =
                    Environment.ExpandEnvironmentVariables(
                        @"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll");
            }

            if (fullPath == null) {
                var errorMessage = $"Failed to resolve!!! {fullName}";
                Log.Error(errorMessage);
                throw new Exception(errorMessage);
            }

            return AssemblyDefinition.ReadAssembly(fullPath, parameters);
        }

        public AssemblyDefinition Resolve(string fullName)
        {
            var dllName = fullName.Split(',')[0] + ".dll";

            var fullPath = _targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(x => File.Exists(x));
            if (fullPath == null) {
                dllName = fullName.Split(',')[0] + ".winmd";
                fullPath = _targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(x => File.Exists(x));
            }

            // NB: This hacks WinRT's weird mscorlib to just use the regular one
            if (fullName.Contains("mscorlib") && fullName.Contains("255")) {
                fullPath =
                    Environment.ExpandEnvironmentVariables(
                        @"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll");
            }

            if (fullPath == null) {
                var errorMessage = $"Failed to resolve!!! {fullName}";
                Log.Error(errorMessage);
                throw new Exception(errorMessage);
            }

            return AssemblyDefinition.ReadAssembly(fullPath);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return Resolve(name.FullName, parameters);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return Resolve(name.FullName);
        }
    }
}