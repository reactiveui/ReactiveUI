// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>Source-generated serialization metadata for <see cref="LibraryAppState"/>, so
/// <see cref="AppSupportJsonSuspensionDriver"/>'s trim- and AOT-safe <c>LoadState{T}(JsonTypeInfo{T})</c> and
/// <c>SaveState{T}(T, JsonTypeInfo{T})</c> overloads never need reflection-based serialization.</summary>
[JsonSerializable(typeof(LibraryAppState))]
[System.Diagnostics.DebuggerDisplay("LibraryAppStateJsonContext")]
public sealed partial class LibraryAppStateJsonContext : JsonSerializerContext;
