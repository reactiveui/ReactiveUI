// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Builder.BlazorServer.Models;

/// <summary>
/// Notification that the chat state has changed and observers should refresh.
/// </summary>
[SuppressMessage("Minor Code Smell", "S2094:Classes should not be empty", Justification = "Marker type for chat state-changed signalling.")]
public sealed class ChatStateChanged;
