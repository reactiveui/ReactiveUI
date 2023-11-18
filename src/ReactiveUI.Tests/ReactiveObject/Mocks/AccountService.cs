// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Account Service.
/// </summary>
/// <seealso cref="ReactiveUI.ReactiveObject" />
public class AccountService : ReactiveObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountService"/> class.
    /// </summary>
    public AccountService()
    {
        AccountUsers.Add(Guid.NewGuid(), new() { LastName = "Harris" });
        AccountUsers.Add(Guid.NewGuid(), new() { LastName = "Jones" });
        AccountUsers.Add(Guid.NewGuid(), new() { LastName = "Smith" });

        AccountUsersNullable.Add(Guid.NewGuid(), new() { LastName = "Harris" });
        AccountUsersNullable.Add(Guid.NewGuid(), new() { LastName = "Jones" });
        AccountUsersNullable.Add(Guid.NewGuid(), new() { LastName = "Smith" });
        AccountUsersNullable.Add(Guid.NewGuid(), null);
    }

    /// <summary>
    /// Gets the account users.
    /// </summary>
    /// <value>
    /// The account users.
    /// </value>
    public Dictionary<Guid, AccountUser> AccountUsers { get; } = [];

    /// <summary>
    /// Gets the account users nullable.
    /// </summary>
    /// <value>
    /// The account users nullable.
    /// </value>
    public Dictionary<Guid, AccountUser?> AccountUsersNullable { get; } = [];
}
