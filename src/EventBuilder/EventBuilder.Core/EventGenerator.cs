// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventBuilder.Core.Entities;
using EventBuilder.Core.PlatformExtractors;
using EventBuilder.Core.Reflection;
using EventBuilder.Core.Reflection.Resolvers;
using Serilog;
using Stubble.Core.Builders;

namespace EventBuilder.Core
{
    /// <summary>
    /// Processes the specified platform and saves out a specified template.
    /// </summary>
    public static class EventGenerator
    {
        private static readonly INamespaceResolver[] _resolvers = new INamespaceResolver[]
        {
            new PublicEventNamespaceResolver(),
            new PublicStaticEventNamespaceResolver(),
            new DelegateTemplateNamespaceResolver()
        };

        private static readonly IDictionary<AutoPlatform, IPlatformExtractor> _platformExtractors = new IPlatformExtractor[]
        {
            new Android(),
            new Essentials(),
            new iOS(),
            new Mac(),
            new Tizen(),
            new TVOS(),
            new UWP(),
            new Winforms(),
            new WPF(),
            new XamForms(),
        }.ToImmutableDictionary(x => x.Platform);

                /// <summary>
        /// Extracts the events and delegates from the specified platform.
        /// </summary>
        /// <param name="outputPath">The path where to output the files.</param>
        /// <param name="defaultReferenceAssemblyLocation">A directory path to where reference assemblies can be located.</param>
        /// <param name="platform">The platforms to generate for.</param>
        /// <returns>A task to monitor the progress.</returns>
        public static Task ExtractEventsFromAssemblies(string outputPath, string defaultReferenceAssemblyLocation, AutoPlatform platform)
        {
            return ExtractEventsFromAssemblies(outputPath, defaultReferenceAssemblyLocation, new AutoPlatform[] { platform });
        }

        /// <summary>
        /// Extracts the events and delegates from the specified platform.
        /// </summary>
        /// <param name="outputPath">The path where to output the files.</param>
        /// <param name="defaultReferenceAssemblyLocation">A directory path to where reference assemblies can be located.</param>
        /// <param name="platforms">The platforms to generate for.</param>
        /// <returns>A task to monitor the progress.</returns>
        public static async Task ExtractEventsFromAssemblies(string outputPath, string defaultReferenceAssemblyLocation, IEnumerable<AutoPlatform> platforms)
        {
            var assemblies = new HashSet<string>();
            var searchDirectories = new HashSet<string>();

            foreach (var platform in platforms)
            {
                var platformExtractor = _platformExtractors[platform];
                await platformExtractor.Extract(defaultReferenceAssemblyLocation).ConfigureAwait(false);

                assemblies.UnionWith(platformExtractor.Assemblies);
                searchDirectories.UnionWith(platformExtractor.SearchDirectories);
            }

            await ExtractEventsFromAssemblies(outputPath, assemblies, searchDirectories).ConfigureAwait(false);
        }

        /// <summary>
        /// Extracts the events and delegates from the specified platform.
        /// </summary>
        /// <param name="outputPath">The path where to output the files.</param>
        /// <param name="assemblyPaths">The paths to the assemblies to extract.</param>
        /// <param name="searchDirectories">Paths to any directories to search for supporting libraries.</param>
        /// <returns>A task to monitor the progress.</returns>
        public static async Task ExtractEventsFromAssemblies(string outputPath, IEnumerable<string> assemblyPaths, IEnumerable<string> searchDirectories)
        {
            var compilation = ReflectionHelpers.GetCompilation(assemblyPaths, searchDirectories);

            var namespaceData = _resolvers.Select(x => new { Namespaces = x.Create(compilation).ToList(), Template = x.TemplatePath }).ToList();

            var results = await Task.WhenAll(namespaceData.Select(x => GenerateAsync(x.Namespaces, x.Template))).ConfigureAwait(false);

            using (StreamWriter streamWriter = new StreamWriter(outputPath))
            {
                await streamWriter.WriteAsync(await TemplateManager.GetTemplateAsync(TemplateManager.HeaderTemplate).ConfigureAwait(false)).ConfigureAwait(false);
                await streamWriter.WriteAsync(Environment.NewLine).ConfigureAwait(false);

                foreach (var result in results)
                {
                    await streamWriter.WriteAsync(result).ConfigureAwait(false);
                    await streamWriter.WriteAsync(Environment.NewLine).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Generates the template from the namespace data.
        /// </summary>
        /// <param name="namespaceData">The namespace data to extract the type from.</param>
        /// <param name="inputTemplatePath">The path to the template to use.</param>
        /// <returns>The formatted value.</returns>
        private static async Task<string> GenerateAsync(IReadOnlyCollection<NamespaceInfo> namespaceData, string inputTemplatePath)
        {
            Log.Debug("Using {inputTemplatePath} as the mustache template", inputTemplatePath);

            var template = await TemplateManager.GetTemplateAsync(inputTemplatePath).ConfigureAwait(false);
            var stubble = new StubbleBuilder().Build();
            return await stubble.RenderAsync(template, new { Namespaces = namespaceData }).ConfigureAwait(false);
        }
    }
}
