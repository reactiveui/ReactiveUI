// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>A view model used for command binding tests.</summary>
public class WinformCommandBindViewModel : ReactiveObject
{
    /// <summary>The multiplier applied to a parameterized command's input.</summary>
    private const int ParameterMultiplier = 10;

    /// <summary>Backing field for the first command.</summary>
    private ReactiveCommand<RxVoid, RxVoid> _command1;

    /// <summary>Backing field for the second command.</summary>
    private ReactiveCommand<RxVoid, RxVoid> _command2;

    /// <summary>Backing field for the third command.</summary>
    private ReactiveCommand<int, RxVoid> _command3;

    /// <summary>Initializes a new instance of the <see cref="WinformCommandBindViewModel"/> class.</summary>
    public WinformCommandBindViewModel()
    {
        _command1 = ReactiveCommand.Create(() => { }, outputScheduler: Sequencer.Immediate);
        _command2 = ReactiveCommand.CreateRunInBackground(() => { }, backgroundScheduler: null, outputScheduler: Sequencer.Immediate);
        _command3 = ReactiveCommand.Create<int>(i => ParameterResult = i * ParameterMultiplier, outputScheduler: Sequencer.Immediate);
    }

    /// <summary>Gets or sets the first command.</summary>
    public ReactiveCommand<RxVoid, RxVoid> Command1
    {
        get => _command1;
        set => this.RaiseAndSetIfChanged(ref _command1, value);
    }

    /// <summary>Gets or sets the second command.</summary>
    public ReactiveCommand<RxVoid, RxVoid> Command2
    {
        get => _command2;
        set => this.RaiseAndSetIfChanged(ref _command2, value);
    }

    /// <summary>Gets or sets the third command, which takes a parameter.</summary>
    public ReactiveCommand<int, RxVoid> Command3
    {
        get => _command3;
        set => this.RaiseAndSetIfChanged(ref _command3, value);
    }

    /// <summary>Gets or sets the command parameter.</summary>
    public int Parameter
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 1;

    /// <summary>Gets or sets the result produced by executing a parameterized command.</summary>
    public int ParameterResult
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
