// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !NET
using Targets = System.AttributeTargets;

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates that certain members on a specified <see cref="Type"/> are accessed dynamically,
/// for example through <see cref="System.Reflection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerNonUserCode]
[AttributeUsage(
    validOn: Targets.Class |
             Targets.Field |
             Targets.GenericParameter |
             Targets.Interface |
             Targets.Method |
             Targets.Parameter |
             Targets.Property |
             Targets.ReturnValue |
             Targets.Struct,
    Inherited = false)]
internal sealed class DynamicallyAccessedMembersAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicallyAccessedMembersAttribute"/> class
    /// with the specified member types.
    /// </summary>
    /// <param name="memberTypes">The types of members dynamically accessed.</param>
    public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes) =>
        MemberTypes = memberTypes;

    /// <summary>
    /// Gets the <see cref="DynamicallyAccessedMemberTypes"/> which specifies the type
    /// of members dynamically accessed.
    /// </summary>
    public DynamicallyAccessedMemberTypes MemberTypes { get; }
}

#else
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(DynamicallyAccessedMembersAttribute))]
#endif
