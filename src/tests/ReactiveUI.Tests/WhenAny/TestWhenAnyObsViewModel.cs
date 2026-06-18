// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>A view model used to exercise the WhenAnyObservable tests.</summary>
public class TestWhenAnyObsViewModel : ReactiveObject
{
    /// <summary>Initializes a new instance of the <see cref="TestWhenAnyObsViewModel" /> class.</summary>
    public TestWhenAnyObsViewModel()
    {
        Command1 = ReactiveCommand.CreateFromObservable<int, int>(
            Signal.Emit,
            outputScheduler: Sequencer.Immediate);
        Command2 = ReactiveCommand.CreateFromObservable<int, int>(
            Signal.Emit,
            outputScheduler: Sequencer.Immediate);
        Command3 = ReactiveCommand.CreateFromObservable<string, string>(
            Signal.Emit,
            outputScheduler: Sequencer.Immediate);
    }

    /// <summary>Gets or sets the first command.</summary>
    public ReactiveCommand<int, int>? Command1 { get; set; }

    /// <summary>Gets or sets the second command.</summary>
    public ReactiveCommand<int, int> Command2 { get; set; }

    /// <summary>Gets or sets the third command.</summary>
    public ReactiveCommand<string, string> Command3 { get; set; }
}
