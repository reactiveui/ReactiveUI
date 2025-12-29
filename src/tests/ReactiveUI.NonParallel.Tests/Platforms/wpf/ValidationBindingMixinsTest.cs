// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="ValidationBindingMixins"/>.
/// </summary>
[NotInParallel]
public class ValidationBindingMixinsTest
{
    /// <summary>
    /// Tests that BindWithValidation throws ArgumentNullException when viewModel property selector is null.
    /// </summary>
    [Test]
    public void BindWithValidation_ThrowsArgumentNullException_WhenViewModelPropertySelectorIsNull()
    {
        var view = new TestView();
        var viewModel = new TestViewModel();

        Assert.Throws<ArgumentNullException>(() =>
            ValidationBindingMixins.BindWithValidation<TestViewModel, TestView, TextBox, string>(
                view, viewModel, null!, v => v.TextBox));
    }

    /// <summary>
    /// Tests that BindWithValidation throws ArgumentNullException when framework element selector is null.
    /// </summary>
    [Test]
    public void BindWithValidation_ThrowsArgumentNullException_WhenFrameworkElementSelectorIsNull()
    {
        var view = new TestView();
        var viewModel = new TestViewModel();

        Assert.Throws<ArgumentNullException>(() =>
            ValidationBindingMixins.BindWithValidation<TestViewModel, TestView, string, string>(view, viewModel, vm => vm.Name, null!));
    }

    /// <summary>
    /// Tests that BindWithValidation creates a binding.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithValidation_CreatesBinding()
    {
        var view = new TestView { ViewModel = new TestViewModel() };
        view.TextBox.Name = "TextBox";

        using var binding = view.BindWithValidation(view.ViewModel!, vm => vm.Name, v => v.TextBox.Text);

        await Assert.That(binding).IsNotNull();
    }

    /// <summary>
    /// Test view for testing.
    /// </summary>
    private class TestView : UserControl, IViewFor<TestViewModel>
    {
        public TestView()
        {
            TextBox = new TextBox();
            Content = TextBox;
        }

        public TextBox TextBox { get; }

        public TestViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TestViewModel;
        }
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel : ReactiveObject
    {
        private string? _name;

        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
    }
}
