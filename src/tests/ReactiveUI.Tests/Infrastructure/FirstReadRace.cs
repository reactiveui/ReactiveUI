// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Infrastructure;

/// <summary>Races several threads on the first read of lazily initialized static state.</summary>
/// <remarks>
/// Each round resets the state, then releases every reader thread from one barrier so they all perform the
/// first read together. The round ends when every reader has checked the value it read.
/// </remarks>
internal static class FirstReadRace
{
    /// <summary>The number of rounds a race test runs.</summary>
    internal const int DefaultRounds = 20_000;

    /// <summary>The fewest reader threads a round uses.</summary>
    private const int MinReaders = 2;

    /// <summary>The most reader threads a round uses.</summary>
    private const int MaxReaders = 8;

    /// <summary>Runs the race and counts the reads that returned an invalid value or threw.</summary>
    /// <typeparam name="T">The type of value the read returns.</typeparam>
    /// <param name="reset">Returns the state to uninitialized before each round.</param>
    /// <param name="read">Performs the first read.</param>
    /// <param name="isValid">Decides whether a value a reader saw is valid.</param>
    /// <param name="rounds">The number of rounds to run.</param>
    /// <returns>The number of reads that returned an invalid value or threw.</returns>
    internal static int CountInvalidReads<T>(Action reset, Func<T> read, Func<T, bool> isValid, int rounds)
    {
        ArgumentNullException.ThrowIfNull(reset);
        ArgumentNullException.ThrowIfNull(read);
        ArgumentNullException.ThrowIfNull(isValid);

        var readers = Math.Clamp(Environment.ProcessorCount, MinReaders, MaxReaders);
        var invalidReads = 0;
        var stop = false;

        using Barrier start = new(readers + 1);
        using Barrier finish = new(readers + 1);

        var threads = new Thread[readers];
        for (var i = 0; i < readers; i++)
        {
            threads[i] = new(() =>
            {
                while (true)
                {
                    start.SignalAndWait();
                    if (Volatile.Read(ref stop))
                    {
                        return;
                    }

                    if (!TryRead(read, isValid))
                    {
                        _ = Interlocked.Increment(ref invalidReads);
                    }

                    finish.SignalAndWait();
                }
            }) { IsBackground = true, Name = $"{nameof(FirstReadRace)} reader {i}" };
            threads[i].Start();
        }

        for (var round = 0; round < rounds; round++)
        {
            reset();
            start.SignalAndWait();
            finish.SignalAndWait();
        }

        Volatile.Write(ref stop, true);
        start.SignalAndWait();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return Volatile.Read(ref invalidReads);
    }

    /// <summary>Performs one read and checks the value.</summary>
    /// <typeparam name="T">The type of value the read returns.</typeparam>
    /// <param name="read">Performs the read.</param>
    /// <param name="isValid">Decides whether the value is valid.</param>
    /// <returns><see langword="true"/> when the read returned a valid value; otherwise <see langword="false"/>.</returns>
    private static bool TryRead<T>(Func<T> read, Func<T, bool> isValid)
    {
        try
        {
            return isValid(read());
        }
        catch (Exception)
        {
            return false;
        }
    }
}
