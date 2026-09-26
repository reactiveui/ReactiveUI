// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Builder.Tests.Executors;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests;

/// <summary>Tests that <see cref="ReactiveUIBuilder.ConfigureViewLocator"/> configures the one locator the app resolves.</summary>
[NotInParallel]
[TestExecutor<ResetOnlyExecutor>]
public class ReactiveUIBuilderViewLocatorTests
{
    /// <summary>Verifies that the configured locator is a single instance and the configure action runs once.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ConfigureViewLocator_configures_one_locator_instance()
    {
        var configureCalls = 0;
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = builder.WithCoreServices();
        _ = builder
            .ConfigureViewLocator(_ => configureCalls++)
            .BuildApp();

        var first = Locator.Current.GetService<IViewLocator>();
        var second = Locator.Current.GetService<IViewLocator>();

        using (Assert.Multiple())
        {
            await Assert.That(first).IsAssignableTo<DefaultViewLocator>();
            await Assert.That(second).IsSameReferenceAs(first);
            await Assert.That(configureCalls).IsEqualTo(1);
        }
    }

    /// <summary>Verifies that mappings from <c>ConfigureViewLocator</c> and <c>RegisterViews</c> both reach the resolved locator.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ConfigureViewLocator_keeps_mappings_added_by_RegisterViews()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = builder.WithCoreServices();
        _ = builder
            .ConfigureViewLocator(static locator => locator.Map<ConfiguredViewModel, ConfiguredView>())
            .RegisterViews(static views => views.Map<MappedViewModel, MappedView>())
            .BuildApp();

        var locator = (DefaultViewLocator)Locator.Current.GetService<IViewLocator>()!;

        using (Assert.Multiple())
        {
            await Assert.That(locator.ResolveView<ConfiguredViewModel>()).IsTypeOf<ConfiguredView>();
            await Assert.That(locator.ResolveView<MappedViewModel>()).IsTypeOf<MappedView>();
        }
    }

    /// <summary>Executor that only resets state, leaving builder configuration to each test.</summary>
    internal sealed class ResetOnlyExecutor : BuilderTestExecutorBase
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder()
        {
            // Tests in this class configure and build the builder themselves.
        }
    }

    /// <summary>View model mapped through <c>ConfigureViewLocator</c>.</summary>
    [SuppressMessage("Major Code Smell", "SST1436:Classes should not be empty", Justification = "Marker type for tests.")]
    internal sealed class ConfiguredViewModel : ReactiveObject;

    /// <summary>View model mapped through <c>RegisterViews</c>.</summary>
    [SuppressMessage("Major Code Smell", "SST1436:Classes should not be empty", Justification = "Marker type for tests.")]
    internal sealed class MappedViewModel : ReactiveObject;

    /// <summary>View for <see cref="ConfiguredViewModel"/>.</summary>
    internal sealed class ConfiguredView : IViewFor<ConfiguredViewModel>
    {
        /// <inheritdoc/>
        public ConfiguredViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ConfiguredViewModel?)value;
        }
    }

    /// <summary>View for <see cref="MappedViewModel"/>.</summary>
    internal sealed class MappedView : IViewFor<MappedViewModel>
    {
        /// <inheritdoc/>
        public MappedViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MappedViewModel?)value;
        }
    }
}
