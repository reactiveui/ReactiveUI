// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Globalization;
using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests targeting the "missed update" race between <c>WhenAnyValue</c>'s initial value read and the <see cref="INotifyPropertyChanged.PropertyChanged"/> handler attachment.</summary>
/// <remarks>
/// In <c>ExpressionChainSink.Sink.Level.SetParent</c>, the subscribing thread (1) reads the current
/// property value via the cached getter and emits it downstream, then (2) subscribes to
/// <c>PropertyChanged</c>. Any mutation that fires <c>PropertyChanged</c> between steps (1) and (2)
/// runs against an empty subscriber list and is silently lost, leaving downstream stuck on the
/// pre-mutation value until the next mutation.
/// </remarks>
public class WhenAnyValueSubscribeRaceTests
{
    /// <summary>The property value written mid-race to prove the update was not lost.</summary>
    private const string RacedWord = "raced";

    /// <summary>
    /// Deterministically reproduces the missed-update race using a synchronous wedge: the subscriber's
    /// initial <c>OnNext</c> mutates the source property before <c>WhenAnyValue</c> has had a chance to
    /// attach its <c>PropertyChanged</c> handler. With the bug present, the new value is never observed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_MutationBetweenInitialEmitAndHandlerAttach_IsLost()
    {
        var fixture = new TestFixture { IsOnlyOneWord = "initial" };
        var values = new List<string?>();

        // Mutate during the initial emission. We are still inside ExpressionChainSink's SetParent,
        // between the value read (already done) and the PropertyChanged handler attachment (not
        // yet done). The PropertyChanged event raised by this setter has no subscriber and is lost.
        using var subscription = fixture.WhenAnyValue(x => x.IsOnlyOneWord).Subscribe(value =>
        {
            values.Add(value);
            if (values.Count != 1)
            {
                return;
            }

            fixture.IsOnlyOneWord = RacedWord;
        });

        // Sanity check: the property actually holds the racing value.
        await Assert.That(fixture.IsOnlyOneWord).IsEqualTo(RacedWord);

        // The subscriber must eventually see the racing value: either because the initial read
        // captured it, or because the PropertyChanged handler picked it up. With the bug, the
        // handler was attached after the event fired, so neither path delivered the update.
        await Assert.That(values).Contains(RacedWord);
    }

    /// <summary>
    /// Same race, but using a hand-rolled <see cref="INotifyPropertyChanged"/> source rather than
    /// <see cref="ReactiveObject"/>, to confirm the bug is in the chain sink (subscriber-side) and
    /// not in any <see cref="ReactiveObject"/> specifics.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_MutationBetweenInitialEmitAndHandlerAttach_IsLost_PlainInpc()
    {
        var notifier = new PlainInpc { Value = 1 };
        var values = new List<int>();

        const int RacedValue = 2;
        using var subscription = notifier.WhenAnyValue(x => x.Value).Subscribe(value =>
        {
            values.Add(value);
            if (values.Count != 1)
            {
                return;
            }

            notifier.Value = RacedValue;
        });

        await Assert.That(notifier.Value).IsEqualTo(RacedValue);
        await Assert.That(values).Contains(RacedValue);
    }

    /// <summary>
    /// Multi-threaded stress test that proves the race condition itself, not just the underlying
    /// ordering defect. One mutator thread writes the property a fixed number of times in a tight
    /// loop while the main thread subscribes via <c>WhenAnyValue</c>. In every iteration, after both
    /// threads have finished, the subscriber's last observed value must equal the property's final
    /// value. Any divergence means a real <c>PropertyChanged</c> raised on the mutator thread fired
    /// during the main thread's read-then-subscribe window and was dropped on the floor.
    /// </summary>
    /// <remarks>
    /// Without the fix in <c>ExpressionChainSink.Level.SetParent</c>, this test fails reliably (on
    /// the order of 5-10% of iterations drop the final mutation). With the fix, the handler is
    /// attached before the kicker read, the mutator's <c>PropertyChanged</c> invocation runs the
    /// handler on the mutator's thread, the handler blocks on <c>sink._gate</c> until the main
    /// thread releases it, and then re-emits the post-mutation value. Every iteration converges.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_ConcurrentMutationDuringSubscribe_NeverLosesFinalValue_Stress()
    {
        const int iterations = 2_000;
        const int mutationsPerIteration = 32;
        var divergences = new List<string>();

        for (var i = 0; i < iterations; i++)
        {
            var fixture = new TestFixture { IsOnlyOneWord = "v0" };
            using var mutatorReady = new ManualResetEventSlim(false);
            using var mutatorDone = new ManualResetEventSlim(false);

            var mutator = new Thread(() =>
            {
                mutatorReady.Set();
                for (var j = 1; j <= mutationsPerIteration; j++)
                {
                    fixture.IsOnlyOneWord = $"v{j.ToString(CultureInfo.InvariantCulture)}";
                }

                mutatorDone.Set();
            })
            { IsBackground = true };
            mutator.Start();
            mutatorReady.Wait();

            string? latest = null;
            using (fixture.WhenAnyValue(x => x.IsOnlyOneWord).Subscribe(v => latest = v))
            {
                mutatorDone.Wait();
                mutator.Join();
            }

            var finalProperty = fixture.IsOnlyOneWord;
            if (!string.Equals(latest, finalProperty, StringComparison.Ordinal))
            {
                divergences.Add($"iter {i}: latest='{latest}' property='{finalProperty}'");
            }
        }

        await Assert.That(divergences).IsEmpty();
    }

    /// <summary>A minimal hand-rolled <see cref="INotifyPropertyChanged"/> source used to isolate the chain sink's behaviour from <see cref="ReactiveObject"/>.</summary>
    private sealed class PlainInpc : INotifyPropertyChanged
    {
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Gets or sets the value, raising <see cref="PropertyChanged"/> only when it actually changes.</summary>
        public int Value
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }

                field = value;
                PropertyChanged?.Invoke(this, new(nameof(Value)));
            }
        }
    }
}
