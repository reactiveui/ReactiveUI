// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI.Tests.Wpf
{
    public class CommandBindingViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<int?> _result;
        private ReactiveCommand<int, int> _command1;
        private ReactiveCommand<Unit, Unit> _command2;
        private ReactiveCommand<Unit, int?> _command3;

        private int _value;

        public CommandBindingViewModel()
        {
            _command1 = ReactiveCommand.Create<int, int>(_ => { return _; }, outputScheduler: ImmediateScheduler.Instance);
            _command2 = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);
            _command3 = ReactiveCommand.CreateFromTask(RunAsync, outputScheduler: RxApp.TaskpoolScheduler);
            _result = _command3.ToProperty(this, x => x.Result, scheduler: RxApp.MainThreadScheduler);
        }

        public ReactiveCommand<int, int> Command1
        {
            get => _command1;
            set => this.RaiseAndSetIfChanged(ref _command1, value);
        }

        public ReactiveCommand<Unit, Unit> Command2
        {
            get => _command2;
            set => this.RaiseAndSetIfChanged(ref _command2, value);
        }

        public ReactiveCommand<Unit, int?> Command3
        {
            get => _command3;
            set => this.RaiseAndSetIfChanged(ref _command3, value);
        }

        public FakeNestedViewModel? NestedViewModel { get; set; }

        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        public int? Result => _result.Value;

        private async Task<int?> RunAsync(CancellationTokenSource cancellationToken)
        {
            await Task.Delay(1000, cancellationToken.Token).ConfigureAwait(false);
            return cancellationToken.IsCancellationRequested ? null : 100;
        }
    }
}
