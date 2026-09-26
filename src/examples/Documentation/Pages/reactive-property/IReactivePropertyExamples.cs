// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary><see cref="ReactiveProperty{T}"/> implements <see cref="IReactiveProperty{T}"/>, so a view model can depend on the interface instead.</summary>
public static class IReactivePropertyExamples
{
    /// <summary><c>Value</c>, <c>HasErrors</c>, <c>ObserveHasErrors</c>, <c>ObserveErrorChanged</c> and <c>Refresh</c> all work through the interface.</summary>
    public static void UseThroughTheInterface()
    {
        static void ReadThroughTheInterface(IReactiveProperty<string> studentName)
        {
            Console.WriteLine(studentName.HasErrors);

            studentName.Value = "Ada Lovelace";
            Console.WriteLine(studentName.HasErrors);
            Console.WriteLine(studentName.Value);

            List<bool> hasErrorsChanges = [];
            List<string> errorMessages = [];
            IDisposable hasErrorsSubscription = studentName.ObserveHasErrors.Subscribe(hasErrorsChanges.Add);
            IDisposable errorSubscription = studentName.ObserveErrorChanged.Subscribe(
                errors => errorMessages.Add(errors?.Cast<string>().FirstOrDefault() ?? "(none)"));

            studentName.Refresh();

            Console.WriteLine(string.Join(", ", hasErrorsChanges));
            Console.WriteLine(string.Join(", ", errorMessages));

            hasErrorsSubscription.Dispose();
            errorSubscription.Dispose();
        }

        ReactiveProperty<string> concrete = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);
        _ = concrete.AddValidationError(static name => string.IsNullOrWhiteSpace(name) ? "Enter the student's name." : null);

        ReadThroughTheInterface(concrete);

        concrete.Dispose();

        // Output:
        // True
        // False
        // Ada Lovelace
        // False, False
        // (none), (none)
    }
}
