// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>A clerk's console screen: its search command, active search session and progress can each be replaced.</summary>
[System.Diagnostics.DebuggerDisplay("HasSearchCommand = {SearchCommand != null}")]
public sealed class DeskConsoleViewModel : ReactiveObject
{
    /// <summary>Gets or sets the command that runs the current search; replaced each time the clerk switches catalogue.</summary>
    public IReactiveCommand<string, IReadOnlyList<Book>>? SearchCommand
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the search currently reporting matches; replaced each time a new search starts.</summary>
    public SearchSession? ActiveSession
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the running match count for the current search; replaced each time a new search starts.</summary>
    public IObservable<int>? Progress
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
