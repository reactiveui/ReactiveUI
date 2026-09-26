// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Reflection;

/// <summary>
/// A screen that raises its own activation, the way a platform control does, and is itself the
/// <see cref="IViewFor{T}"/> for its view model. Passing no view to <c>WhenActivated</c> lets it discover
/// <see cref="ViewModel"/> on itself through reflection.
/// </summary>
/// <remarks>
/// <see cref="PreserveAttribute"/> tells a linker not to trim this type or its members: nothing in the code calls
/// this class by name, so a trimmed build could otherwise remove it before <c>RegisterViewsForViewModels</c> gets
/// the chance to find it by scanning the assembly.
/// </remarks>
[Preserve(AllMembers = true)]
[System.Diagnostics.DebuggerDisplay("NowPlayingText = {NowPlayingText}")]
public sealed class MusicPlayerScreen : ReactiveObject, IViewFor<MusicPlayerViewModel>, ICanActivate, IDisposable
{
    /// <summary>Raised when the screen is shown.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>Raised when the screen is hidden.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Gets or sets the view model the screen shows.</summary>
    public MusicPlayerViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (MusicPlayerViewModel?)value;
    }

    /// <summary>Gets or sets the text the screen shows for whatever is playing.</summary>
    public string NowPlayingText { get; set; } = string.Empty;

    /// <inheritdoc/>
    public IObservable<RxVoid> Activated => _activated;

    /// <inheritdoc/>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <summary>Shows the screen, as a window does when it is navigated to.</summary>
    public void Show() => _activated.OnNext(RxVoid.Default);

    /// <summary>Hides the screen, as a window does when it is navigated away from.</summary>
    public void Hide() => _deactivated.OnNext(RxVoid.Default);

    /// <inheritdoc/>
    public void Dispose()
    {
        _activated.Dispose();
        _deactivated.Dispose();
    }
}
