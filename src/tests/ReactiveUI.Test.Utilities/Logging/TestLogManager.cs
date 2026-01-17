// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Tests.Utilities.Logging;

/// <summary>
/// Provides a test implementation of the <see cref="ILogManager"/> interface that always returns the same logger
/// instance.
/// </summary>
/// <param name="logger">The logger instance to be returned by this log manager. Cannot be null.</param>
public class TestLogManager(IFullLogger logger) : ILogManager
{
    /// <summary>
    /// Gets a logger instance associated with the specified type.
    /// </summary>
    /// <param name="type">The type for which to retrieve the logger instance.</param>
    /// <returns>An <see cref="IFullLogger"/> instance for the specified type.</returns>
    public IFullLogger GetLogger(Type type) => logger;
}
