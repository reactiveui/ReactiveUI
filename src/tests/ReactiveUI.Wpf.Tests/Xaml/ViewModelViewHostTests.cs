// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Threading;

using DynamicData;
using ReactiveUI.Builder;
using ReactiveUI.Tests.Utilities;
using ReactiveUI.Tests.Utilities.AppBuilder;
using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests for ViewModelViewHost.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because tests modify
/// global service locator state.
/// </remarks>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class ViewModelViewHostTests
{
    private static readonly bool[] _expectedActivated = [true];

    /// <summary>
    /// Verifies that the view-model view host shows its default content when activated with no view model.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHostDefaultContentNotNull()
    {
        var uc = new ViewModelViewHost
        {
            DefaultContent = new System.Windows.Controls.Label()
        };

        var activation = new ActivationForViewFetcher();
        activation.GetActivationForView(uc)
            .ToObservableChangeSet(scheduler: ImmediateScheduler.Instance)
            .Bind(out var controlActivated)
            .Subscribe();

        // Simulate activation by raising the Loaded event
        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };
        uc.RaiseEvent(loaded);

        await _expectedActivated.AssertAreEqual(controlActivated);
        await Assert.That(uc.Content).IsNotNull();
    }

    /// <summary>
    /// Verifies the view-model view host resolves the matching view once activated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WpfWithViewExecutor>]
    public async Task ViewModelViewHostContentNotNullWithViewModelAndActivated()
    {
        var viewModel = new TestViewModel();

        var uc = new ViewModelViewHost
        {
            DefaultContent = new System.Windows.Controls.Label(),
            ViewModel = viewModel
        };

        var activation = new ActivationForViewFetcher();
        activation.GetActivationForView(uc).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var controlActivated).Subscribe();

        // Simulate activation by raising the Loaded event
        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };
        uc.RaiseEvent(loaded);

        await _expectedActivated.AssertAreEqual(controlActivated);

        // Test IViewFor<ViewModel> after activated
        await Assert.That(uc.Content).IsTypeOf<TestView>();
    }

    /// <summary>
    /// A test executor that registers the test view for view-resolution tests.
    /// </summary>
    public class WpfWithViewExecutor : STAThreadExecutor
    {
        /// <summary>
        /// Helper that manages app builder setup and teardown for the test.
        /// </summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(builder =>
            {
                // Include WPF platform services and register test view
                builder
                    .WithWpf()
                    .RegisterView<TestView, TestViewModel>()
                    .WithCoreServices();

                // Configure WPF scheduler for test execution
                // Note: WithWpf() skips scheduler setup when InUnitTestRunner() is true,
                // so we must manually configure it for tests that need WPF controls
                var dispatcher = Dispatcher.CurrentDispatcher;
                RxSchedulers.MainThreadScheduler = new DispatcherScheduler(dispatcher);
                RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
            });
        }

        /// <inheritdoc/>
        protected override void CleanUp()
        {
            _helper.CleanUp();
            base.CleanUp();
        }
    }
}
