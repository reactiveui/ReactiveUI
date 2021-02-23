// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;

namespace ReactiveUI.Tests.Winforms
{
    public class FakeViewModel : ReactiveObject
    {
        public FakeViewModel() => Cmd = ReactiveCommand.Create(() => { });

        public ReactiveCommand<Unit, Unit> Cmd { get; protected set; }
    }
}
