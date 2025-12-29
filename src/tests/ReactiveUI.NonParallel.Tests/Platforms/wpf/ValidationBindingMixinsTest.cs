// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Windows.Controls;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="ValidationBindingMixins"/>.
/// </summary>
[NotInParallel]
public class ValidationBindingMixinsTest
{
    /// <summary>
    /// Tests that BindWithValidation throws ArgumentNullException when viewModelPropertySelector is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithValidation_ThrowsOnNullViewModelProperty()
    {
        var view = new TestView();
        var viewModel = new TestViewModel();

        await Assert.That(() => view.BindWithValidation<TestViewModel, TestView, Control, string>(
            viewModel,
            null!,
            v => v.TestControl))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that BindWithValidation throws ArgumentNullException when frameworkElementSelector is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithValidation_ThrowsOnNullFrameworkElementSelector()
    {
        var view = new TestView();
        var viewModel = new TestViewModel();

        await Assert.That(() => view.BindWithValidation<TestViewModel, TestView, Control, string>(
            viewModel,
            vm => vm.TestProperty,
            null!))
            .Throws<ArgumentNullException>();
    }

    private class TestView : Control, IViewFor<TestViewModel>
    {
        public TestViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TestViewModel;
        }

        public Control TestControl { get; } = new();
    }

    private class TestViewModel
    {
        public string? TestProperty { get; set; }
    }
}
