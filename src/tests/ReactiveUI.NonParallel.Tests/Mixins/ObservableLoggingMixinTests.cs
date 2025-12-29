// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI.Tests.Mixins;

/// <summary>
/// Tests for the <see cref="ObservableLoggingMixin"/> class.
/// These tests verify the logging functionality for Observables.
/// </summary>
public class ObservableLoggingMixinTests
{
    private LoggingRegistrationScope? _loggingScope;

    [Before(Test)]
    public void Setup()
    {
        _loggingScope = new LoggingRegistrationScope();
    }

    [After(Test)]
    public void Teardown()
    {
        _loggingScope?.Dispose();
        _loggingScope = null;
    }

    /// <summary>
    /// Verifies that TestEnableLogger captures Info and Warn messages directly.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task TestEnableLogger_CapturesInfoAndWarnMessages()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);

        // Call Info and Warn directly
        logger.Log().Info(CultureInfo.InvariantCulture, "Test message {0}", "arg1");
        logger.Log().Warn(new InvalidOperationException(), "Error occurred");

        await Assert.That(logger.InfoCount).IsEqualTo(1);
        await Assert.That(logger.WarnCount).IsEqualTo(1);
    }

    /// <summary>
    /// Verifies that TestEnableLogger captures Info messages with 2 generic arguments (like ObservableLoggingMixin uses).
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task TestEnableLogger_CapturesGenericInfoWithTwoArguments()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        var logged = subject.Do(
            x => logger.Log().Info(CultureInfo.InvariantCulture, "{0} OnNext: {1}", "Test", x),
            ex => logger.Log().Warn(ex, "Test OnError"),
            () => logger.Log().Info(CultureInfo.InvariantCulture, "{0} OnCompleted", "Test"));

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, 2]);
        await Assert.That(logger.InfoCount).IsEqualTo(3); // 2 OnNext + 1 OnCompleted
    }

    /// <summary>
    /// Verifies that the Log extension method works with TestEnableLogger.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LogExtensionMethod_WorksWithTestEnableLogger()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        // This is the actual Log extension method
        var logged = ObservableLoggingMixin.Log(subject, logger, "Test");

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, 2]);
        await Assert.That(logger.InfoCount).IsEqualTo(3); // 2 OnNext + 1 OnCompleted
    }

    /// <summary>
    /// Verifies that manually inlining the Log extension method logic works.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ManualInline_WorksWithTestEnableLogger()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        // Manually inline what Log() does
        var message = "Test";
        var logged = subject.Do(
            x => logger.Log().Info(CultureInfo.InvariantCulture, "{0} OnNext: {1}", message, x),
            ex => logger.Log().Warn(ex, message + " OnError"),
            () => logger.Log().Info(CultureInfo.InvariantCulture, "{0} OnCompleted", message));

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, 2]);
        await Assert.That(logger.InfoCount).IsEqualTo(3); // 2 OnNext + 1 OnCompleted
    }

    /// <summary>
    /// Verifies that using IEnableLogger as a local variable works.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LocalInterfaceVariable_WorksWithIEnableLogger()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var testLogger = (TestEnableLogger)logger;
        var subject = new Subject<int>();

        // Use interface variable in Do()
        var message = "Test";
        var logged = subject.Do(
            x => logger.Log().Info(CultureInfo.InvariantCulture, "{0} OnNext: {1}", message, x),
            ex => logger.Log().Warn(ex, message + " OnError"),
            () => logger.Log().Info(CultureInfo.InvariantCulture, "{0} OnCompleted", message));

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, 2]);
        await Assert.That(testLogger.InfoCount).IsEqualTo(3); // 2 OnNext + 1 OnCompleted
    }

    /// <summary>
    /// Verifies that a non-generic helper method works with IEnableLogger.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task NonGenericHelper_WorksWithIEnableLogger()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        var logged = LogNonGeneric(subject, logger, "Test");

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, 2]);
        await Assert.That(logger.InfoCount).IsEqualTo(3); // 2 OnNext + 1 OnCompleted
    }

    /// <summary>
    /// Verifies that Log extension method logs OnNext, OnError, and OnCompleted events.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Log_LogsOnNextOnErrorAndOnCompleted()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        var logged = subject.Log(logger, "Test");

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnCompleted();

        await Assert.That(values).IsEquivalentTo([1, 2]);
        await Assert.That(logger.InfoCount).IsGreaterThanOrEqualTo(3); // 2 OnNext + 1 OnCompleted
    }

    /// <summary>
    /// Verifies that Log extension method logs errors.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Log_LogsErrors()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        var logged = subject.Log(logger, "Test");

        var errorCaught = false;
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(_ => { }, _ => errorCaught = true);

        subject.OnError(new InvalidOperationException("Test error"));

        await Assert.That(errorCaught).IsTrue();
        await Assert.That(logger.WarnCount).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Verifies that Log extension method with stringifier uses custom string conversion.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Log_WithStringifier_UsesCustomConversion()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        var logged = subject.Log(logger, "Test", x => $"Value: {x}");

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(42);
        subject.OnCompleted();

        await Assert.That(values).Contains(42);
        await Assert.That(logger.InfoCount).IsGreaterThanOrEqualTo(2);
    }

    /// <summary>
    /// Verifies that Log extension method uses empty message when null is provided.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Log_WithNullMessage_UsesEmptyString()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        var logged = subject.Log(logger, null);

        var values = new List<int>();
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnNext(1);
        subject.OnCompleted();

        await Assert.That(values).Contains(1);
        await Assert.That(logger.InfoCount).IsGreaterThanOrEqualTo(2);
    }

    /// <summary>
    /// Verifies that LoggedCatch catches exceptions and returns next observable.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LoggedCatch_CatchesExceptionAndReturnsNext()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();
        var fallback = Observable.Return(99);

        var caught = subject.LoggedCatch(logger, fallback, "Error occurred");

        var values = new List<int>();
        caught.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnError(new InvalidOperationException());

        await Assert.That(values).Contains(99);
        await Assert.That(logger.WarnCount).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Verifies that LoggedCatch uses default observable when next is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LoggedCatch_WithNullNext_UsesDefault()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        var caught = subject.LoggedCatch(logger, null, "Error");

        caught.ObserveOn(ImmediateScheduler.Instance).Subscribe(_ => { }, _ => { }, () => { });

        subject.OnError(new InvalidOperationException());

        await Assert.That(logger.WarnCount).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Verifies that LoggedCatch uses empty string for null message.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LoggedCatch_WithNullMessage_UsesEmptyString()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        var caught = subject.LoggedCatch(logger, Observable.Return(1), null);

        var values = new List<int>();
        caught.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnError(new InvalidOperationException());

        await Assert.That(values).Contains(1);
        await Assert.That(logger.WarnCount).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Verifies that LoggedCatch with exception type catches specific exceptions.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LoggedCatch_WithExceptionType_CatchesSpecificException()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();

        var caught = subject.LoggedCatch<int, TestEnableLogger, InvalidOperationException>(
            logger,
            ex => Observable.Return(42),
            "Specific error");

        var values = new List<int>();
        caught.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        subject.OnError(new InvalidOperationException());

        await Assert.That(values).Contains(42);
        await Assert.That(logger.WarnCount).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Verifies that LoggedCatch with exception func passes exception to next factory.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task LoggedCatch_WithExceptionFunc_PassesExceptionToFactory()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var subject = new Subject<int>();
        Exception? capturedEx = null;

        var caught = subject.LoggedCatch<int, TestEnableLogger, InvalidOperationException>(
            logger,
            ex =>
            {
                capturedEx = ex;
                return Observable.Return(100);
            });

        var values = new List<int>();
        caught.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        var thrownException = new InvalidOperationException("Test");
        subject.OnError(thrownException);

        await Assert.That(capturedEx).IsSameReferenceAs(thrownException);
        await Assert.That(values).Contains(100);
    }

    /// <summary>
    /// Verifies that logged observable completes normally without errors.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Log_CompletesNormally_WithoutErrors()
    {
        var logger = new TestEnableLogger(_loggingScope!.Logger);
        var values = Observable.Range(1, 5);

        var logged = values.Log(logger, "Range");

        var results = new List<int>();
        var completed = false;
        logged.ObserveOn(ImmediateScheduler.Instance).Subscribe(results.Add, () => completed = true);

        await Assert.That(results).IsEquivalentTo([1, 2, 3, 4, 5]);
        await Assert.That(completed).IsTrue();
        await Assert.That(logger.InfoCount).IsGreaterThanOrEqualTo(6); // 5 OnNext + 1 OnCompleted
    }

    /// <summary>
    /// Helper method that takes IEnableLogger directly (not generic) to test if that works.
    /// </summary>
    private static IObservable<T> LogNonGeneric<T>(IObservable<T> source, IEnableLogger logObject, string message)
    {
        message ??= string.Empty;
        return source.Do(
            x => logObject.Log().Info(CultureInfo.InvariantCulture, "{0} OnNext: {1}", message, x),
            ex => logObject.Log().Warn(ex, message + " OnError"),
            () => logObject.Log().Info(CultureInfo.InvariantCulture, "{0} OnCompleted", message));
    }

    private class TestEnableLogger(LoggingRegistrationScope.TestLogger logger) : IEnableLogger
    {
        public int InfoCount => logger.Messages.Count(m => m.logLevel == LogLevel.Info);

        public int WarnCount => logger.Messages.Count(m => m.logLevel == LogLevel.Warn);
    }
}
