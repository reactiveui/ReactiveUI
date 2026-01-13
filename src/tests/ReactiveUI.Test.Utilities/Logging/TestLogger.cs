// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.ComponentModel;
using Splat;

namespace ReactiveUI.Tests.Utilities.Logging;

/// <summary>
///     Provides a test implementation of the ILogger interface for capturing and inspecting log messages during unit
///     tests.
/// </summary>
/// <remarks>
///     TestLogger records log messages in memory, allowing test code to verify logging behavior without
///     external dependencies. The logger is initialized with an empty message collection and the log level set to Debug.
///     Use the Messages property to access the recorded log entries for assertions or analysis in test scenarios.
/// </remarks>
public class TestLogger : ILogger
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TestLogger" /> class with default settings.
    /// </summary>
    public TestLogger()
    {
        Messages = [];
        Level = LogLevel.Debug;
    }

    /// <summary>
    ///     Gets the collection of log messages recorded by the logger.
    /// </summary>
    /// <remarks>
    ///     Each entry in the collection contains the message text, the associated type, and the log
    ///     level. The collection is read-only; to add messages, use the appropriate logging methods provided by the
    ///     class.
    /// </remarks>
    public List<(string message, Type type, LogLevel logLevel)> Messages { get; }

    /// <inheritdoc />
    public LogLevel Level { get; set; }

    /// <inheritdoc />
    public void Write(Exception exception, string message, Type type, LogLevel logLevel) =>
        Messages.Add((message, typeof(TestLogger), logLevel));

    /// <inheritdoc />
    public void Write(string message, LogLevel logLevel) => Messages.Add((message, typeof(TestLogger), logLevel));

    /// <inheritdoc />
    public void Write(Exception exception, string message, LogLevel logLevel) =>
        Messages.Add((message, typeof(TestLogger), logLevel));

    /// <inheritdoc />
    public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel) =>
        Messages.Add((message, type, logLevel));
}
