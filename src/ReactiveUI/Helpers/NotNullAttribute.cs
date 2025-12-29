// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !NETCOREAPP3_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
///   Specifies that an output is not <see langword="null"/> even if the
///   corresponding type allows it.
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerNonUserCode]
[AttributeUsage(
    validOn: AttributeTargets.Field |
             AttributeTargets.Parameter |
             AttributeTargets.Property |
             AttributeTargets.ReturnValue)]
internal sealed class NotNullAttribute : Attribute;
#else
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(NotNullAttribute))]
#endif
