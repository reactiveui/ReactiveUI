// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.ComponentModel;

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>Shows synchronous validation on <see cref="ReactiveProperty{T}"/>, and how errors are read back.</summary>
public static class SyncValidationExamples
{
    /// <summary>
    /// A validator that returns a single message runs on the initial value unless <c>ignoreInitialError</c> is
    /// <see langword="true"/>; <see cref="ReactiveProperty{T}.CheckValidation"/> re-runs a validator without
    /// changing <c>Value</c>.
    /// </summary>
    public static void ValidateWithAStringMessage()
    {
        ReactiveProperty<string> validatesImmediately = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);
        _ = validatesImmediately.AddValidationError(static name => string.IsNullOrWhiteSpace(name) ? "Enter the student's name." : null);

        ReactiveProperty<string> ignoresInitialError = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);
        _ = ignoresInitialError.AddValidationError(
            static name => string.IsNullOrWhiteSpace(name) ? "Enter the student's name." : null,
            ignoreInitialError: true);

        Console.WriteLine(validatesImmediately.HasErrors);
        Console.WriteLine(ignoresInitialError.HasErrors);

        ignoresInitialError.CheckValidation();
        Console.WriteLine(ignoresInitialError.HasErrors);

        validatesImmediately.Value = "Ada Lovelace";
        Console.WriteLine(validatesImmediately.HasErrors);

        validatesImmediately.Dispose();
        ignoresInitialError.Dispose();

        // Output:
        // True
        // False
        // True
        // False
    }

    /// <summary>A validator can return more than one error message through <see cref="IEnumerable"/>.</summary>
    public static void ValidateWithMultipleErrors()
    {
        ReactiveProperty<int> age = new(5, RxSchedulers.MainThreadScheduler, false, false);
        _ = age.AddValidationError(static value => value < 8 ? new[] { "The student is too young for any club." } : null);

        ReactiveProperty<int> ageIgnoringInitialError = new(5, RxSchedulers.MainThreadScheduler, false, false);
        _ = ageIgnoringInitialError.AddValidationError(
            static value => value < 8 ? new[] { "The student is too young for any club." } : null,
            ignoreInitialError: true);

        Console.WriteLine(age.HasErrors);
        Console.WriteLine(ageIgnoringInitialError.HasErrors);

        ageIgnoringInitialError.Value = 6;
        Console.WriteLine(ageIgnoringInitialError.HasErrors);

        age.Dispose();
        ageIgnoringInitialError.Dispose();

        // Output:
        // True
        // False
        // True
    }

    /// <summary>
    /// <c>GetErrors</c> hands back the current errors or <see langword="null"/>; the explicit
    /// <see cref="INotifyDataErrorInfo.GetErrors"/> implementation always hands back an enumerable, empty when there
    /// are no errors.
    /// </summary>
    public static void GetErrorsReturnsCurrentErrors()
    {
        ReactiveProperty<int> age = new(5, RxSchedulers.MainThreadScheduler, false, false);
        _ = age.AddValidationError(static value => value < 8 ? new[] { "The student is too young for any club." } : null);

        IEnumerable? errors = age.GetErrors(nameof(age.Value));
        if (errors is not null)
        {
            Console.WriteLine(string.Join(", ", errors.Cast<string>()));
        }

        INotifyDataErrorInfo asDataErrorInfo = age;
        age.Value = 10;
        Console.WriteLine(asDataErrorInfo.GetErrors(nameof(age.Value)).Cast<object>().Count());

        age.Dispose();

        // Output:
        // The student is too young for any club.
        // 0
    }

    /// <summary>
    /// <c>ErrorsChanged</c> hands every subscriber the same cached <see cref="DataErrorsChangedEventArgs"/> instance
    /// instead of allocating a new one each time; <see cref="SingletonPropertyChangedEventArgs"/> supplies the
    /// property-name text ReactiveProperty raises <c>PropertyChanged</c> with.
    /// </summary>
    public static void EventArgsAreCachedInstances()
    {
        ReactiveProperty<string> studentName = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);
        _ = studentName.AddValidationError(static name => string.IsNullOrWhiteSpace(name) ? "Enter the student's name." : null);

        List<string?> propertyNames = [];
        studentName.PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName);

        DataErrorsChangedEventArgs? errorsChangedRaised = null;
        studentName.ErrorsChanged += (_, e) => errorsChangedRaised = e;

        studentName.Value = "Ada Lovelace";

        Console.WriteLine(string.Join(", ", propertyNames));
        Console.WriteLine(propertyNames.Contains(SingletonPropertyChangedEventArgs.HasErrors.PropertyName));
        Console.WriteLine(ReferenceEquals(errorsChangedRaised, SingletonDataErrorsChangedEventArgs.Value));
        Console.WriteLine(SingletonPropertyChangedEventArgs.Value.PropertyName);
        Console.WriteLine(SingletonPropertyChangedEventArgs.ErrorMessage.PropertyName);

        studentName.Dispose();

        // Output:
        // HasErrors, Value
        // True
        // True
        // Value
        // ErrorMessage
    }
}
