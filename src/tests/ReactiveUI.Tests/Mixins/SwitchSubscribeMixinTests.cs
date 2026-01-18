// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Subjects;

namespace ReactiveUI.Tests.Mixins;

/// <summary>
///     Tests for SwitchSubscribeMixin extension methods that handle switching between observables.
/// </summary>
public class SwitchSubscribeMixinTests
{
    [Test]
    public async Task SwitchSubscribe_Observable_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        IObservable<IObservable<int>?> source = null!;

        // Act & Assert
        await Assert.That(() => source.SwitchSubscribe(_ => { }))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSubscribe_Observable_ShouldThrowArgumentNullException_WhenOnNextIsNull()
    {
        // Arrange
        var source = Observable.Return(Observable.Return(1));

        // Act & Assert
        await Assert.That(() => source.SwitchSubscribe(null!))
            .Throws<ArgumentException>();
    }

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

    [Test]
    public async Task SwitchSubscribe_Observable_ShouldSwitchToNewObservable()
    {
        // Arrange
        var inner1 = new BehaviorSubject<int>(1);
        var inner2 = new BehaviorSubject<int>(10);
        var outer = new BehaviorSubject<IObservable<int>?>(inner1);
        var values = new List<int>();

        using var subscription = outer.SwitchSubscribe(values.Add);

        // Act - Switch to new observable
        outer.OnNext(inner2);

        // Assert - Should receive value from new observable
        await Assert.That(values).IsEquivalentTo([1, 10]);
    }

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

    [Test]
    public async Task SwitchSubscribe_WithHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var source = Observable.Return(Observable.Return(1));
        Action<int> onNext = _ => { };
        Action<Exception> onError = _ => { };
        Action onCompleted = () => { };

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

    [Test]
    public async Task SwitchSelect_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        IObservable<string?> source = null!;

