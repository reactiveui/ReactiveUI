// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Commands.Mocks;

/// <summary>A ReactiveObject which hosts a ReactiveCommand.</summary>
/// <seealso cref="ReactiveObject" />
public class ReactiveCommandHolder : ReactiveObject
{
    /// <summary>Gets or sets the command.</summary>
    public ReactiveCommand<int, RxVoid>? TheCommand
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
