// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveObjects;

/// <summary>The base class every row in the school's data layer derives from, recording when it was written.</summary>
[System.Diagnostics.DebuggerDisplay("CreatedAt = {CreatedAt}")]
public class AuditedEntity
{
    /// <summary>Gets the moment the row was written to the store.</summary>
    public DateTimeOffset CreatedAt { get; init; } = TimeProvider.System.GetUtcNow();
}
