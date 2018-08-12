// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace EventBuilder.Platforms
{
    public class WPF : BasePlatform
    {
        public override AutoPlatform Platform => AutoPlatform.WPF;
            
        public WPF()
        {
            if (PlatformHelper.IsRunningOnMono()) {
                throw new NotSupportedException("Building events for WPF on Mac is not implemented.");
            } else {
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\WindowsBase.dll");
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\PresentationCore.dll");
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\PresentationFramework.dll");

                CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1");
            }
        }
    }
}
