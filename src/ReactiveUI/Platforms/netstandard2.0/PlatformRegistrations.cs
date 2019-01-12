// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI
{
    /// <summary>
    /// A mock platform registration for the .Net Standard.
    /// It will fire an exception since we need a target platform to run.
    /// </summary>
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        /// <inheritdoc/>
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            throw new Exception("You are referencing the Portable version of ReactiveUI in an App. Please change your reference to the specific version for your platform found here: https://reactiveui.net/docs/getting-started/installation/nuget-packages");
        }
    }
}
