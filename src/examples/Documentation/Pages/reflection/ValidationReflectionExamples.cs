// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Reflection;

/// <summary>
/// Shows <c>AddValidation(Expression)</c>: it reads the <c>[Required]</c>, <c>[StringLength]</c> and similar
/// DataAnnotations attributes off the property the expression names, by reflection. Calling
/// <c>AddValidationError</c> with a plain delegate, as the <c>reactive-property</c> page shows, checks the same rule
/// without reflecting over any attribute.
/// </summary>
public static class ValidationReflectionExamples
{
    /// <summary>An empty title fails the <c>[Required]</c> attribute discovered on <see cref="PlaylistTrackFormViewModel.Title"/>.</summary>
    public static void RequiredAttributeFailsForAnEmptyTitle()
    {
        PlaylistTrackFormViewModel form = new();

        form.Title.Value = "Africa";
        Console.WriteLine(form.Title.HasErrors);

        string? latestError = null;
        using IDisposable subscription = form.Title.ObserveValidationErrors().Subscribe(error => latestError = error);

        form.Title.Value = string.Empty;
        Console.WriteLine(form.Title.HasErrors);
        Console.WriteLine(latestError);

        form.Title.Value = "Clocks";
        Console.WriteLine(form.Title.HasErrors);

        // Output:
        // False
        // True
        // A track needs a title.
        // False
    }
}
