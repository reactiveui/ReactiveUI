// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace EventBuilder.Platforms
{
    public class UWP : BasePlatform
    {
        public override AutoPlatform Platform => AutoPlatform.UWP;

        public UWP()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                throw new NotSupportedException("Building events for UWP on Mac is not implemented yet.");
            }
            Assemblies.Add(@"C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.16299.0\Windows.winmd");
        }
    }
}
