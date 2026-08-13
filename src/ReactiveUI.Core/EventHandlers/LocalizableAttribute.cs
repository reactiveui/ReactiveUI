// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics;

#if PORTABLE || ANDROID
namespace ReactiveUI;

/// <summary>A attribute to indicate if the target is localizable or not.</summary>
/// <param name="isLocalizable">If the target is localizable or not.</param>
/// <remarks>
/// Initializes a new instance of the <see cref="LocalizableAttribute"/> class.
/// </remarks>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
[DebuggerDisplay("{IsLocalizable}")]
public sealed class LocalizableAttribute(bool isLocalizable) : Attribute
{
    /// <summary>Gets a value indicating whether the target is localizable.</summary>
    public bool IsLocalizable { get; } = isLocalizable;
}
#endif
