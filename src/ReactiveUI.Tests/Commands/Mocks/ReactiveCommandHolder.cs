﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;

namespace ReactiveUI.Tests
{
    public class ReactiveCommandHolder : ReactiveObject
    {
        private ReactiveCommand<int, Unit>? _theCommand;

        public ReactiveCommand<int, Unit>? TheCommand
        {
            get => _theCommand;
            set => this.RaiseAndSetIfChanged(ref _theCommand, value);
        }
    }
}
