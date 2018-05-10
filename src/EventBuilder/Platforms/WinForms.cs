// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;

namespace EventBuilder.Platforms
{
    public class WinForms : BasePlatform
    {
        public WinForms()
        {
            if (PlatformHelper.IsRunningOnMono()) {
                throw new NotSupportedException("Building events for WPF on Mac is not implemented yet.");
            }

            Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\WindowsBase.dll");
            Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Windows.Forms.dll");

            CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5");

        }
    }
}
