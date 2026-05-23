// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
#if !NET
using System.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Represents a range with a start and an end index.
/// Polyfill for the framework targets that predate <c>System.Range</c>, enabling the <c>..</c> range operator.
/// </summary>
[ExcludeFromCodeCoverage]
internal readonly struct Range : IEquatable<Range>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Range"/> struct.
    /// </summary>
    /// <param name="start">The inclusive start index of the range.</param>
    /// <param name="end">The exclusive end index of the range.</param>
    public Range(Index start, Index end)
    {
        Start = start;
        End = end;
    }

    /// <summary>Gets a <see cref="Range"/> that covers the entire collection.</summary>
    public static Range All => new(Index.Start, Index.End);

    /// <summary>Gets the inclusive start index of the range.</summary>
    public Index Start { get; }

    /// <summary>Gets the exclusive end index of the range.</summary>
    public Index End { get; }

    /// <summary>Determines whether two ranges are equal.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> if the ranges are equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Range left, Range right) => left.Equals(right);

    /// <summary>Determines whether two ranges are not equal.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> if the ranges are not equal; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Range left, Range right) => !left.Equals(right);

    /// <summary>Creates a range that starts at the supplied index and runs to the end of the collection.</summary>
    /// <param name="start">The inclusive start index.</param>
    /// <returns>The created <see cref="Range"/>.</returns>
    public static Range StartAt(Index start) => new(start, Index.End);

    /// <summary>Creates a range that starts at the beginning of the collection and ends at the supplied index.</summary>
    /// <param name="end">The exclusive end index.</param>
    /// <returns>The created <see cref="Range"/>.</returns>
    public static Range EndAt(Index end) => new(Index.Start, end);

    /// <summary>Calculates the start offset and length of the range for a collection of the supplied length.</summary>
    /// <param name="length">The length of the collection.</param>
    /// <returns>A tuple containing the start offset and the length of the range.</returns>
    public (int Offset, int Length) GetOffsetAndLength(int length)
    {
        var start = Start.GetOffset(length);
        var end = End.GetOffset(length);

        if ((uint)end > (uint)length || (uint)start > (uint)end)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        return (start, end - start);
    }

    /// <inheritdoc/>
    public bool Equals(Range other) => Start.Equals(other.Start) && End.Equals(other.End);

    /// <inheritdoc/>
    public override bool Equals(object? value) => value is Range other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Start.GetHashCode() ^ End.GetHashCode();
}

#endif
