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
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Platform registration uses ComponentModelTypeConverter and RxApp which require dynamic code generation")]
    [RequiresUnreferencedCode("Platform registration uses ComponentModelTypeConverter and RxApp which may require unreferenced code")]
#endif
    public void Register(Action<Func<object>, Type> registerFunction) // TODO: Create Test
    {
        ArgumentNullException.ThrowIfNull(registerFunction);

        registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));
        registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new AndroidObservableForWidgets(), typeof(ICreatesObservableForProperty));
        registerFunction(() => new AndroidCommandBinders(), typeof(ICreatesCommandBinding));

        if (!ModeDetector.InUnitTestRunner())
        {
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxApp.MainThreadScheduler = HandlerScheduler.MainThreadScheduler;
        }

        registerFunction(() => new BundleSuspensionDriver(), typeof(ISuspensionDriver));
    }
}
