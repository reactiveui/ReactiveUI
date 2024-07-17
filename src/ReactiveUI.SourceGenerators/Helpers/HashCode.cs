// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

#pragma warning disable CS0809

namespace System;

/// <summary>
/// A polyfill type that mirrors some methods from <see cref="HashCode"/> on .NET 6.
/// </summary>
#pragma warning disable CA1066 // Implement IEquatable when overriding Object.Equals
internal struct HashCode
#pragma warning restore CA1066 // Implement IEquatable when overriding Object.Equals
{
    private const uint Prime1 = 2654435761U;
    private const uint Prime2 = 2246822519U;
    private const uint Prime3 = 3266489917U;
    private const uint Prime4 = 668265263U;
    private const uint Prime5 = 374761393U;

    private static readonly uint seed = GenerateGlobalSeed();

    private uint _v1;
    private uint _v2;
    private uint _v3;
    private uint _v4;
    private uint _queue1;
    private uint _queue2;
    private uint _queue3;
    private uint _length;

    /// <summary>
    /// Adds a single value to the current hash.
    /// </summary>
    /// <typeparam name="T">The type of the value to add into the hash code.</typeparam>
    /// <param name="value">The value to add into the hash code.</param>
    public void Add<T>(T value) => Add(value?.GetHashCode() ?? 0);

    /// <summary>
    /// Gets the resulting hashcode from the current instance.
    /// </summary>
    /// <returns>The resulting hashcode from the current instance.</returns>
    public int ToHashCode()
    {
        var length = _length;
        var position = length % 4;
        var hash = length < 4 ? MixEmptyState() : MixState(_v1, _v2, _v3, _v4);

        hash += length * 4;

        if (position > 0)
        {
            hash = QueueRound(hash, _queue1);

            if (position > 1)
            {
                hash = QueueRound(hash, _queue2);

                if (position > 2)
                {
                    hash = QueueRound(hash, _queue3);
                }
            }
        }

        hash = MixFinal(hash);

        return (int)hash;
    }

    /// <inheritdoc/>
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException();

    /// <inheritdoc/>
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    /// <summary>
    /// Rotates the specified value left by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROL.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint RotateLeft(uint value, int offset) => (value << offset) | (value >> (32 - offset));

    /// <summary>
    /// Initializes the default seed.
    /// </summary>
    /// <returns>A random seed.</returns>
    private static unsafe uint GenerateGlobalSeed()
    {
        var bytes = new byte[4];

        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(bytes);
        }

        return BitConverter.ToUInt32(bytes, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
    {
        v1 = seed + Prime1 + Prime2;
        v2 = seed + Prime2;
        v3 = seed;
        v4 = seed - Prime1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Round(uint hash, uint input) => RotateLeft(hash + (input * Prime2), 13) * Prime1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint QueueRound(uint hash, uint queuedValue) => RotateLeft(hash + (queuedValue * Prime3), 17) * Prime4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixState(uint v1, uint v2, uint v3, uint v4) => RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixEmptyState() => seed + Prime5;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixFinal(uint hash)
    {
        hash ^= hash >> 15;
        hash *= Prime2;
        hash ^= hash >> 13;
        hash *= Prime3;
        hash ^= hash >> 16;

        return hash;
    }

    private void Add(int value)
    {
        var val = (uint)value;
        var previousLength = _length++;
        var position = previousLength % 4;

        if (position == 0)
        {
            _queue1 = val;
        }
        else if (position == 1)
        {
            _queue2 = val;
        }
        else if (position == 2)
        {
            _queue3 = val;
        }
        else
        {
            if (previousLength == 3)
            {
                Initialize(out _v1, out _v2, out _v3, out _v4);
            }

            _v1 = Round(_v1, _queue1);
            _v2 = Round(_v2, _queue2);
            _v3 = Round(_v3, _queue3);
            _v4 = Round(_v4, val);
        }
    }
}
