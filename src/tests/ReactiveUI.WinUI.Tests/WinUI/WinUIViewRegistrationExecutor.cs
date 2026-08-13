// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;
using ReactiveUI.Tests.WinUI.Mocks;
using TUnit.Core.Interfaces;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Runs a test on the XAML UI thread with the mock views registered in the service locator.</summary>
/// <remarks>
/// The view hosts fall back to <see cref="ViewLocator.Current"/> when no locator is assigned to them, so the tests
/// covering that fallback need the registrations this executor installs.
/// </remarks>
public sealed class WinUIViewRegistrationExecutor : ITestExecutor
{
    /// <summary>Manages the ReactiveUI builder lifetime for the test.</summary>
    private readonly AppBuilderTestHelper _helper = new();

    /// <inheritdoc/>
    public ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        return new(WinUIApplicationHost.RunOnUIThreadAsync(async () =>
        {
            _helper.Initialize(static builder => builder
                .WithWinUI()
                .RegisterView<RoutedTestView, RoutedTestViewModel>()
                .RegisterView<PlainTestView, PlainTestViewModel>());

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
