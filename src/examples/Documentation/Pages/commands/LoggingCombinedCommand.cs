// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>
/// A <see cref="CombinedReactiveCommand{TParam, TResult}"/> that writes a line each time it runs, using the three
/// constructors the base class exposes to a subclass (<c>ReactiveCommand.CreateCombined</c> calls the same
/// constructors, but gives you no way to react to every run).
/// </summary>
/// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
/// <typeparam name="TResult">The type of the command's result.</typeparam>
[System.Diagnostics.DebuggerDisplay("LoggingCombinedCommand")]
public sealed class LoggingCombinedCommand<TParam, TResult> : CombinedReactiveCommand<TParam, TResult>
{
    /// <summary>Watches this command's own <see cref="ReactiveCommandBase{TParam, TResult}.IsExecuting"/> to log every run.</summary>
    private readonly IDisposable _loggingSubscription;

    /// <summary>Initializes a new instance of the <see cref="LoggingCombinedCommand{TParam, TResult}"/> class with a can-execute observable and an output sequencer.</summary>
    /// <param name="childCommands">The child commands the combined command runs.</param>
    /// <param name="canExecute">An observable governing whether the combined command can execute, in addition to its children.</param>
    /// <param name="outputScheduler">The sequencer on which output is delivered.</param>
    public LoggingCombinedCommand(
        IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
        IObservable<bool>? canExecute,
        ISequencer? outputScheduler)
        : base(childCommands, canExecute, outputScheduler) =>
        _loggingSubscription = IsExecuting.Where(static isExecuting => isExecuting)
            .Subscribe(static _ => Console.WriteLine("Combined command running"));

    /// <summary>Initializes a new instance of the <see cref="LoggingCombinedCommand{TParam, TResult}"/> class with a can-execute observable and the default output sequencer.</summary>
    /// <param name="childCommands">The child commands the combined command runs.</param>
    /// <param name="canExecute">An observable governing whether the combined command can execute, in addition to its children.</param>
    public LoggingCombinedCommand(IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands, IObservable<bool>? canExecute)
        : this(childCommands, canExecute, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="LoggingCombinedCommand{TParam, TResult}"/> class with the default can-execute behavior and an output sequencer.</summary>
    /// <param name="childCommands">The child commands the combined command runs.</param>
    /// <param name="outputScheduler">The sequencer on which output is delivered.</param>
    public LoggingCombinedCommand(IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands, ISequencer? outputScheduler)
        : this(childCommands, null, outputScheduler)
    {
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _loggingSubscription.Dispose();
        }

        base.Dispose(disposing);
    }
}
