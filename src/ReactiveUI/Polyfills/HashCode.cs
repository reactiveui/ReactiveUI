// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// Minimal polyfill for the static System.HashCode.Combine overloads on .NET Framework, which lacks System.HashCode.
// Only the static Combine methods are used in this assembly, so the value combination is delegated to ValueTuple's
// own hash combination rather than reimplementing the xxHash32 accumulator.
#if NETFRAMEWORK

using System.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Combines the hash codes of multiple values into a single hash code.
/// </summary>
internal static class HashCode
{
    /// <summary>Combines a single value into a hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <returns>The combined hash code.</returns>
    public static int Combine<T1>(T1 value1) => value1?.GetHashCode() ?? 0;

    /// <summary>Combines two values into a hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The combined hash code.</returns>
    public static int Combine<T1, T2>(T1 value1, T2 value2) =>
        (value1, value2).GetHashCode();

    /// <summary>Combines three values into a hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="value3">The third value.</param>
    /// <returns>The combined hash code.</returns>
    public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3) =>
        (value1, value2, value3).GetHashCode();

    /// <summary>Combines four values into a hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="value3">The third value.</param>
    /// <param name="value4">The fourth value.</param>
    /// <returns>The combined hash code.</returns>
    public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4) =>
        (value1, value2, value3, value4).GetHashCode();

    /// <summary>Combines five values into a hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="value3">The third value.</param>
    /// <param name="value4">The fourth value.</param>
    /// <param name="value5">The fifth value.</param>
    /// <returns>The combined hash code.</returns>
    public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) =>
        (value1, value2, value3, value4, value5).GetHashCode();

    /// <summary>Combines six values into a hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="value3">The third value.</param>
    /// <param name="value4">The fourth value.</param>
    /// <param name="value5">The fifth value.</param>
    /// <param name="value6">The sixth value.</param>
    /// <returns>The combined hash code.</returns>
    public static int Combine<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) =>
        (value1, value2, value3, value4, value5, value6).GetHashCode();

    /// <summary>Combines seven values into a hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <typeparam name="T7">The type of the seventh value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="value3">The third value.</param>
    /// <param name="value4">The fourth value.</param>
    /// <param name="value5">The fifth value.</param>
    /// <param name="value6">The sixth value.</param>
    /// <param name="value7">The seventh value.</param>
    /// <returns>The combined hash code.</returns>
    public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) =>
        (value1, value2, value3, value4, value5, value6, value7).GetHashCode();

    /// <summary>Combines eight values into a hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <typeparam name="T7">The type of the seventh value.</typeparam>
    /// <typeparam name="T8">The type of the eighth value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="value3">The third value.</param>
    /// <param name="value4">The fourth value.</param>
    /// <param name="value5">The fifth value.</param>
    /// <param name="value6">The sixth value.</param>
    /// <param name="value7">The seventh value.</param>
    /// <param name="value8">The eighth value.</param>
    /// <returns>The combined hash code.</returns>
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Mirrors the System.HashCode.Combine 8-arity overload.")]
    public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) =>
        (value1, value2, value3, value4, value5, value6, value7, value8).GetHashCode();
}

#endif
