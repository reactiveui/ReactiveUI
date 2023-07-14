// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.Winforms;

/// <summary>
/// .NET Framework platform registrations.
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

        registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));

        registerFunction(() => new CreatesWinformsCommandBinding(), typeof(ICreatesCommandBinding));
        registerFunction(() => new WinformsCreatesObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        registerFunction(() => new PanelSetMethodBindingConverter(), typeof(ISetMethodBindingConverter));
        registerFunction(() => new TableContentSetMethodBindingConverter(), typeof(ISetMethodBindingConverter));
        registerFunction(() => new StringConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new SingleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new DoubleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new DecimalToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));

        if (!ModeDetector.InUnitTestRunner())
        {
            WindowsFormsSynchronizationContext.AutoInstall = true;
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new SynchronizationContextScheduler(new WindowsFormsSynchronizationContext()));
        }
    }
}