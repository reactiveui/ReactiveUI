// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using ReactiveUI.Device.Tests.Runner;
using UIKit;

namespace ReactiveUI.Device.Tests;

/// <summary>
/// The iOS head: runs every test in-process on the simulator, writes a TRX report and an exit-code file, and exits
/// with the test platform's exit code.
/// </summary>
/// <remarks>
/// <para>
/// The device-tests script launches the app with <c>xcrun simctl launch</c> and passes two environment variables
/// through <c>SIMCTL_CHILD_</c>: <c>RXUI_DEVICE_TEST_RESULTS</c>, a host folder for the report, and
/// <c>RXUI_DEVICE_TEST_ARGS</c>, extra test platform arguments. A simulator app shares the Mac's file system, so the
/// script reads the report where the app wrote it.
/// </para>
/// <para>Launch arguments also reach the test platform, so <c>dotnet test --device</c> can drive the app directly.</para>
/// </remarks>
internal static class IosTestHost
{
    /// <summary>The environment variable naming the results folder.</summary>
    private const string ResultsVariable = "RXUI_DEVICE_TEST_RESULTS";

    /// <summary>The environment variable carrying extra test platform arguments.</summary>
    private const string ArgumentsVariable = "RXUI_DEVICE_TEST_ARGS";

    /// <summary>The file the app writes its exit code to, next to the TRX report.</summary>
    private const string ExitCodeFileName = "exit-code.txt";

    /// <summary>The console, which <c>simctl launch --console-pty</c> streams to the script.</summary>
    private static readonly TextWriter Output = Console.Out;

    /// <summary>Gets the app's window, which the tests host controllers in.</summary>
    internal static UIWindow Window { get; private set; } = null!;

    /// <summary>Records the window and starts the test run on a pool thread.</summary>
    /// <param name="window">The app's window.</param>
    internal static void Start(UIWindow window)
    {
        Window = window;
        _ = Task.Run(RunTestsAsync);
    }

    /// <summary>Runs the tests, records the exit code and ends the process.</summary>
    /// <returns>A task that ends with the process.</returns>
    private static async Task RunTestsAsync()
    {
        var resultsDirectory = Environment.GetEnvironmentVariable(ResultsVariable) is { Length: > 0 } fromHost
            ? fromHost
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TestResults");

        string[] extraArguments =
        [
            .. (Environment.GetEnvironmentVariable(ArgumentsVariable) ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            .. Environment.GetCommandLineArgs().Skip(1),
        ];

        int exitCode;
        try
        {
            exitCode = await DeviceTestSession.RunAsync(resultsDirectory, extraArguments, new ConsoleReporter()).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await Output.WriteLineAsync($"[ERROR] The test session failed: {ex}").ConfigureAwait(false);
            exitCode = 1;
        }

        await File.WriteAllTextAsync(Path.Combine(resultsDirectory, ExitCodeFileName), exitCode.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        await Output.WriteLineAsync($"[EXIT] {exitCode}").ConfigureAwait(false);
        await Output.FlushAsync().ConfigureAwait(false);
        Environment.Exit(exitCode);
    }

    /// <summary>Writes each finished test to the console.</summary>
    private sealed class ConsoleReporter : IDeviceTestReporter
    {
        /// <inheritdoc/>
        public void OnResult(DeviceTestResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            if (result.Outcome is DeviceTestOutcome.Running)
            {
                return;
            }

            Output.WriteLine($"[{result.Outcome.ToString().ToUpperInvariant()}] {result.ClassName}.{result.DisplayName}");
            if (result.Outcome is not DeviceTestOutcome.Failed || result.Details is null)
            {
                return;
            }

            Output.WriteLine(result.Details);
        }

        /// <inheritdoc/>
        public void OnReport(string path) => Output.WriteLine($"[TRX] {path}");
    }
}
