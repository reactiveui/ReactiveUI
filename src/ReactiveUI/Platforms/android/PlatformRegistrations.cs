// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Android platform registrations.
/// </summary>
/// <seealso cref="ReactiveUI.IWantsToRegisterStuff" />
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
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
