// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using ReactiveUI.Maui.Internal;
using TUnit.Core.Executors;

namespace ReactiveUI.WinUI.Tests;

/// <summary>Tests the shared WinUI routed-host initialization.</summary>
public sealed class MauiReactiveHelpersTest
{
    /// <summary>Verifies that routed-host initialization assigns its contract stream and starts route resolution.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WinUITestExecutor>]
    public async Task InitializeRoutedViewHost_InitializesContractAndRouteSubscriptions()
    {
        var host = new TestWinUiRoutedHost();
        var resolutions = new List<(IRoutableViewModel? viewModel, string? contract)>();
        MultipleDisposable subscriptions = [];

        MauiReactiveHelpers.InitializeRoutedViewHost(
            host,
            TestWinUiRoutedHost.RouterProperty,
            TestWinUiRoutedHost.ViewContractObservableProperty,
            subscriptions,
            resolutions.Add);

        await Assert.That(host.ViewContractObservable).IsNotNull();
        await Assert.That(resolutions.Count).IsEqualTo(1);
        await Assert.That(resolutions[0].viewModel).IsNull();
        await Assert.That(resolutions[0].contract).IsNull();

        subscriptions.Dispose();
    }

    /// <summary>Verifies that the non-generic routed-host overload uses the shared initialization.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WinUITestExecutor>]
    public async Task RoutedViewHost_InitializesViewContractObservable()
    {
        var host = new RoutedViewHost();

        await Assert.That(host.ViewContractObservable).IsNotNull();
        await Assert.That(host.ViewContract).IsNull();
    }

    /// <summary>Verifies that the generic routed-host overload uses the shared initialization.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WinUITestExecutor>]
    public async Task GenericRoutedViewHost_InitializesViewContractObservable()
    {
        var host = new RoutedViewHost<TestWinUiRoutableViewModel>();

        await Assert.That(host.ViewContractObservable).IsNotNull();
        await Assert.That(host.ViewContract).IsNull();
    }

    /// <summary>Minimal routed host used to exercise the shared initialization helper directly.</summary>
    private sealed class TestWinUiRoutedHost : FrameworkElement, IMauiRoutedViewHost
    {
        /// <summary>The router dependency property.</summary>
        public static readonly DependencyProperty RouterProperty =
            DependencyProperty.Register(nameof(Router), typeof(RoutingState), typeof(TestWinUiRoutedHost), new(null));

        /// <summary>The view-contract observable dependency property.</summary>
        public static readonly DependencyProperty ViewContractObservableProperty =
            DependencyProperty.Register(
                nameof(ViewContractObservable),
                typeof(IObservable<string?>),
                typeof(TestWinUiRoutedHost),
                new(Signal.Emit<string?>(null)));

        /// <inheritdoc/>
        public RoutingState Router
        {
            get => (RoutingState)GetValue(RouterProperty);
            set => SetValue(RouterProperty, value);
        }

        /// <inheritdoc/>
        public IObservable<string?> ViewContractObservable
        {
            get => (IObservable<string?>)GetValue(ViewContractObservableProperty);
            set => SetValue(ViewContractObservableProperty, value);
        }

        /// <inheritdoc/>
        public string? ViewContract { get; private set; }

        /// <inheritdoc/>
        void IMauiRoutedViewHost.SetObservedViewContract(string? contract) => ViewContract = contract;
    }

    /// <summary>Minimal view model used to construct the generic routed host.</summary>
    private sealed class TestWinUiRoutableViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment => null;

        /// <inheritdoc/>
        public IScreen HostScreen { get; } = new TestWinUiScreen();
    }

    /// <summary>Minimal screen used by the generic routed-host view model.</summary>
    private sealed class TestWinUiScreen : IScreen
    {
        /// <inheritdoc/>
        public RoutingState Router { get; } = new();
    }
}
