// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>
/// A <see cref="ReactiveCommand{TParam, TResult}"/> that keeps every parameter it has run with. It passes both of its
/// constructors straight to the base class and records each parameter in <see cref="Execute(TParam)"/>, something
/// the factory methods on <see cref="ReactiveCommand"/> give you no place to do.
/// </summary>
/// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
/// <typeparam name="TResult">The type of the values that are the result of command execution.</typeparam>
[System.Diagnostics.DebuggerDisplay("History.Count = {_history.Count}")]
public sealed class RecordingCommand<TParam, TResult> : ReactiveCommand<TParam, TResult>
{
    /// <summary>Every parameter this command has run with, in order.</summary>
    private readonly List<TParam> _history = [];

    /// <summary>Initializes a new instance of the <see cref="RecordingCommand{TParam, TResult}"/> class for execution logic that produces a result observable.</summary>
    /// <param name="execute">Provides an observable representing the command's execution logic.</param>
    /// <param name="canExecute">An observable governing whether the command can execute.</param>
    /// <param name="outputScheduler">The sequencer on which output is delivered.</param>
    public RecordingCommand(
        Func<TParam, IObservable<TResult>> execute,
        IObservable<bool>? canExecute,
        ISequencer? outputScheduler)
        : base(execute, canExecute, outputScheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordingCommand{TParam, TResult}"/> class for execution logic
    /// that signals cancellation through a callback rather than by unsubscribing.
    /// </summary>
    /// <param name="execute">Provides the result observable and a cancel callback for each execution.</param>
    /// <param name="canExecute">An observable governing whether the command can execute.</param>
    /// <param name="outputScheduler">The sequencer on which output is delivered.</param>
    public RecordingCommand(
        Func<TParam, IObservable<(IObservable<TResult> Result, Action Cancel)>> execute,
        IObservable<bool>? canExecute,
        ISequencer? outputScheduler)
        : base(execute, canExecute, outputScheduler)
    {
    }

    /// <summary>Gets every parameter this command has run with, in order.</summary>
    public IReadOnlyList<TParam> History => _history;

    /// <summary>Records the parameter, then runs the command as usual.</summary>
    /// <param name="parameter">The parameter to run the command with.</param>
    /// <returns>An observable that runs the command when subscribed and delivers its result.</returns>
    public override IObservable<TResult> Execute(TParam parameter)
    {
        _history.Add(parameter);
        return base.Execute(parameter);
    }
}
