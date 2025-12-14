// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading;
using Microsoft.Maui.Controls;
using NUnit.Framework;
using ReactiveUI;
using ReactiveUI.Maui;
using Splat;
using Splat.Builder;

namespace ReactiveUI.Builder.Maui.Tests.Activation;

/// <summary>
/// Tests for the MAUI ActivationForViewFetcher.
/// </summary>
[TestFixture]
[Apartment(ApartmentState.STA)]
public sealed partial class ActivationForViewFetcherTests
{
    /// <summary>
    /// Verifies that a page and its child view activate and deactivate via the fetcher.
    /// </summary>
    [Test]
    public void PageAndChildViewActivateAndDeactivate()
    {
        if (!OperatingSystem.IsWindows())
        {
            Assert.Ignore("MAUI lifecycle simulation is only available on Windows test runs.");
        }

        if (FindLifecycleMethod(typeof(TestPage), "SendAppearing") is null ||
            FindLifecycleMethod(typeof(TestView), "OnLoaded", "OnLoadedCore") is null)
        {
            Assert.Ignore("MAUI lifecycle hooks are not available on this target framework.");
        }

        new MauiTestEnvironment().Initialize();
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.RegisterConstant<IActivationForViewFetcher>(new ReactiveUI.Maui.ActivationForViewFetcher());

        using (resolver.WithResolver())
        {
            var page = new TestPage();
            var child = new TestView();
            var pageViewModel = new TestActivatableViewModel();
            var childViewModel = new TestActivatableViewModel();

            page.Content = child;
            page.ViewModel = pageViewModel;
            child.ViewModel = childViewModel;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(page.ActivationCount, Is.Zero);
                Assert.That(child.ActivationCount, Is.Zero);
            }

            try
            {
                TriggerPageAppearing(page);
                TriggerViewLoaded(child);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.StartsWith("Unable to locate lifecycle method", StringComparison.Ordinal))
                {
                    Assert.Ignore($"MAUI lifecycle hooks unavailable: {ex.Message}");
                }

                throw;
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(page.ActivationCount, Is.EqualTo(1));
                Assert.That(child.ActivationCount, Is.EqualTo(1));
                Assert.That(pageViewModel.ActivationCount, Is.EqualTo(1));
                Assert.That(childViewModel.ActivationCount, Is.EqualTo(1));
            }

            TriggerViewUnloaded(child);
            TriggerPageDisappearing(page);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(page.ActivationCount, Is.Zero);
                Assert.That(child.ActivationCount, Is.Zero);
                Assert.That(pageViewModel.ActivationCount, Is.Zero);
                Assert.That(childViewModel.ActivationCount, Is.Zero);
            }
        }
    }

    private static void TriggerPageAppearing(Page page) => InvokeLifecycleMethod(page, "SendAppearing");

    private static void TriggerPageDisappearing(Page page) => InvokeLifecycleMethod(page, "SendDisappearing");

    private static void TriggerViewLoaded(View view) => InvokeLifecycleMethod(view, "OnLoaded", "OnLoadedCore");

    private static void TriggerViewUnloaded(View view) => InvokeLifecycleMethod(view, "OnUnloaded", "OnUnloadedCore");

    private static void InvokeLifecycleMethod(object target, params string[] methodCandidates)
    {
        foreach (var name in methodCandidates)
        {
            var method = FindLifecycleMethod(target.GetType(), name);
            if (method is null)
            {
                continue;
            }

            var parameters = method.GetParameters();
            var args = parameters.Length switch
            {
                0 => [],
                1 when typeof(EventArgs).IsAssignableFrom(parameters[0].ParameterType) => new object?[] { EventArgs.Empty },
                _ => throw new InvalidOperationException($"Unsupported signature for lifecycle method '{name}'.")
            };

            method.Invoke(target, args);
            return;
        }

        throw new InvalidOperationException($"Unable to locate lifecycle method on {target.GetType().FullName}.");
    }

    private static MethodInfo? FindLifecycleMethod(Type type, params string[] names)
    {
        if (names.Length == 0)
        {
            return null;
        }

        foreach (var name in names)
        {
            var currentType = type;
            while (currentType is not null)
            {
                var method = currentType.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (method is not null)
                {
                    return method;
                }

                currentType = currentType.BaseType!;
            }
        }

        return null;
    }

    private sealed partial class TestPage : ReactiveContentPage<TestActivatableViewModel>, IActivatableView
    {
        public TestPage() => this.WhenActivated(d =>
                                      {
                                          ActivationCount++;
                                          d(Disposable.Create(() => ActivationCount--));
                                      });

        public int ActivationCount { get; private set; }
    }

    private sealed partial class TestView : ReactiveContentView<TestActivatableViewModel>, IActivatableView
    {
        public TestView() => this.WhenActivated(d =>
                                      {
                                          ActivationCount++;
                                          d(Disposable.Create(() => ActivationCount--));
                                      });

        public int ActivationCount { get; private set; }
    }

    private sealed partial class TestActivatableViewModel : ReactiveObject, IActivatableViewModel
    {
        public TestActivatableViewModel() => this.WhenActivated(d =>
                                                      {
                                                          ActivationCount++;
                                                          d(Disposable.Create(() => ActivationCount--));
                                                      });

        public ViewModelActivator Activator { get; } = new();

        public int ActivationCount { get; private set; }
    }
}
