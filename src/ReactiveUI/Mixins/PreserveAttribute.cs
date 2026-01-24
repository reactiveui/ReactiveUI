// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Indicates that the attributed code element should be preserved during linking or code optimization processes.
/// </summary>
/// <remarks>Apply this attribute to prevent code elements from being removed or altered by tools that perform
/// code trimming, such as linkers or obfuscators. This is typically used to ensure that reflection or dynamic access to
/// the member remains functional after build-time optimizations.</remarks>
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.All)]
internal sealed class PreserveAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether all members are included in the operation.
    /// </summary>
    public bool AllMembers { get; set; }
}
