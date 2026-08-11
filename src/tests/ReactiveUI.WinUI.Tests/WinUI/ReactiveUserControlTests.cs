// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;
using ReactiveUI.Tests.WinUI.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for <see cref="ReactiveUserControl{TViewModel}"/>.</summary>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class ReactiveUserControlTests
{
    /// <summary>Verifies the control is a user control, so it can be used as a XAML document root.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_ProducesAUserControl()
    {
        var control = new TestUserControl();

        _ = await Assert.That(control).IsAssignableTo<UserControl>();
    }

    /// <summary>Verifies the view model is stored in the dependency property, so XAML can bind to it.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_IsBackedByTheDependencyProperty()
    {
        var viewModel = new PlainTestViewModel();
        var control = new TestUserControl { ViewModel = viewModel };

        await Assert.That(control.GetValue(ReactiveUserControl<PlainTestViewModel>.ViewModelProperty))
            .IsSameReferenceAs(viewModel);
    }

    /// <summary>Verifies the binding root exposes the same view model as the typed accessor.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindingRoot_IsTheViewModel()
    {
        var viewModel = new PlainTestViewModel();
        var control = new TestUserControl { ViewModel = viewModel };

        await Assert.That(control.BindingRoot).IsSameReferenceAs(viewModel);
    }

    /// <summary>Verifies the non-generic accessor reads and writes the same view model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_IsSharedWithTheNonGenericAccessor()
    {
        var viewModel = new PlainTestViewModel();
        var control = new TestUserControl();

        ((IViewFor)control).ViewModel = viewModel;

        using (Assert.Multiple())
        {
            await Assert.That(control.ViewModel).IsSameReferenceAs(viewModel);
            await Assert.That(((IViewFor)control).ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Verifies the view model starts out unset.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_DefaultsToNull()
    {
        var control = new TestUserControl();

        await Assert.That(control.ViewModel).IsNull();
    }
}
