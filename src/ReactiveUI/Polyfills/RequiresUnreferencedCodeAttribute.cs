// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// Polyfill implementation adapted from Simon Cropp's Polyfill library
// https://github.com/SimonCropp/Polyfill
#if !NET

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates that the specified method requires dynamic access to code that is not referenced
/// statically, for example through <see cref="System.Reflection"/>.
/// </summary>
/// <remarks>
/// Link: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.requiresunreferencedcodeattribute.
/// </remarks>
[ExcludeFromCodeCoverage]
[DebuggerNonUserCode]
[AttributeUsage(
    AttributeTargets.Method |
    AttributeTargets.Constructor |
    AttributeTargets.Class,
    Inherited = false)]
internal sealed class RequiresUnreferencedCodeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiresUnreferencedCodeAttribute"/> class
    /// with the specified message.
    /// </summary>
    /// <param name="message">A message that contains information about the usage of unreferenced code.</param>
    public RequiresUnreferencedCodeAttribute(string message) =>
        Message = message;

    /// <summary>
    /// Gets a message that contains information about the usage of unreferenced code.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets or sets an optional URL that contains more information about the method,
    /// why it requires unreferenced code, and what options a consumer has to deal with it.
    /// </summary>
    public string? Url { get; set; }
}

#else
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute))]
#endif
