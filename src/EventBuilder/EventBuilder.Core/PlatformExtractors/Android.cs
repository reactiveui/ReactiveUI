// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventBuilder.Core.PlatformExtractors
{
    /// <inheritdoc />
    /// <summary>
    /// The Android platform.
    /// </summary>
    /// <seealso cref="BasePlatform" />
    public class Android : BasePlatform
    {
        private const string DesiredVersion = "v8.1";

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.Android;

        /// <inheritdoc />
        public override Task Extract(string referenceAssembliesLocation)
        {
            var sdks = new List<string>();
            SearchDirectories.Add(Path.Combine(referenceAssembliesLocation, "MonoAndroid", "v1.0"));
            sdks.AddRange(Directory.GetFiles(
                   Path.Combine(referenceAssembliesLocation, "MonoAndroid"),
                   "Mono.Android.dll",
                   SearchOption.AllDirectories));

            // Pin to a particular framework version https://github.com/reactiveui/ReactiveUI/issues/1517
            var latestVersion = sdks.Last(x => x.Contains(DesiredVersion));
            Assemblies.Add(latestVersion);
            SearchDirectories.Add(Path.GetDirectoryName(latestVersion));

            return Task.CompletedTask;
        }
    }
}
