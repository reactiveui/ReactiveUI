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
    public void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        registrar.RegisterConstant<IPlatformOperations>(static () => new PlatformOperations());
        registrar.RegisterConstant<IActivationForViewFetcher>(static () => new ActivationForViewFetcher());
        registrar.RegisterConstant<ICreatesObservableForProperty>(static () => new DependencyObjectObservableForProperty());

        // WPF-specific command rebinding optimization
        registrar.RegisterConstant<ICreatesCustomizedCommandRebinding>(static () => new WpfCommandRebindingCustomizer());

        // WPF-specific converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new BooleanToVisibilityTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new VisibilityToBooleanTypeConverter());

        registrar.RegisterConstant<IPropertyBindingHook>(static () => new AutoDataTemplateBindingHook());
        registrar.RegisterConstant<IBindingFallbackConverter>(static () => new ComponentModelFallbackConverter());

        if (!ModeDetector.InUnitTestRunner())
        {
            // NB: On .NET Core, trying to touch DispatcherScheduler blows up :cry:
            RxSchedulers.MainThreadScheduler = new WaitForDispatcherScheduler(static () => DispatcherScheduler.Current);
            RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
        }

        RxSchedulers.SuppressViewCommandBindingMessage = true;
    }
}
