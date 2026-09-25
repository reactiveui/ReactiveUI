// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Todo;

/// <summary>The error a <see cref="ITodoStore"/> throws when it refuses a write.</summary>
public sealed class TodoStoreException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="TodoStoreException"/> class.</summary>
    public TodoStoreException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TodoStoreException"/> class.</summary>
    /// <param name="message">The reason the write was refused.</param>
    public TodoStoreException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TodoStoreException"/> class.</summary>
    /// <param name="message">The reason the write was refused.</param>
    /// <param name="innerException">The error that caused the refusal.</param>
    public TodoStoreException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
