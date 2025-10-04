// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Winforms;

/// <summary>
/// .NET Framework platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
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

        registerFunction(static () => new PlatformOperations(), typeof(IPlatformOperations));

        registerFunction(static () => new CreatesWinformsCommandBinding(), typeof(ICreatesCommandBinding));
        registerFunction(static () => new WinformsCreatesObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(static () => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        registerFunction(static () => new PanelSetMethodBindingConverter(), typeof(ISetMethodBindingConverter));
        registerFunction(static () => new TableContentSetMethodBindingConverter(), typeof(ISetMethodBindingConverter));
        registerFunction(static () => new StringConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new SingleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new DoubleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new DecimalToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));

        if (!ModeDetector.InUnitTestRunner())
        {
            WindowsFormsSynchronizationContext.AutoInstall = true;
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(static () => new SynchronizationContextScheduler(new WindowsFormsSynchronizationContext()));
        }
    }
}
