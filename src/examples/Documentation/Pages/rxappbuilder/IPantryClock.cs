// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>A clock the pantry uses to date-stamp what it has in stock.</summary>
public interface IPantryClock
{
    /// <summary>Gets the current moment.</summary>
    DateTimeOffset Now { get; }
}
