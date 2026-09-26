// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests;

/// <summary>The application state the suspension driver tests save and load.</summary>
/// <param name="Name">A saved name.</param>
/// <param name="Count">A saved count.</param>
public sealed record SuspensionState(string Name, int Count);
