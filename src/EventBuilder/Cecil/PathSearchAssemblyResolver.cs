// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Serilog;

namespace EventBuilder.Cecil
{
    /// <summary>
    /// Assembly resolver.
    /// </summary>
    /// <seealso cref="Mono.Cecil.IAssemblyResolver" />
    public sealed class PathSearchAssemblyResolver : IAssemblyResolver
    {
        private readonly string[] _targetAssemblyDirs;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathSearchAssemblyResolver"/> class.
        /// </summary>
        /// <param name="targetAssemblyDirs">The target assembly dirs.</param>
        public PathSearchAssemblyResolver(string[] targetAssemblyDirs)
        {
            _targetAssemblyDirs = targetAssemblyDirs;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <summary>
        /// Resolves the specified full assembly name.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The assembly definition.</returns>
        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {
            var dllName = fullName.Split(',')[0] + ".dll";

            var fullPath = _targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(File.Exists);
            if (fullPath == null)
            {
                dllName = fullName.Split(',')[0] + ".winmd";
                fullPath = _targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(File.Exists);
            }

            // NB: This hacks WinRT's weird mscorlib to just use the regular one
            // We forget why this was needed, maybe it's not needed anymore?
            if (fullName.Contains("mscorlib", StringComparison.InvariantCulture) && fullName.Contains("255", StringComparison.InvariantCulture))
            {
                fullPath =
                    Environment.ExpandEnvironmentVariables(
                        @"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll");
            }

            if (fullPath == null)
            {
                var errorMessage = $"Failed to resolve!!! {fullName}";
                Log.Error(errorMessage);
                throw new Exception(errorMessage);
            }

            return AssemblyDefinition.ReadAssembly(fullPath, parameters);
        }

        /// <summary>
        /// Resolves the specified full assembly name.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns>The assembly definition.</returns>
        public AssemblyDefinition Resolve(string fullName)
        {
            var dllName = fullName.Split(',')[0] + ".dll";

            var fullPath = _targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(File.Exists);
            if (fullPath == null)
            {
                dllName = fullName.Split(',')[0] + ".winmd";
                fullPath = _targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(File.Exists);
            }

            // NB: This hacks WinRT's weird mscorlib to just use the regular one
            if (fullName.Contains("mscorlib", StringComparison.InvariantCulture) && fullName.Contains("255", StringComparison.InvariantCulture))
            {
                fullPath = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll");
            }

            if (fullPath == null)
            {
                var errorMessage = $"Failed to resolve!!! {fullName}";
                Log.Error(errorMessage);
                throw new Exception(errorMessage);
            }

            return AssemblyDefinition.ReadAssembly(fullPath);
        }

        /// <summary>
        /// Resolves the specified full assembly name.
        /// </summary>
        /// <param name="name">The assembly name reference.</param>
        /// <param name="parameters">The reader parameters.</param>
        /// <returns>The assembly definition.</returns>
        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return Resolve(name.FullName, parameters);
        }

        /// <summary>
        /// Resolves the specified full assembly name.
        /// </summary>
        /// <param name="name">The assembly name reference.</param>
        /// <returns>The assembly definition.</returns>
        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return Resolve(name.FullName);
        }
    }
}
