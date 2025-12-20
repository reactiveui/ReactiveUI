// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// An exception that is thrown if ReactiveUI fails to locate an <see cref="IViewLocator"/> implementation.
/// </summary>
/// <remarks>
/// <para>
/// This exception typically indicates that the application's dependency resolver has not been initialized via
/// <c>UseReactiveUI</c>, <c>services.AddReactiveUI()</c>, or custom DI registration. It can also surface when assemblies
/// containing the default locator are trimmed out of the app package. Catch the exception at startup to log
/// configuration issues and ensure the container registers an <see cref="IViewLocator"/> before resolving views.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// try
/// {
///     var view = ViewLocator.Current.ResolveView(viewModel);
/// }
/// catch (ViewLocatorNotFoundException ex)
/// {
///     logger.LogCritical(ex, "ReactiveUI was not initialized; cannot resolve views.");
/// }
/// ]]>
/// </code>
/// </example>
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
