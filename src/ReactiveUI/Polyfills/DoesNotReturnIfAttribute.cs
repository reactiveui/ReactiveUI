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
/// Specifies that the method will not return if the associated <see cref="bool"/>
/// parameter is passed the specified value.
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerNonUserCode]
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class DoesNotReturnIfAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoesNotReturnIfAttribute"/>
    /// class with the specified parameter value.
    /// </summary>
    /// <param name="parameterValue">
    /// The condition parameter value. Code after the method is considered unreachable
    /// by diagnostics if the argument to the associated parameter matches this value.
    /// </param>
    public DoesNotReturnIfAttribute(bool parameterValue) =>
        ParameterValue = parameterValue;

    /// <summary>
    /// Gets a value indicating whether code after the method is considered unreachable
    /// by diagnostics if the argument to the associated parameter matches this value.
    /// </summary>
    public bool ParameterValue { get; }
}
#else
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.DoesNotReturnIfAttribute))]
#endif
