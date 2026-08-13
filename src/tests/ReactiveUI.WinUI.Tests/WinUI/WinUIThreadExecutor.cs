// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Interfaces;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Runs a test on the XAML application's UI thread and leaves the service locator alone.</summary>
/// <remarks>For tests that configure their own dependency resolver but still need a live dispatcher queue.</remarks>
public sealed class WinUIThreadExecutor : ITestExecutor
{
    /// <inheritdoc/>
    public ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        return new(WinUIApplicationHost.RunOnUIThreadAsync(action));
    }
}
