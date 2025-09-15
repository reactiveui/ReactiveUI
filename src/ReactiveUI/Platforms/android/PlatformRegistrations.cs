// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Android platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(Action<Func<object>, Type> registerFunction) // TODO: Create Test
    {
        ArgumentNullException.ThrowIfNull(registerFunction);

        registerFunction(static () => new PlatformOperations(), typeof(IPlatformOperations));
        registerFunction(static () => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new AndroidObservableForWidgets(), typeof(ICreatesObservableForProperty));
        registerFunction(static () => new AndroidCommandBinders(), typeof(ICreatesCommandBinding));

        if (!ModeDetector.InUnitTestRunner())
        {
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxApp.MainThreadScheduler = HandlerScheduler.MainThreadScheduler;
        }

        registerFunction(static () => new BundleSuspensionDriver(), typeof(ISuspensionDriver));
    }
}
