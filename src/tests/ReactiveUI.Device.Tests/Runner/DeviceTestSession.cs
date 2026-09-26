// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Testing.Extensions;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using TUnit.Engine.Extensions;

namespace ReactiveUI.Device.Tests.Runner;

/// <summary>
/// Runs every test in this assembly in-process on the device through Microsoft.Testing.Platform, and reports each
/// result to a platform head as it finishes.
/// </summary>
/// <remarks>
/// The Android head is an instrumentation that relays results to <c>dotnet test</c>; the iOS head is an app delegate
/// that writes a TRX report and an exit-code file for the device-tests script. Both share this session.
/// </remarks>
internal static class DeviceTestSession
{
    /// <summary>The TRX report's file name inside the results directory.</summary>
    internal const string ReportFileName = "device-tests.trx";

    /// <summary>Runs the tests and returns the Microsoft.Testing.Platform exit code.</summary>
    /// <param name="resultsDirectory">The directory the TRX report is written to.</param>
    /// <param name="extraArguments">Extra Microsoft.Testing.Platform arguments, such as a tree-node filter.</param>
    /// <param name="reporter">Receives each test result and the TRX report path.</param>
    /// <returns>The Microsoft.Testing.Platform exit code: 0 when every test passed.</returns>
    internal static async Task<int> RunAsync(string resultsDirectory, IReadOnlyList<string> extraArguments, IDeviceTestReporter reporter)
    {
        ArgumentNullException.ThrowIfNull(resultsDirectory);
        ArgumentNullException.ThrowIfNull(extraArguments);
        ArgumentNullException.ThrowIfNull(reporter);

        _ = Directory.CreateDirectory(resultsDirectory);
        ConsoleCancelKeyShim.Install();

        string[] arguments =
        [
            "--results-directory", resultsDirectory,
            "--report-trx",
            "--report-trx-filename", ReportFileName,
            "--disable-logo",
            .. extraArguments,
        ];

        var builder = await TestApplication.CreateBuilderAsync(arguments).ConfigureAwait(false);
        builder.AddTUnit();
        builder.AddTrxReportProvider();
        builder.TestHost.AddDataConsumer(_ => new ResultRelay(reporter));

        using var app = await builder.BuildAsync().ConfigureAwait(false);
        return await app.RunAsync().ConfigureAwait(false);
    }

    /// <summary>Maps a Microsoft.Testing.Platform test node to a <see cref="DeviceTestResult"/>.</summary>
    /// <param name="node">The test node from an update message.</param>
    /// <returns>The result, or <see langword="null"/> for a discovery update.</returns>
    internal static DeviceTestResult? ToResult(TestNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var state = node.Properties.SingleOrDefault<TestNodeStateProperty>();
        if (ToOutcome(state) is not { } outcome)
        {
            return null;
        }

        var exception = state switch
        {
            FailedTestNodeStateProperty failed => failed.Exception,
            ErrorTestNodeStateProperty error => error.Exception,
            _ => null,
        };

        return new(
            node.Uid.Value,
            node.DisplayName,
            ClassNameOf(node),
            outcome,
            exception?.Message ?? state!.Explanation,
            exception?.ToString());
    }

    /// <summary>Maps a test node state to an outcome.</summary>
    /// <param name="state">The state, which a discovery update does not carry.</param>
    /// <returns>The outcome, or <see langword="null"/> when the node carries no execution state.</returns>
    private static DeviceTestOutcome? ToOutcome(TestNodeStateProperty? state) => state switch
    {
        PassedTestNodeStateProperty => DeviceTestOutcome.Passed,
        SkippedTestNodeStateProperty => DeviceTestOutcome.Skipped,
        FailedTestNodeStateProperty or ErrorTestNodeStateProperty or TimeoutTestNodeStateProperty => DeviceTestOutcome.Failed,
        InProgressTestNodeStateProperty => DeviceTestOutcome.Running,
        _ => null,
    };

    /// <summary>Returns the fully qualified class name of a test node's method, when the node has one.</summary>
    /// <param name="node">The test node.</param>
    /// <returns>The class name, or <see langword="null"/>.</returns>
    private static string? ClassNameOf(TestNode node)
    {
        if (node.Properties.SingleOrDefault<TestMethodIdentifierProperty>() is not { } method)
        {
            return null;
        }

        return method.Namespace.Length == 0 ? method.TypeName : $"{method.Namespace}.{method.TypeName}";
    }

    /// <summary>Forwards each test update and the TRX artifact to the platform reporter.</summary>
    /// <param name="reporter">The platform reporter.</param>
    private sealed class ResultRelay(IDeviceTestReporter reporter) : IDataConsumer
    {
        /// <inheritdoc/>
        public Type[] DataTypesConsumed => [typeof(TestNodeUpdateMessage), typeof(SessionFileArtifact)];

        /// <inheritdoc/>
        public string Uid => nameof(ResultRelay);

        /// <inheritdoc/>
        public string Version => "1.0.0";

        /// <inheritdoc/>
        public string DisplayName => "ReactiveUI device result relay";

        /// <inheritdoc/>
        public string Description => "Reports each device test result to the platform head.";

        /// <inheritdoc/>
        public Task<bool> IsEnabledAsync() => Task.FromResult(true);

        /// <inheritdoc/>
        public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
        {
            switch (value)
            {
                case SessionFileArtifact artifact when artifact.FileInfo.Extension is ".trx":
                    {
                        reporter.OnReport(artifact.FileInfo.FullName);
                        break;
                    }

                case TestNodeUpdateMessage { TestNode: var node } when ToResult(node) is { } result:
                    {
                        reporter.OnResult(result);
                        break;
                    }
            }

            return Task.CompletedTask;
        }
    }
}
