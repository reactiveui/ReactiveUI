// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Testing;

/// <summary>
/// Test Sequencer.
/// </summary>
/// <seealso cref="IDisposable" />
public class TestSequencer : IDisposable
{
    private readonly Barrier _phaseSync;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestSequencer"/> class.
    /// </summary>
    public TestSequencer() => _phaseSync = new(2);

    /// <summary>
    /// Gets the number of completed phases.
    /// </summary>
    /// <value>
    /// The completed phases.
    /// </value>
    public long CompletedPhases => _phaseSync.CurrentPhaseNumber;

    /// <summary>
    /// Gets the current phase.
    /// </summary>
    /// <value>
    /// The current phase.
    /// </value>
    public long CurrentPhase { get; private set; }

    /// <summary>
    /// Advances this phase instance.
    /// </summary>
    /// <param name="comment">The comment for Test visual identification Purposes only.</param>
    /// <returns>
    /// A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    public Task AdvancePhaseAsync(string comment = "")
    {
        if (_phaseSync.ParticipantCount == _phaseSync.ParticipantsRemaining)
        {
            CurrentPhase = CompletedPhases + 1;
        }

        return Task.Run(() => _phaseSync?.SignalAndWait(CancellationToken.None));
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _phaseSync.Dispose();
            }

            _disposedValue = true;
        }
    }
}
