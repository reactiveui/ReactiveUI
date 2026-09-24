// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Mocks;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for the WPF View Resolver.</summary>
public sealed class WpfViewDependencyResolverTests
{
    /// <summary>Tests that  Register views for view model should register all views.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<DispatcherThreadExecutor>]
    public async Task RegisterViewsForViewModelShouldRegisterAllViews()
    {
        using ModernDependencyResolver resolver = new();
        _ = resolver.CreateReactiveUIBuilder()
            .WithCoreServices()
            .WithWpf()
            .WithViewsFromAssembly(GetType().Assembly)
            .BuildApp();

        using (resolver.WithResolver())
        {
            await Assert.That(resolver.GetServices<IViewFor<ExampleWindowViewModel>>()).Count().IsEqualTo(1);
        }
    }
}
