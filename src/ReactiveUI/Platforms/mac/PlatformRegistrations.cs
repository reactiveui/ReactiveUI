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
    public void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        registrar.RegisterConstant<IPlatformOperations>(static () => new PlatformOperations());
        registrar.RegisterConstant<IBindingFallbackConverter>(static () => new ComponentModelFallbackConverter());
        registrar.RegisterConstant<ICreatesObservableForProperty>(static () => new AppKitObservableForProperty());
        registrar.RegisterConstant<ICreatesCommandBinding>(static () => new TargetActionCommandBinder());

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
            RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxSchedulers.MainThreadScheduler = new WaitForDispatcherScheduler(static () => new NSRunloopScheduler());
        }

        registrar.RegisterConstant<ISuspensionDriver>(static () => new AppSupportJsonSuspensionDriver());
    }
}
