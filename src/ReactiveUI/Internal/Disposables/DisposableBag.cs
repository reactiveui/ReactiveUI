// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// A small composite-disposable replacement specialised for the common 2-slot
/// "subscription + sink" pair found throughout this codebase. Avoids the
/// <see cref="System.Collections.Generic.List{T}"/> backing field of
/// <c>System.Reactive.Disposables.CompositeDisposable</c>. Replaces
/// <c>CompositeDisposable</c> on internal code paths.
/// </summary>
/// <remarks>
/// The first two added entries are stored inline. A third or later entry causes a fall-back
/// to a heap-allocated array. Disposal is idempotent and disposes every contained entry,
/// in registration order, exactly once.
/// </remarks>
internal sealed class DisposableBag : IDisposable
{
    /// <summary>Starting capacity of the overflow array once the two inline slots are taken.</summary>
    private const int OverflowInitialCapacity = 2;

    /// <summary>Growth factor applied when the overflow array is full.</summary>
    private const int OverflowGrowthFactor = 2;

#if NET9_0_OR_GREATER
    /// <summary>Guards the slots, the overflow array, and the disposed flag.</summary>
    private readonly Lock _gate = new();
#else
    /// <summary>Guards the slots, the overflow array, and the disposed flag.</summary>
    private readonly object _gate = new();
#endif

    /// <summary>The first inline disposable slot.</summary>
    private IDisposable? _slot0;

    /// <summary>The second inline disposable slot.</summary>
    private IDisposable? _slot1;

    /// <summary>Heap-allocated overflow for the third and later entries.</summary>
    private IDisposable[]? _overflow;

    /// <summary>The number of entries currently held in <see cref="_overflow"/>.</summary>
    private int _overflowCount;

    /// <summary>Whether the bag has been disposed.</summary>
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableBag"/> class.
    /// </summary>
    public DisposableBag()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableBag"/> class with two pre-populated slots.
    /// </summary>
    /// <param name="first">The first disposable.</param>
    /// <param name="second">The second disposable.</param>
    public DisposableBag(IDisposable first, IDisposable second)
    {
        _slot0 = first;
        _slot1 = second;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableBag"/> class with three pre-populated slots.
    /// </summary>
    /// <param name="first">The first disposable.</param>
    /// <param name="second">The second disposable.</param>
    /// <param name="third">The third disposable.</param>
    public DisposableBag(IDisposable first, IDisposable second, IDisposable third)
    {
        _slot0 = first;
        _slot1 = second;
        _overflow = new IDisposable[OverflowInitialCapacity];
        _overflow[0] = third;
        _overflowCount = 1;
    }

    /// <summary>
    /// Adds a disposable to the bag. If the bag is already disposed, the supplied
    /// disposable is disposed immediately.
    /// </summary>
    /// <param name="disposable">The disposable to add.</param>
    public void Add(IDisposable disposable)
    {
        if (disposable is null)
        {
            return;
        }

        var disposeNow = false;
        lock (_gate)
        {
            if (_disposed)
            {
                disposeNow = true;
            }
            else if (_slot0 is null)
            {
                _slot0 = disposable;
            }
            else if (_slot1 is null)
            {
                _slot1 = disposable;
            }
            else
            {
                if (_overflow is null)
                {
                    _overflow = new IDisposable[OverflowInitialCapacity];
                }
                else if (_overflowCount == _overflow.Length)
                {
                    var grown = new IDisposable[_overflow.Length * OverflowGrowthFactor];
                    Array.Copy(_overflow, grown, _overflowCount);
                    _overflow = grown;
                }

                _overflow[_overflowCount++] = disposable;
            }
        }

        if (!disposeNow)
        {
            return;
        }

        disposable.Dispose();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        IDisposable? s0;
        IDisposable? s1;
        IDisposable[]? overflow;
        int overflowCount;

        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            s0 = _slot0;
            s1 = _slot1;
            overflow = _overflow;
            overflowCount = _overflowCount;
            _slot0 = null;
            _slot1 = null;
            _overflow = null;
            _overflowCount = 0;
        }

        s0?.Dispose();
        s1?.Dispose();
        if (overflow is null)
        {
            return;
        }

        for (var i = 0; i < overflowCount; i++)
        {
            overflow[i].Dispose();
        }
    }
}
