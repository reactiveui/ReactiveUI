// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

using FluentAssertions;

using Splat;

using Xunit;

namespace ReactiveUI.Tests
{
    public class PocoObservableForPropertyTests
    {
        private static TestLogger? _testLoggerForNotificationPocoErrorOnBind;

        [Fact]
        public void CheckGetAffinityForObjectValues()
        {
            var instance = new POCOObservableForProperty();

            Assert.Equal(1, instance.GetAffinityForObject(typeof(PocoType), null!, false));
            Assert.Equal(1, instance.GetAffinityForObject(typeof(INPCClass), null!, false));
        }

        [Fact]
        public void NotificationPocoErrorOnBind()
        {
            // Use same logger, when the test is executed multiple times in the same AndroidRunner/AppDomain/AssemblyLoadContext
            if (_testLoggerForNotificationPocoErrorOnBind is null)
            {
                _testLoggerForNotificationPocoErrorOnBind = new TestLogger();
            }

            // Run test twice and verify that POCO message is logged only once.
            for (int i = 0; i < 2; i++)
            {
                using (var testLoggerRegistration = new TestLoggerRegistration(_testLoggerForNotificationPocoErrorOnBind))
                {
                    var instance = new POCOObservableForProperty();

                    var testLogger = testLoggerRegistration.Logger;

                    var testClass = new PocoType();

                    Expression<Func<PocoType, string>> expr = x => x.Property1!;
                    var exp = Reflection.Rewrite(expr.Body);

                    var propertyName = exp.GetMemberInfo()?.Name;

                    if (propertyName is null)
                    {
                        throw new InvalidOperationException("propertyName should not be null");
                    }

                    instance.GetNotificationForProperty(testClass, exp, propertyName, false).Subscribe(_ => { });

                    Assert.True(testLogger.LastMessages.Count > 0);

                    var expectedMessage = $"{nameof(POCOObservableForProperty)}: The class {typeof(PocoType).FullName} property {nameof(PocoType.Property1)} is a POCO type and won't send change notifications, WhenAny will only return a single value!";
                    Assert.Equal(expectedMessage, testLogger.LastMessages[0]);

                    // Verify that the message is logged only once
                    foreach (var logMessage in testLogger.LastMessages.Skip(1))
                    {
                        Assert.NotEqual(expectedMessage, logMessage);
                    }
                }
            }
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Not in NET472")]
        public void NotificationPocoSuppressErrorOnBind()
        {
            using (var testLoggerRegistration = new TestLoggerRegistration())
            {
                var instance = new POCOObservableForProperty();

                var testLogger = testLoggerRegistration.Logger;

                var testClass = new PocoType();

                Expression<Func<PocoType, string>> expr = x => x.Property1!;
                var exp = Reflection.Rewrite(expr.Body);

                var propertyName = exp.GetMemberInfo()?.Name;

                if (propertyName is null)
                {
                    throw new InvalidOperationException("propertyName should not be null");
                }

                instance.GetNotificationForProperty(testClass, exp, propertyName, false, true).Subscribe(_ => { });

                testLogger.LastMessages.Should().NotContain(m => m.Contains(nameof(POCOObservableForProperty)));
            }
        }

        private class PocoType
        {
            public string? Property1 { get; set; }

            public string? Property2 { get; set; }
        }

#pragma warning disable CA1812 // Class is not instantiated
        private class INPCClass : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            public void NotifyPropertyChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
#pragma warning restore CA1812 // Class is not instantiated

        private class TestLogger : ILogger
        {
            public List<string> LastMessages { get; } = new();

            public LogLevel Level => LogLevel.Debug;

            public void Write(Exception exception, string message, Type type, LogLevel logLevel) => LastMessages.Add(message);

            public void Write(string message, LogLevel logLevel) => LastMessages.Add(message);

            public void Write(Exception exception, string message, LogLevel logLevel) => LastMessages.Add(message);

            public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel) => LastMessages.Add(message);
        }

        private sealed class TestLoggerRegistration : IDisposable
        {
            private readonly List<ILogger> _originalLoggers;

            public TestLoggerRegistration()
                : this(null)
            {
            }

            public TestLoggerRegistration(TestLogger? testLogger)
            {
                _originalLoggers = Locator.Current.GetServices<ILogger>().ToList();

                Logger = testLogger ?? new TestLogger();
                Locator.CurrentMutable.RegisterConstant<ILogger>(Logger);
            }

            public TestLogger Logger { get; }

            public void Dispose()
            {
                // It's not possible to unregister specific logger,
                // so all are unregistered and originals are re-registered.
                Locator.CurrentMutable.UnregisterAll<ILogger>();

                foreach (var logger in _originalLoggers)
                {
                    Locator.CurrentMutable.RegisterConstant<ILogger>(logger);
                }
            }
        }
    }
}
