// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

public class FooViewModel : ReactiveObject
{
    private int _setpoint;

    public FooViewModel(Foo foo)
    {
        Foo = foo ?? throw new ArgumentNullException(nameof(foo));

        this.WhenAnyValue(x => x.Setpoint)
            ////.Skip(1) // Skip the initial value
            .SelectMany(foo.SetValueAsync)
            .Subscribe();
    }

    public int Setpoint { get => _setpoint; set => this.RaiseAndSetIfChanged(ref _setpoint, value); }

    public Foo Foo { get; }
}
