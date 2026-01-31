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
    public void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        registrar.RegisterConstant<IPlatformOperations>(static () => new PlatformOperations());
        registrar.RegisterConstant<ICreatesCommandBinding>(static () => new CreatesWinformsCommandBinding());
        registrar.RegisterConstant<ICreatesObservableForProperty>(static () => new WinformsCreatesObservableForProperty());
        registrar.RegisterConstant<IActivationForViewFetcher>(static () => new ActivationForViewFetcher());
        registrar.RegisterConstant<ISetMethodBindingConverter>(static () => new PanelSetMethodBindingConverter());
        registrar.RegisterConstant<ISetMethodBindingConverter>(static () => new TableContentSetMethodBindingConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new SingleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DoubleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DecimalToStringTypeConverter());
        registrar.RegisterConstant<IBindingFallbackConverter>(static () => new ComponentModelFallbackConverter());

        if (!ModeDetector.InUnitTestRunner())
        {
            WindowsFormsSynchronizationContext.AutoInstall = true;
        }
    }
}
