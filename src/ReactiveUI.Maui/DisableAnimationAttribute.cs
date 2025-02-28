// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui;

/// <summary>
/// Attribute that disables animation for a view.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.All)]
public sealed class DisableAnimationAttribute : Attribute
{
}