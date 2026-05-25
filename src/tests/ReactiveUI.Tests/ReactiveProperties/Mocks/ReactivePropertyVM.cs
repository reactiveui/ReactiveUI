// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.Reactive.Concurrency;
using ReactiveUI.Tests.Properties;

namespace ReactiveUI.Tests.ReactiveProperties.Mocks;

/// <summary>
/// A view model used to exercise <see cref="ReactiveProperty{T}"/> validation scenarios.
/// </summary>
public class ReactivePropertyVm : ReactiveObject
{
    /// <summary>
    /// The maximum allowed length for <see cref="LengthLessThanFiveProperty"/>.
    /// </summary>
    private const int MaxLength = 5;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePropertyVm"/> class using the default scheduler.
    /// </summary>
    public ReactivePropertyVm()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePropertyVm"/> class.
    /// </summary>
    /// <param name="scheduler">The scheduler used by the reactive properties.</param>
    public ReactivePropertyVm(IScheduler? scheduler)
    {
        scheduler ??= ImmediateScheduler.Instance;

        IsRequiredProperty = new ReactiveProperty<string>(null, scheduler, false, false).AddValidation(() => IsRequiredProperty);

        LengthLessThanFiveProperty = new ReactiveProperty<string>(null, scheduler, false, false).AddValidation(() => LengthLessThanFiveProperty)
            .AddValidationError(s => string.IsNullOrWhiteSpace(s) ? "required" : null);

        TaskValidationTestProperty = new ReactiveProperty<string>(null, scheduler, false, false)
            .AddValidationError(s => Task.FromResult(string.IsNullOrWhiteSpace(s) ? "required" : null));

        CustomValidationErrorMessageProperty = new ReactiveProperty<string>(null, scheduler, false, false).AddValidation(() => CustomValidationErrorMessageProperty);

        CustomValidationErrorMessageWithDisplayNameProperty =
            new ReactiveProperty<string>(null, scheduler, false, false).AddValidation(() => CustomValidationErrorMessageWithDisplayNameProperty);

        CustomValidationErrorMessageWithResourceProperty =
            new ReactiveProperty<string>(null, scheduler, false, false).AddValidation(() => CustomValidationErrorMessageWithResourceProperty);
    }

    /// <summary>
    /// Gets a property that validates with a custom error message.
    /// </summary>
    [Required(ErrorMessage = "Custom validation error message for {0}")]
    public ReactiveProperty<string> CustomValidationErrorMessageProperty { get; }

    /// <summary>
    /// Gets a property that validates with a custom error message using a display name.
    /// </summary>
    [Required(ErrorMessage = "Custom validation error message for {0}")]
    [Display(Name = "CustomName")]
    public ReactiveProperty<string> CustomValidationErrorMessageWithDisplayNameProperty { get; }

    /// <summary>
    /// Gets a property that validates with a custom error message sourced from a resource.
    /// </summary>
    [Required(
        ErrorMessageResourceType = typeof(Resources),
        ErrorMessageResourceName = nameof(Resources.ValidationErrorMessage))]
    [Display(ResourceType = typeof(Resources), Name = nameof(Resources.ValidationTargetPropertyName))]
    public ReactiveProperty<string> CustomValidationErrorMessageWithResourceProperty { get; }

    /// <summary>
    /// Gets a property that is required.
    /// </summary>
    [Required(ErrorMessage = "error!")]
    public ReactiveProperty<string> IsRequiredProperty { get; }

    /// <summary>
    /// Gets a property whose length must be less than five characters.
    /// </summary>
    [StringLength(MaxLength, ErrorMessage = "5over")]
    public ReactiveProperty<string> LengthLessThanFiveProperty { get; }

    /// <summary>
    /// Gets a property validated by an asynchronous validation task.
    /// </summary>
    public ReactiveProperty<string> TaskValidationTestProperty { get; }
}
