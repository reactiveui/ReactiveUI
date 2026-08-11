// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;
using TUnit.Core.Interfaces;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Runs a test on the XAML application's UI thread with the WinUI ReactiveUI services registered.</summary>
/// <remarks>
/// Both halves matter: WinUI controls can only be constructed on the thread that owns the XAML core, and
/// <c>WithWinUI</c> resolves its main-thread scheduler from <c>DispatcherQueue.GetForCurrentThread()</c>,
/// so the builder has to be configured from that same thread. The service locator is torn back down after
/// every test so registrations cannot leak between them.
/// </remarks>
public sealed class WinUITestExecutor : ITestExecutor
{
    /// <summary>Manages the ReactiveUI builder lifetime for the test.</summary>
    private readonly AppBuilderTestHelper _helper = new();

    /// <inheritdoc/>
    public ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        return new(WinUIApplicationHost.RunOnUIThreadAsync(async () =>
        {
            _helper.Initialize(static builder => builder.WithWinUI());

            try
            {
                await action().ConfigureAwait(true);
            }
            finally
            {
                _helper.CleanUp();
            }
        }));
    }
}
