// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Infrastructure.StaticState;

/// <summary>
/// A disposable scope that snapshots and restores RxApp scheduler state.
/// Use this in test fixtures that modify RxApp.MainThreadScheduler or RxApp.TaskpoolScheduler
/// to ensure static state is properly restored after tests complete.
/// </summary>
/// <remarks>
/// This helper is necessary because RxApp maintains static/global scheduler references
/// that can leak between parallel test executions, causing intermittent failures.
/// Tests using this scope should also be marked with [NonParallelizable] to prevent
/// concurrent modifications to the shared state.
/// </remarks>
/// <example>
/// <code>
/// [TestFixture]
/// [NonParallelizable]
/// public class MyTests
/// {
///     private RxAppSchedulersScope _schedulersScope;
///
///     [SetUp]
///     public void SetUp()
///     {
///         _schedulersScope = new RxAppSchedulersScope();
///         // Now safe to modify RxApp schedulers
///     }
///
///     [TearDown]
///     public void TearDown()
///     {
///         _schedulersScope?.Dispose();
///     }
/// }
/// </code>
/// </example>
public sealed class RxAppSchedulersScope : IDisposable
{
    private readonly IScheduler _mainThreadScheduler;
    private readonly IScheduler _taskpoolScheduler;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="RxAppSchedulersScope"/> class.
    /// Snapshots the current RxApp scheduler state.
    /// </summary>
    public RxAppSchedulersScope()
    {
        _mainThreadScheduler = RxApp.MainThreadScheduler;
        _taskpoolScheduler = RxApp.TaskpoolScheduler;
    }

    /// <summary>
    /// Restores the RxApp scheduler state to what it was when this scope was created.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        RxApp.MainThreadScheduler = _mainThreadScheduler;
        RxApp.TaskpoolScheduler = _taskpoolScheduler;
        _disposed = true;
    }
}
