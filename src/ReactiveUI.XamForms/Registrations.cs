﻿// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI.XamForms;

/// <summary>
/// The main registration for common classes for the Splat dependency injection.
/// We have code that runs reflection through the different ReactiveUI classes
/// searching for IWantsToRegisterStuff and will register all our required DI
/// interfaces. The registered items in this classes are common for all Platforms.
/// To get these registrations after the main ReactiveUI Initialization use the
/// DependencyResolverMixins.InitializeReactiveUI() extension method.
/// </summary>
/// <seealso cref="ReactiveUI.IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        if (registerFunction is null)
        {
            throw new ArgumentNullException(nameof(registerFunction));
        }

        registerFunction(() => new StringConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new ByteToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableByteToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new ShortToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableShortToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new IntegerToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableIntegerToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new LongToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableLongToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new SingleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableSingleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new DoubleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableDoubleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new DecimalToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableDecimalToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
    }
}
