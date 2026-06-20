// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.BlazorServer.Services;

/// <summary>Provides a process-wide identifier for this running application instance.</summary>
/// <remarks>
/// The sample uses this id to tag MessageBus payloads so that an instance can
/// ignore the chat and room events it broadcast itself, distinguishing them from
/// events that originated in other instances.
/// </remarks>
internal static class AppInstance
{
    /// <summary>A unique identifier generated once per application instance.</summary>
    public static readonly Guid Id = Guid.NewGuid();
}
