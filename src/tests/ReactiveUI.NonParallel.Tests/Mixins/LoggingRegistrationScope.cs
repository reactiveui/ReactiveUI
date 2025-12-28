// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mixins;

internal class LoggingRegistrationScope : IDisposable
{
    private static IDisposable? _logManagerRegistration;
    private ILogManager? _previousLogManager;
    private TestLogger _logger;
    private IFullLogger _fullLogger;

    public LoggingRegistrationScope()
    {
        // Save the current ILogManager if one exists
        _previousLogManager = Locator.Current.GetService<ILogManager>();

        _logger = new TestLogger();
        _fullLogger = new WrappingFullLogger(_logger);

        // Register a default ILogManager for tests
        // This ensures ILogManager is available even if tests don't set up their own
        var currentLogManager = new TestLogManager(_fullLogger);
        Locator.CurrentMutable.Register<ILogManager>(() => currentLogManager);
    }

    internal TestLogger Logger => _logger;

    public void Dispose()
    {
        // Dispose of the log manager registration
        _logManagerRegistration?.Dispose();
        _logManagerRegistration = null;

        // Restore the previous ILogManager if there was one
        // Note: We can't easily unregister, so we just re-register the old one if it existed
        if (_previousLogManager != null)
        {
            Locator.CurrentMutable.Register(() => _previousLogManager);
        }
    }

    internal class TestLogger : ILogger
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

    private class TestLogManager(IFullLogger logger) : ILogManager
    {
        public IFullLogger GetLogger(Type type) => logger;
    }
}
