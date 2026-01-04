// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.PlatformServices;

namespace ReactiveUI.Blazor;

/// <summary>
/// Blazor Framework platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(registrar);
#else
        if (registrar is null)
        {
            throw new ArgumentNullException(nameof(registrar));
        }
#endif

        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new ByteToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableByteToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new ShortToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableShortToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new IntegerToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableIntegerToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new LongToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableLongToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new SingleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableSingleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DoubleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDoubleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DecimalToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDecimalToStringTypeConverter());
        registrar.RegisterConstant<IPlatformOperations>(static () => new PlatformOperations());

        if (Type.GetType("Mono.Runtime") is not null)
        {
            PlatformEnlightenmentProvider.Current.EnableWasm();
        }

        RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
        RxSchedulers.MainThreadScheduler = CurrentThreadScheduler.Instance;
    }
}
