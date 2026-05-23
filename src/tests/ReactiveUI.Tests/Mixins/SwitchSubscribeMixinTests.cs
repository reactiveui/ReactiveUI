// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mixins;

/// <summary>
///     Tests for SwitchSubscribeMixin extension methods that handle switching between observables.
/// </summary>
public class SwitchSubscribeMixinTests
{
    private const int DoubleMultiplier = 2;
    private const int TripleMultiplier = 3;
    private const int CommandInput = 5;

    /// <summary>
    ///     Verifies that SwitchSubscribe on an observable throws when the source is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        const IObservable<IObservable<int>?> source = null!;

        // Act & Assert
        await Assert.That(() => source.SwitchSubscribe(_ => { }))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on an observable throws when the onNext handler is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldThrowArgumentNullException_WhenOnNextIsNull()
    {
        // Arrange
        var source = Observable.Return(Observable.Return(1));

        // Act & Assert
        await Assert.That(() => source.SwitchSubscribe(null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on an observable receives values from the inner observable.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldReceiveValues()
    {
        // Arrange
        var inner = new BehaviorSubject<int>(1);
        var outer = new BehaviorSubject<IObservable<int>?>(inner);
        var values = new List<int>();

        // Act
        using var subscription = outer.SwitchSubscribe(values.Add);

        // Assert
        await Assert.That(values).IsEquivalentTo([1]);
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on an observable switches to a newly emitted inner observable.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldSwitchToNewObservable()
    {
        // Arrange
        const int SecondInnerValue = 10;
        var inner1 = new BehaviorSubject<int>(1);
        var inner2 = new BehaviorSubject<int>(SecondInnerValue);
        var outer = new BehaviorSubject<IObservable<int>?>(inner1);
        var values = new List<int>();

        using var subscription = outer.SwitchSubscribe(values.Add);

        // Act - Switch to new observable
        outer.OnNext(inner2);

        // Assert - Should receive value from new observable
        await Assert.That(values).IsEquivalentTo([1, SecondInnerValue]);
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on an observable ignores null inner observables.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldIgnoreNullObservables()
    {
        // Arrange
        var inner = new BehaviorSubject<int>(1);
        var outer = new BehaviorSubject<IObservable<int>?>(inner);
        var values = new List<int>();

        using var subscription = outer.SwitchSubscribe(values.Add);

        // Act - Emit null
        outer.OnNext(null);

        // Assert - Should not crash and should still have first value
        await Assert.That(values).IsEquivalentTo([1]);
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe with handlers throws when any parameter is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var source = Observable.Return(Observable.Return(1));
        Action<int> onNext = _ => { };
        Action<Exception> onError = _ => { };
        var onCompleted = () => { };

        // Act & Assert
        await Assert.That(() => ((IObservable<IObservable<int>?>)null!).SwitchSubscribe(onNext, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(null!, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(onNext, null!, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(onNext, onError, null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe with handlers invokes the onError handler.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithHandlers_ShouldCallOnError()
    {
        // Arrange
        var inner = new Subject<int>();
        var outer = Observable.Return<IObservable<int>?>(inner);
        Exception? capturedError = null;

        // Act
        using var subscription = outer.SwitchSubscribe(_ => { }, ex => capturedError = ex, () => { });
        inner.OnError(new InvalidOperationException("test error"));

        // Assert
        await Assert.That(capturedError).IsNotNull();
        await Assert.That(capturedError).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe with handlers invokes the onCompleted handler.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithHandlers_ShouldCallOnCompleted()
    {
        // Arrange
        var inner = new Subject<int>();
        var outer = new BehaviorSubject<IObservable<int>?>(inner);
        var completed = false;

        // Act
        using var subscription = outer.SwitchSubscribe(_ => { }, _ => { }, () => completed = true);
        inner.OnCompleted(); // Complete the inner observable first
        outer.OnCompleted(); // Then complete the outer observable

        // Assert
        await Assert.That(completed).IsTrue();
    }

    /// <summary>
    ///     Verifies that SwitchSelect throws when the source is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        const IObservable<string?> source = null!;

        // Act & Assert
        await Assert.That(() => source.SwitchSelect(_ => Observable.Return(1)))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSelect throws when the selector is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_ShouldThrowArgumentNullException_WhenSelectorIsNull()
    {
        // Arrange
        var source = Observable.Return("test");

        // Act & Assert
        await Assert.That(() => source.SwitchSelect<string, int>(null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSelect projects the source and switches to the projected observable.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_ShouldProjectAndSwitch()
    {
        // Arrange
        const int SecondSubjectValue = 10;
        var subject1 = new BehaviorSubject<int>(1);
        var subject2 = new BehaviorSubject<int>(SecondSubjectValue);
        var outer = new BehaviorSubject<TestViewModel?>(new() { Observable = subject1 });
        var values = new List<int>();

        // Act
        using var subscription = outer.SwitchSelect(vm => vm.Observable).Subscribe(values.Add);

        outer.OnNext(new() { Observable = subject2 });

        // Assert
        await Assert.That(values).IsEquivalentTo([1, SecondSubjectValue]);
    }

    /// <summary>
    ///     Verifies that SwitchSelect ignores null source values.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_ShouldIgnoreNullValues()
    {
        // Arrange
        var subject = new BehaviorSubject<int>(1);
        var outer = new BehaviorSubject<TestViewModel?>(new() { Observable = subject });
        var values = new List<int>();

        using var subscription = outer.SwitchSelect(vm => vm.Observable).Subscribe(values.Add);

        // Act
        outer.OnNext(null);

        // Assert - Should not crash and should only have first value
        await Assert.That(values).IsEquivalentTo([1]);
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe with a selector throws when any parameter is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithSelector_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var source = Observable.Return(new TestViewModel { Observable = Observable.Return(1) });
        Func<TestViewModel, IObservable<int>> selector = vm => vm.Observable;
        Action<int> onNext = _ => { };

        // Act & Assert
        await Assert.That(() => ((IObservable<TestViewModel?>)null!).SwitchSubscribe(selector, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(null!, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe with a selector projects and subscribes to the projected observable.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithSelector_ShouldProjectAndSubscribe()
    {
        // Arrange
        var subject = new BehaviorSubject<int>(1);
        var outer = new BehaviorSubject<TestViewModel?>(new() { Observable = subject });
        var values = new List<int>();

        const int SecondValue = 2;

        // Act
        using var subscription = outer.SwitchSubscribe(vm => vm.Observable, values.Add);
        subject.OnNext(SecondValue);

        // Assert
        await Assert.That(values).IsEquivalentTo([1, SecondValue]);
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe with a selector and handlers throws when any parameter is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithSelectorAndHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var source = Observable.Return(new TestViewModel { Observable = Observable.Return(1) });
        Func<TestViewModel, IObservable<int>> selector = vm => vm.Observable;
        Action<int> onNext = _ => { };
        Action<Exception> onError = _ => { };
        var onCompleted = () => { };

        // Act & Assert
        await Assert.That(() =>
                ((IObservable<TestViewModel?>)null!).SwitchSubscribe(selector, onNext, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(null!, onNext, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, null!, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, onNext, null!, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, onNext, onError, null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on a command observable throws when the source is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Command_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        const IObservable<IReactiveCommand<int, int>?> source = null!;

        // Act & Assert
        await Assert.That(() => source.SwitchSubscribe(_ => { }))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on a command observable throws when the onNext handler is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Command_ShouldThrowArgumentNullException_WhenOnNextIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return<IReactiveCommand<int, int>?>(command);

        // Act & Assert
        await Assert.That(() => source.SwitchSubscribe(null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on a command observable receives command results.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Command_ShouldReceiveCommandResults()
    {
        // Arrange
        const int ExpectedResult = CommandInput * DoubleMultiplier;
        var command = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command);
        var results = new List<int>();

        // Act
        using var subscription = outer.SwitchSubscribe(results.Add);
        await command.Execute(CommandInput);

        // Assert
        await Assert.That(results).IsEquivalentTo([ExpectedResult]);
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on a command observable switches to a newly emitted command.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Command_ShouldSwitchToNewCommand()
    {
        // Arrange
        const int FirstExpectedResult = CommandInput * DoubleMultiplier;
        const int SecondExpectedResult = CommandInput * TripleMultiplier;
        var command1 = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var command2 = ReactiveCommand.Create<int, int>(x => x * TripleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command1);
        var results = new List<int>();

        using var subscription = outer.SwitchSubscribe(results.Add);
        await command1.Execute(CommandInput);

        // Act - Switch to new command
        outer.OnNext(command2);
        await command2.Execute(CommandInput);

        // Assert
        await Assert.That(results).IsEquivalentTo([FirstExpectedResult, SecondExpectedResult]);
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on a command observable ignores null commands.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Command_ShouldIgnoreNullCommands()
    {
        // Arrange
        const int ExpectedResult = CommandInput * DoubleMultiplier;
        var command = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command);
        var results = new List<int>();

        using var subscription = outer.SwitchSubscribe(results.Add);
        await command.Execute(CommandInput);

        // Act
        outer.OnNext(null);

        // Assert - Should not crash
        await Assert.That(results).IsEquivalentTo([ExpectedResult]);
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on a command observable with handlers throws when any parameter is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_CommandWithHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return<IReactiveCommand<int, int>?>(command);
        Action<int> onNext = _ => { };
        Action<Exception> onError = _ => { };
        var onCompleted = () => { };

        // Act & Assert
        await Assert.That(() =>
                ((IObservable<IReactiveCommand<int, int>?>)null!).SwitchSubscribe(onNext, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(null!, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(onNext, null!, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(onNext, onError, null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSelect on a command observable throws when the source is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_Command_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        const IObservable<IReactiveCommand<int, int>?> source = null!;

        // Act & Assert
        await Assert.That(() => source.SwitchSelect(cmd => cmd.IsExecuting))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSelect on a command observable throws when the selector is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_Command_ShouldThrowArgumentNullException_WhenSelectorIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return<IReactiveCommand<int, int>?>(command);

        // Act & Assert
        await Assert.That(() => source.SwitchSelect<int, int, bool>(null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSelect on a command observable projects a command property.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_Command_ShouldProjectCommandProperty()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command);
        var isExecutingValues = new List<bool>();

        // Act
        using var subscription = outer.SwitchSelect(cmd => cmd.IsExecuting).Subscribe(isExecutingValues.Add);

        await command.Execute(CommandInput);

        // Assert - Should have received at least the initial false value
        await Assert.That(isExecutingValues).IsNotEmpty();
        await Assert.That(isExecutingValues[0]).IsFalse();
    }

    /// <summary>
    ///     Verifies that SwitchSelect on a command observable switches to a new command property.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_Command_ShouldSwitchToNewCommandProperty()
    {
        // Arrange
        const int MinimumValueCount = 2;
        var command1 = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var command2 = ReactiveCommand.Create<int, int>(x => x * TripleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command1);
        var canExecuteValues = new List<bool>();

        using var subscription = outer.SwitchSelect(cmd => cmd.CanExecute).Subscribe(canExecuteValues.Add);

        // Act - Switch to new command
        outer.OnNext(command2);

        // Assert - Should receive initial values from both commands
        await Assert.That(canExecuteValues).Count().IsGreaterThanOrEqualTo(MinimumValueCount);
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on a command observable with a selector throws when any parameter is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_CommandWithSelector_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return<IReactiveCommand<int, int>?>(command);
        Func<IReactiveCommand<int, int>, IObservable<bool>> selector = cmd => cmd.IsExecuting;
        Action<bool> onNext = _ => { };

        // Act & Assert
        await Assert.That(() => ((IObservable<IReactiveCommand<int, int>?>)null!).SwitchSubscribe(selector, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(null!, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on a command observable with a selector projects and subscribes.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_CommandWithSelector_ShouldProjectAndSubscribe()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command);
        var isExecutingValues = new List<bool>();

        // Act
        using var subscription = outer.SwitchSubscribe(cmd => cmd.IsExecuting, isExecutingValues.Add);
        await command.Execute(CommandInput);

        // Assert
        await Assert.That(isExecutingValues).IsNotEmpty();
    }

    /// <summary>
    ///     Verifies that SwitchSubscribe on a command observable with a selector and handlers throws when any parameter is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task
        SwitchSubscribe_CommandWithSelectorAndHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * DoubleMultiplier, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return<IReactiveCommand<int, int>?>(command);
        Func<IReactiveCommand<int, int>, IObservable<bool>> selector = cmd => cmd.IsExecuting;
        Action<bool> onNext = _ => { };
        Action<Exception> onError = _ => { };
        var onCompleted = () => { };

        // Act & Assert
        await Assert.That(() =>
                ((IObservable<IReactiveCommand<int, int>?>)null!).SwitchSubscribe(
                    selector,
                    onNext,
                    onError,
                    onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(null!, onNext, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, null!, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, onNext, null!, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, onNext, onError, null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     A test view model exposing an observable used to verify switch operators.
    /// </summary>
    private sealed class TestViewModel
    {
        /// <summary>
        ///     Gets or sets the observable projected by the switch operators.
        /// </summary>
        public IObservable<int> Observable { get; set; } = System.Reactive.Linq.Observable.Return(0);
    }
}
