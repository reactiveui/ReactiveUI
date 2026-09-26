// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>
/// A command built directly on <see cref="ReactiveCommandBase{TParam, TResult}"/>, rather than through
/// <c>ReactiveCommand.Create</c>. A realistic reason to derive from the base class instead is adapting an existing,
/// non-reactive command type into ReactiveUI's contract; here it wraps a plain <see cref="Func{TResult}"/> that
/// produces an announcement, and may run only while the front desk is open.
/// </summary>
[System.Diagnostics.DebuggerDisplay("CanExecute = {_canExecuteValue}")]
public sealed class DeskAnnouncementCommand : ReactiveCommandBase<RxVoid, string>
{
    /// <summary>The desk this command announces for.</summary>
    private readonly LibraryDesk _desk;

    /// <summary>Produces the announcement text.</summary>
    private readonly Func<string> _announce;

    /// <summary>Pushes every <see cref="CanExecute"/> change to subscribers.</summary>
    private readonly ScheduledSignal<bool> _canExecuteChanges = new(Sequencer.Immediate);

    /// <summary>Pushes every <see cref="IsExecuting"/> change to subscribers.</summary>
    private readonly ScheduledSignal<bool> _isExecutingChanges = new(Sequencer.Immediate);

    /// <summary>Pushes every exception thrown while announcing.</summary>
    private readonly ScheduledSignal<Exception> _thrownExceptions = new(Sequencer.Immediate);

    /// <summary>Pushes every announcement produced by <see cref="Execute()"/> to command subscribers.</summary>
    private readonly ScheduledSignal<string> _results = new(Sequencer.Immediate);

    /// <summary>Follows the desk's <c>IsOpen</c> property to keep <see cref="CanExecute"/> current.</summary>
    private readonly IDisposable _canExecuteSubscription;

    /// <summary>The current <see cref="CanExecute"/> value, replayed to a new subscriber.</summary>
    private bool _canExecuteValue;

    /// <summary>Latched once this command has been disposed.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="DeskAnnouncementCommand"/> class.</summary>
    /// <param name="desk">The desk the announcement is about.</param>
    /// <param name="announce">Produces the announcement text.</param>
    public DeskAnnouncementCommand(LibraryDesk desk, Func<string> announce)
    {
        _desk = desk;
        _announce = announce;
        _canExecuteValue = desk.IsOpen;
        _canExecuteSubscription = _desk.WhenAnyValue(d => d.IsOpen).Skip(1).Subscribe(isOpen =>
        {
            _canExecuteValue = isOpen;
            OnCanExecuteChanged(isOpen);
            _canExecuteChanges.OnNext(isOpen);
        });
    }

    /// <inheritdoc/>
    public override IObservable<bool> CanExecute => Signal.Concat(Signal.Emit(_canExecuteValue), _canExecuteChanges);

    /// <inheritdoc/>
    public override IObservable<bool> IsExecuting => Signal.Concat(Signal.Emit(false), _isExecutingChanges);

    /// <inheritdoc/>
    public override IObservable<Exception> ThrownExceptions => _thrownExceptions;

    /// <inheritdoc/>
    public override IObservable<string> Execute() => Execute(RxVoid.Default);

    /// <inheritdoc/>
    public override IObservable<string> Execute(RxVoid parameter)
    {
        _isExecutingChanges.OnNext(true);
        try
        {
            string announcement = _announce();
            _results.OnNext(announcement);
            return Signal.Emit(announcement);
        }
        catch (Exception ex)
        {
            _thrownExceptions.OnNext(ex);
            return Signal.Fail<string>(ex);
        }
        finally
        {
            _isExecutingChanges.OnNext(false);
        }
    }

    /// <inheritdoc/>
    public override IDisposable Subscribe(IObserver<string> observer) => _results.Subscribe(observer);

    /// <summary>Reads <see cref="CanExecute"/>'s current value directly, so an <see cref="System.Windows.Input.ICommand"/> consumer sees it without subscribing.</summary>
    /// <param name="parameter">The <see cref="System.Windows.Input.ICommand"/> parameter; unused, since this command takes none.</param>
    /// <returns><see langword="true"/> while the desk is open.</returns>
    protected override bool ICommandCanExecute(object? parameter) => _canExecuteValue;

    /// <summary>Runs the command from an <see cref="System.Windows.Input.ICommand"/> caller, such as a bound button.</summary>
    /// <param name="parameter">The <see cref="System.Windows.Input.ICommand"/> parameter; unused, since this command takes none.</param>
    protected override void ICommandExecute(object? parameter) => _ = Execute();

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (!disposing || _disposed)
        {
            return;
        }

        _disposed = true;
        _canExecuteSubscription.Dispose();
        _canExecuteChanges.Dispose();
        _isExecutingChanges.Dispose();
        _thrownExceptions.Dispose();
        _results.Dispose();
    }
}
