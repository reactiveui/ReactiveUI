// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace EventBuilder.Platforms
{
    /// <summary>
    /// UWP platform assemblies and events.
    /// </summary>
    /// <seealso cref="EventBuilder.Platforms.BasePlatform" />
    public class UWP : BasePlatform
    {
        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.UWP;

        /// <inheritdoc />
        public override Task Extract()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                throw new NotSupportedException("Building events for UWP on Mac is not implemented yet.");
            }

            Assemblies.Add(@"C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.16299.0\Windows.winmd");

            return Task.CompletedTask;
        }
    }
}
