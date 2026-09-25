// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Globalization;
using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests that <c>WhenAnyValue</c> delivers updates made while its initial value is emitted.</summary>
public class WhenAnyValueSubscribeRaceTests
{
    /// <summary>The property value written mid-race to prove the update was not lost.</summary>
    private const string RacedWord = "raced";

    /// <summary>Mutates the source property during the initial emission and verifies delivery.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_MutationDuringInitialEmit_IsDelivered()
    {
        var fixture = new TestFixture { IsOnlyOneWord = "initial" };
        var values = new List<string?>();

        // Mutate while the observer is delivering its initial value.
        using var subscription = fixture.WhenAnyValue(x => x.IsOnlyOneWord).Subscribe(value =>
        {
            values.Add(value);
            if (values.Count != 1)
            {
                return;
            }

            fixture.IsOnlyOneWord = RacedWord;
        });

        await Assert.That(fixture.IsOnlyOneWord).IsEqualTo(RacedWord);
        await Assert.That(values).Contains(RacedWord);
    }

    /// <summary>Checks delivery with a plain <see cref="INotifyPropertyChanged"/> source.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_MutationDuringInitialEmit_IsDelivered_PlainInpc()
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

    /// <summary>Checks that concurrent mutation during subscription delivers the final value.</summary>
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

    /// <summary>A plain <see cref="INotifyPropertyChanged"/> source for the subscription test.</summary>
    internal sealed class PlainInpc : INotifyPropertyChanged
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
