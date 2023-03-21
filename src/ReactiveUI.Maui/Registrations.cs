﻿// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

#if HAS_WINUI
using System.Reactive.Concurrency;
using Splat;
#endif

namespace ReactiveUI.Maui;

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

        registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        registerFunction(() => new BooleanToVisibilityTypeConverter(), typeof(IBindingTypeConverter));

#if HAS_WINUI
        registerFunction(() => new DependencyObjectObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(() => new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
        registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));

        if (!ModeDetector.InUnitTestRunner())
        {
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => DispatcherQueueScheduler.Current);
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
        }

        RxApp.SuppressViewCommandBindingMessage = true;
#endif
    }
}
