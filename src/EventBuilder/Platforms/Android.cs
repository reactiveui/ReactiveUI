// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    /// <inheritdoc />
    /// <summary>
    /// The Android platform.
    /// </summary>
    /// <seealso cref="T:EventBuilder.Platforms.BasePlatform" />
    public class Android : BasePlatform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EventBuilder.Platforms.Android" /> class.
        /// </summary>
        /// <param name="referenceAssembliesLocation">The reference assemblies location.</param>
        public Android(string referenceAssembliesLocation)
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
                       Path.Combine(referenceAssembliesLocation, "MonoAndroid"),
                       "Mono.Android.dll",
                       SearchOption.AllDirectories);

                // Pin to a particular framework version https://github.com/reactiveui/ReactiveUI/issues/1517
                var latestVersion = assemblies.Last(x => x.Contains("v8"));
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
                CecilSearchDirectories.Add(Path.Combine(referenceAssembliesLocation, "MonoAndroid", "v1.0"));
            }
        }

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.Android;
    }
}
