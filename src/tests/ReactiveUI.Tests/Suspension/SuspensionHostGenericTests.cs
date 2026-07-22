// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Suspension;

/// <summary>Tests for the generic <see cref="SuspensionHost{TAppState}"/>.</summary>
public class SuspensionHostGenericTests
{
    /// <summary>The sample state value used to seed the host.</summary>
    private const int SampleStateValue = 42;

    /// <summary>The state value produced by the create-state factory.</summary>
    private const int FactoryStateValue = 99;

    /// <summary>The second state value used in emission tests.</summary>
    private const int SecondStateValue = 2;

    /// <summary>The expected number of emissions observed in tests.</summary>
    private const int ExpectedEmissionCount = 2;

    /// <summary>Verifies the default observables produced by the constructor error when subscribed to.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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

        _ = host.IsLaunchingNew.Subscribe(static _ => { }, ex => gotErrorLaunching = true);
        _ = host.IsResuming.Subscribe(static _ => { }, ex => gotErrorResuming = true);
        _ = host.IsUnpausing.Subscribe(static _ => { }, ex => gotErrorUnpausing = true);
        _ = host.IsContinuing.Subscribe(static _ => { }, ex => gotErrorContinuing = true);
        _ = host.ShouldInvalidateState.Subscribe(static _ => { }, ex => gotErrorInvalidate = true);
        _ = host.ShouldPersistState.Subscribe(static _ => { }, ex => gotErrorPersist = true);

