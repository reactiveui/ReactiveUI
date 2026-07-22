// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mixins;

/// <summary>Tests for SwitchSubscribeMixins extension methods that handle switching between observables.</summary>
public class SwitchSubscribeMixinTests
{
    /// <summary>The multiplier used to double values in the tests.</summary>
    private const int DoubleMultiplier = 2;

    /// <summary>The multiplier used to triple values in the tests.</summary>
    private const int TripleMultiplier = 3;

    /// <summary>The input value supplied to commands in the tests.</summary>
    private const int CommandInput = 5;

    /// <summary>Verifies that SwitchSubscribe on an observable throws when the source is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        const IObservable<IObservable<int>?> source = null!;

        // Act & Assert
        await Assert.That(static () => source.SwitchSubscribe(static _ => { }))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that SwitchSubscribe on an observable throws when the onNext handler is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldThrowArgumentNullException_WhenOnNextIsNull()
    {
        // Arrange
        var source = Signal.Emit(Signal.Emit(1));

        // Act & Assert
        await Assert.That(() => source.SwitchSubscribe(null!))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that SwitchSubscribe on an observable receives values from the inner observable.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldReceiveValues()
    {
        // Arrange
        var inner = new BehaviorSignal<int>(1);
        var outer = new BehaviorSignal<IObservable<int>?>(inner);
        var values = new List<int>();

        // Act
        using var subscription = outer.SwitchSubscribe(values.Add);

        // Assert
        await Assert.That(values).IsEquivalentTo([1]);
    }

    /// <summary>Verifies that SwitchSubscribe on an observable switches to a newly emitted inner observable.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldSwitchToNewObservable()
    {
        // Arrange
        const int SecondInnerValue = 10;
        var inner1 = new BehaviorSignal<int>(1);
        var inner2 = new BehaviorSignal<int>(SecondInnerValue);
        var outer = new BehaviorSignal<IObservable<int>?>(inner1);
        var values = new List<int>();

        using var subscription = outer.SwitchSubscribe(values.Add);

        // Act - Switch to new observable
        outer.OnNext(inner2);

        // Assert - Should receive value from new observable
        await Assert.That(values).IsEquivalentTo([1, SecondInnerValue]);
    }

    /// <summary>Verifies that SwitchSubscribe on an observable ignores null inner observables.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldIgnoreNullObservables()
    {
        // Arrange
        var inner = new BehaviorSignal<int>(1);
        var outer = new BehaviorSignal<IObservable<int>?>(inner);
        var values = new List<int>();

        using var subscription = outer.SwitchSubscribe(values.Add);

        // Act - Emit null
        outer.OnNext(null);

        // Assert - Should not crash and should still have first value
        await Assert.That(values).IsEquivalentTo([1]);
    }

    /// <summary>Verifies that SwitchSubscribe with handlers throws when any parameter is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var source = Signal.Emit(Signal.Emit(1));
        Action<int> onNext = static _ => { };
        Action<Exception> onError = static _ => { };
        var onCompleted = static () => { };

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

    /// <summary>Verifies that SwitchSubscribe with handlers invokes the onError handler.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithHandlers_ShouldCallOnError()
    {
        // Arrange
        var inner = new Signal<int>();
        var outer = Signal.Emit<IObservable<int>?>(inner);
        Exception? capturedError = null;

        // Act
        using var subscription = outer.SwitchSubscribe(static _ => { }, ex => capturedError = ex, static () => { });
        inner.OnError(new InvalidOperationException("test error"));

        // Assert
        await Assert.That(capturedError).IsNotNull();
        await Assert.That(capturedError).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>Verifies that SwitchSubscribe with handlers invokes the onCompleted handler.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithHandlers_ShouldCallOnCompleted()
    {
        // Arrange
        var inner = new Signal<int>();
        var outer = new BehaviorSignal<IObservable<int>?>(inner);
        var completed = false;

        // Act
        using var subscription = outer.SwitchSubscribe(static _ => { }, static _ => { }, () => completed = true);
        inner.OnCompleted(); // Complete the inner observable first
        outer.OnCompleted(); // Then complete the outer observable

        // Assert
        await Assert.That(completed).IsTrue();
    }

    /// <summary>Verifies that SwitchSelect throws when the source is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        const IObservable<string?> source = null!;

        // Act & Assert
        await Assert.That(static () => SwitchSubscribeMixins.SwitchSelect(source, static _ => Signal.Emit(1)))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that SwitchSelect throws when the selector is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_ShouldThrowArgumentNullException_WhenSelectorIsNull()
    {
        // Arrange
        var source = Signal.Emit("test");

        // Act & Assert
        await Assert.That(() => SwitchSubscribeMixins.SwitchSelect<string, int>(source, null!))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that SwitchSelect projects the source and switches to the projected observable.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_ShouldProjectAndSwitch()
    {
        // Arrange
        const int SecondSubjectValue = 10;
        var subject1 = new BehaviorSignal<int>(1);
        var subject2 = new BehaviorSignal<int>(SecondSubjectValue);
        var outer = new BehaviorSignal<TestViewModel?>(new() { Observable = subject1 });
        var values = new List<int>();

        // Act
        using var subscription = SwitchSubscribeMixins.SwitchSelect(outer, static vm => vm.Observable).Subscribe(values.Add);

        outer.OnNext(new() { Observable = subject2 });

        // Assert
        await Assert.That(values).IsEquivalentTo([1, SecondSubjectValue]);
    }

    /// <summary>Verifies that SwitchSelect ignores null source values.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_ShouldIgnoreNullValues()
    {
        // Arrange
        var subject = new BehaviorSignal<int>(1);
        var outer = new BehaviorSignal<TestViewModel?>(new() { Observable = subject });
        var values = new List<int>();

        using var subscription = SwitchSubscribeMixins.SwitchSelect(outer, static vm => vm.Observable).Subscribe(values.Add);

        // Act
        outer.OnNext(null);

        // Assert - Should not crash and should only have first value
        await Assert.That(values).IsEquivalentTo([1]);
    }

    /// <summary>Verifies that SwitchSubscribe with a selector throws when any parameter is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithSelector_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var source = Signal.Emit(new TestViewModel { Observable = Signal.Emit(1) });
        Func<TestViewModel, IObservable<int>> selector = static vm => vm.Observable;
        Action<int> onNext = static _ => { };

        // Act & Assert
        await Assert.That(() => ((IObservable<TestViewModel?>)null!).SwitchSubscribe(selector, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(null!, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, null!))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that SwitchSubscribe with a selector projects and subscribes to the projected observable.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithSelector_ShouldProjectAndSubscribe()
    {
        // Arrange
        var subject = new BehaviorSignal<int>(1);
        var outer = new BehaviorSignal<TestViewModel?>(new() { Observable = subject });
        var values = new List<int>();

        const int SecondValue = 2;

        // Act
        using var subscription = outer.SwitchSubscribe(static vm => vm.Observable, values.Add);
        subject.OnNext(SecondValue);

        // Assert
        await Assert.That(values).IsEquivalentTo([1, SecondValue]);
    }

    /// <summary>Verifies that SwitchSubscribe with a selector and handlers throws when any parameter is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_WithSelectorAndHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var source = Signal.Emit(new TestViewModel { Observable = Signal.Emit(1) });
        Func<TestViewModel, IObservable<int>> selector = static vm => vm.Observable;
        Action<int> onNext = static _ => { };
        Action<Exception> onError = static _ => { };
        var onCompleted = static () => { };

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

    /// <summary>Verifies that SwitchSubscribe on a command observable throws when the source is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Command_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        const IObservable<IReactiveCommand<int, int>?> source = null!;

        // Act & Assert
        await Assert.That(static () => source.SwitchSubscribe(static _ => { }))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that SwitchSubscribe on a command observable throws when the onNext handler is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Command_ShouldThrowArgumentNullException_WhenOnNextIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var source = Signal.Emit<IReactiveCommand<int, int>?>(command);

        // Act & Assert
        await Assert.That(() => source.SwitchSubscribe(null!))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that SwitchSubscribe on a command observable receives command results.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Command_ShouldReceiveCommandResults()
    {
        // Arrange
        const int ExpectedResult = CommandInput * DoubleMultiplier;
        var command = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var outer = new BehaviorSignal<IReactiveCommand<int, int>?>(command);
        var results = new List<int>();

        // Act
        using var subscription = outer.SwitchSubscribe(results.Add);
        await command.Execute(CommandInput);

        // Assert
        await Assert.That(results).IsEquivalentTo([ExpectedResult]);
    }

    /// <summary>Verifies that SwitchSubscribe on a command observable switches to a newly emitted command.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Command_ShouldSwitchToNewCommand()
    {
        // Arrange
        const int FirstExpectedResult = CommandInput * DoubleMultiplier;
        const int SecondExpectedResult = CommandInput * TripleMultiplier;
        var command1 = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var command2 = ReactiveCommand.Create<int, int>(static x => x * TripleMultiplier, outputScheduler: Sequencer.Immediate);
        var outer = new BehaviorSignal<IReactiveCommand<int, int>?>(command1);
        var results = new List<int>();

        using var subscription = outer.SwitchSubscribe(results.Add);
        await command1.Execute(CommandInput);

        // Act - Switch to new command
        outer.OnNext(command2);
        await command2.Execute(CommandInput);

        // Assert
        await Assert.That(results).IsEquivalentTo([FirstExpectedResult, SecondExpectedResult]);
    }

    /// <summary>Verifies that SwitchSubscribe on a command observable ignores null commands.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_Command_ShouldIgnoreNullCommands()
    {
        // Arrange
        const int ExpectedResult = CommandInput * DoubleMultiplier;
        var command = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var outer = new BehaviorSignal<IReactiveCommand<int, int>?>(command);
        var results = new List<int>();

        using var subscription = outer.SwitchSubscribe(results.Add);
        await command.Execute(CommandInput);

        // Act
        outer.OnNext(null);

        // Assert - Should not crash
        await Assert.That(results).IsEquivalentTo([ExpectedResult]);
    }

    /// <summary>Verifies that SwitchSubscribe on a command observable with handlers throws when any parameter is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_CommandWithHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var source = Signal.Emit<IReactiveCommand<int, int>?>(command);
        Action<int> onNext = static _ => { };
        Action<Exception> onError = static _ => { };
        var onCompleted = static () => { };

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

    /// <summary>Verifies that SwitchSelect on a command observable throws when the source is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_Command_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        const IObservable<IReactiveCommand<int, int>?> source = null!;

        // Act & Assert
        await Assert.That(static () => SwitchSubscribeMixins.SwitchSelect(source, static cmd => cmd.IsExecuting))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that SwitchSelect on a command observable throws when the selector is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_Command_ShouldThrowArgumentNullException_WhenSelectorIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var source = Signal.Emit<IReactiveCommand<int, int>?>(command);

        // Act & Assert
        await Assert.That(() => source.SwitchSelect<int, int, bool>(null!))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that SwitchSelect on a command observable projects a command property.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_Command_ShouldProjectCommandProperty()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var outer = new BehaviorSignal<IReactiveCommand<int, int>?>(command);
        var isExecutingValues = new List<bool>();

        // Act
        using var subscription = SwitchSubscribeMixins.SwitchSelect(outer, static cmd => cmd.IsExecuting).Subscribe(isExecutingValues.Add);

        await command.Execute(CommandInput);

        // Assert - Should have received at least the initial false value
        await Assert.That(isExecutingValues).IsNotEmpty();
        await Assert.That(isExecutingValues[0]).IsFalse();
    }

    /// <summary>Verifies that SwitchSelect on a command observable switches to a new command property.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSelect_Command_ShouldSwitchToNewCommandProperty()
    {
        // Arrange
        const int MinimumValueCount = 2;
        var command1 = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var command2 = ReactiveCommand.Create<int, int>(static x => x * TripleMultiplier, outputScheduler: Sequencer.Immediate);
        var outer = new BehaviorSignal<IReactiveCommand<int, int>?>(command1);
        var canExecuteValues = new List<bool>();

        using var subscription = SwitchSubscribeMixins.SwitchSelect(outer, static cmd => cmd.CanExecute).Subscribe(canExecuteValues.Add);

        // Act - Switch to new command
        outer.OnNext(command2);

        // Assert - Should receive initial values from both commands
        await Assert.That(canExecuteValues).Count().IsGreaterThanOrEqualTo(MinimumValueCount);
    }

    /// <summary>Verifies that SwitchSubscribe on a command observable with a selector throws when any parameter is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_CommandWithSelector_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var source = Signal.Emit<IReactiveCommand<int, int>?>(command);
        Func<IReactiveCommand<int, int>, IObservable<bool>> selector = static cmd => cmd.IsExecuting;
        Action<bool> onNext = static _ => { };

        // Act & Assert
        await Assert.That(() => ((IObservable<IReactiveCommand<int, int>?>)null!).SwitchSubscribe(selector, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(null!, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, null!))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that SwitchSubscribe on a command observable with a selector projects and subscribes.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SwitchSubscribe_CommandWithSelector_ShouldProjectAndSubscribe()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var outer = new BehaviorSignal<IReactiveCommand<int, int>?>(command);
        var isExecutingValues = new List<bool>();

        // Act
        using var subscription = outer.SwitchSubscribe(static cmd => cmd.IsExecuting, isExecutingValues.Add);
        await command.Execute(CommandInput);

        // Assert
        await Assert.That(isExecutingValues).IsNotEmpty();
    }

    /// <summary>Verifies that SwitchSubscribe on a command observable with a selector and handlers throws when any parameter is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task
        SwitchSubscribe_CommandWithSelectorAndHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(static x => x * DoubleMultiplier, outputScheduler: Sequencer.Immediate);
        var source = Signal.Emit<IReactiveCommand<int, int>?>(command);
        Func<IReactiveCommand<int, int>, IObservable<bool>> selector = static cmd => cmd.IsExecuting;
        Action<bool> onNext = static _ => { };
        Action<Exception> onError = static _ => { };
        var onCompleted = static () => { };

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

    /// <summary>A test view model exposing an observable used to verify switch operators.</summary>
    private sealed class TestViewModel
    {
        /// <summary>Gets or sets the observable projected by the switch operators.</summary>
        public IObservable<int> Observable { get; set; } = Signal.Emit(0);
    }
}
