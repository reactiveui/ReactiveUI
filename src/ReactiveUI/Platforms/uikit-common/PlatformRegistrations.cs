// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// UIKit platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
[Preserve(AllMembers = true)]
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection to create instances of types.")]
    [RequiresDynamicCode("Uses reflection to create instances of types.")]
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        ArgumentExceptionHelper.ThrowIfNull(registerFunction);

        registerFunction(static () => new PlatformOperations(), typeof(IPlatformOperations));
        registerFunction(static () => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new UIKitObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(static () => new UIKitCommandBinders(), typeof(ICreatesCommandBinding));
        registerFunction(static () => new DateTimeNSDateConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new KVOObservableForProperty(), typeof(ICreatesObservableForProperty));

        if (!ModeDetector.InUnitTestRunner())
        {
            RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxSchedulers.MainThreadScheduler = new WaitForDispatcherScheduler(static () => new NSRunloopScheduler());
        }

        registerFunction(static () => new AppSupportJsonSuspensionDriver(), typeof(ISuspensionDriver));
    }
}
