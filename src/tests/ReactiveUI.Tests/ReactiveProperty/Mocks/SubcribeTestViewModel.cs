// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Tests.ReactiveProperty.Mocks;

public class SubcribeTestViewModel : IDisposable
{
    private static readonly List<int> _items = [];
    private readonly List<BasicViewModel> _cache = [];
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubcribeTestViewModel" /> class.
    /// </summary>
    /// <param name="count">The count.</param>
    public SubcribeTestViewModel(int count)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (var i = 0; i < count; i++)
        {
            _cache.Add(new BasicViewModel(Property));
        }

        stopwatch.Stop();

        StartupTime = stopwatch.ElapsedMilliseconds;
        SubscriberCount = _cache.Count;
        SubscriberEvents = _items.Count;
    }

    public ReactiveProperty<int> Property { get; } = new(1, ImmediateScheduler.Instance, false, false);

    public int SubscriberCount { get; }

    public int SubscriberEvents { get; }

    public long StartupTime { get; }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue && disposing)
        {
            foreach (var item in _cache)
            {
                item.Dispose();
            }

            _disposedValue = true;
        }
    }

    private class BasicViewModel(IObservable<int> observable) : IDisposable
    {
        private readonly IDisposable _subscription = observable.Subscribe(_items.Add);
        private bool _disposedValue;

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue && disposing)
            {
                _subscription.Dispose();
                _disposedValue = true;
            }
        }
    }
}
