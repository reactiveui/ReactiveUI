// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.DefaultExceptionHandler;

/// <summary>The error a bank account server returns, such as when it is down or the account is unknown.</summary>
public sealed class AccountServiceException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="AccountServiceException"/> class.</summary>
    public AccountServiceException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AccountServiceException"/> class.</summary>
    /// <param name="message">The message the server returned.</param>
    public AccountServiceException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AccountServiceException"/> class.</summary>
    /// <param name="message">The message the server returned.</param>
    /// <param name="innerException">The error that caused this one.</param>
    public AccountServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
