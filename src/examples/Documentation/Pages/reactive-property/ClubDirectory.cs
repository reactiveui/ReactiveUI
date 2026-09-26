// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>The school's clubs and the usernames already taken, held in memory for the sign-up form.</summary>
public static class ClubDirectory
{
    /// <summary>Gets the clubs a student can join.</summary>
    public static IReadOnlyList<Club> Clubs { get; } =
    [
        new Club("Chess Club", 10),
        new Club("Robotics Club", 12),
        new Club("Art Club", 8),
    ];

    /// <summary>Gets the usernames already registered by another student.</summary>
    public static IReadOnlyList<string> TakenUsernames { get; } = ["ada.lovelace", "grace.hopper"];

    /// <summary>Gets the guardian-email domains the school's mail filter rejects.</summary>
    public static IReadOnlyList<string> BlockedEmailDomains { get; } = ["mailinator.com", "example.com"];

    /// <summary>Checks, asynchronously, whether a username is already registered.</summary>
    /// <param name="username">The username to look up.</param>
    /// <returns>A task that completes with <see langword="true"/> when the username is already taken.</returns>
    public static Task<bool> IsUsernameTakenAsync(string username) =>
        Task.Run(() => TakenUsernames.Contains(username, StringComparer.OrdinalIgnoreCase));

    /// <summary>Checks, asynchronously, whether a guardian's email uses a blocked domain.</summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>A task that completes with the reasons the address is rejected, or an empty list when it is accepted.</returns>
    public static Task<IEnumerable?> CheckGuardianEmailAsync(string email) =>
        Task.Run(IEnumerable? () =>
        {
            int atIndex = email.IndexOf('@');
            string domain = atIndex >= 0 ? email[(atIndex + 1) ..] : string.Empty;
            List<string> reasons = BlockedEmailDomains.Contains(domain, StringComparer.OrdinalIgnoreCase)
                ? [$"'{domain}' cannot receive club mail."]
                : [];
            return reasons;
        });
}
