﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
        private const string DesiredVersion = "v8.1";

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
            var sdks = new List<string>();
            if (PlatformHelper.IsRunningOnMono())
            {
                CecilSearchDirectories.Add(
                    "/Library/Frameworks/Xamarin.Android.framework/Libraries/xbuild-frameworks/MonoAndroid/v1.0");

                sdks.AddRange(Directory.GetFiles(
                        "/Library/Frameworks/Xamarin.Android.framework/Libraries/xbuild-frameworks/MonoAndroid",
                        "Mono.Android.dll",
                        SearchOption.AllDirectories));
            }
            else
            {
                CecilSearchDirectories.Add(Path.Combine(_referenceAssembliesLocation, "MonoAndroid", "v1.0"));
                sdks.AddRange(Directory.GetFiles(
                       Path.Combine(_referenceAssembliesLocation, "MonoAndroid"),
                       "Mono.Android.dll",
                       SearchOption.AllDirectories));
            }

            // Pin to a particular framework version https://github.com/reactiveui/ReactiveUI/issues/1517
            var latestVersion = sdks.Last(x => x.Contains(DesiredVersion, StringComparison.InvariantCulture));
            Assemblies.Add(latestVersion);
            CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));

            return Task.CompletedTask;
        }
    }
}
