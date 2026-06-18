// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

/// <summary>A mock view model.</summary>
public class CommandBindViewModel : ReactiveObject
{
    /// <summary>Initializes a new instance of the <see cref="CommandBindViewModel" /> class.</summary>
    public CommandBindViewModel()
    {
        Command1 = ReactiveCommand.Create<int, RxVoid>(static _ => RxVoid.Default);
        Command2 = ReactiveCommand.Create(static () => { });
        NestedViewModel = new();
    }

    /// <summary>Gets or sets the command1.</summary>
    public ReactiveCommand<int, RxVoid> Command1
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the command2.</summary>
    public ReactiveCommand<RxVoid, RxVoid> Command2
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the nested view model.</summary>
    public FakeNestedViewModel NestedViewModel { get; set; }

    /// <summary>Gets or sets the value.</summary>
    public int Value
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
