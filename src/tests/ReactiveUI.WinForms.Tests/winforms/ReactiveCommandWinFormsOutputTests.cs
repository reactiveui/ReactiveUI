// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Tests for ReactiveCommand output propagation in WinForms context.
/// Validates the scenarios reported where ReactiveCommand doesn't propagate output on WinForms.
/// </summary>
/// <remarks>
/// These tests verify the behavior described in the bug report:
/// - command.Subscribe() should receive output when command is executed
/// - command.IsExecuting should track execution state
/// - WhenAnyObservable(vm => vm.Command) should propagate output
/// - InvokeCommand should execute and propagate output
/// - command.Execute().Subscribe() should receive output
///
/// The WinForms initialization is performed via
/// AppLocator.CurrentMutable.CreateReactiveUIBuilder().WithWinForms().BuildApp()
/// (which now includes core services) as called by the <see cref="WinFormsTestExecutor"/>.
/// </remarks>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class ReactiveCommandWinFormsOutputTests
{
    private const int Two = 2;
    private const int Three = 3;
    private const string HelloInput = "hello";
    private const string HelloUpper = "HELLO";
    private const string Page1 = "page1";

    /// <summary>
    /// Verifies that subscribing directly to a ReactiveCommand receives the output value
    /// when the command executes. Reproduces the bug where command.Subscribe() did nothing.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReactiveCommand_Subscribe_ReceivesOutput_WhenCommandExecutes()
    {
        var command = ReactiveCommand.CreateFromObservable(
            () => Observable.Return("result"),
            outputScheduler: ImmediateScheduler.Instance);

        var results = new List<string>();
        command.Subscribe(results.Add);

        await command.Execute();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).IsEqualTo("result");
    }

    /// <summary>
    /// Verifies that subscribing to a ReactiveCommand with a parameter receives the output value.
    /// Reproduces the bug where command.Subscribe() did nothing for parameterized commands.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReactiveCommand_Subscribe_ReceivesOutput_WithParameter()
    {
        var command = ReactiveCommand.CreateFromObservable(
            (string input) => Observable.Return(input.ToUpperInvariant()),
            outputScheduler: ImmediateScheduler.Instance);

        var results = new List<string>();
        command.Subscribe(results.Add);

        await command.Execute(HelloInput);

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).IsEqualTo(HelloUpper);
    }

    /// <summary>
    /// Verifies that multiple executions each propagate their output to subscribers.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReactiveCommand_Subscribe_ReceivesAllOutputs_OnMultipleExecutions()
    {
        var counter = 0;
        var command = ReactiveCommand.Create(
            () => ++counter,
            outputScheduler: ImmediateScheduler.Instance);

        var results = new List<int>();
        command.Subscribe(results.Add);

        await command.Execute();
        await command.Execute();
        await command.Execute();

        using var multiple = Assert.Multiple();
        await Assert.That(results).Count().IsEqualTo(Three);
        await Assert.That(results[0]).IsEqualTo(1);
        await Assert.That(results[1]).IsEqualTo(Two);
        await Assert.That(results[Two]).IsEqualTo(Three);
    }

    /// <summary>
    /// Verifies that IsExecuting transitions correctly during command execution.
    /// Reproduces the bug where IsExecuting only emitted the initial false value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReactiveCommand_IsExecuting_TracksExecutionState()
    {
        var gate = new Subject<Unit>();
        var command = ReactiveCommand.CreateFromObservable(
            () => gate.Take(1),
            outputScheduler: ImmediateScheduler.Instance);

        var executingValues = new List<bool>();
        command.IsExecuting.Subscribe(executingValues.Add);

        // Start execution (don't await yet)
        var executeTask = command.Execute().ToTask();

        // Signal the command to complete
        gate.OnNext(Unit.Default);
        await executeTask;

        using var multiple = Assert.Multiple();

        // Should have: false (initial), true (executing), false (completed)
        await Assert.That(executingValues).Count().IsGreaterThanOrEqualTo(Three);
        await Assert.That(executingValues[0]).IsFalse();
        await Assert.That(executingValues[1]).IsTrue();
        await Assert.That(executingValues[executingValues.Count - 1]).IsFalse();
    }

    /// <summary>
    /// Verifies that WhenAnyObservable with a command property propagates output.
    /// Reproduces the bug where WhenAnyObservable(vm => vm.Command) did nothing.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task WhenAnyObservable_Command_PropagatesOutput()
    {
        var viewModel = new ReactiveCommandOutputViewModel();

        var results = new List<string>();
        viewModel.WhenAnyObservable(vm => vm.NavigateCommand)
                 .Subscribe(results.Add);

        await viewModel.NavigateCommand.Execute(Page1);

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).IsEqualTo(Page1);
    }

    /// <summary>
    /// Verifies that InvokeCommand executes the command and the output is propagated to subscribers.
    /// The user confirmed InvokeCommand executes correctly; this test verifies output propagation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task InvokeCommand_ExecutesCommand_AndOutputPropagates()
    {
        var command = ReactiveCommand.CreateFromObservable(
            (string input) => Observable.Return(input.ToUpperInvariant()),
            outputScheduler: ImmediateScheduler.Instance);

        var results = new List<string>();
        command.Subscribe(results.Add);

        var source = new Subject<string>();
        source.InvokeCommand(command);
        source.OnNext(HelloInput);

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).IsEqualTo(HelloUpper);
    }

    /// <summary>
    /// Verifies that InvokeCommand with a target ViewModel executes and output propagates.
    /// Reproduces the exact pattern from the bug report:
    /// Observable.Return(ShellPages.Clients).InvokeCommand(this, vm => vm.NavigateToCommand).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task InvokeCommand_WithTarget_ExecutesCommand_AndOutputPropagates()
    {
        var viewModel = new ReactiveCommandOutputViewModel();

        var results = new List<string>();
        viewModel.NavigateCommand.Subscribe(results.Add);

        var source = new Subject<string>();
        source.InvokeCommand(viewModel, vm => vm.NavigateCommand);
        source.OnNext(Page1);

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).IsEqualTo(Page1);
    }

    /// <summary>
    /// Verifies that Execute().Subscribe() receives the output value.
    /// The user reported this executes but "propagates nothing".
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Execute_Subscribe_ReceivesOutput()
    {
        var command = ReactiveCommand.CreateFromObservable(
            (string input) => Observable.Return(input.ToUpperInvariant()),
            outputScheduler: ImmediateScheduler.Instance);

        var executedResults = new List<string>();
        var subscribedResults = new List<string>();

        // Subscribe to the command output stream
        command.Subscribe(subscribedResults.Add);

        // Execute the command
        await command.Execute(HelloInput).Do(executedResults.Add);

        using var multiple = Assert.Multiple();
        await Assert.That(executedResults).Count().IsEqualTo(1);
        await Assert.That(subscribedResults).Count().IsEqualTo(1);
        await Assert.That(executedResults[0]).IsEqualTo(HelloUpper);
        await Assert.That(subscribedResults[0]).IsEqualTo(HelloUpper);
    }

    /// <summary>
    /// Verifies that a command returning an IObservable propagates output.
    /// Reproduces the exact command pattern from the bug report (CreateFromObservable with Observable.Create).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromObservable_WithObservableCreate_PropagatesOutput()
    {
        var command = ReactiveCommand.CreateFromObservable(
            (string page) => Observable.Create<string>(observer =>
            {
                var result = "navigated-to-" + page;
                observer.OnNext(result);
                observer.OnCompleted();
                return Disposable.Empty;
            }),
            outputScheduler: ImmediateScheduler.Instance);

        var results = new List<string>();
        command.Subscribe(results.Add);

        await command.Execute("clients");

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).IsEqualTo("navigated-to-clients");
    }

    /// <summary>
    /// Verifies that commands chained with SelectMany propagate the final output.
    /// Reproduces the bug report pattern where NavigateAndReset.Execute was chained via SelectMany.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromObservable_WithSelectManyChain_PropagatesOutput()
    {
        var innerCommand = ReactiveCommand.CreateFromObservable(
            (string page) => Observable.Return("inner-" + page),
            outputScheduler: ImmediateScheduler.Instance);

        var outerCommand = ReactiveCommand.CreateFromObservable(
            (string page) => Observable.Return(page)
                .SelectMany(p => innerCommand.Execute(p)),
            outputScheduler: ImmediateScheduler.Instance);

        var results = new List<string>();
        outerCommand.Subscribe(results.Add);

        await outerCommand.Execute("clients");

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).IsEqualTo("inner-clients");
    }
}
