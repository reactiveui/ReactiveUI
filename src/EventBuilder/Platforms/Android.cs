// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventBuilder.Platforms
{
    /// <inheritdoc />
    /// <summary>
    /// The Android platform.
    /// </summary>
    /// <seealso cref="BasePlatform" />
    public class Android : BasePlatform
    {
        private readonly string _referenceAssembliesLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Android" /> class.
        /// </summary>
        /// <param name="referenceAssembliesLocation">The reference assemblies location.</param>
        public Android(string referenceAssembliesLocation)
        {
            _referenceAssembliesLocation = referenceAssembliesLocation;
        }

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.Android;

        /// <inheritdoc />
        public override Task Extract()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                var sdks =
                    Directory.GetFiles(
                        @"/Library/Frameworks/Xamarin.Android.framework/Libraries/xbuild-frameworks/MonoAndroid",
                        "Mono.Android.dll",
                        SearchOption.AllDirectories);

                var latestVersion = sdks.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
                CecilSearchDirectories.Add(
                    "/Library/Frameworks/Xamarin.Android.framework/Libraries/xbuild-frameworks/MonoAndroid/v1.0");
            }
            else
            {
                var assemblies =
                   Directory.GetFiles(
                       Path.Combine(_referenceAssembliesLocation, "MonoAndroid"),
                       "Mono.Android.dll",
                       SearchOption.AllDirectories);

                // Pin to a particular framework version https://github.com/reactiveui/ReactiveUI/issues/1517
                var latestVersion = assemblies.Last(x => x.Contains("v8.1", StringComparison.InvariantCulture));
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
                CecilSearchDirectories.Add(Path.Combine(_referenceAssembliesLocation, "MonoAndroid", "v1.0"));
            }

            return Task.CompletedTask;
        }
    }
}
