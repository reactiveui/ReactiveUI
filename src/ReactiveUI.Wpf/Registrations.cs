// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Wpf;

/// <summary>
/// Registrations specific to the WPF platform.
/// </summary>
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Register uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("Register uses methods that may require unreferenced code")]
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Not all paths use reflection")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Not all paths use reflection")]
#endif
    public void Register(Action<Func<object>, Type> registerFunction)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(registerFunction);
#else
        if (registerFunction is null)
        {
            throw new ArgumentNullException(nameof(registerFunction));
        }
#endif

        registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));

        registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        registerFunction(() => new DependencyObjectObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(() => new StringConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new SingleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new DoubleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new DecimalToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new BooleanToVisibilityTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
        registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));

        if (!ModeDetector.InUnitTestRunner())
        {
            // NB: On .NET Core, trying to touch DispatcherScheduler blows up :cry:
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => DispatcherScheduler.Current);
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
        }

        RxApp.SuppressViewCommandBindingMessage = true;
    }
}
