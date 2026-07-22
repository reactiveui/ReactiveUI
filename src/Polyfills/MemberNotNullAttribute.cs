// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !NET
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Targets = System.AttributeTargets;

/// <summary>Specifies that the method or property will ensure that the listed field and property members have not-<see langword="null"/> values.</summary>
[ExcludeFromCodeCoverage]
[DebuggerNonUserCode]
[AttributeUsage(
    validOn: Targets.Method |
             Targets.Property,
    Inherited = false,
    AllowMultiple = true)]
[SuppressMessage(
    "Design",
    "CA1019:Define accessors for attribute arguments",
    Justification = "Faithful BCL polyfill.")]
[SuppressMessage(
    "Design",
    "SST2312:Types should be declared in a named namespace",
    Justification = "Mirrors the shape of the corresponding BCL type (System.Diagnostics.CodeAnalysis.MemberNotNullAttribute); the polyfill compiles only where the BCL lacks it.")]
[SuppressMessage(
    "Design",
    "SST2324:Do not declare a member more accessible than its containing type",
    Justification = "Mirrors the shape of the corresponding BCL type (System.Diagnostics.CodeAnalysis.MemberNotNullAttribute); the polyfill compiles only where the BCL lacks it.")]
internal sealed class MemberNotNullAttribute :
    Attribute
{
    /// <summary>Initializes a new instance of the <see cref="MemberNotNullAttribute"/> class.</summary>
    /// <param name="member">Field or property member name.</param>
    public MemberNotNullAttribute(string member) =>
        Members = [member];

    /// <summary>Initializes a new instance of the <see cref="MemberNotNullAttribute"/> class.</summary>
    /// <param name="members">Field or property member names.</param>
    public MemberNotNullAttribute(params string[] members) =>
        Members = members;

    /// <summary>Gets field or property member names.</summary>
    public string[] Members { get; }
}

#else
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(MemberNotNullAttribute))]
#endif
