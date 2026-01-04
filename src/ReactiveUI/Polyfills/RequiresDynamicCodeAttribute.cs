// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// Polyfill implementation adapted from Simon Cropp's Polyfill library
// https://github.com/SimonCropp/Polyfill
#if !NET7_0_OR_GREATER

#nullable enable

using Targets = System.AttributeTargets;

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates that the specified method requires the ability to generate new code at runtime,
/// for example through <see cref="System.Reflection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerNonUserCode]
[AttributeUsage(
    validOn: Targets.Method |
             Targets.Constructor |
             Targets.Class,
    Inherited = false)]
internal sealed class RequiresDynamicCodeAttribute :
    Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiresDynamicCodeAttribute"/> class
    /// with the specified message.
    /// </summary>
    public RequiresDynamicCodeAttribute(string message) =>
        Message = message;

    /// <summary>
    /// Gets or sets a value indicating whether the annotation should not apply to static members.
    /// </summary>
    public bool ExcludeStatics { get; set; }

    /// <summary>
    /// Gets a message that contains information about the usage of dynamic code.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets or sets an optional URL that contains more information about the method,
    /// why it requires dynamic code, and what options a consumer has to deal with it.
    /// </summary>
    public string? Url { get; set; }
}
#else
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute))]
#endif
