﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Tests.Wpf
{
    public class CommandBindingViewModel : ReactiveObject
    {
        private ReactiveCommand<int, Unit> _Command1;
        private ReactiveCommand<Unit, Unit> _Command2;

        private int _value;

        public CommandBindingViewModel()
        {
            _Command1 = ReactiveCommand.Create<int, Unit>(_ => Unit.Default, outputScheduler: ImmediateScheduler.Instance);
            _Command2 = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);
        }

        public ReactiveCommand<int, Unit> Command1
        {
            get => _Command1;
            set => this.RaiseAndSetIfChanged(ref _Command1, value);
        }

        public ReactiveCommand<Unit, Unit> Command2
        {
            get => _Command2;
            set => this.RaiseAndSetIfChanged(ref _Command2, value);
        }

        public FakeNestedViewModel? NestedViewModel { get; set; }

        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }
    }
}