        // Act & Assert
        await Assert.That(() => source.SwitchSelect(_ => Observable.Return(1)))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSelect_ShouldThrowArgumentNullException_WhenSelectorIsNull()
    {
        // Arrange
        var source = Observable.Return("test");

        // Act & Assert
        await Assert.That(() => source.SwitchSelect<string, int>(null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSelect_ShouldProjectAndSwitch()
    {
        // Arrange
        var subject1 = new BehaviorSubject<int>(1);
        var subject2 = new BehaviorSubject<int>(10);
        var outer = new BehaviorSubject<TestViewModel?>(new TestViewModel { Observable = subject1 });
        var values = new List<int>();

        // Act
        using var subscription = outer
            .SwitchSelect(vm => vm.Observable)
            .Subscribe(values.Add);

        outer.OnNext(new TestViewModel { Observable = subject2 });

        // Assert
        await Assert.That(values).IsEquivalentTo([1, 10]);
    }

    [Test]
    public async Task SwitchSelect_ShouldIgnoreNullValues()
    {
        // Arrange
        var subject = new BehaviorSubject<int>(1);
        var outer = new BehaviorSubject<TestViewModel?>(new TestViewModel { Observable = subject });
        var values = new List<int>();

        using var subscription = outer
            .SwitchSelect(vm => vm.Observable)
            .Subscribe(values.Add);

        // Act
        outer.OnNext(null);

        // Assert - Should not crash and should only have first value
        await Assert.That(values).IsEquivalentTo([1]);
    }

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
        await Assert.That(() => source.SwitchSubscribe<TestViewModel, int>(null!, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSubscribe_WithSelector_ShouldProjectAndSubscribe()
    {
        // Arrange
        var subject = new BehaviorSubject<int>(1);
        var outer = new BehaviorSubject<TestViewModel?>(new TestViewModel { Observable = subject });
        var values = new List<int>();

        // Act
        using var subscription = outer.SwitchSubscribe(vm => vm.Observable, values.Add);
        subject.OnNext(2);

        // Assert
        await Assert.That(values).IsEquivalentTo([1, 2]);
    }

    [Test]
    public async Task SwitchSubscribe_WithSelectorAndHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var source = Observable.Return(new TestViewModel { Observable = Observable.Return(1) });
        Func<TestViewModel, IObservable<int>> selector = vm => vm.Observable;
        Action<int> onNext = _ => { };
        Action<Exception> onError = _ => { };
        Action onCompleted = () => { };

        // Act & Assert
        await Assert.That(() => ((IObservable<TestViewModel?>)null!).SwitchSubscribe(selector, onNext, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe<TestViewModel, int>(null!, onNext, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, null!, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, onNext, null!, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, onNext, onError, null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSubscribe_Command_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        IObservable<IReactiveCommand<int, int>?> source = null!;

        // Act & Assert
        await Assert.That(() => source.SwitchSubscribe(_ => { }))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSubscribe_Command_ShouldThrowArgumentNullException_WhenOnNextIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return<IReactiveCommand<int, int>?>(command);

        // Act & Assert
        await Assert.That(() => source.SwitchSubscribe(null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSubscribe_Command_ShouldReceiveCommandResults()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command);
        var results = new List<int>();

        // Act
        using var subscription = outer.SwitchSubscribe(results.Add);
        await command.Execute(5);

        // Assert
        await Assert.That(results).IsEquivalentTo([10]);
    }

    [Test]
    public async Task SwitchSubscribe_Command_ShouldSwitchToNewCommand()
    {
        // Arrange
        var command1 = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var command2 = ReactiveCommand.Create<int, int>(x => x * 3, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command1);
        var results = new List<int>();

        using var subscription = outer.SwitchSubscribe(results.Add);
        await command1.Execute(5);

        // Act - Switch to new command
        outer.OnNext(command2);
        await command2.Execute(5);

        // Assert
        await Assert.That(results).IsEquivalentTo([10, 15]);
    }

    [Test]
    public async Task SwitchSubscribe_Command_ShouldIgnoreNullCommands()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command);
        var results = new List<int>();

        using var subscription = outer.SwitchSubscribe(results.Add);
        await command.Execute(5);

        // Act
        outer.OnNext(null);

        // Assert - Should not crash
        await Assert.That(results).IsEquivalentTo([10]);
    }

    [Test]
    public async Task SwitchSubscribe_CommandWithHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return<IReactiveCommand<int, int>?>(command);
        Action<int> onNext = _ => { };
        Action<Exception> onError = _ => { };
        Action onCompleted = () => { };

        // Act & Assert
        await Assert.That(() => ((IObservable<IReactiveCommand<int, int>?>)null!).SwitchSubscribe(onNext, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(null!, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(onNext, null!, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(onNext, onError, null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSelect_Command_ShouldThrowArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        IObservable<IReactiveCommand<int, int>?> source = null!;

        // Act & Assert
        await Assert.That(() => source.SwitchSelect(cmd => cmd.IsExecuting))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSelect_Command_ShouldThrowArgumentNullException_WhenSelectorIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return<IReactiveCommand<int, int>?>(command);

        // Act & Assert
        await Assert.That(() => source.SwitchSelect<int, int, bool>(null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSelect_Command_ShouldProjectCommandProperty()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command);
        var isExecutingValues = new List<bool>();

        // Act
        using var subscription = outer
            .SwitchSelect(cmd => cmd.IsExecuting)
            .Subscribe(isExecutingValues.Add);

        await command.Execute(5);

        // Assert - Should have received at least the initial false value
        await Assert.That(isExecutingValues).IsNotEmpty();
        await Assert.That(isExecutingValues[0]).IsFalse();
    }

    [Test]
    public async Task SwitchSelect_Command_ShouldSwitchToNewCommandProperty()
    {
        // Arrange
        var command1 = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var command2 = ReactiveCommand.Create<int, int>(x => x * 3, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command1);
        var canExecuteValues = new List<bool>();

        using var subscription = outer
            .SwitchSelect(cmd => cmd.CanExecute)
            .Subscribe(canExecuteValues.Add);

        // Act - Switch to new command
        outer.OnNext(command2);

        // Assert - Should receive initial values from both commands
        await Assert.That(canExecuteValues).Count().IsGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task SwitchSubscribe_CommandWithSelector_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return<IReactiveCommand<int, int>?>(command);
        Func<IReactiveCommand<int, int>, IObservable<bool>> selector = cmd => cmd.IsExecuting;
        Action<bool> onNext = _ => { };

        // Act & Assert
        await Assert.That(() => ((IObservable<IReactiveCommand<int, int>?>)null!).SwitchSubscribe(selector, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe<int, int, bool>(null!, onNext))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SwitchSubscribe_CommandWithSelector_ShouldProjectAndSubscribe()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var outer = new BehaviorSubject<IReactiveCommand<int, int>?>(command);
        var isExecutingValues = new List<bool>();

        // Act
        using var subscription = outer.SwitchSubscribe(cmd => cmd.IsExecuting, isExecutingValues.Add);
        await command.Execute(5);

        // Assert
        await Assert.That(isExecutingValues).IsNotEmpty();
    }

    [Test]
    public async Task SwitchSubscribe_CommandWithSelectorAndHandlers_ShouldThrowArgumentNullException_WhenAnyParameterIsNull()
    {
        // Arrange
        var command = ReactiveCommand.Create<int, int>(x => x * 2, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return<IReactiveCommand<int, int>?>(command);
        Func<IReactiveCommand<int, int>, IObservable<bool>> selector = cmd => cmd.IsExecuting;
        Action<bool> onNext = _ => { };
        Action<Exception> onError = _ => { };
        Action onCompleted = () => { };

        // Act & Assert
        await Assert.That(() => ((IObservable<IReactiveCommand<int, int>?>)null!).SwitchSubscribe(selector, onNext, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe<int, int, bool>(null!, onNext, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, null!, onError, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, onNext, null!, onCompleted))
            .Throws<ArgumentException>();
        await Assert.That(() => source.SwitchSubscribe(selector, onNext, onError, null!))
            .Throws<ArgumentException>();
    }

    private sealed class TestViewModel
    {
        public IObservable<int> Observable { get; set; } = System.Reactive.Linq.Observable.Return(0);
    }
}
