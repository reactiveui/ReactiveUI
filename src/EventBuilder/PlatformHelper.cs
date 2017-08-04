// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;

namespace EventBuilder
{
    public static class PlatformHelper
    {
        private static readonly Lazy<bool> _IsRunningOnMono = new Lazy<bool>(() => Type.GetType("Mono.Runtime") != null);

        public static bool IsRunningOnMono()
        {
            return _IsRunningOnMono.Value;
        }
    }
}