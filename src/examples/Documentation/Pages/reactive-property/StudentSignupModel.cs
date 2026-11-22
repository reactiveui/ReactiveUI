// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using ReactiveUI.SourceGenerators;

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>A signup form whose DataAnnotations rules are wired at compile time by <see cref="ReactiveValidationAttribute"/>.</summary>
[ReactiveValidation]
[System.Diagnostics.DebuggerDisplay("{Name.Value} ({Age.Value})")]
public sealed partial class StudentSignupModel : IDisposable
{
    /// <summary>Maximum allowed length for the student's name.</summary>
    private const int MaxNameLength = 50;

    /// <summary>Minimum allowed length for the student's name.</summary>
    private const int MinNameLength = 3;

    /// <summary>Minimum allowed age for club signup.</summary>
    private const int MinAge = 8;

    /// <summary>Maximum allowed age for club signup.</summary>
    private const int MaxAge = 18;

    /// <summary>Initial invalid age used to demonstrate initial validation.</summary>
    private const int InitialAge = 5;

    /// <summary>Initializes a new instance of the <see cref="StudentSignupModel"/> class and wires generated validation.</summary>
    public StudentSignupModel() => ConfigureValidation();

    /// <summary>Gets the student's name property validated with <c>[Required]</c> and <c>[StringLength]</c>.</summary>
    [Display(Name = "Student Name")]
    [Required(ErrorMessage = "Enter the student's name.")]
    [StringLength(MaxNameLength, MinimumLength = MinNameLength)]
    public ReactiveProperty<string> Name { get; } = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);

    /// <summary>Gets the student's age property validated with <c>[Range]</c>.</summary>
    [Range(MinAge, MaxAge)]
    public ReactiveProperty<int> Age { get; } = new(InitialAge, RxSchedulers.MainThreadScheduler, false, false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Name.Dispose();
        Age.Dispose();
    }
}
