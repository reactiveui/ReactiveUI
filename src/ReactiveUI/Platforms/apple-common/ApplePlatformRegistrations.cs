// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// The service registrations shared by every Apple platform. AppKit and UIKit differ only in which property
/// observer and command binder they contribute, so each platform registration class supplies those two
/// factories and this class registers the rest in the order the platforms expect.
/// </summary>
internal static class ApplePlatformRegistrations
{
    /// <summary>Registers the services common to every Apple platform.</summary>
    /// <param name="registrar">The registrar the services are registered with.</param>
    /// <param name="observableForProperty">Creates the platform's UI framework property observer.</param>
    /// <param name="commandBinding">Creates the platform's UI framework command binder.</param>
    internal static void Register(
        IRegistrar registrar,
        Func<ICreatesObservableForProperty> observableForProperty,
        Func<ICreatesCommandBinding> commandBinding)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        registrar.RegisterConstant<IPlatformOperations>(static () => new PlatformOperations());
        registrar.RegisterConstant<IBindingFallbackConverter>(static () => new ComponentModelFallbackConverter());
        registrar.RegisterConstant(observableForProperty);
        registrar.RegisterConstant(commandBinding);

        // DateTime ↔ NSDate converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DateTimeToNSDateConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDateTimeToNSDateConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NSDateToDateTimeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NSDateToNullableDateTimeConverter());

        // DateTimeOffset ↔ NSDate converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DateTimeOffsetToNSDateConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDateTimeOffsetToNSDateConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NSDateToDateTimeOffsetConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NSDateToNullableDateTimeOffsetConverter());

        registrar.RegisterConstant<ICreatesObservableForProperty>(static () => new KVOObservableForProperty());

        if (!ModeDetector.InUnitTestRunner())
        {
            RxSchedulers.TaskpoolScheduler = Sequencer.Default;
            RxSchedulers.MainThreadScheduler = NSRunloopSequencer.Main;
        }

        registrar.RegisterConstant<ISuspensionDriver>(static () => new AppSupportJsonSuspensionDriver());
    }
}