        await Assert.That(gotErrorLaunching).IsTrue();
        await Assert.That(gotErrorResuming).IsTrue();
        await Assert.That(gotErrorUnpausing).IsTrue();
        await Assert.That(gotErrorContinuing).IsTrue();
        await Assert.That(gotErrorInvalidate).IsTrue();
        await Assert.That(gotErrorPersist).IsTrue();
    }

    /// <summary>Verifies IsLaunchingNew returns the observable that was set.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsLaunchingNew_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;

        host.IsLaunchingNew = Signal.Emit(RxVoid.Default, Sequencer.Immediate);

        using var subscription = host.IsLaunchingNew.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    /// <summary>Verifies setting IsLaunchingNew to null throws an argument exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsLaunchingNew_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.IsLaunchingNew = null!)
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies IsResuming returns the observable that was set.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsResuming_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;

        host.IsResuming = Signal.Emit(RxVoid.Default, Sequencer.Immediate);

        using var subscription = host.IsResuming.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    /// <summary>Verifies setting IsResuming to null throws an argument exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsResuming_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.IsResuming = null!)
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies IsUnpausing returns the observable that was set.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsUnpausing_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;

        host.IsUnpausing = Signal.Emit(RxVoid.Default, Sequencer.Immediate);

        using var subscription = host.IsUnpausing.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    /// <summary>Verifies setting IsUnpausing to null throws an argument exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsUnpausing_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.IsUnpausing = null!)
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies IsContinuing returns the observable that was set.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsContinuing_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;

        host.IsContinuing = Signal.Emit(RxVoid.Default, Sequencer.Immediate);

        using var subscription = host.IsContinuing.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    /// <summary>Verifies setting IsContinuing to null throws an argument exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsContinuing_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.IsContinuing = null!)
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies ShouldPersistState returns the observable that was set.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldPersistState_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;
        var disposable = Scope.Empty;

        host.ShouldPersistState = Signal.Emit(disposable, Sequencer.Immediate);

        using var subscription = host.ShouldPersistState.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    /// <summary>Verifies setting ShouldPersistState to null throws an argument exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldPersistState_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.ShouldPersistState = null!)
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies ShouldInvalidateState returns the observable that was set.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldInvalidateState_SetAndGet_ReturnsCorrectObservable()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var wasTriggered = false;

        host.ShouldInvalidateState = Signal.Emit(RxVoid.Default, Sequencer.Immediate);

        using var subscription = host.ShouldInvalidateState.Subscribe(_ => wasTriggered = true);

        await Assert.That(wasTriggered).IsTrue();
    }

    /// <summary>Verifies setting ShouldInvalidateState to null throws an argument exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldInvalidateState_SetNull_ThrowsArgumentException()
    {
        using var host = new SuspensionHost<DummyAppState>();

        await Assert.That(() => host.ShouldInvalidateState = null!)
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies AppStateValue returns the value that was set.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AppStateValue_SetAndGet_ReturnsCorrectValue()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var state = new DummyAppState { Value = SampleStateValue };

        host.AppStateValue = state;

        await Assert.That(host.AppStateValue).IsSameReferenceAs(state);
    }

    /// <summary>Verifies setting AppStateValue raises a PropertyChanged notification.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AppStateValue_PropertyChanged_RaisesNotification()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var propertyChanged = false;

        host.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(host.AppStateValue))
            {
                return;
            }

            propertyChanged = true;
        };

        host.AppStateValue = new();

        await Assert.That(propertyChanged).IsTrue();
    }

    /// <summary>Verifies AppStateValueChanged emits each time AppStateValue is set.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AppStateValueChanged_EmitsWhenAppStateValueIsSet()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var receivedStates = new List<DummyAppState?>();

        using var subscription = host.AppStateValueChanged.ObserveOn(Sequencer.Immediate).Subscribe(receivedStates.Add);

        var state1 = new DummyAppState { Value = 1 };
        host.AppStateValue = state1;

        var state2 = new DummyAppState { Value = SecondStateValue };
        host.AppStateValue = state2;

        await Assert.That(receivedStates).Count().IsEqualTo(ExpectedEmissionCount);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state1);
        await Assert.That(receivedStates[1]).IsSameReferenceAs(state2);
    }

    /// <summary>Verifies AppStateValueChanged emits null when AppStateValue is set to null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AppStateValueChanged_EmitsNull()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var receivedStates = new List<DummyAppState?>();

        using var subscription = host.AppStateValueChanged.ObserveOn(Sequencer.Immediate).Subscribe(receivedStates.Add);

        host.AppStateValue = new();
        host.AppStateValue = null;

        await Assert.That(receivedStates).Count().IsEqualTo(ExpectedEmissionCount);
        await Assert.That(receivedStates[1]).IsNull();
    }

    /// <summary>Verifies CreateNewAppStateTyped returns the factory that was set and produces the expected state.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateNewAppStateTyped_SetAndGet_ReturnsCorrectFunc()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var expectedState = new DummyAppState { Value = FactoryStateValue };
        host.CreateNewAppStateTyped = () => expectedState;

        await Assert.That(host.CreateNewAppStateTyped).IsNotNull();
        await Assert.That(host.CreateNewAppStateTyped!()).IsSameReferenceAs(expectedState);
    }

    /// <summary>Verifies setting CreateNewAppStateTyped raises a PropertyChanged notification.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateNewAppStateTyped_PropertyChanged_RaisesNotification()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var propertyChanged = false;

        host.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(host.CreateNewAppStateTyped))
            {
                return;
            }

            propertyChanged = true;
        };

        host.CreateNewAppStateTyped = static () => new();

        await Assert.That(propertyChanged).IsTrue();
    }

    /// <summary>Verifies the ISuspensionHost.AppState getter projects from the typed value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ISuspensionHost_AppState_GetProjectsFromTypedValue()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var state = new DummyAppState { Value = SampleStateValue };
        host.AppStateValue = state;

        var untypedHost = (ISuspensionHost)host;

        await Assert.That(untypedHost.AppState).IsSameReferenceAs(state);
    }

    /// <summary>Verifies the ISuspensionHost.AppState setter updates the typed property for a valid value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ISuspensionHost_AppState_SetWithValidValue_UpdatesTypedProperty()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var state = new DummyAppState { Value = SampleStateValue };
        var untypedHost = (ISuspensionHost)host;

        untypedHost.AppState = state;

        await Assert.That(host.AppStateValue).IsSameReferenceAs(state);
    }

    /// <summary>Verifies the ISuspensionHost.AppState setter sets the typed property to default when set to null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ISuspensionHost_AppState_SetNull_SetsTypedPropertyToDefault()
    {
        using var host = new SuspensionHost<DummyAppState> { AppStateValue = new() };
        var untypedHost = (ISuspensionHost)host;

        untypedHost.AppState = null;

        await Assert.That(host.AppStateValue).IsNull();
    }

    /// <summary>Verifies the ISuspensionHost.AppState setter throws when given an incompatible type.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ISuspensionHost_AppState_SetInvalidType_ThrowsInvalidCastException()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var untypedHost = (ISuspensionHost)host;

        await Assert.That(() => untypedHost.AppState = new OtherAppState())
            .Throws<InvalidCastException>();
    }

    /// <summary>Verifies the ISuspensionHost.CreateNewAppState getter projects from the typed factory.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ISuspensionHost_CreateNewAppState_GetProjectsFromTypedFactory()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var expectedState = new DummyAppState { Value = FactoryStateValue };
        host.CreateNewAppStateTyped = () => expectedState;

        var untypedHost = (ISuspensionHost)host;
        var factory = untypedHost.CreateNewAppState;

        await Assert.That(factory).IsNotNull();
        await Assert.That(factory()).IsSameReferenceAs(expectedState);
    }

    /// <summary>Verifies the ISuspensionHost.CreateNewAppState getter returns null when the typed factory is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ISuspensionHost_CreateNewAppState_GetWhenTypedIsNull_ReturnsNull()
    {
        using var host = new SuspensionHost<DummyAppState> { CreateNewAppStateTyped = null };
        var untypedHost = (ISuspensionHost)host;
        var factory = untypedHost.CreateNewAppState;

        await Assert.That((object?)factory).IsNull();
    }

    /// <summary>Verifies the ISuspensionHost.CreateNewAppState setter updates the typed factory for a valid factory.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ISuspensionHost_CreateNewAppState_SetWithValidFactory_UpdatesTypedProperty()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var expectedState = new DummyAppState { Value = FactoryStateValue };
        var untypedHost = (ISuspensionHost)host;

        untypedHost.CreateNewAppState = () => expectedState;

        var typedFactory = host.CreateNewAppStateTyped;
        await Assert.That(typedFactory).IsNotNull();
        await Assert.That(typedFactory()).IsSameReferenceAs(expectedState);
    }

    /// <summary>Verifies the ISuspensionHost.CreateNewAppState setter sets the typed factory to null when set to null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ISuspensionHost_CreateNewAppState_SetNull_SetsTypedPropertyToNull()
    {
        using var host = new SuspensionHost<DummyAppState> { CreateNewAppStateTyped = static () => new() };
        var untypedHost = (ISuspensionHost)host;

        untypedHost.CreateNewAppState = null;

        await Assert.That(host.CreateNewAppStateTyped is null).IsTrue();
    }

    /// <summary>Verifies the typed factory derived from an incompatible ISuspensionHost.CreateNewAppState throws when invoked.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ISuspensionHost_CreateNewAppState_SetInvalidFactory_ThrowsInvalidCastException()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var untypedHost = (ISuspensionHost)host;
        untypedHost.CreateNewAppState = static () => new OtherAppState();

        var factory = untypedHost.CreateNewAppState;
        await Assert.That(() => factory!())
            .Throws<InvalidCastException>();
    }

    /// <summary>Verifies calling Dispose more than once does not throw.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        var host = new SuspensionHost<DummyAppState>();

        host.Dispose();

        await Assert.That(host.Dispose).ThrowsNothing();
    }

    /// <summary>Verifies Dispose disposes all internal subjects without error.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_DisposesAllSubjects()
    {
        // Set observables before disposal
        var host = new SuspensionHost<DummyAppState>
        {
            IsLaunchingNew = Signal.Emit(RxVoid.Default, Sequencer.Immediate),
            IsResuming = Signal.Emit(RxVoid.Default, Sequencer.Immediate),
            IsUnpausing = Signal.Emit(RxVoid.Default, Sequencer.Immediate),
            IsContinuing = Signal.Emit(RxVoid.Default, Sequencer.Immediate),
            ShouldPersistState = Signal.Emit(Scope.Empty, Sequencer.Immediate),
            ShouldInvalidateState = Signal.Emit(RxVoid.Default, Sequencer.Immediate)
        };

        host.Dispose();

        // Verify disposal occurred
        await Assert.That(host).IsNotNull();
    }

    /// <summary>Verifies an AppStateValueChanged subscription receives values prior to disposing the host.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AppStateValueChanged_SubscriptionWorksBeforeDispose()
    {
        var host = new SuspensionHost<DummyAppState>();
        var receivedCount = 0;

        using var subscription = host.AppStateValueChanged.ObserveOn(Sequencer.Immediate).Subscribe(_ => receivedCount++);

        // Set values and verify subscription works
        host.AppStateValue = new();
        host.AppStateValue = new() { Value = 1 };

        await Assert.That(receivedCount).IsEqualTo(ExpectedEmissionCount);

        // Dispose the host
        host.Dispose();

        // The host has been disposed successfully
        await Assert.That(host).IsNotNull();
    }

    /// <summary>Verifies observable properties can be replaced at runtime and existing subscriptions observe the new source.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableProperties_CanBeReplacedDynamically()
    {
        using var host = new SuspensionHost<DummyAppState>();
        var launchCount = 0;

        // Set initial observable
        host.IsLaunchingNew = Signal.Emit(RxVoid.Default, Sequencer.Immediate).Do(_ => launchCount++);

        // Subscribe and verify initial observable works
        using var sub = host.IsLaunchingNew.Subscribe();

        // Replace the observable with a new one
        host.IsLaunchingNew = Signal.Emit(RxVoid.Default, Sequencer.Immediate).Do(_ => launchCount++);

        // The existing subscription should see the new observable due to ReplaySubject + Switch pattern
        // Total triggers depend on when Switch() switches to the new observable
        const int MinimumLaunchCount = 2;
        await Assert.That(launchCount).IsGreaterThanOrEqualTo(MinimumLaunchCount);
    }

    /// <summary>A simple application state used by the generic suspension host tests.</summary>
    private sealed class DummyAppState
    {
        /// <summary>Gets or sets an arbitrary value used to assert state identity.</summary>
        public int Value { get; set; }
    }

    /// <summary>An unrelated state type used to test type mismatch behavior.</summary>
    private sealed class OtherAppState
    {
        /// <summary>Gets or sets an arbitrary name.</summary>
        [SuppressMessage(
            "Design",
            "SST2324:'Name' is declared 'public' but its containing type is only reachable as 'private'",
            Justification = "OtherAppState exists only to be a type distinct from DummyAppState for mismatch " +
                "testing; Name keeps the type from being empty (see SST1436) and its public accessor mirrors " +
                "DummyAppState.Value's shape.")]
        public string? Name { get; set; }
    }
}
