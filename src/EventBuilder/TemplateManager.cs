// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventBuilder
{
    internal static class TemplateManager
    {
        /// <summary>
        /// Gets the default mustache template file name.
        /// </summary>
        public const string DefaultMustacheTemplate = "EventBuilder.Templates.DefaultTemplate.mustache";

        /// <summary>
        /// Gets the template for static based events.
        /// </summary>
        public const string StaticMustacheTemplate = "EventBuilder.Templates.StaticTemplate.mustache";

        /// <summary>
        /// Gets the template for the header of the file.
        /// </summary>
        public const string HeaderTemplate = "EventBuilder.Templates.HeaderTemplate.txt";

        /// <summary>
        /// Gets the template for the delegate information.
        /// </summary>
        public const string DelegateTemplate = "EventBuilder.Templates.DelegateTemplate.mustache";

        public static async Task<string> GetTemplateAsync(string templateName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var streamReader = new StreamReader(assembly.GetManifestResourceStream(templateName), Encoding.UTF8))
            {
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}
