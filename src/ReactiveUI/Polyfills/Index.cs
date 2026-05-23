// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !NET

namespace System;

/// <summary>
/// Represents a type that can be used to index a collection either from the start or the end.
/// Polyfill for the framework targets that predate <c>System.Index</c>, enabling the <c>^</c> index operator.
/// </summary>
[ExcludeFromCodeCoverage]
internal readonly struct Index : IEquatable<Index>
{
    /// <summary>The encoded value; non-negative is measured from the start, the bitwise complement is measured from the end.</summary>
    private readonly int _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> struct.
    /// </summary>
    /// <param name="value">The index value. Must be zero or positive.</param>
    /// <param name="fromEnd">Indicates whether the index is counted from the start or the end.</param>
    public Index(int value, bool fromEnd = false)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Non-negative number required.");
        }

        _value = fromEnd ? ~value : value;
    }

    /// <summary>Gets an <see cref="Index"/> that points at the first element of a collection.</summary>
    public static Index Start => new(0);

    /// <summary>Gets an <see cref="Index"/> that points beyond the last element of a collection.</summary>
    public static Index End => new(0, true);

    /// <summary>Gets the index value (without the from-end flag).</summary>
    public int Value => _value < 0 ? ~_value : _value;

    /// <summary>Gets a value indicating whether the index is counted from the end of a collection.</summary>
    public bool IsFromEnd => _value < 0;

    /// <summary>Determines whether two indexes are equal.</summary>
    /// <param name="left">The first index.</param>
    /// <param name="right">The second index.</param>
    /// <returns><see langword="true"/> if the indexes are equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Index left, Index right) => left.Equals(right);

    /// <summary>Determines whether two indexes are not equal.</summary>
    /// <param name="left">The first index.</param>
    /// <param name="right">The second index.</param>
    /// <returns><see langword="true"/> if the indexes are not equal; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Index left, Index right) => !left.Equals(right);

    /// <summary>Creates an <see cref="Index"/> measured from the start of a collection.</summary>
    /// <param name="value">The zero-based index from the start.</param>
    /// <returns>The created <see cref="Index"/>.</returns>
    public static Index FromStart(int value) => new(value);

    /// <summary>Creates an <see cref="Index"/> measured from the end of a collection.</summary>
    /// <param name="value">The index from the end (1 refers to the last element).</param>
    /// <returns>The created <see cref="Index"/>.</returns>
    public static Index FromEnd(int value) => new(value, true);

    /// <summary>Calculates the offset from the start of a collection of the supplied length.</summary>
    /// <param name="length">The length of the collection.</param>
    /// <returns>The offset from the start of the collection.</returns>
    public int GetOffset(int length)
    {
        var offset = _value;
        if (IsFromEnd)
        {
            offset += length + 1;
        }

        return offset;
    }

    /// <inheritdoc/>
    public bool Equals(Index other) => _value == other._value;

    /// <inheritdoc/>
    public override bool Equals(object? value) => value is Index other && _value == other._value;

    /// <inheritdoc/>
    public override int GetHashCode() => _value;
}

#endif
