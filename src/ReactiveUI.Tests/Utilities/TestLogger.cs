// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

public class TestLogger : ILogger
{
    public TestLogger()
    {
        Messages = [];
        Level = LogLevel.Debug;
    }

    public List<(string message, Type type, LogLevel logLevel)> Messages { get; }

    /// <inheritdoc/>
    public LogLevel Level { get; set; }

    /// <inheritdoc/>
    public void Write(Exception exception, string message, Type type, LogLevel logLevel) => Messages.Add((message, typeof(TestLogger), logLevel));

    /// <inheritdoc/>
    public void Write(string message, LogLevel logLevel) => Messages.Add((message, typeof(TestLogger), logLevel));

    /// <inheritdoc/>
    public void Write(Exception exception, string message, LogLevel logLevel) => Messages.Add((message, typeof(TestLogger), logLevel));

    /// <inheritdoc/>
    public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel) => Messages.Add((message, type, logLevel));
}
