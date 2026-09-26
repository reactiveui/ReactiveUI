// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Reflection;

/// <summary>The music player screen's view model: it plays while the screen is shown and pauses when it is hidden.</summary>
[System.Diagnostics.DebuggerDisplay("Title = {Title}, IsPlaying = {IsPlaying}")]
public sealed class MusicPlayerViewModel : ReactiveObject, IActivatableViewModel, IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="MusicPlayerViewModel"/> class.</summary>
    /// <param name="title">The track this view model plays.</param>
    public MusicPlayerViewModel(string title)
    {
        Title = title;

        this.WhenActivated(disposables =>
        {
            IsPlaying = true;
            disposables.Add(new ActionDisposable(() => IsPlaying = false));
        });
    }

    /// <inheritdoc/>
    public ViewModelActivator Activator { get; } = new();

    /// <summary>Gets the track this view model plays.</summary>
    public string Title { get; }

    /// <summary>Gets a value indicating whether the track is playing. True only while the screen is shown.</summary>
    public bool IsPlaying
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    public void Dispose() => Activator.Dispose();
}
