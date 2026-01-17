// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using ReactiveUI.Tests.Properties;

namespace ReactiveUI.Tests.ReactiveProperties.Mocks;

public class ReactivePropertyVM : ReactiveObject
{
    public ReactivePropertyVM(IScheduler? scheduler = null)
    {
        scheduler ??= ImmediateScheduler.Instance;

        IsRequiredProperty = new ReactiveProperty<string>(default, scheduler, false, false)
            .AddValidation(() => IsRequiredProperty);

        LengthLessThanFiveProperty = new ReactiveProperty<string>(default, scheduler, false, false)
            .AddValidation(() => LengthLessThanFiveProperty)
            .AddValidationError(s => string.IsNullOrWhiteSpace(s) ? "required" : null);

        TaskValidationTestProperty = new ReactiveProperty<string>(default, scheduler, false, false)
            .AddValidationError(async s => await Task.FromResult(string.IsNullOrWhiteSpace(s) ? "required" : null));

        CustomValidationErrorMessageProperty = new ReactiveProperty<string>(default, scheduler, false, false)
            .AddValidation(() => CustomValidationErrorMessageProperty);

        CustomValidationErrorMessageWithDisplayNameProperty =
            new ReactiveProperty<string>(default, scheduler, false, false)
                .AddValidation(() => CustomValidationErrorMessageWithDisplayNameProperty);

        CustomValidationErrorMessageWithResourceProperty =
            new ReactiveProperty<string>(default, scheduler, false, false)
                .AddValidation(() => CustomValidationErrorMessageWithResourceProperty);
    }

    [Required(ErrorMessage = "Custom validation error message for {0}")]
    public ReactiveProperty<string> CustomValidationErrorMessageProperty { get; }

    [Required(ErrorMessage = "Custom validation error message for {0}")]
    [Display(Name = "CustomName")]
    public ReactiveProperty<string> CustomValidationErrorMessageWithDisplayNameProperty { get; }

    [Required(
        ErrorMessageResourceType = typeof(Resources),
        ErrorMessageResourceName = nameof(Resources.ValidationErrorMessage))]
    [Display(ResourceType = typeof(Resources), Name = nameof(Resources.ValidationTargetPropertyName))]
    public ReactiveProperty<string> CustomValidationErrorMessageWithResourceProperty { get; }

    [Required(ErrorMessage = "error!")]
    public ReactiveProperty<string> IsRequiredProperty { get; }

    [StringLength(5, ErrorMessage = "5over")]
    public ReactiveProperty<string> LengthLessThanFiveProperty { get; }

    public ReactiveProperty<string> TaskValidationTestProperty { get; }
}
