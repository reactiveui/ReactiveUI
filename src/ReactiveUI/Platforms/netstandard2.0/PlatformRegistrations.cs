// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// A mock platform registration for the .Net Standard.
/// It will fire an exception since we need a target platform to run.
/// </summary>
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Platform registration uses RxApp which requires dynamic code generation")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Platform registration uses RxApp which may require unreferenced code")]
#endif
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        registerFunction.ArgumentNullExceptionThrowIfNull(nameof(registerFunction));

        if (!ModeDetector.InUnitTestRunner())
        {
            RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxSchedulers.MainThreadScheduler = DefaultScheduler.Instance;
        }
    }
}
