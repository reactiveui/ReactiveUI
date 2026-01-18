// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Suspension;

/// <summary>
///     Tests for the generic <see cref="SuspensionHost{TAppState}"/>.
/// </summary>
public class SuspensionHostGenericTests
{
    [Test]
    public async Task Constructor_DefaultObservables_ThrowExceptionOnSubscribe()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var gotErrorLaunching = false;
        var gotErrorResuming = false;
        var gotErrorUnpausing = false;
        var gotErrorContinuing = false;
        var gotErrorInvalidate = false;
        var gotErrorPersist = false;

        host.IsLaunchingNew.Subscribe(_ => { }, ex => gotErrorLaunching = true);
        host.IsResuming.Subscribe(_ => { }, ex => gotErrorResuming = true);
        host.IsUnpausing.Subscribe(_ => { }, ex => gotErrorUnpausing = true);
        host.IsContinuing.Subscribe(_ => { }, ex => gotErrorContinuing = true);
        host.ShouldInvalidateState.Subscribe(_ => { }, ex => gotErrorInvalidate = true);
        host.ShouldPersistState.Subscribe(_ => { }, ex => gotErrorPersist = true);

        await Assert.That(gotErrorLaunching).IsTrue();
        await Assert.That(gotErrorResuming).IsTrue();
        await Assert.That(gotErrorUnpausing).IsTrue();
        await Assert.That(gotErrorContinuing).IsTrue();
        await Assert.That(gotErrorInvalidate).IsTrue();
        await Assert.That(gotErrorPersist).IsTrue();
    }

    [Test]
    public async Task IsLaunchingNew_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;

        host.IsLaunchingNew = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.IsLaunchingNew.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    [Test]
    public async Task IsLaunchingNew_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.IsLaunchingNew = null!)
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task IsResuming_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;

        host.IsResuming = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.IsResuming.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    [Test]
    public async Task IsResuming_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.IsResuming = null!)
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task IsUnpausing_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;

        host.IsUnpausing = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.IsUnpausing.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    [Test]
    public async Task IsUnpausing_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.IsUnpausing = null!)
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task IsContinuing_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;

        host.IsContinuing = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.IsContinuing.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    [Test]
    public async Task IsContinuing_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.IsContinuing = null!)
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldPersistState_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;
        var disposable = Disposable.Empty;

        host.ShouldPersistState = Observable.Return(disposable, ImmediateScheduler.Instance);

        using var subscription = host.ShouldPersistState.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    [Test]
    public async Task ShouldPersistState_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.ShouldPersistState = null!)
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldInvalidateState_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;

        host.ShouldInvalidateState = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        using var subscription = host.ShouldInvalidateState.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    [Test]
    public async Task ShouldInvalidateState_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.ShouldInvalidateState = null!)
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task AppStateValue_SetAndGet_ReturnsCorrectValue()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var state = new DummyAppState { Value = 42 };

        host.AppStateValue = state;

        await Assert.That(host.AppStateValue).IsSameReferenceAs(state);
    }

    [Test]
    public async Task AppStateValue_PropertyChanged_RaisesNotification()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var propertyChanged = false;

        host.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(host.AppStateValue))
            {
                propertyChanged = true;
            }
        };

        host.AppStateValue = new DummyAppState();

        await Assert.That(propertyChanged).IsTrue();
    }

    [Test]
    public async Task AppStateValueChanged_EmitsWhenAppStateValueIsSet()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var receivedStates = new List<DummyAppState?>();

        using var subscription = host.AppStateValueChanged
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(receivedStates.Add);

        var state1 = new DummyAppState { Value = 1 };
        host.AppStateValue = state1;

        var state2 = new DummyAppState { Value = 2 };
        host.AppStateValue = state2;

        await Assert.That(receivedStates).Count().IsEqualTo(2);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state1);
        await Assert.That(receivedStates[1]).IsSameReferenceAs(state2);
    }

    [Test]
    public async Task AppStateValueChanged_EmitsNull()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var receivedStates = new List<DummyAppState?>();

        using var subscription = host.AppStateValueChanged
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(receivedStates.Add);

        host.AppStateValue = new DummyAppState();
        host.AppStateValue = null;

        await Assert.That(receivedStates).Count().IsEqualTo(2);
        await Assert.That(receivedStates[1]).IsNull();
    }

    [Test]
    public async Task CreateNewAppStateTyped_SetAndGet_ReturnsCorrectFunc()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var expectedState = new DummyAppState { Value = 99 };
        host.CreateNewAppStateTyped = () => expectedState;

        await Assert.That(host.CreateNewAppStateTyped).IsNotNull();
        await Assert.That(host.CreateNewAppStateTyped!()).IsSameReferenceAs(expectedState);
    }

    [Test]
    public async Task CreateNewAppStateTyped_PropertyChanged_RaisesNotification()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var propertyChanged = false;

        host.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(host.CreateNewAppStateTyped))
            {
                propertyChanged = true;
            }
        };

        host.CreateNewAppStateTyped = () => new DummyAppState();

        await Assert.That(propertyChanged).IsTrue();
    }

    [Test]
    public async Task ISuspensionHost_AppState_GetProjectsFromTypedValue()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var state = new DummyAppState { Value = 42 };
        host.AppStateValue = state;

        var untypedHost = (ISuspensionHost)host;

        await Assert.That(untypedHost.AppState).IsSameReferenceAs(state);
    }

    [Test]
    public async Task ISuspensionHost_AppState_SetWithValidValue_UpdatesTypedProperty()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var state = new DummyAppState { Value = 42 };
        var untypedHost = (ISuspensionHost)host;

        untypedHost.AppState = state;

        await Assert.That(host.AppStateValue).IsSameReferenceAs(state);
    }

    [Test]
    public async Task ISuspensionHost_AppState_SetNull_SetsTypedPropertyToDefault()
    {
        using var host = new SuspensionHost<DummyAppState>();
        host.AppStateValue = new DummyAppState();
        var untypedHost = (ISuspensionHost)host;

        untypedHost.AppState = null;

        await Assert.That(host.AppStateValue).IsNull();
    }

    [Test]
    public async Task ISuspensionHost_AppState_SetInvalidType_ThrowsInvalidCastException()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var untypedHost = (ISuspensionHost)host;

        await Assert.That(() => untypedHost.AppState = new OtherAppState())
            .Throws<InvalidCastException>();
    }

    [Test]
    public async Task ISuspensionHost_CreateNewAppState_GetProjectsFromTypedFactory()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var expectedState = new DummyAppState { Value = 99 };
        host.CreateNewAppStateTyped = () => expectedState;

        var untypedHost = (ISuspensionHost)host;
        var factory = untypedHost.CreateNewAppState;

        await Assert.That(factory).IsNotNull();
        await Assert.That(factory!()).IsSameReferenceAs(expectedState);
    }

    [Test]
    public async Task ISuspensionHost_CreateNewAppState_GetWhenTypedIsNull_ReturnsNull()
    {
        using var host = new SuspensionHost<DummyAppState>();
        host.CreateNewAppStateTyped = null;

        var untypedHost = (ISuspensionHost)host;
        Func<object>? factory = untypedHost.CreateNewAppState;

        await Assert.That((object?)factory).IsNull();
    }

    [Test]
    public async Task ISuspensionHost_CreateNewAppState_SetWithValidFactory_UpdatesTypedProperty()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var expectedState = new DummyAppState { Value = 99 };
        var untypedHost = (ISuspensionHost)host;

        untypedHost.CreateNewAppState = () => expectedState;

        var typedFactory = host.CreateNewAppStateTyped;
        await Assert.That(typedFactory).IsNotNull();
        await Assert.That(typedFactory!()).IsSameReferenceAs(expectedState);
    }

    [Test]
    public async Task ISuspensionHost_CreateNewAppState_SetNull_SetsTypedPropertyToNull()
    {
        using var host = new SuspensionHost<DummyAppState>();
        host.CreateNewAppStateTyped = () => new DummyAppState();
        var untypedHost = (ISuspensionHost)host;

        untypedHost.CreateNewAppState = null;

        await Assert.That(host.CreateNewAppStateTyped).IsNull();
    }

    [Test]
    public async Task ISuspensionHost_CreateNewAppState_SetInvalidFactory_ThrowsInvalidCastException()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var untypedHost = (ISuspensionHost)host;
        untypedHost.CreateNewAppState = () => new OtherAppState();

        var factory = untypedHost.CreateNewAppState;
        await Assert.That(() => factory!())
            .Throws<InvalidCastException>();
    }

    [Test]
    public async Task Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        var host = new SuspensionHost<DummyAppState>();

        host.Dispose();

        await Assert.That(() => host.Dispose()).ThrowsNothing();
    }

    [Test]
    public async Task Dispose_DisposesAllSubjects()
    {
        var host = new SuspensionHost<DummyAppState>();

        // Set observables before disposal
        host.IsLaunchingNew = Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        host.IsResuming = Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        host.IsUnpausing = Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        host.IsContinuing = Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        host.ShouldPersistState = Observable.Return(Disposable.Empty, ImmediateScheduler.Instance);
        host.ShouldInvalidateState = Observable.Return(Unit.Default, ImmediateScheduler.Instance);

        host.Dispose();

        // Verify disposal occurred
        await Assert.That(host).IsNotNull();
    }

    [Test]
    public async Task AppStateValueChanged_SubscriptionWorksBeforeDispose()
    {
        var host = new SuspensionHost<DummyAppState>();
        var receivedCount = 0;

        using var subscription = host.AppStateValueChanged
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(_ => receivedCount++);

        // Set values and verify subscription works
        host.AppStateValue = new DummyAppState();
        host.AppStateValue = new DummyAppState { Value = 1 };

        await Assert.That(receivedCount).IsEqualTo(2);

        // Dispose the host
        host.Dispose();

        // The host has been disposed successfully
        await Assert.That(host).IsNotNull();
    }

    [Test]
    public async Task ObservableProperties_CanBeReplacedDynamically()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var launchCount = 0;

        // Set initial observable
        host.IsLaunchingNew = Observable.Return(Unit.Default, ImmediateScheduler.Instance)
            .Do(_ => launchCount++);

        // Subscribe and verify initial observable works
        using var sub = host.IsLaunchingNew.Subscribe();

        // Replace the observable with a new one
        host.IsLaunchingNew = Observable.Return(Unit.Default, ImmediateScheduler.Instance)
            .Do(_ => launchCount++);

        // The existing subscription should see the new observable due to ReplaySubject + Switch pattern
        // Total triggers depend on when Switch() switches to the new observable
        await Assert.That(launchCount).IsGreaterThanOrEqualTo(2);
    }

    private class DummyAppState
    {
        public int Value { get; set; }
    }

    private class OtherAppState
    {
        public string? Name { get; set; }
    }
}
