// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Concurrency;

namespace ReactiveUI.Documentation.Scheduling;

/// <summary>A <see cref="ScheduledSubject{T}"/> that records to a log when its base class releases its resources.</summary>
/// <typeparam name="T">The value type the subject carries.</typeparam>
/// <param name="scheduler">The sequencer that delivers notifications.</param>
/// <param name="disposalLog">The log entries this subject appends to when disposed.</param>
public sealed class LoggingScheduledSubject<T>(ISequencer scheduler, List<string> disposalLog) : ScheduledSubject<T>(scheduler)
{
    /// <inheritdoc/>
    protected override void Dispose(bool isDisposing)
    {
        disposalLog.Add("Disposed");
        base.Dispose(isDisposing);
    }
}
