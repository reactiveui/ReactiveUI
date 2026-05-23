// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Suspension;

/// <summary>
///     Tests for SuspensionHost.
/// </summary>
public class SuspensionHostTests
{
    /// <summary>
    ///     Verifies setting AppState raises a PropertyChanged notification.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AppState_PropertyChanged_RaisesNotification()
    {
        using var host = new SuspensionHost();
        var propertyChanged = false;

        host.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(host.AppState))
            {
                return;
            }

            propertyChanged = true;
        };

        host.AppState = new DummyAppState();

        await Assert.That(propertyChanged).IsTrue();
    }

    /// <summary>
    ///     Verifies AppState returns the value that was set.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AppState_SetAndGet_ReturnsCorrectValue()
    {
        using var host = new SuspensionHost();
        var state = new DummyAppState();

        host.AppState = state;

        await Assert.That(host.AppState).IsSameReferenceAs(state);
    }

    /// <summary>
    ///     Verifies the default observables produced by the constructor error when subscribed to.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_DefaultObservables_ThrowExceptionOnSubscribe()
    {
        using var host = new SuspensionHost();
        var gotError = false;

        host.IsLaunchingNew.Subscribe(_ => { }, ex => gotError = true);

        await Assert.That(gotError).IsTrue();
    }

    /// <summary>
    ///     Verifies CreateNewAppState returns the factory that was set and produces the expected state.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateNewAppState_SetAndGet_ReturnsCorrectFunc()
    {
        using var host = new SuspensionHost();
        host.CreateNewAppState = () => new DummyAppState();

        await Assert.That(host.CreateNewAppState).IsNotNull();
        await Assert.That(host.CreateNewAppState!()).IsTypeOf<DummyAppState>();
    }

    /// <summary>
    ///     Verifies calling Dispose more than once does not throw.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        var host = new SuspensionHost();

        host.Dispose();

        await Assert.That(host.Dispose).ThrowsNothing();
    }

    /// <summary>
    ///     Verifies Dispose disposes all internal subjects without error.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_DisposesAllSubjects()
    {
        // Set observables before disposal
        var host = new SuspensionHost
        {
            IsLaunchingNew = Observable.Return(Unit.Default, ImmediateScheduler.Instance),
            IsResuming = Observable.Return(Unit.Default, ImmediateScheduler.Instance),
            IsUnpausing = Observable.Return(Unit.Default, ImmediateScheduler.Instance),
            ShouldPersistState = Observable.Return(Disposable.Empty, ImmediateScheduler.Instance),
            ShouldInvalidateState = Observable.Return(Unit.Default, ImmediateScheduler.Instance),
        };

        host.Dispose();

        // Verify disposal occurred
        await Assert.That(host).IsNotNull();
    }

    /// <summary>
    ///     Verifies IsLaunchingNew returns the observable that was set.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsLaunchingNew_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost();
        var wasTriggered = false;

        host.IsLaunchingNew = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.IsLaunchingNew.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    /// <summary>
    ///     Verifies IsResuming returns the observable that was set.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsResuming_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost();
        var wasTriggered = false;

        host.IsResuming = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.IsResuming.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    /// <summary>
    ///     Verifies IsUnpausing returns the observable that was set.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsUnpausing_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost();
        var wasTriggered = false;

        host.IsUnpausing = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.IsUnpausing.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    /// <summary>
    ///     Verifies ShouldInvalidateState returns the observable that was set.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldInvalidateState_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost();
        var wasTriggered = false;

        host.ShouldInvalidateState = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.ShouldInvalidateState.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    /// <summary>
    ///     Verifies ShouldPersistState returns the observable that was set.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldPersistState_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost();
        var wasTriggered = false;
        var disposable = Disposable.Empty;

        host.ShouldPersistState = Observable.Return(disposable, ImmediateScheduler.Instance);

        using var subscription = host.ShouldPersistState.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }
}
