// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Messaging;

/// <summary>A notice that one class will not run today; the message bus contract carries the year group it affects.</summary>
/// <param name="ClassName">The subject whose class was cancelled.</param>
[System.Diagnostics.DebuggerDisplay("{ClassName}")]
public sealed record ClassCancelled(string ClassName);
