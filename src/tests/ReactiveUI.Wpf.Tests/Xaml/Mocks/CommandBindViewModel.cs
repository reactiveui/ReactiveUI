// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;

namespace ReactiveUI.Tests.Mocks;

/// <summary>
/// A mock view model.
/// </summary>
public class CommandBindViewModel : ReactiveObject
{
    /// <summary>
    /// Backing field for the <see cref="Command1"/> property.
    /// </summary>
    private ReactiveCommand<int, Unit> _command1 = null!;

    /// <summary>
    /// Backing field for the <see cref="Command2"/> property.
    /// </summary>
    private ReactiveCommand<Unit, Unit> _command2 = null!;

    /// <summary>
    /// Backing field for the <see cref="Value"/> property.
    /// </summary>
    private int _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBindViewModel" /> class.
    /// </summary>
    public CommandBindViewModel()
    {
        Command1 = ReactiveCommand.Create<int, Unit>(static _ => Unit.Default);
        Command2 = ReactiveCommand.Create(static () => { });
        NestedViewModel = new();
    }

    /// <summary>
    /// Gets or sets the command1.
    /// </summary>
    public ReactiveCommand<int, Unit> Command1
    {
        get => _command1;
        set => this.RaiseAndSetIfChanged(ref _command1, value);
    }

    /// <summary>
    /// Gets or sets the command2.
    /// </summary>
    public ReactiveCommand<Unit, Unit> Command2
    {
        get => _command2;
        set => this.RaiseAndSetIfChanged(ref _command2, value);
    }

    /// <summary>
    /// Gets or sets the nested view model.
    /// </summary>
    public FakeNestedViewModel NestedViewModel { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public int Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
}
