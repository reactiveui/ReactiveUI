// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventBuilder.Core.PlatformExtractors
{
    /// <summary>
    /// TV OS platform assemblies and events.
    /// </summary>
    public class TVOS : BasePlatform
    {
        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.TVOS;

        /// <inheritdoc />
        public override Task Extract(string referenceAssembliesLocation)
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                referenceAssembliesLocation = Path.Combine(referenceAssembliesLocation, "Xamarin.iOS.framework/Versions/Current/lib/mono");
            }

            var assemblies =
                Directory.GetFiles(
                    Path.Combine(referenceAssembliesLocation, "Xamarin.TVOS"),
                    "Xamarin.TVOS.dll",
                    SearchOption.AllDirectories);

            var latestVersion = assemblies.Last();
            Assemblies.Add(latestVersion);

            SearchDirectories.Add(Path.GetDirectoryName(latestVersion));

            return Task.CompletedTask;
        }
    }
}
