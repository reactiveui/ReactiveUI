// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using NuGet;
using Polly;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    public class Caboodle : BasePlatform
    {
        private const string _packageName = "Microsoft.Caboodle";

        public Caboodle(string referenceAssembliesLocation)
        {
            if (PlatformHelper.IsRunningOnMono()) {
                throw new NotImplementedException("Mono isn't implemented");
            }

            Assemblies.Add("Microsoft.Caboodle.dll");

            //var assemblies =
            //   Directory.GetFiles(Path.Combine(referenceAssembliesLocation, "MonoAndroid"),
            //       "Mono.Android.dll", SearchOption.AllDirectories);

            //// Pin to a particular framework version https://github.com/reactiveui/ReactiveUI/issues/1517
            //var latestVersion = assemblies.Last(x => x.Contains("v8"));
            //Assemblies.Add(latestVersion);

            //CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));

            CecilSearchDirectories.Add(Path.Combine(referenceAssembliesLocation, "MonoAndroid", "v1.0"));

            //CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETPortable\v4.5\Profile\Profile111");
        }
    }
}
