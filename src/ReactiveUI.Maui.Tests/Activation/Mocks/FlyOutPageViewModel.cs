// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests.Activation;

/// <summary>
/// FlyOutPageViewModel.
/// </summary>
/// <seealso cref="ReactiveUI.ReactiveObject" />
/// <seealso cref="ReactiveUI.IActivatableViewModel" />
public class FlyOutPageViewModel : ReactiveObject, IActivatableViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlyOutPageViewModel"/> class.
    /// </summary>
    public FlyOutPageViewModel()
    {
        Activator = new ViewModelActivator();

        this.WhenActivated(d =>
        {
            IsActiveCount++;
            d(Disposable.Create(() => IsActiveCount--));
        });
    }

    /// <summary>
    /// Gets or sets the Activator which will be used by the View when Activation/Deactivation occurs.
    /// </summary>
    public ViewModelActivator Activator { get; protected set; }

    /// <summary>
    /// Gets or sets the active count.
    /// </summary>
    public int IsActiveCount { get; protected set; }
}
