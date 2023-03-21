// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using Splat;

namespace ReactiveUI;

/// <summary>
/// A mock platform registration for the .Net Standard.
/// It will fire an exception since we need a target platform to run.
/// </summary>
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        if (registerFunction is null)
        {
            throw new ArgumentNullException(nameof(registerFunction));
        }

        if (!ModeDetector.InUnitTestRunner())
        {
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxApp.MainThreadScheduler = DefaultScheduler.Instance;
        }
    }
}