// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ReactiveUI
{
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            throw new Exception("You are referencing the Portable version of ReactiveUI in an App. Please change your reference to the specific version for your platform found here: https://reactiveui.net/docs/getting-started/installation/nuget-packages");
        }
    }
}
