// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// Polyfill implementation adapted from SimonCropp/Polyfill
// https://github.com/SimonCropp/Polyfill
#if !NETCOREAPP3_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER

using System.Diagnostics;

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that when a method returns <see cref="ReturnValue"/>,
/// the parameter may be <see langword="null"/> even if the corresponding type disallows it.
/// Modification of Using SimonCropp's polyfill's library.
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerNonUserCode]
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class MaybeNullWhenAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MaybeNullWhenAttribute"/> class.
    /// </summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value,
    /// the associated parameter may be <see langword="null"/>.
    /// </param>
    public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

    /// <summary>
    /// Gets a value indicating whether the return condition has been satisfied.
    /// If the method returns this value, the associated parameter may be <see langword="null"/>.
    /// </summary>
    public bool ReturnValue { get; }
}

#else
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(MaybeNullWhenAttribute))]
#endif
