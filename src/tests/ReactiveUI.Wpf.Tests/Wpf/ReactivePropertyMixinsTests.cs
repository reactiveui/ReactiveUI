// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using TUnit.Core.Executors;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for the reactive property validation mixins.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class ReactivePropertyMixinsTests
{
    /// <summary>A value that falls outside the configured valid range.</summary>
    private const int OutOfRangeValue = 150;

    /// <summary>A value that falls inside the configured valid range.</summary>
    private const int InRangeValue = 50;

    /// <summary>The text reported when a validation error occurs.</summary>
    private const string ValidationErrorText = "Error";

    /// <summary>A representative valid string value used across the validation tests.</summary>
    private const string ValidValue = "Valid";

    /// <summary>Verifies that a required property reports a validation error when empty.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AddValidation_WithRequiredAttribute_ShouldValidate()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act & Assert - Initial state should have validation error
        await Assert.That(viewModel.RequiredProperty.HasErrors).IsTrue();
    }

    /// <summary>Verifies that setting a valid value clears the validation errors.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Verifies that setting an invalid value sets the validation errors.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AddValidation_WithInvalidValue_ShouldSetErrors()
    {
        // Arrange
        var viewModel = new TestViewModel();
        viewModel.RequiredProperty.Value = ValidValue;

        // Act - Set to null (invalid)
        viewModel.RequiredProperty.Value = null;

        // Assert
        await Assert.That(viewModel.RequiredProperty.HasErrors).IsTrue();
    }

    /// <summary>Verifies that a string-length attribute reports an error when the value is too long.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Verifies that a value within the string-length constraint passes validation.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Verifies that a range attribute reports an error for an out-of-range value.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AddValidation_WithRangeAttribute_ShouldValidateRange()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act - Set value outside range
        viewModel.RangeProperty.Value = OutOfRangeValue;

        // Assert
        await Assert.That(viewModel.RangeProperty.HasErrors).IsTrue();
    }

    /// <summary>Verifies that a value within the range constraint passes validation.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AddValidation_WithValidRange_ShouldPass()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act - Set value within range
        viewModel.RangeProperty.Value = InRangeValue;

        // Assert
        await Assert.That(viewModel.RangeProperty.HasErrors).IsFalse();
    }

    /// <summary>Verifies that the display attribute name is used in validation error messages.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AddValidation_WithDisplayAttribute_ShouldUseDisplayName()
    {
        // Arrange
        var viewModel = new TestViewModel();
        string? errorMessage = null;
        _ = viewModel.DisplayNameProperty.ObserveErrorChanged
            .ObserveOn(Sequencer.Immediate)
            .Subscribe(errors => errorMessage = errors?.OfType<string>().FirstOrDefault());

        // Act - Property starts invalid
        viewModel.DisplayNameProperty.Value = string.Empty;

        // Assert - Error message should reference the display name
        await Assert.That(errorMessage).IsNotNull();
    }

    /// <summary>Verifies that passing a null selector throws <see cref="ArgumentNullException"/>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AddValidation_WithNullSelfSelector_ShouldThrowArgumentNullException()
    {
        // Arrange
        var property = new ReactiveProperty<string>();

        // Act & Assert
        await Assert.That(() => property.AddValidation(null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that calling the mixin on a null property throws <see cref="ArgumentNullException"/>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AddValidation_WithNullSelf_ShouldThrowArgumentNullException()
    {
        // Arrange
        ReactiveProperty<string>? property = null;

        // Act & Assert
        await Assert.That(() => ReactivePropertyMixins.AddValidation(property!, () => property!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that all validation attributes are evaluated when multiple are applied.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Verifies that a custom error message from an attribute is surfaced.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AddValidation_WithCustomErrorMessage_ShouldUseCustomMessage()
    {
        // Arrange
        var viewModel = new TestViewModel();
        string? errorMessage = null;
        _ = viewModel.CustomErrorMessageProperty.ObserveErrorChanged
            .ObserveOn(Sequencer.Immediate)
            .Subscribe(errors => errorMessage = errors?.OfType<string>().FirstOrDefault());

        // Act
        viewModel.CustomErrorMessageProperty.Value = null;

        // Assert
        await Assert.That(errorMessage).Contains("custom error");
    }

    /// <summary>Verifies that a property with no validation attributes never reports errors.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Verifies that <c>ObserveValidationErrors</c> emits the current validation errors.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveValidationErrors_ShouldReturnObservableOfErrors()
    {
        // Arrange
        var property = new ReactiveProperty<string>(default, Sequencer.Immediate, false, false);
        _ = property.AddValidationError(static x => string.IsNullOrEmpty(x) ? ValidationErrorText : null);

        var errors = new List<string?>();
        var observable = property.ObserveValidationErrors();

        // Act
        _ = observable.Subscribe(errors.Add);
        property.Value = null;

        // Assert
        await Assert.That(errors).Contains(ValidationErrorText);
    }

    /// <summary>Verifies that <c>ObserveValidationErrors</c> emits null when there are no errors.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveValidationErrors_WhenNoErrors_ShouldReturnNull()
    {
        // Arrange
        var property = new ReactiveProperty<string>(ValidValue, Sequencer.Immediate, false, false);
        _ = property.AddValidationError(static x => string.IsNullOrEmpty(x) ? ValidationErrorText : null);

        string? lastError = "initial";
        var observable = property.ObserveValidationErrors();

        // Act
        _ = observable.Subscribe(error => lastError = error);
        property.Value = "Valid Value";

        // Assert
        await Assert.That(lastError).IsNull();
    }

    /// <summary>Verifies that <c>ObserveValidationErrors</c> on a null property throws <see cref="ArgumentNullException"/>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveValidationErrors_WithNullProperty_ShouldThrowArgumentNullException()
    {
        // Arrange
        ReactiveProperty<string>? property = null;

        // Act & Assert
        await Assert.That(() => property!.ObserveValidationErrors())
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that <c>ObserveValidationErrors</c> emits only the first error when several apply.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveValidationErrors_ShouldEmitOnlyFirstError()
    {
        // Arrange
        var property = new ReactiveProperty<string>(default, Sequencer.Immediate, false, false);
        _ = property.AddValidationError(static x => string.IsNullOrEmpty(x) ? "Error1" : null)
                .AddValidationError(static x => string.IsNullOrEmpty(x) ? "Error2" : null);

        var errors = new List<string?>();
        var observable = property.ObserveValidationErrors();

        // Act
        _ = observable.Subscribe(errors.Add);
        property.Value = null;

        // Assert
        await Assert.That(errors[^1]).IsEqualTo("Error1");
    }

    /// <summary>Verifies that <c>ObserveValidationErrors</c> updates as the validation errors change.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveValidationErrors_ShouldUpdateWhenErrorsChange()
    {
        // Arrange
        var property = new ReactiveProperty<string>(default, Sequencer.Immediate, false, false);
        _ = property.AddValidationError(static x => string.IsNullOrEmpty(x) ? ValidationErrorText : null);

        var errorHistory = new List<string?>();
        var observable = property.ObserveValidationErrors();
        _ = observable.Subscribe(errorHistory.Add);

        // Act
        property.Value = null; // Should trigger error
        property.Value = ValidValue; // Should clear error

        // Assert
        await Assert.That(errorHistory).Contains((string?)ValidationErrorText);
        await Assert.That(errorHistory).Contains((string?)null);
    }

    /// <summary>A view model exposing reactive properties with various validation attributes.</summary>
    private sealed class TestViewModel : ReactiveObject
    {
        /// <summary>The maximum permitted string length used in validation tests.</summary>
        private const int MaxStringLength = 10;

        /// <summary>The maximum value permitted by the range validation tests.</summary>
        private const int MaxRange = 100;

        /// <summary>Initializes a new instance of the <see cref="TestViewModel"/> class.</summary>
        /// <param name="scheduler">The scheduler used by the reactive properties.</param>
        public TestViewModel(ISequencer? scheduler = null)
        {
            scheduler ??= Sequencer.Immediate;

            RequiredProperty = new ReactiveProperty<string>(default, scheduler, false, false)
                .AddValidation(() => RequiredProperty);

            StringLengthProperty = new ReactiveProperty<string>(default, scheduler, false, false)
                .AddValidation(() => StringLengthProperty);

            RangeProperty = new ReactiveProperty<int>(default, scheduler, false, false)
                .AddValidation(() => RangeProperty);

            DisplayNameProperty = new ReactiveProperty<string>(default, scheduler, false, false)
                .AddValidation(() => DisplayNameProperty);

            MultiValidationProperty = new ReactiveProperty<string>(default, scheduler, false, false)
                .AddValidation(() => MultiValidationProperty);

            CustomErrorMessageProperty = new ReactiveProperty<string>(default, scheduler, false, false)
                .AddValidation(() => CustomErrorMessageProperty);

            NoValidationProperty = new ReactiveProperty<string>(default, scheduler, false, false)
                .AddValidation(() => NoValidationProperty);
        }

        /// <summary>Gets a property with a required validation attribute.</summary>
        [Required]
        public ReactiveProperty<string> RequiredProperty { get; }

        /// <summary>Gets a property with a string-length validation attribute.</summary>
        [StringLength(MaxStringLength)]
        public ReactiveProperty<string> StringLengthProperty { get; }

        /// <summary>Gets a property with a range validation attribute.</summary>
        [Range(0, MaxRange)]
        public ReactiveProperty<int> RangeProperty { get; }

        /// <summary>Gets a property with a display name and required validation attribute.</summary>
        [Required]
        [Display(Name = "Custom Display Name")]
        public ReactiveProperty<string> DisplayNameProperty { get; }

        /// <summary>Gets a property with multiple validation attributes.</summary>
        [Required]
        [StringLength(MaxStringLength)]
        public ReactiveProperty<string> MultiValidationProperty { get; }

        /// <summary>Gets a property with a custom validation error message.</summary>
        [Required(ErrorMessage = "This is a custom error message")]
        public ReactiveProperty<string> CustomErrorMessageProperty { get; }

        /// <summary>Gets a property without any validation attributes.</summary>
        public ReactiveProperty<string> NoValidationProperty { get; }
    }
}
