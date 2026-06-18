// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Tests.ReactiveProperties.Mocks;

/// <summary>A view model used to measure subscription behaviour across many reactive property subscribers.</summary>
public sealed class SubcribeTestViewModel : IDisposable
{
    /// <summary>Collects the values received by all subscribers.</summary>
    private static readonly List<int> _items = [];

    /// <summary>Holds the created subscribers so they can be disposed.</summary>
    private readonly List<BasicViewModel> _cache = [];

    /// <summary>Tracks whether the instance has been disposed.</summary>
    private bool _disposedValue;

    /// <summary>Initializes a new instance of the <see cref="SubcribeTestViewModel" /> class.</summary>
    /// <param name="count">The count.</param>
    public SubcribeTestViewModel(int count)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (var i = 0; i < count; i++)
        {
            _cache.Add(new(Property));
        }

        stopwatch.Stop();

        StartupTime = stopwatch.ElapsedMilliseconds;
        SubscriberCount = _cache.Count;
        SubscriberEvents = _items.Count;
    }

    /// <summary>Gets the reactive property that subscribers observe.</summary>
    public ReactiveProperty<int> Property { get; } = new(1, Sequencer.Immediate, false, false);

    /// <summary>Gets the time, in milliseconds, taken to create all subscribers.</summary>
    public long StartupTime { get; }

    /// <summary>Gets the number of subscribers created.</summary>
    public int SubscriberCount { get; }

    /// <summary>Gets the number of subscription events received.</summary>
    public int SubscriberEvents { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposedValue)
        {
            return;
        }

        foreach (var item in _cache)
        {
            item.Dispose();
        }

        _disposedValue = true;
    }

    /// <summary>A simple subscriber that records values from the supplied observable.</summary>
    /// <param name="observable">The observable to subscribe to.</param>
    private sealed class BasicViewModel(IObservable<int> observable) : IDisposable
    {
        /// <summary>The subscription to the observable.</summary>
        private readonly IDisposable _subscription = observable.Subscribe(_items.Add);

        /// <summary>Tracks whether the instance has been disposed.</summary>
        private bool _disposedValue;

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposedValue)
            {
                return;
            }

            _subscription.Dispose();
            _disposedValue = true;
        }
    }
}
