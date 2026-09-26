// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>A fixed, in-memory rack of spices.</summary>
[System.Diagnostics.DebuggerDisplay("SpiceRack Count = {Spices.Count}")]
public sealed class SpiceRack : ISpiceRack
{
    /// <inheritdoc/>
    public IReadOnlyList<string> Spices { get; } = ["Cumin", "Paprika"];
}
