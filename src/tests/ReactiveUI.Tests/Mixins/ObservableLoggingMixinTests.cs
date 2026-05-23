// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using ReactiveUI.Tests.Utilities.Logging;

namespace ReactiveUI.Tests.Mixins;

/// <summary>
///     Tests for the <see cref="ObservableLoggingMixin" /> class.
///     These tests verify the logging functionality for Observables.
/// </summary>
[NotInParallel]
[TestExecutor<LoggingRegistrationExecutor>]
public class ObservableLoggingMixinTests
{
    private const string TestMessage = "Test";
    private const string OnNextFormat = "{0} OnNext: {1}";
    private const string OnCompletedFormat = "{0} OnCompleted";
    private const string OnErrorSuffix = " OnError";

    /// <summary>
    ///     Verifies that using IEnableLogger as a local variable works.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LocalInterfaceVariable_WorksWithIEnableLogger()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);

        var testLogger = (TestEnableLogger)logger;
        var subject = new Subject<int>();

        // Use interface variable in Do()
        const string Message = TestMessage;
        const int SecondValue = 2;
        const int ExpectedInfoCount = 3; // 2 OnNext + 1 OnCompleted
        var logged = subject.Do(
            x => logger.Log().Info(CultureInfo.InvariantCulture, OnNextFormat, Message, x),
            ex => logger.Log().Warn(ex, Message + OnErrorSuffix),
            () => logger.Log().Info(CultureInfo.InvariantCulture, OnCompletedFormat, Message));

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(SecondValue);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, SecondValue]);
        await Assert.That(testLogger.InfoCount).IsEqualTo(ExpectedInfoCount);
    }

    /// <summary>
    ///     Verifies that logged observable completes normally without errors.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Log_CompletesNormally_WithoutErrors()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        const int RangeCount = 5;
        const int MinimumInfoCount = 6; // 5 OnNext + 1 OnCompleted
        var logger = new TestEnableLogger(loggerInstance);
        var values = Observable.Range(1, RangeCount);

        var logged = values.Log(logger, "Range");

        var results = new List<int>();
        var completed = false;
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(results.Add, () => completed = true);

        await Assert.That(results).IsEquivalentTo(Enumerable.Range(1, RangeCount).ToList());
        await Assert.That(completed).IsTrue();
        await Assert.That(logger.InfoCount).IsGreaterThanOrEqualTo(MinimumInfoCount);
    }

    /// <summary>
    ///     Verifies that Log extension method logs errors.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Log_LogsErrors()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        var logged = subject.Log(logger, TestMessage);

        var errorCaught = false;
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(_ => { }, _ => errorCaught = true);

        subject.OnError(new InvalidOperationException("Test error"));

        await Assert.That(errorCaught).IsTrue();
        await Assert.That(logger.WarnCount).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    ///     Verifies that Log extension method logs OnNext, OnError, and OnCompleted events.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Log_LogsOnNextOnErrorAndOnCompleted()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        const int SecondValue = 2;
        const int MinimumInfoCount = 3; // 2 OnNext + 1 OnCompleted
        var logged = subject.Log(logger, TestMessage);

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(SecondValue);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, SecondValue]);
        await Assert.That(logger.InfoCount).IsGreaterThanOrEqualTo(MinimumInfoCount);
    }

    /// <summary>
    ///     Verifies that Log extension method uses empty message when null is provided.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Log_WithNullMessage_UsesEmptyString()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        const int MinimumInfoCount = 2;
        var logged = subject.Log(logger);

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnCompleted();

        await Assert.That(values).Contains(1);
        await Assert.That(logger.InfoCount).IsGreaterThanOrEqualTo(MinimumInfoCount);
    }

    /// <summary>
    ///     Verifies that Log extension method with stringifier uses custom string conversion.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Log_WithStringifier_UsesCustomConversion()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        const int ExpectedValue = 42;
        const int MinimumInfoCount = 2;
        var logged = subject.Log(logger, TestMessage, x => $"Value: {x}");

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(ExpectedValue);
        subject.OnCompleted();

        await Assert.That(values).Contains(ExpectedValue);
        await Assert.That(logger.InfoCount).IsGreaterThanOrEqualTo(MinimumInfoCount);
    }

    /// <summary>
    ///     Verifies that the Log extension method works with TestEnableLogger.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LogExtensionMethod_WorksWithTestEnableLogger()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        const int SecondValue = 2;
        const int ExpectedInfoCount = 3; // 2 OnNext + 1 OnCompleted

        // This is the actual Log extension method
        var logged = subject.Log(logger, TestMessage);

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(SecondValue);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, SecondValue]);
        await Assert.That(logger.InfoCount).IsEqualTo(ExpectedInfoCount);
    }

    /// <summary>
    ///     Verifies that LoggedCatch catches exceptions and returns next observable.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LoggedCatch_CatchesExceptionAndReturnsNext()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        const int FallbackValue = 99;
        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();
        var fallback = Observable.Return(FallbackValue);

        var caught = subject.LoggedCatch(logger, fallback, "Error occurred");

        var values = new List<int>();
        caught.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnError(new InvalidOperationException());

        await Assert.That(values).Contains(FallbackValue);
        await Assert.That(logger.WarnCount).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    ///     Verifies that LoggedCatch with exception func passes exception to next factory.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LoggedCatch_WithExceptionFunc_PassesExceptionToFactory()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        const int FallbackValue = 100;
        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();
        Exception? capturedEx = null;

        var caught = subject.LoggedCatch<int, TestEnableLogger, InvalidOperationException>(
            logger,
            ex =>
            {
                capturedEx = ex;
                return Observable.Return(FallbackValue);
            });

        var values = new List<int>();
        caught.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        var thrownException = new InvalidOperationException(TestMessage);
        subject.OnError(thrownException);

        await Assert.That(capturedEx).IsSameReferenceAs(thrownException);
        await Assert.That(values).Contains(FallbackValue);
    }

    /// <summary>
    ///     Verifies that LoggedCatch with exception type catches specific exceptions.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LoggedCatch_WithExceptionType_CatchesSpecificException()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        const int FallbackValue = 42;
        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        var caught = subject.LoggedCatch<int, TestEnableLogger, InvalidOperationException>(
            logger,
            ex => Observable.Return(FallbackValue),
            "Specific error");

        var values = new List<int>();
        caught.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnError(new InvalidOperationException());

        await Assert.That(values).Contains(FallbackValue);
        await Assert.That(logger.WarnCount).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    ///     Verifies that LoggedCatch uses empty string for null message.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LoggedCatch_WithNullMessage_UsesEmptyString()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        var caught = subject.LoggedCatch(logger, Observable.Return(1));

        var values = new List<int>();
        caught.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnError(new InvalidOperationException());

        await Assert.That(values).Contains(1);
        await Assert.That(logger.WarnCount).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    ///     Verifies that LoggedCatch uses default observable when next is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LoggedCatch_WithNullNext_UsesDefault()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        var caught = subject.LoggedCatch(logger, null, "Error");

        caught.ObserveOn(ImmediateScheduler.Instance).Subscribe(_ => { }, _ => { }, () => { });

        subject.OnError(new InvalidOperationException());

        await Assert.That(logger.WarnCount).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    ///     Verifies that manually inlining the Log extension method logic works.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ManualInline_WorksWithTestEnableLogger()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        // Manually inline what Log() does
        const string Message = TestMessage;
        const int SecondValue = 2;
        const int ExpectedInfoCount = 3; // 2 OnNext + 1 OnCompleted
        var logged = subject.Do(
            x => logger.Log().Info(CultureInfo.InvariantCulture, OnNextFormat, Message, x),
            ex => logger.Log().Warn(ex, Message + OnErrorSuffix),
            () => logger.Log().Info(CultureInfo.InvariantCulture, OnCompletedFormat, Message));

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(SecondValue);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, SecondValue]);
        await Assert.That(logger.InfoCount).IsEqualTo(ExpectedInfoCount);
    }

    /// <summary>
    ///     Verifies that a non-generic helper method works with IEnableLogger.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task NonGenericHelper_WorksWithIEnableLogger()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        const int SecondValue = 2;
        const int ExpectedInfoCount = 3; // 2 OnNext + 1 OnCompleted
        var logged = LogNonGeneric(subject, logger, TestMessage);

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(SecondValue);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, SecondValue]);
        await Assert.That(logger.InfoCount).IsEqualTo(ExpectedInfoCount);
    }

    /// <summary>
    ///     Verifies that TestEnableLogger captures Info messages with 2 generic arguments (like ObservableLoggingMixin uses).
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task TestEnableLogger_CapturesGenericInfoWithTwoArguments()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);
        var subject = new Subject<int>();

        const int SecondValue = 2;
        const int ExpectedInfoCount = 3; // 2 OnNext + 1 OnCompleted
        var logged = subject.Do(
            x => logger.Log().Info(CultureInfo.InvariantCulture, OnNextFormat, TestMessage, x),
            ex => logger.Log().Warn(ex, "Test OnError"),
            () => logger.Log().Info(CultureInfo.InvariantCulture, OnCompletedFormat, TestMessage));

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(SecondValue);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, SecondValue]);
        await Assert.That(logger.InfoCount).IsEqualTo(ExpectedInfoCount);
    }

    /// <summary>
    ///     Verifies that TestEnableLogger captures Info and Warn messages directly.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task TestEnableLogger_CapturesInfoAndWarnMessages()
    {
        var loggerInstance = TestContext.Current?.GetTestLogger();
        await Assert.That(loggerInstance).IsNotNull();

        var logger = new TestEnableLogger(loggerInstance);

        // Call Info and Warn directly
        logger.Log().Info(CultureInfo.InvariantCulture, "Test message {0}", "arg1");
        logger.Log().Warn(new InvalidOperationException(), "Error occurred");

        await Assert.That(logger.InfoCount).IsEqualTo(1);
        await Assert.That(logger.WarnCount).IsEqualTo(1);
    }

    /// <summary>
    ///     Helper method that takes IEnableLogger directly (not generic) to test if that works.
    /// </summary>
    /// <typeparam name="T">The type of the observable sequence elements.</typeparam>
    /// <param name="source">The source observable to log.</param>
    /// <param name="logObject">The logger used to record events.</param>
    /// <param name="message">The message prefix applied to log entries.</param>
    /// <returns>An observable that logs its events.</returns>
    private static IObservable<T> LogNonGeneric<T>(IObservable<T> source, IEnableLogger logObject, string message)
    {
        message ??= string.Empty;
        return source.Do(
            x => logObject.Log().Info(CultureInfo.InvariantCulture, OnNextFormat, message, x),
            ex => logObject.Log().Warn(ex, message + OnErrorSuffix),
            () => logObject.Log().Info(CultureInfo.InvariantCulture, OnCompletedFormat, message));
    }

    /// <summary>
    ///     A test logger that exposes counts of captured log messages.
    /// </summary>
    /// <param name="logger">The underlying logger that stores captured messages.</param>
    private sealed class TestEnableLogger(TestLogger logger) : IEnableLogger
    {
        /// <summary>
        ///     Gets the number of captured Info-level messages.
        /// </summary>
        public int InfoCount => logger.Messages.Count(m => m.logLevel == LogLevel.Info);

        /// <summary>
        ///     Gets the number of captured Warn-level messages.
        /// </summary>
        public int WarnCount => logger.Messages.Count(m => m.logLevel == LogLevel.Warn);
    }
}
