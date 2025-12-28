// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace ReactiveUI.Tests.Wpf;

public class ReactivePropertyMixinsTests
{
    [Test]
    public async Task AddValidation_WithRequiredAttribute_ShouldValidate()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act & Assert - Initial state should have validation error
        await Assert.That(viewModel.RequiredProperty.HasErrors).IsTrue();
    }

    [Test]
    public async Task AddValidation_WithValidValue_ShouldClearErrors()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act
        viewModel.RequiredProperty.Value = "Valid Value";

        // Assert
        await Assert.That(viewModel.RequiredProperty.HasErrors).IsFalse();
    }

    [Test]
    public async Task AddValidation_WithInvalidValue_ShouldSetErrors()
    {
        // Arrange
        var viewModel = new TestViewModel();
        viewModel.RequiredProperty.Value = "Valid";

        // Act - Set to null (invalid)
        viewModel.RequiredProperty.Value = null;

        // Assert
        await Assert.That(viewModel.RequiredProperty.HasErrors).IsTrue();
    }

    [Test]
    public async Task AddValidation_WithStringLengthAttribute_ShouldValidateLength()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act - Set value longer than max length
        viewModel.StringLengthProperty.Value = "This is a very long string that exceeds the limit";

        // Assert
        await Assert.That(viewModel.StringLengthProperty.HasErrors).IsTrue();
    }

    [Test]
    public async Task AddValidation_WithValidStringLength_ShouldPass()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act - Set value within length constraint
        viewModel.StringLengthProperty.Value = "Short";

        // Assert
        await Assert.That(viewModel.StringLengthProperty.HasErrors).IsFalse();
    }

    [Test]
    public async Task AddValidation_WithRangeAttribute_ShouldValidateRange()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act - Set value outside range
        viewModel.RangeProperty.Value = 150;

        // Assert
        await Assert.That(viewModel.RangeProperty.HasErrors).IsTrue();
    }

    [Test]
    public async Task AddValidation_WithValidRange_ShouldPass()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act - Set value within range
        viewModel.RangeProperty.Value = 50;

        // Assert
        await Assert.That(viewModel.RangeProperty.HasErrors).IsFalse();
    }

    [Test]
    public async Task AddValidation_WithDisplayAttribute_ShouldUseDisplayName()
    {
        // Arrange
        var viewModel = new TestViewModel();
        string? errorMessage = null;
        viewModel.DisplayNameProperty.ObserveErrorChanged
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(errors => errorMessage = errors?.OfType<string>().FirstOrDefault());

        // Act - Property starts invalid
        viewModel.DisplayNameProperty.Value = string.Empty;

        // Assert - Error message should reference the display name
        await Assert.That(errorMessage).IsNotNull();
    }

    [Test]
    public async Task AddValidation_WithNullSelfSelector_ShouldThrowArgumentNullException()
    {
        // Arrange
        var property = new ReactiveProperty<string>();

        // Act & Assert
        await Assert.That(() => property.AddValidation(null!))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task AddValidation_WithNullSelf_ShouldThrowArgumentNullException()
    {
        // Arrange
        ReactiveProperty<string>? property = null;

        // Act & Assert
        await Assert.That(() => ReactivePropertyMixins.AddValidation(property!, () => property!))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task AddValidation_WithMultipleValidationAttributes_ShouldValidateAll()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act - Set value that violates multiple rules
        viewModel.MultiValidationProperty.Value = string.Empty;

        // Assert
        await Assert.That(viewModel.MultiValidationProperty.HasErrors).IsTrue();
    }

    [Test]
    public async Task AddValidation_WithCustomErrorMessage_ShouldUseCustomMessage()
    {
        // Arrange
        var viewModel = new TestViewModel();
        string? errorMessage = null;
        viewModel.CustomErrorMessageProperty.ObserveErrorChanged
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(errors => errorMessage = errors?.OfType<string>().FirstOrDefault());

        // Act
        viewModel.CustomErrorMessageProperty.Value = null;

        // Assert
        await Assert.That(errorMessage).Contains("custom error");
    }

    [Test]
    public async Task AddValidation_WithNoValidationAttributes_ShouldNotAddValidation()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act - Set any value
        viewModel.NoValidationProperty.Value = string.Empty;

        // Assert - Should have no errors
        await Assert.That(viewModel.NoValidationProperty.HasErrors).IsFalse();
    }

    [Test]
    public async Task ObserveValidationErrors_ShouldReturnObservableOfErrors()
    {
        // Arrange
        var property = new ReactiveProperty<string>();
        property.AddValidationError(x => string.IsNullOrEmpty(x) ? "Error" : null);

        var errors = new List<string?>();
        var observable = property.ObserveValidationErrors();

        // Act
        observable.ObserveOn(ImmediateScheduler.Instance)
                  .Subscribe(error => errors.Add(error));
        property.Value = null;

        // Assert
        await Assert.That(errors).Contains("Error");
    }

    [Test]
    public async Task ObserveValidationErrors_WhenNoErrors_ShouldReturnNull()
    {
        // Arrange
        var property = new ReactiveProperty<string>("Valid");
        property.AddValidationError(x => string.IsNullOrEmpty(x) ? "Error" : null);

        string? lastError = "initial";
        var observable = property.ObserveValidationErrors();

        // Act
        observable.ObserveOn(ImmediateScheduler.Instance)
                  .Subscribe(error => lastError = error);
        property.Value = "Valid Value";

        // Assert
        await Assert.That(lastError).IsNull();
    }

    [Test]
    public async Task ObserveValidationErrors_WithNullProperty_ShouldThrowArgumentNullException()
    {
        // Arrange
        ReactiveProperty<string>? property = null;

        // Act & Assert
        await Assert.That(() => property!.ObserveValidationErrors())
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task ObserveValidationErrors_ShouldEmitOnlyFirstError()
    {
        // Arrange
        var property = new ReactiveProperty<string>();
        property.AddValidationError(x => string.IsNullOrEmpty(x) ? "Error1" : null)
                .AddValidationError(x => string.IsNullOrEmpty(x) ? "Error2" : null);

        var errors = new List<string?>();
        var observable = property.ObserveValidationErrors();

        // Act
        observable.ObserveOn(ImmediateScheduler.Instance)
                  .Subscribe(error => errors.Add(error));
        property.Value = null;

        // Assert
        await Assert.That(errors.Last()).IsEqualTo("Error1");
    }

    [Test]
    public async Task ObserveValidationErrors_ShouldUpdateWhenErrorsChange()
    {
        // Arrange
        var property = new ReactiveProperty<string>();
        property.AddValidationError(x => string.IsNullOrEmpty(x) ? "Error" : null);

        var errorHistory = new List<string?>();
        var observable = property.ObserveValidationErrors();
        observable.ObserveOn(ImmediateScheduler.Instance)
                  .Subscribe(error => errorHistory.Add(error));

        // Act
        property.Value = null;  // Should trigger error
        property.Value = "Valid";  // Should clear error

        // Assert
        await Assert.That(errorHistory).Contains((string?)"Error");
        await Assert.That(errorHistory).Contains((string?)null);
    }

    private class TestViewModel : ReactiveObject
    {
        public TestViewModel()
        {
            RequiredProperty = new ReactiveProperty<string>()
                .AddValidation(() => RequiredProperty);

            StringLengthProperty = new ReactiveProperty<string>()
                .AddValidation(() => StringLengthProperty);

            RangeProperty = new ReactiveProperty<int>()
                .AddValidation(() => RangeProperty);

            DisplayNameProperty = new ReactiveProperty<string>()
                .AddValidation(() => DisplayNameProperty);

            MultiValidationProperty = new ReactiveProperty<string>()
                .AddValidation(() => MultiValidationProperty);

            CustomErrorMessageProperty = new ReactiveProperty<string>()
                .AddValidation(() => CustomErrorMessageProperty);

            NoValidationProperty = new ReactiveProperty<string>()
                .AddValidation(() => NoValidationProperty);
        }

        [Required]
        public ReactiveProperty<string> RequiredProperty { get; }

        [StringLength(10)]
        public ReactiveProperty<string> StringLengthProperty { get; }

        [Range(0, 100)]
        public ReactiveProperty<int> RangeProperty { get; }

        [Required]
        [Display(Name = "Custom Display Name")]
        public ReactiveProperty<string> DisplayNameProperty { get; }

        [Required]
        [StringLength(10)]
        public ReactiveProperty<string> MultiValidationProperty { get; }

        [Required(ErrorMessage = "This is a custom error message")]
        public ReactiveProperty<string> CustomErrorMessageProperty { get; }

        public ReactiveProperty<string> NoValidationProperty { get; }
    }
}
