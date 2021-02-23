// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;

namespace ReactiveUI.Tests.Winforms
{
    public class WinformCommandBindViewModel : ReactiveObject
    {
        private ReactiveCommand<Unit, Unit> _command1;

        private ReactiveCommand<Unit, Unit> _command2;

        public WinformCommandBindViewModel()
        {
            _command1 = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);
            _command2 = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);
        }

        public ReactiveCommand<Unit, Unit> Command1
        {
            get => _command1;
            set => this.RaiseAndSetIfChanged(ref _command1, value);
        }

        public ReactiveCommand<Unit, Unit> Command2
        {
            get => _command2;
            set => this.RaiseAndSetIfChanged(ref _command2, value);
        }
    }
}
