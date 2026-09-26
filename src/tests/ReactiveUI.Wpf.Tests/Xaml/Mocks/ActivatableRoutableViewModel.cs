// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>A routable view model that records whether its activator is active.</summary>
public sealed class ActivatableRoutableViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel, IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="ActivatableRoutableViewModel"/> class.</summary>
    public ActivatableRoutableViewModel()
    {
        _ = Activator.Activated.Subscribe(_ => IsActive = true);
        _ = Activator.Deactivated.Subscribe(_ => IsActive = false);
    }

    /// <inheritdoc/>
    public ViewModelActivator Activator { get; } = new();

    /// <summary>Gets a value indicating whether the activator is active.</summary>
    public bool IsActive { get; private set; }

    /// <inheritdoc/>
    public string? UrlPathSegment => "activatable";

    /// <inheritdoc/>
    public IScreen HostScreen => null!;

    /// <inheritdoc/>
    public void Dispose() => Activator.Dispose();
}
