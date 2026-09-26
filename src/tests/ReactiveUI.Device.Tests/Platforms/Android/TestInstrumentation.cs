// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;
using Android.Runtime;
using ReactiveUI.Device.Tests.Runner;
using AndroidResult = Android.App.Result;

namespace ReactiveUI.Device.Tests;

/// <summary>
/// The Android head: <c>dotnet test --device</c> starts this instrumentation with <c>am instrument</c>, and it runs
/// every test in-process on the emulator.
/// </summary>
/// <remarks>
/// <para>
/// The .NET Android SDK's <c>Microsoft.Android.Run</c> host reads the instrumentation's status stream. Each test sends an
/// <c>event=start</c> status when it starts and an <c>event=finish</c> status with its outcome when it finishes, so
/// <c>dotnet test</c> shows results live. The final bundle carries the counts and the on-device TRX path, which the
/// host pulls when the stream is incomplete, for example after a crash.
/// </para>
/// <para>
/// Extra instrumentation arguments reach the test platform: <c>adb shell am instrument -e args "--treenode-filter /*/*/Foo/*"</c>.
/// </para>
/// </remarks>
[Instrumentation(Name = "net.reactiveui.devicetests.TestInstrumentation")]
public sealed class TestInstrumentation : Instrumentation
{
    /// <summary>The instrumentation arguments, read when the instrumentation starts.</summary>
    private Bundle? _arguments;

    /// <summary>Initializes a new instance of the <see cref="TestInstrumentation"/> class.</summary>
    /// <param name="handle">The JNI handle.</param>
    /// <param name="ownership">The handle ownership.</param>
    public TestInstrumentation(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <summary>Gets the running instrumentation, which tests use to start activities.</summary>
    internal static TestInstrumentation Current { get; private set; } = null!;

    /// <inheritdoc/>
    public override void OnCreate(Bundle? arguments)
    {
        base.OnCreate(arguments);
        _arguments = arguments;
        Current = this;
        Start();
    }

    /// <inheritdoc/>
    public override void OnStart()
    {
        base.OnStart();

        var reporter = new StatusReporter(this);
        var summary = new Bundle();

        try
        {
            var resultsDirectory = Path.Combine(
                Context!.GetExternalFilesDir(null)?.AbsolutePath ?? Path.GetTempPath(),
                "TestResults");
            var exitCode = DeviceTestSession.RunAsync(resultsDirectory, ReadExtraArguments(_arguments), reporter)
                .GetAwaiter()
                .GetResult();

            summary.PutInt("passed", reporter.Passed);
            summary.PutInt("failed", reporter.Failed);
            summary.PutInt("skipped", reporter.Skipped);
            summary.PutInt("exitCode", exitCode);
            if (reporter.ReportPath is not null)
            {
                summary.PutString("resultsPath", reporter.ReportPath);
            }

            Finish(AndroidResult.Ok, summary);
        }
        catch (Exception ex)
        {
            summary.PutString("error", ex.ToString());
            Finish(AndroidResult.Canceled, summary);
        }
    }

    /// <summary>Splits the optional <c>args</c> instrumentation extra into test platform arguments.</summary>
    /// <param name="arguments">The instrumentation arguments.</param>
    /// <returns>The extra arguments; empty when none were passed.</returns>
    private static string[] ReadExtraArguments(Bundle? arguments) =>
        arguments?.GetString("args") is { Length: > 0 } extra
            ? extra.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : [];

    /// <summary>Sends each result as an instrumentation status and counts the outcomes.</summary>
    /// <param name="instrumentation">The instrumentation that sends the statuses.</param>
    private sealed class StatusReporter(Instrumentation instrumentation) : IDeviceTestReporter
    {
        /// <summary>The status code <c>am instrument -r</c> reports for a test that started (1).</summary>
        private const AndroidResult StartedStatus = AndroidResult.FirstUser;

        /// <summary>The status code <c>am instrument -r</c> reports for a test that passed or was skipped (0).</summary>
        private const AndroidResult FinishedStatus = AndroidResult.Canceled;

        /// <summary>The status code <c>am instrument -r</c> reports for a test that failed.</summary>
        private const AndroidResult FailedStatus = (AndroidResult)(-2);

        /// <summary>The number of passed tests.</summary>
        private int _passed;

        /// <summary>The number of failed tests.</summary>
        private int _failed;

        /// <summary>The number of skipped tests.</summary>
        private int _skipped;

        /// <summary>Gets the number of passed tests.</summary>
        public int Passed => Volatile.Read(ref _passed);

        /// <summary>Gets the number of failed tests.</summary>
        public int Failed => Volatile.Read(ref _failed);

        /// <summary>Gets the number of skipped tests.</summary>
        public int Skipped => Volatile.Read(ref _skipped);

        /// <summary>Gets the on-device path of the TRX report, once written.</summary>
        public string? ReportPath { get; private set; }

        /// <inheritdoc/>
        public void OnReport(string path) => ReportPath = path;

        /// <inheritdoc/>
        public void OnResult(DeviceTestResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            var status = new Bundle();
            status.PutString("test", result.Uid);
            status.PutString("name", result.DisplayName);
            if (result.ClassName is not null)
            {
                status.PutString("class", result.ClassName);
            }

            if (result.Outcome is DeviceTestOutcome.Running)
            {
                status.PutString("event", "start");
                instrumentation.SendStatus(StartedStatus, status);
                return;
            }

            status.PutString("event", "finish");
            status.PutString("outcome", result.Outcome switch
            {
                DeviceTestOutcome.Passed => "passed",
                DeviceTestOutcome.Skipped => "skipped",
                _ => "failed",
            });

            if (result.Message is not null)
            {
                status.PutString("message-b64", ToBase64(result.Message));
            }

            if (result.Details is not null)
            {
                status.PutString("stack-b64", ToBase64(result.Details));
            }

            _ = result.Outcome switch
            {
                DeviceTestOutcome.Passed => Interlocked.Increment(ref _passed),
                DeviceTestOutcome.Skipped => Interlocked.Increment(ref _skipped),
                _ => Interlocked.Increment(ref _failed),
            };

            instrumentation.SendStatus(result.Outcome is DeviceTestOutcome.Failed ? FailedStatus : FinishedStatus, status);
        }

        /// <summary>Encodes text as base64 so multi-line messages survive the status stream.</summary>
        /// <param name="text">The text to encode.</param>
        /// <returns>The base64 text.</returns>
        private static string ToBase64(string text) => Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
    }
}
