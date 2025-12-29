// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Mac platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Platform registration uses ComponentModelTypeConverter and RxApp which require dynamic code generation")]
    [RequiresUnreferencedCode("Platform registration uses ComponentModelTypeConverter and RxApp which may require unreferenced code")]
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Not all paths use reflection")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Not all paths use reflection")]
#endif
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        ArgumentExceptionHelper.ThrowIfNull(registerFunction);

        registerFunction(static () => new PlatformOperations(), typeof(IPlatformOperations));
        registerFunction(static () => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new AppKitObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(static () => new TargetActionCommandBinder(), typeof(ICreatesCommandBinding));
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
