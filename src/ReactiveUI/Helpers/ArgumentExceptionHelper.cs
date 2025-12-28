// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI.Helpers;

/// <summary>
/// Provides helper methods for argument validation.
/// These methods serve as polyfills for ArgumentNullException.ThrowIfNull and related methods
/// that are only available in newer .NET versions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ArgumentExceptionHelper
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
    /// </summary>
    /// <param name="argument">The reference type argument to validate as non-null.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
    /// </summary>
    /// <param name="argument">The reference type argument to validate as non-null.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    public static void ThrowIfNullWithMessage([NotNull] object? argument, string message, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName, message);
        }
    }

    /// <summary>
    /// Throws an exception if <paramref name="argument"/> is null or empty.
    /// </summary>
    /// <param name="argument">The string argument to validate as non-null and non-empty.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="argument"/> is empty.</exception>
    public static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }

        if (argument.Length == 0)
        {
            throw new ArgumentException("The value cannot be an empty string.", paramName);
        }
    }

    /// <summary>
    /// Throws an exception if <paramref name="argument"/> is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="argument">The string argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="argument"/> is empty or consists only of white-space characters.</exception>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }

        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentException("The value cannot be an empty string or composed entirely of whitespace.", paramName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.
    /// </summary>
    /// <param name="value">The argument to validate as non-negative.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "The value cannot be negative.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal to <paramref name="other"/>.
    /// </summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="other">The value to compare with.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfLessThanOrEqual(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value <= other)
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value cannot be less than or equal to {other}.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if <paramref name="condition"/> is true.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="condition"/> corresponds.</param>
    public static void ThrowIf([DoesNotReturnIf(true)] bool condition, string message, [CallerArgumentExpression(nameof(condition))] string? paramName = null)
    {
        if (condition)
        {
            throw new ArgumentException(message, paramName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.
    /// </summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfZero(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value == 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "The value cannot be zero.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative or zero.
    /// </summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfNegativeOrZero(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "The value cannot be negative or zero.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is equal to <paramref name="other"/>.
    /// </summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="other">The value to compare with.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IEquatable<T>
    {
        if (value.Equals(other))
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value cannot be equal to {other}.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is not equal to <paramref name="other"/>.
    /// </summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="other">The value to compare with.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfNotEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IEquatable<T>
    {
        if (!value.Equals(other))
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value must be equal to {other}.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.
    /// </summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="other">The value to compare with.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfGreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) > 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value cannot be greater than {other}.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal to <paramref name="other"/>.
    /// </summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="other">The value to compare with.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfGreaterThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) >= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value cannot be greater than or equal to {other}.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.
    /// </summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="other">The value to compare with.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value cannot be less than {other}.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal to <paramref name="other"/>.
    /// </summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="other">The value to compare with.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfLessThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value cannot be less than or equal to {other}.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if <paramref name="argument"/> is not of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    public static void ThrowIfNotOfType<T>([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is not T)
        {
            throw new ArgumentException($"Argument must be of type {typeof(T).Name}.", paramName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is default.
    /// </summary>
    /// <typeparam name="T">The struct type.</typeparam>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    public static void ThrowIfDefault<T>(T argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(argument, default))
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
