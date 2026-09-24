// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using System.Windows.Threading;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Runs a test on an STA thread that owns a WPF dispatcher, as a WPF UI thread does.</summary>
/// <remarks>
/// <see cref="DispatcherSequencer.Main"/> never creates a dispatcher, so <c>WithWpf()</c> throws on a thread that has
/// none. A real application configures ReactiveUI once its <see cref="System.Windows.Application"/> exists; tests
/// have no application, so this executor gives the test thread its dispatcher before the test configures anything.
/// </remarks>
[SupportedOSPlatform("windows")]
public class DispatcherThreadExecutor : STAThreadExecutor
{
    /// <inheritdoc/>
    protected override void Initialize()
    {
        base.Initialize();
        _ = Dispatcher.CurrentDispatcher;
    }
}
