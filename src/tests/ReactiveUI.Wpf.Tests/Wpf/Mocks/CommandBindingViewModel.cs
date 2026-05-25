// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI.Tests.Mocks;

namespace ReactiveUI.Tests.Wpf.Mocks;

/// <summary>
/// A mock view model used by WPF command binding tests.
/// </summary>
public class CommandBindingViewModel : ReactiveUI.ReactiveObject
{
    /// <summary>
    /// The delay, in milliseconds, applied by the asynchronous command body.
    /// </summary>
    private const int CommandDelayMilliseconds = 1000;

    /// <summary>
    /// The result value produced by the asynchronous command body.
    /// </summary>
    private const int CommandResultValue = 100;

    /// <summary>
    /// Backing field for the <see cref="Result"/> property.
    /// </summary>
    private readonly ObservableAsPropertyHelper<int?> _result;

    /// <summary>
    /// Backing field for the <see cref="Command1"/> property.
    /// </summary>
    private ReactiveCommand<int, int> _command1;

    /// <summary>
    /// Backing field for the <see cref="Command2"/> property.
    /// </summary>
    private ReactiveCommand<Unit, Unit> _command2;

    /// <summary>
    /// Backing field for the <see cref="Command3"/> property.
    /// </summary>
    private ReactiveCommand<Unit, int?> _command3;

    /// <summary>
    /// Backing field for the <see cref="Value"/> property.
    /// </summary>
    private int _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBindingViewModel"/> class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S3366:\"this\" should not be exposed from constructors",
        Justification = "OAPH/WhenAny initialization requires 'this'; single-threaded test fixture.")]
    public CommandBindingViewModel()
    {
        _command1 = ReactiveCommand.Create<int, int>(static _ => _, outputScheduler: ImmediateScheduler.Instance);
        _command2 = ReactiveCommand.Create(static () => { }, outputScheduler: ImmediateScheduler.Instance);
        _command3 = ReactiveCommand.CreateFromTask(RunAsync, outputScheduler: RxSchedulers.TaskpoolScheduler);
        _result = _command3.ToProperty(this, static x => x.Result, scheduler: ImmediateScheduler.Instance);
    }

    /// <summary>
    /// Gets or sets the first command.
    /// </summary>
    public ReactiveCommand<int, int> Command1
    {
        get => _command1;
        set => this.RaiseAndSetIfChanged(ref _command1, value);
    }

    /// <summary>
    /// Gets or sets the second command.
    /// </summary>
    public ReactiveCommand<Unit, Unit> Command2
    {
        get => _command2;
        set => this.RaiseAndSetIfChanged(ref _command2, value);
    }

    /// <summary>
    /// Gets or sets the third command.
    /// </summary>
    public ReactiveCommand<Unit, int?> Command3
    {
        get => _command3;
        set => this.RaiseAndSetIfChanged(ref _command3, value);
    }

    /// <summary>
    /// Gets or sets the nested view model.
    /// </summary>
    public FakeNestedViewModel? NestedViewModel { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public int Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    /// <summary>
    /// Gets the result produced by the third command.
    /// </summary>
    public int? Result => _result.Value;

    /// <summary>
    /// The asynchronous body for the third command.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The result value, or <see langword="null"/> if cancelled.</returns>
    private async Task<int?> RunAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(CommandDelayMilliseconds, cancellationToken).ConfigureAwait(false);
        return cancellationToken.IsCancellationRequested ? null : CommandResultValue;
    }
}
