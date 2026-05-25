// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.ReactiveObjects.Mocks;

/// <summary>
///     Account Service.
/// </summary>
/// <seealso cref="ReactiveObject" />
public class AccountService : ReactiveObject
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AccountService" /> class.
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
    ///     Gets the account users.
    /// </summary>
    public Dictionary<Guid, AccountUser> AccountUsers { get; } = [];

    /// <summary>
    ///     Gets the account users nullable.
    /// </summary>
    public Dictionary<Guid, AccountUser?> AccountUsersNullable { get; } = [];
}
