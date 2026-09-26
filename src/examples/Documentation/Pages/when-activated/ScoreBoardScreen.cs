// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>
/// The scoreboard screen itself. It raises <see cref="Activated"/> and <see cref="Deactivated"/> the way a real
/// platform control does, rather than relying on a UI framework the console has none of. It can hold any content
/// view model, and reports every change to that content through <see cref="ContentChanged"/>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Title = {Title}")]
public sealed class ScoreBoardScreen : IActivatableView, ICanActivate, IDisposable
{
    /// <summary>Raised when the screen is shown.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>Raised when the screen is hidden.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Replays the current content to each new subscriber, then every later change.</summary>
    private readonly BehaviorSignal<object?> _contentChanged;

    /// <summary>Initializes a new instance of the <see cref="ScoreBoardScreen"/> class.</summary>
    /// <param name="content">The view model the screen starts out showing.</param>
    public ScoreBoardScreen(object? content) => _contentChanged = new(content);

    /// <inheritdoc/>
    public IObservable<RxVoid> Activated => _activated;

    /// <inheritdoc/>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <summary>Gets the stream of content changes, including the current content for a new subscriber.</summary>
    public IObservable<object?> ContentChanged => _contentChanged;

    /// <summary>Gets or sets the content view model the screen shows.</summary>
    public object? Content
    {
        get => _contentChanged.Value;
        set => _contentChanged.OnNext(value);
    }

    /// <summary>Gets or sets the title the screen last chose for its current content.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Shows the screen, as a window does when it is navigated to.</summary>
    public void Show() => _activated.OnNext(RxVoid.Default);

    /// <summary>Hides the screen, as a window does when it is navigated away from.</summary>
    public void Hide() => _deactivated.OnNext(RxVoid.Default);

    /// <inheritdoc/>
    public void Dispose()
    {
        _activated.Dispose();
        _deactivated.Dispose();
        _contentChanged.Dispose();
    }
}
