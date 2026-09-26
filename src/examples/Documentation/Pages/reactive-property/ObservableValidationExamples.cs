// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>Shows validators that react to the whole stream of values, rather than one value at a time.</summary>
public static class ObservableValidationExamples
{
    /// <summary>A stream validator returning a single message runs on every value the source emits, including the initial one.</summary>
    public static void ValidateAStreamWithAStringMessage()
    {
        ReactiveProperty<string> email = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);
        _ = email.AddValidationError(static stream => stream.Select(static value => value is not null && value.Contains('@') ? null : "Enter a valid email address."));

        ReactiveProperty<string> emailIgnoringInitialError = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);
        _ = emailIgnoringInitialError.AddValidationError(
            static stream => stream.Select(static value => value is not null && value.Contains('@') ? null : "Enter a valid email address."),
            ignoreInitialError: true);

        Console.WriteLine(email.HasErrors);
        Console.WriteLine(emailIgnoringInitialError.HasErrors);

        email.Value = "ada@school.edu";
        Console.WriteLine(email.HasErrors);

        email.Dispose();
        emailIgnoringInitialError.Dispose();

        // Output:
        // True
        // False
        // False
    }

    /// <summary>A stream validator can return more than one error message through <see cref="IEnumerable"/>.</summary>
    public static void ValidateAStreamWithMultipleErrors()
    {
        ReactiveProperty<string> club = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);
        _ = club.AddValidationError(static stream => stream.Select(static value =>
            ClubDirectory.Clubs.Any(candidate => candidate.Name == value)
                ? null
                : (IEnumerable?)new[] { $"'{value}' is not one of the school's clubs." }));

        ReactiveProperty<string> clubIgnoringInitialError = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);
        _ = clubIgnoringInitialError.AddValidationError(
            static stream => stream.Select(static value =>
                ClubDirectory.Clubs.Any(candidate => candidate.Name == value)
                    ? null
                    : (IEnumerable?)new[] { $"'{value}' is not one of the school's clubs." }),
            ignoreInitialError: true);

        Console.WriteLine(club.HasErrors);
        Console.WriteLine(clubIgnoringInitialError.HasErrors);

        club.Value = "Chess Club";
        Console.WriteLine(club.HasErrors);

        club.Dispose();
        clubIgnoringInitialError.Dispose();

        // Output:
        // True
        // False
        // False
    }

    /// <summary><c>ObserveHasErrors</c> and <c>ObserveErrorChanged</c> stream the property's validation state.</summary>
    public static void ObserveErrorStreams()
    {
        ReactiveProperty<string> email = new(string.Empty, RxSchedulers.MainThreadScheduler, false, false);
        _ = email.AddValidationError(static stream => stream.Select(static value => value is not null && value.Contains('@') ? null : "Enter a valid email address."));

        List<bool> hasErrorsChanges = [];
        List<string> errorMessages = [];
        IDisposable hasErrorsSubscription = email.ObserveHasErrors.Subscribe(hasErrorsChanges.Add);
        IDisposable errorsSubscription = email.ObserveErrorChanged.Subscribe(
            errors => errorMessages.Add(errors?.Cast<string>().FirstOrDefault() ?? "(none)"));

        email.Value = "ada@school.edu";

        Console.WriteLine(string.Join(", ", hasErrorsChanges));
        Console.WriteLine(string.Join(", ", errorMessages));

        hasErrorsSubscription.Dispose();
        errorsSubscription.Dispose();
        email.Dispose();

        // Output:
        // True, False
        // Enter a valid email address., (none)
    }
}
