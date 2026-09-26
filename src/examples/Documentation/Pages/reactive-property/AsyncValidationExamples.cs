// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>
/// Shows validators that check a value against another system, such as a database, without blocking the caller.
/// Each example waits for <c>ObserveHasErrors</c> to report <see langword="true"/>, because the validator's task
/// completes on a background thread.
/// </summary>
public static class AsyncValidationExamples
{
    /// <summary>An asynchronous validator that returns a single message runs the same way a synchronous one does, just later.</summary>
    /// <returns>A task that completes once both usernames have been checked.</returns>
    public static async Task ValidateAsynchronouslyWithAStringMessage()
    {
        ReactiveProperty<string> username = new("ada.lovelace", RxSchedulers.MainThreadScheduler, false, false);
        Task<bool> becameInvalid = username.ObserveHasErrors.Where(static hasErrors => hasErrors).FirstAsync();

        _ = username.AddValidationError(static async name =>
        {
            bool taken = await ClubDirectory.IsUsernameTakenAsync(name ?? string.Empty);
            return taken ? "That username is already taken." : null;
        });

        Console.WriteLine(await becameInvalid);

        ReactiveProperty<string> usernameIgnoringInitialError = new("ada.lovelace", RxSchedulers.MainThreadScheduler, false, false);
        _ = usernameIgnoringInitialError.AddValidationError(
            static async name =>
            {
                bool taken = await ClubDirectory.IsUsernameTakenAsync(name ?? string.Empty);
                return taken ? "That username is already taken." : null;
            },
            ignoreInitialError: true);

        Console.WriteLine(usernameIgnoringInitialError.HasErrors);

        Task<bool> becameInvalidAfterCheck = usernameIgnoringInitialError.ObserveHasErrors.Where(static hasErrors => hasErrors).FirstAsync();
        usernameIgnoringInitialError.CheckValidation();
        Console.WriteLine(await becameInvalidAfterCheck);

        username.Dispose();
        usernameIgnoringInitialError.Dispose();

        // Output:
        // True
        // False
        // True
    }

    /// <summary>An asynchronous validator can return more than one error message through <see cref="IEnumerable"/>.</summary>
    /// <returns>A task that completes once the guardian email has been checked.</returns>
    public static async Task ValidateAsynchronouslyWithMultipleErrors()
    {
        ReactiveProperty<string> guardianEmail = new("family@example.com", RxSchedulers.MainThreadScheduler, false, false);
        Task<bool> becameInvalid = guardianEmail.ObserveHasErrors.Where(static hasErrors => hasErrors).FirstAsync();

        _ = guardianEmail.AddValidationError(static email => ClubDirectory.CheckGuardianEmailAsync(email ?? string.Empty));

        Console.WriteLine(await becameInvalid);

        IEnumerable? errors = guardianEmail.GetErrors(nameof(guardianEmail.Value));
        if (errors is not null)
        {
            Console.WriteLine(string.Join(", ", errors.Cast<string>()));
        }

        ReactiveProperty<string> guardianEmailIgnoringInitialError = new("family@example.com", RxSchedulers.MainThreadScheduler, false, false);
        _ = guardianEmailIgnoringInitialError.AddValidationError(
            static email => ClubDirectory.CheckGuardianEmailAsync(email ?? string.Empty),
            ignoreInitialError: true);

        Console.WriteLine(guardianEmailIgnoringInitialError.HasErrors);

        guardianEmail.Dispose();
        guardianEmailIgnoringInitialError.Dispose();

        // Output:
        // True
        // 'example.com' cannot receive club mail.
        // False
    }
}
