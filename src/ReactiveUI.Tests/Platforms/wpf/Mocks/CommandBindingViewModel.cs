// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
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
        private ReactiveCommand<int, int> _Command1;
        private ReactiveCommand<Unit, Unit> _Command2;
        private ReactiveCommand<Unit, int?> _Command3;
        private ObservableAsPropertyHelper<int?> _result;

        private int _value;

        public CommandBindingViewModel()
        {
            _Command1 = ReactiveCommand.Create<int, int>(_ => { return _; }, outputScheduler: ImmediateScheduler.Instance, canExecuteScheduler: ImmediateScheduler.Instance);
            _Command2 = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance, canExecuteScheduler: ImmediateScheduler.Instance);
            _Command3 = ReactiveCommand.CreateFromTask(RunAsync, outputScheduler: RxApp.TaskpoolScheduler, canExecuteScheduler: ImmediateScheduler.Instance);
            _result = _Command3.ToProperty(this, x => x.Result, scheduler: RxApp.MainThreadScheduler);
        }

        public ReactiveCommand<int, int> Command1
        {
            get => _Command1;
            set => this.RaiseAndSetIfChanged(ref _Command1, value);
        }

        public ReactiveCommand<Unit, Unit> Command2
        {
            get => _Command2;
            set => this.RaiseAndSetIfChanged(ref _Command2, value);
        }

        public ReactiveCommand<Unit, int?> Command3
        {
            get => _Command3;
            set => this.RaiseAndSetIfChanged(ref _Command3, value);
        }

        public FakeNestedViewModel? NestedViewModel { get; set; }

        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        public int? Result => _result.Value;

        private async Task<int?> RunAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            return cancellationToken.IsCancellationRequested ? null : 100;
        }
    }
}
