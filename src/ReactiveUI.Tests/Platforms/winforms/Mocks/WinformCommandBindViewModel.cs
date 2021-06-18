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
        private ReactiveCommand<int, Unit> _command3;
        private int _parameter = 1;
        private int _parameterResult;

        public WinformCommandBindViewModel()
        {
            _command1 = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);
            _command2 = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);
            _command3 = ReactiveCommand.Create<int>(i => ParameterResult = i * 10, outputScheduler: ImmediateScheduler.Instance);
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

        public ReactiveCommand<int, Unit> Command3
        {
            get => _command3;
            set => this.RaiseAndSetIfChanged(ref _command3, value);
        }

        public int Parameter
        {
            get => _parameter;
            set => this.RaiseAndSetIfChanged(ref _parameter, value);
        }

        public int ParameterResult
        {
            get => _parameterResult;
            set => this.RaiseAndSetIfChanged(ref _parameterResult, value);
        }
    }
}
