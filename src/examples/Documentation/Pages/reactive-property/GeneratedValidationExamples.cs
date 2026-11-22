// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>
/// Shows compile-time DataAnnotations validation on <see cref="ReactiveProperty{T}"/> using
/// <c>[ReactiveValidation]</c> and its generated <c>ConfigureValidation()</c> method.
/// </summary>
public static class GeneratedValidationExamples
{
    /// <summary>Valid age assigned after checking the initial validation errors.</summary>
    private const int ValidAge = 12;

    /// <summary>Verifies generated DataAnnotations validation runs without reflection and clears on valid values.</summary>
    public static void ValidateWithSourceGeneratedAttributes()
    {
        using StudentSignupModel model = new();

        Console.WriteLine(model.Name.HasErrors);
        Console.WriteLine(model.Age.HasErrors);

        model.Name.Value = "Ada Lovelace";
        model.Age.Value = ValidAge;

        Console.WriteLine(model.Name.HasErrors);
        Console.WriteLine(model.Age.HasErrors);

        // Output:
        // True
        // True
        // False
        // False
    }
}
