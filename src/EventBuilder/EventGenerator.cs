// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventBuilder.Entities;
using EventBuilder.Platforms;
using EventBuilder.Reflection;
using EventBuilder.Reflection.Resolvers;
using Serilog;
using Stubble.Core.Builders;

namespace EventBuilder
{
    /// <summary>
    /// Processes the specified platform and saves out a specified template.
    /// </summary>
    internal static class EventGenerator
    {
        private static readonly INamespaceResolver[] _resolvers = new INamespaceResolver[]
{
            new PublicEventNamespaceResolver(),
            new PublicStaticEventNamespaceResolver(),
            new DelegateTemplateNamespaceResolver()
};

        public static async Task ExtractEventsFromAssemblies(string outputPath, IPlatform platform)
        {
            await platform.Extract().ConfigureAwait(false);

            var compilation = ReflectionHelpers.GetCompilation(platform.Assemblies, platform.SearchDirectories);

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
            return (await stubble.RenderAsync(template, new { Namespaces = namespaceData }).ConfigureAwait(false))
                .Replace("System.String", "string", StringComparison.InvariantCulture)
                .Replace("System.Object", "object", StringComparison.InvariantCulture)
                .Replace("&lt;", "<", StringComparison.InvariantCulture)
                .Replace("&gt;", ">", StringComparison.InvariantCulture)
                .Replace("`1", string.Empty, StringComparison.InvariantCulture)
                .Replace("`2", string.Empty, StringComparison.InvariantCulture)
                .Replace("`3", string.Empty, StringComparison.InvariantCulture);
        }
    }
}
