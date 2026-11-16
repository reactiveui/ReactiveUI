// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>Provides compatibility with linkers that recognize the legacy Preserve attribute.</summary>
/// <remarks>Use <c>DynamicDependency</c> on reachable methods to preserve known members, or
/// <c>DynamicallyAccessedMembers</c> to describe reflection requirements on type parameters and values.</remarks>
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.All)]
[DebuggerDisplay("{AllMembers}")]
public sealed class PreserveAttribute : Attribute
{
    /// <summary>Gets or sets a value indicating whether all members are included in the operation.</summary>
    public bool AllMembers { get; set; }
}
