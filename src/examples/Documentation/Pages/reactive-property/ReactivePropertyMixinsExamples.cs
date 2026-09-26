// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>Shows the <see cref="ReactivePropertyMixins"/> extension members.</summary>
public static class ReactivePropertyMixinsExamples
{
    /// <summary><c>ObserveValidationErrors</c> narrows the error collection down to its first string message.</summary>
    public static void ObserveValidationErrorsAsAString()
    {
        ReactiveProperty<string> studentName = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);
        _ = studentName.AddValidationError(static name => string.IsNullOrWhiteSpace(name) ? "Enter the student's name." : null);

        List<string> messages = [];
        IDisposable subscription = studentName.ObserveValidationErrors().Subscribe(message => messages.Add(message ?? "(none)"));

        studentName.Value = "Ada Lovelace";
        studentName.Value = string.Empty;

        Console.WriteLine(string.Join(" | ", messages));

        subscription.Dispose();
        studentName.Dispose();

        // Output:
        // Enter the student's name. | (none) | Enter the student's name.
    }
}
