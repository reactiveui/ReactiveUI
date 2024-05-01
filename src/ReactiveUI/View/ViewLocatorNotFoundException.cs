// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// An exception that is thrown if we are unable to find the View Locator.
/// </summary>
#if !NET8_0_OR_GREATER
[Serializable]
#endif
public class ViewLocatorNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLocatorNotFoundException"/> class.
    /// </summary>
    public ViewLocatorNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLocatorNotFoundException"/> class.
    /// </summary>
    /// <param name="message">A user friendly message.</param>
    public ViewLocatorNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLocatorNotFoundException"/> class.
    /// </summary>
    /// <param name="message">A user friendly message.</param>
    /// <param name="innerException">Any exception this exception is wrapping.</param>
    public ViewLocatorNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

#if !NET8_0_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLocatorNotFoundException"/> class.
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The serialization context.</param>
#if NET6_0_OR_GREATER
    protected ViewLocatorNotFoundException(SerializationInfo info, in StreamingContext context)
#else
    protected ViewLocatorNotFoundException(SerializationInfo info, StreamingContext context)
#endif
        : base(info, context)
    {
    }
#endif
}
