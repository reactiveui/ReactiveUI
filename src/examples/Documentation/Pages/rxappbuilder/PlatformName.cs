// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>Names the platform a <c>ForCustomPlatform</c> or <c>ForPlatforms</c> call registers for.</summary>
/// <param name="Name">The platform's name.</param>
[System.Diagnostics.DebuggerDisplay("PlatformName Name = {Name}")]
public sealed record PlatformName(string Name);
