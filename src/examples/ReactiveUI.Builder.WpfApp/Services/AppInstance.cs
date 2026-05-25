// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>
/// Provides a stable identifier that is unique to the current running instance of the application.
/// </summary>
/// <remarks>
/// The sample uses this id to tell instances apart when several copies of the app are running at once,
/// for example to ignore network packets that an instance broadcast to itself.
/// </remarks>
internal static class AppInstance
{
    /// <summary>
    /// The identifier generated once per process, used to distinguish this app instance from others.
    /// </summary>
    public static readonly Guid Id = Guid.NewGuid();
}
