// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;
using ReactiveUI.Tests.WinUI.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for <see cref="ReactivePage{TViewModel}"/>.</summary>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class ReactivePageTests
{
    /// <summary>Verifies the control is a page, so it can be navigated to by a frame.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_ProducesAPage()
    {
        var page = new TestPage();

        _ = await Assert.That(page).IsAssignableTo<Page>();
    }

    /// <summary>Verifies the view model is stored in the dependency property, so XAML can bind to it.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_IsBackedByTheDependencyProperty()
    {
        var viewModel = new PlainTestViewModel();
        var page = new TestPage { ViewModel = viewModel };

        await Assert.That(page.GetValue(ReactivePage<PlainTestViewModel>.ViewModelProperty))
            .IsSameReferenceAs(viewModel);
    }

    /// <summary>Verifies the binding root exposes the same view model as the typed accessor.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindingRoot_IsTheViewModel()
    {
        var viewModel = new PlainTestViewModel();
        var page = new TestPage { ViewModel = viewModel };

        await Assert.That(page.BindingRoot).IsSameReferenceAs(viewModel);
    }

    /// <summary>Verifies the non-generic accessor reads and writes the same view model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_IsSharedWithTheNonGenericAccessor()
    {
        var viewModel = new PlainTestViewModel();
        var page = new TestPage();

        ((IViewFor)page).ViewModel = viewModel;

        using (Assert.Multiple())
        {
            await Assert.That(page.ViewModel).IsSameReferenceAs(viewModel);
            await Assert.That(((IViewFor)page).ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Verifies the view model starts out unset.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_DefaultsToNull()
    {
        var page = new TestPage();

        await Assert.That(page.ViewModel).IsNull();
    }
}
