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
/// Tests using this scope should also be marked with [NotInParallel] to prevent
/// concurrent modifications to the shared state.
/// </remarks>
/// <example>
/// <code>
/// [TestFixture]
/// [NotInParallel]
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
public sealed class RxSchedulersSchedulersScope : IDisposable
{
    private readonly IScheduler _mainThreadScheduler;
    private readonly IScheduler _taskpoolScheduler;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="RxSchedulersSchedulersScope"/> class.
    /// Snapshots the current RxApp scheduler state.
    /// </summary>
    public RxSchedulersSchedulersScope()
    {
        _mainThreadScheduler = RxSchedulers.MainThreadScheduler;
        _taskpoolScheduler = RxSchedulers.TaskpoolScheduler;
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

        RxSchedulers.MainThreadScheduler = _mainThreadScheduler;
        RxSchedulers.TaskpoolScheduler = _taskpoolScheduler;
        _disposed = true;
    }
}
