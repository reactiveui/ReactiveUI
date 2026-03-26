// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A view model with a ReactiveCommand that returns an observable result.
/// Used to test command output propagation scenarios reported in the WinForms bug.
/// </summary>
public class ReactiveCommandOutputViewModel : ReactiveObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCommandOutputViewModel"/> class.
    /// </summary>
    public ReactiveCommandOutputViewModel() =>
        NavigateCommand = ReactiveCommand.CreateFromObservable(
            (string page) => Observable.Return(page),
            outputScheduler: ImmediateScheduler.Instance);

    /// <summary>
    /// Gets a command that simulates navigation and returns the page name.
    /// Modelled after the NavigateToCommand from the bug report.
    /// </summary>
    public ReactiveCommand<string, string> NavigateCommand { get; }
}
