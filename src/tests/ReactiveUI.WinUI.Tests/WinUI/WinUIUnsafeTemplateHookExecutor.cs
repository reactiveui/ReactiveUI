// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;
using TUnit.Core.Interfaces;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Runs a test on the XAML UI thread with the app built as the <see cref="AutoDataTemplateBindingHookUnsafe"/> example shows.</summary>
/// <remarks>
/// The binding hooks are cached on first use, so the cache is dropped before and after the test. That keeps the
/// Unsafe hook this executor registers from leaking into other tests.
/// </remarks>
public sealed class WinUIUnsafeTemplateHookExecutor : ITestExecutor
{
    /// <summary>Manages the ReactiveUI builder lifetime for the test.</summary>
    private readonly AppBuilderTestHelper _helper = new();

    /// <inheritdoc/>
    public ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        return new(WinUIApplicationHost.RunOnUIThreadAsync(async () =>
        {
            // The builder chain below is the documented example, less the CreateReactiveUIBuilder and BuildApp calls
            // the helper makes itself.
            _helper.Initialize(static builder => builder
                .WithWinUI()
                .WithRegistration(static r => r.RegisterConstant<IPropertyBindingHook>(new AutoDataTemplateBindingHookUnsafe())));
            BindingHooks.Refresh();

            try
            {
                await action().ConfigureAwait(true);
            }
            finally
            {
                _helper.CleanUp();
                BindingHooks.Refresh();
            }
        }));
    }
}
