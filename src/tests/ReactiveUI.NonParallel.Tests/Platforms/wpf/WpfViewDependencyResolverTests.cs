// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for the WPF View Resolver.
/// </summary>
/// <seealso cref="System.IDisposable" />
public sealed class WpfViewDependencyResolverTests : IDisposable
{
    private readonly IDependencyResolver _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfViewDependencyResolverTests"/> class.
    /// </summary>
    public WpfViewDependencyResolverTests()
    {
        _resolver = new ModernDependencyResolver();
        _resolver.InitializeSplat();
        _resolver.InitializeReactiveUI();
        _resolver.RegisterViewsForViewModels(GetType().Assembly);
    }

    /// <summary>
    /// Tests that  Register views for view model should register all views.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task RegisterViewsForViewModelShouldRegisterAllViews()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(_resolver.GetServices<IViewFor<ExampleWindowViewModel>>()).Count().IsEqualTo(1);
        }
    }

    /// <inheritdoc/>
    public void Dispose() => _resolver?.Dispose();
}
