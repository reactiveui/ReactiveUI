// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if IS_WINUI
#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.WinUI;
#else
namespace ReactiveUI.WinUI;
#endif
#endif
#if IS_MAUI
#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif
#endif

/// <summary>
/// The main registration for common classes for the Splat dependency injection.
/// We have code that runs reflection through the different ReactiveUI classes
/// searching for IWantsToRegisterStuff and will register all our required DI
/// interfaces. The registered items in this classes are common for all Platforms.
/// To get these registrations after the main ReactiveUI Initialization use the
/// DependencyResolverMixins.InitializeReactiveUI() extension method.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar)
    {
        ArgumentNullException.ThrowIfNull(registrar);

        registrar.RegisterConstant<IActivationForViewFetcher>(static () => new ActivationForViewFetcher());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new BooleanToVisibilityTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new VisibilityToBooleanTypeConverter());

#if WINUI_TARGET
        registrar.RegisterConstant<IPlatformOperations>(static () => new PlatformOperations());
        registrar.RegisterConstant<ICreatesObservableForProperty>(static () => new DependencyObjectObservableForProperty());
        registrar.RegisterConstant<IPropertyBindingHook>(static () => new AutoDataTemplateBindingHook());
        registrar.RegisterConstant<IBindingFallbackConverter>(static () => new ComponentModelFallbackConverter());

        RxSchedulers.SuppressViewCommandBindingMessage = true;
#endif
    }
}
