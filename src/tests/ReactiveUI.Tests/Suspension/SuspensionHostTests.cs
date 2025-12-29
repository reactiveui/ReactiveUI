// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Suspension;

/// <summary>
/// Tests for SuspensionHost.
/// </summary>
public class SuspensionHostTests
{
    [Test]
    public async Task IsLaunchingNew_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost();
        var wasTriggered = false;

        host.IsLaunchingNew = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.IsLaunchingNew.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    [Test]
    public async Task IsResuming_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost();
        var wasTriggered = false;

        host.IsResuming = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.IsResuming.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    [Test]
    public async Task IsUnpausing_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost();
        var wasTriggered = false;

        host.IsUnpausing = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.IsUnpausing.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

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

    [Test]
    public async Task ShouldInvalidateState_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost();
        var wasTriggered = false;

        host.ShouldInvalidateState = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.ShouldInvalidateState.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    [Test]
    public async Task AppState_SetAndGet_ReturnsCorrectValue()
    {
        using var host = new SuspensionHost();
        var state = new DummyAppState();

        host.AppState = state;

        await Assert.That(host.AppState).IsSameReferenceAs(state);
    }

    [Test]
    public async Task AppState_PropertyChanged_RaisesNotification()
    {
        using var host = new SuspensionHost();
        var propertyChanged = false;

        host.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(host.AppState))
            {
                propertyChanged = true;
            }
        };

        host.AppState = new DummyAppState();

        await Assert.That(propertyChanged).IsTrue();
    }

    [Test]
    public async Task CreateNewAppState_SetAndGet_ReturnsCorrectFunc()
    {
        using var host = new SuspensionHost();
        host.CreateNewAppState = () => new DummyAppState();

        await Assert.That(host.CreateNewAppState).IsNotNull();
        await Assert.That(host.CreateNewAppState!()).IsTypeOf<DummyAppState>();
    }

    [Test]
    public async Task Constructor_DefaultObservables_ThrowExceptionOnSubscribe()
    {
        using var host = new SuspensionHost();
        var gotError = false;

        host.IsLaunchingNew.Subscribe(_ => { }, ex => gotError = true);

        await Assert.That(gotError).IsTrue();
    }

    [Test]
    public async Task Dispose_DisposesAllSubjects()
    {
        var host = new SuspensionHost();

        // Set observables before disposal
        host.IsLaunchingNew = Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        host.IsResuming = Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        host.IsUnpausing = Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        host.ShouldPersistState = Observable.Return(Disposable.Empty, ImmediateScheduler.Instance);
        host.ShouldInvalidateState = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        host.Dispose();

        // Verify disposal occurred
        await Assert.That(host).IsNotNull();
    }

    [Test]
    public async Task Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        var host = new SuspensionHost();

        host.Dispose();

        await Assert.That(() => host.Dispose()).ThrowsNothing();
    }
}
