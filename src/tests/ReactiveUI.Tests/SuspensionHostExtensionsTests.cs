// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Tests.Utilities.SuspensionHost;
using TUnit.Core.Executors;

#if REACTIVE_SHIM
using ISuspensionDriverContract = ReactiveUI.Reactive.IUnsafeSuspensionDriver;
#else
using ISuspensionDriverContract = ReactiveUI.IUnsafeSuspensionDriver;
#endif

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for SuspensionHostExtensions that use static state.
///     These tests must run in NonParallel suite due to static fields in SuspensionHostExtensions.
/// </summary>
[NotInParallel]
[TestExecutor<SuspensionHostTestExecutor>]
public class SuspensionHostExtensionsTests
{
    /// <summary>Verifies that DummySuspensionDriver.InvalidateState returns a unit observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_InvalidateState_ReturnsUnitObservable()
    {
        var driver = new DummySuspensionDriver();
        var wasCalled = false;

        using var subscription = driver.InvalidateState().ObserveOn(Sequencer.Immediate).Subscribe(_ => wasCalled = true);

        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>Verifies that DummySuspensionDriver.LoadState returns a default observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_LoadState_ReturnsDefaultObservable()
    {
        var driver = new DummySuspensionDriverUnsafe();
        object? result = null;

        using var subscription = driver.LoadState().ObserveOn(Sequencer.Immediate).Subscribe(state => result = state);

        await Assert.That(result).IsNull();
    }

    /// <summary>Verifies that DummySuspensionDriver.SaveState returns a unit observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_SaveState_ReturnsUnitObservable()
    {
        var driver = new DummySuspensionDriverUnsafe();
        var wasCalled = false;

        using var subscription = driver.SaveState(new DummyAppState()).ObserveOn(Sequencer.Immediate).Subscribe(_ => wasCalled = true);

        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>Verifies that EnsureLoadAppState still loads through the setup driver after the locator loses its drivers.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_LocatorDriversRemovedAfterSetup_LoadsThroughSetupDriver()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = static () => new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new TestSuspensionDriver { StateToLoad = new DummyAppState() };

        // Set up with a driver
        using var disposable = host.SetupDefaultSuspendResume(driver);

        var previousDrivers = Splat.Locator.Current.GetServices<ISuspensionDriver>().ToList();
        Splat.Locator.CurrentMutable.UnregisterAll<ISuspensionDriver>();
        try
        {
            var state = host.GetAppState<DummyAppState>();

            await Assert.That(state).IsSameReferenceAs(driver.StateToLoad);
            await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
        }
        finally
        {
            foreach (var previousDriver in previousDrivers)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousDriver);
            }
        }
    }

    /// <summary>Verifies that EnsureLoadAppState creates a new app state when LoadState throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_LoadStateThrows_CreatesNewAppState()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = static () => new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new TestSuspensionDriver { ShouldThrowOnLoad = true };

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(state).IsNotNull();
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    /// <summary>Verifies that EnsureLoadAppState does not load when an app state already exists.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_WithExistingAppState_DoesNotLoad()
    {
        var existingState = new DummyAppState();
        using var host = new SuspensionHost
        {
            AppState = existingState,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new TestSuspensionDriver();

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(0);
        await Assert.That(state).IsSameReferenceAs(existingState);
    }

    /// <summary>Verifies that EnsureLoadAppState sets the app state to null when CreateNewAppState is null and load fails.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_WithNullCreateNewAppState_SetsAppStateToNull()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = null,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new TestSuspensionDriver { ShouldThrowOnLoad = true };

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(state).IsNull();
    }

    /// <summary>Verifies that EnsureLoadAppState creates a new app state when the driver loads nothing.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_DriverLoadsNothing_CreatesNewAppState()
    {
        var created = new DummyAppState();
        using var host = new SuspensionHost
        {
            CreateNewAppState = () => created,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new TestSuspensionDriver { LoadsNothing = true };

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(state).IsSameReferenceAs(created);
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    /// <summary>Verifies that EnsureLoadAppState leaves the app state null when the driver loads nothing and no factory is set.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_DriverLoadsNothingAndNoFactory_LeavesAppStateNull()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = null,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new TestSuspensionDriver { LoadsNothing = true };

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(state).IsNull();
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    /// <summary>Verifies that GetAppState only triggers a state load once.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppState_OnlyLoadsOnce()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = static () => new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new TestSuspensionDriver { StateToLoad = new DummyAppState() };

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state1 = host.GetAppState<DummyAppState>();
        var state2 = host.GetAppState<DummyAppState>();

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
        await Assert.That(state1).IsSameReferenceAs(state2);
    }

    /// <summary>Verifies that GetAppState triggers a state load on the first call.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppState_TriggersLoadOnFirstCall()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = static () => new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new TestSuspensionDriver { StateToLoad = new DummyAppState() };

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(state).IsNotNull();
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    /// <summary>Verifies that GetAppState correctly retrieves the current app state.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppStateReturns()
    {
        var fixture = new SuspensionHost { AppState = new DummyAppState() };

        var result = fixture.GetAppState<DummyAppState>();

        await Assert.That(result).IsSameReferenceAs(fixture.AppState);
    }

    /// <summary>Verifies that GetAppState throws for null ISuspensionHost.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppStateThrowsForNullHost() => await Assert
        .That(static () => ((ISuspensionHost)null!).GetAppState<DummyAppState>()).Throws<ArgumentNullException>();

    /// <summary>Verifies that a null AppState does not throw when calling SetupDefaultSuspendResume.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullAppStateDoesNotThrowException()
    {
        var fixture = new SuspensionHost();

        await Assert.That(() => fixture.SetupDefaultSuspendResume()).ThrowsNothing();
    }

    /// <summary>Verifies that a null <see cref="SuspensionHost" /> throws <see cref="ArgumentNullException" /> when calling SetupDefaultSuspendResume.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullSuspensionHostThrowsException() => await Assert
        .That(static () => ((SuspensionHost)null!).SetupDefaultSuspendResume()).Throws<ArgumentNullException>();

    /// <summary>Verifies that observing AppState does not throw.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateDoesNotThrowException()
    {
        var fixture = new SuspensionHost();

        await Assert.That(() => fixture.ObserveAppState<DummyAppState>().Subscribe()).ThrowsNothing();
    }

    /// <summary>Verifies that ObserveAppState emits values when AppState changes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateEmitsValues()
    {
        using var fixture = new SuspensionHost();
        var receivedStates = new List<DummyAppState>();

        using var subscription = fixture.ObserveAppState<DummyAppState>().ObserveOn(Sequencer.Immediate).Subscribe(receivedStates.Add);

        var state1 = new DummyAppState();
        fixture.AppState = state1;

        var state2 = new DummyAppState();
        fixture.AppState = state2;

        const int ExpectedCount = 2;
        await Assert.That(receivedStates).Count().IsEqualTo(ExpectedCount);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state1);
        await Assert.That(receivedStates[1]).IsSameReferenceAs(state2);
    }

    /// <summary>Verifies that ObserveAppState filters null values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateFiltersNullValues()
    {
        using var fixture = new SuspensionHost();
        var receivedStates = new List<DummyAppState>();

        using var subscription = fixture.ObserveAppState<DummyAppState>().ObserveOn(Sequencer.Immediate).Subscribe(receivedStates.Add);

        fixture.AppState = null;
        var state = new DummyAppState();
        fixture.AppState = state;
        fixture.AppState = null;

        await Assert.That(receivedStates).Count().IsEqualTo(1);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state);
    }

    /// <summary>Verifies that ObserveAppState throws for null ISuspensionHost.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateThrowsForNullHost() => await Assert
        .That(static () => ((ISuspensionHost)null!).ObserveAppState<DummyAppState>()).Throws<ArgumentNullException>();

    /// <summary>Verifies that SetupDefaultSuspendResume triggers a state load when launching new or resuming.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_IsResumingOrIsLaunchingNew_TriggersStateLoad()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = static () => new DummyAppState(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new TestSuspensionDriver();
        var launchSubject = new Signal<RxVoid>();
        var resumeSubject = new Signal<RxVoid>();

        host.IsLaunchingNew = launchSubject.ObserveOn(Sequencer.Immediate);
        host.IsResuming = resumeSubject.ObserveOn(Sequencer.Immediate);

        using var disposable = host.SetupDefaultSuspendResume(driver);

        launchSubject.OnNext(RxVoid.Default);

        await Assert.That(host.AppState).IsNotNull();
    }

    /// <summary>Verifies that SetupDefaultSuspendResume calls the driver's InvalidateState when state should be invalidated.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_ShouldInvalidateState_CallsDriverInvalidateState()
    {
        using var host = new SuspensionHost { IsLaunchingNew = Signal.Silent<RxVoid>(), IsResuming = Signal.Silent<RxVoid>(), ShouldPersistState = Signal.Silent<IDisposable>() };

        var driver = new TestSuspensionDriver();
        var invalidateSubject = new Signal<RxVoid>();
        host.ShouldInvalidateState = invalidateSubject.ObserveOn(Sequencer.Immediate);

        using var disposable = host.SetupDefaultSuspendResume(driver);

        invalidateSubject.OnNext(RxVoid.Default);

        await Assert.That(driver.InvalidateStateCallCount).IsEqualTo(1);
    }

    /// <summary>Verifies that SetupDefaultSuspendResume calls the driver's SaveState when state should be persisted.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_ShouldPersistState_CallsDriverSaveState()
    {
        var appState = new DummyAppState();
        using var host = new SuspensionHost { AppState = appState, IsLaunchingNew = Signal.Silent<RxVoid>(), IsResuming = Signal.Silent<RxVoid>(), ShouldInvalidateState = Signal.Silent<RxVoid>() };

        var driver = new TestSuspensionDriver();
        var persistSubject = new Signal<IDisposable>();
        host.ShouldPersistState = persistSubject.ObserveOn(Sequencer.Immediate);

        using var disposable = host.SetupDefaultSuspendResume(driver);

        persistSubject.OnNext(Scope.Empty);

        await Assert.That(driver.SaveStateCallCount).IsEqualTo(1);
        await Assert.That(driver.LastSavedState).IsSameReferenceAs(appState);
    }

    /// <summary>Verifies that SetupDefaultSuspendResume logs an error when no driver is available.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_WithNullDriver_LogsError()
    {
        using var host = new SuspensionHost
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var previousDrivers = Splat.Locator.Current.GetServices<ISuspensionDriver>().ToList();
        Splat.Locator.CurrentMutable.UnregisterAll<ISuspensionDriver>();
        try
        {
            // First call sets up with null driver and logs error
            var disposable = host.SetupDefaultSuspendResume();

            // Verify a disposable is returned (might be empty or composite depending on static state)
            await Assert.That(disposable).IsNotNull();
        }
        finally
        {
            foreach (var driver in previousDrivers)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(driver);
            }
        }
    }

    /// <summary>Verifies that SetupDefaultSuspendResume persists and loads through a locator driver that only implements the safe contract.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_LocatorDriverIsSafeOnly_UsesItThroughReflectionMetadata()
    {
        var appState = new DummyAppState();
        using var host = new SuspensionHost { AppState = appState, IsLaunchingNew = Signal.Silent<RxVoid>(), IsResuming = Signal.Silent<RxVoid>(), ShouldInvalidateState = Signal.Silent<RxVoid>() };
        using var loadingHost = new SuspensionHost
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new SafeOnlySuspensionDriver();
        var persistSubject = new Signal<IDisposable>();
        host.ShouldPersistState = persistSubject.ObserveOn(Sequencer.Immediate);

        var previousDrivers = Splat.Locator.Current.GetServices<ISuspensionDriver>().ToList();
        Splat.Locator.CurrentMutable.UnregisterAll<ISuspensionDriver>();
        Splat.Locator.CurrentMutable.RegisterConstant<ISuspensionDriver>(driver);
        try
        {
            using var disposable = host.SetupDefaultSuspendResume();
            using var loadingDisposable = loadingHost.SetupDefaultSuspendResume();

            persistSubject.OnNext(Scope.Empty);
            _ = loadingHost.GetAppState<object>();

            await Assert.That(driver.LastSavedState).IsSameReferenceAs(appState);
            await Assert.That(driver.LastSavedType).IsEqualTo(typeof(object));
            await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
        }
        finally
        {
            Splat.Locator.CurrentMutable.UnregisterAll<ISuspensionDriver>();
            foreach (var previousDriver in previousDrivers)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousDriver);
            }
        }
    }

    /// <summary>Verifies that SetupDefaultSuspendResume uses a locator driver registered under the safe contract when it also implements the reflection contract.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_LocatorDriverImplementsUnsafeContract_UsesItDirectly()
    {
        using var host = new SuspensionHost
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
        };

        var driver = new TestSuspensionDriver { StateToLoad = new DummyAppState() };

        var previousDrivers = Splat.Locator.Current.GetServices<ISuspensionDriver>().ToList();
        Splat.Locator.CurrentMutable.UnregisterAll<ISuspensionDriver>();
        Splat.Locator.CurrentMutable.RegisterConstant<ISuspensionDriver>(driver);
        try
        {
            using var disposable = host.SetupDefaultSuspendResume();

            var state = host.GetAppState<DummyAppState>();

            await Assert.That(state).IsSameReferenceAs(driver.StateToLoad);
            await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
        }
        finally
        {
            Splat.Locator.CurrentMutable.UnregisterAll<ISuspensionDriver>();
            foreach (var previousDriver in previousDrivers)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousDriver);
            }
        }
    }

    /// <summary>Verifies that the reflection adapter forwards the typed members and invalidation to the driver it wraps.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReflectionSuspensionDriverAdapter_TypedMembersAndInvalidate_ForwardToInnerDriver()
    {
        var inner = new SafeOnlySuspensionDriver();
        var adapter = new ReflectionSuspensionDriverAdapter(inner);
        var typeInfo = JsonTypeInfo.CreateJsonTypeInfo<DummyAppState>(JsonSerializerOptions.Default);
        var state = new DummyAppState();

        using var save = adapter.SaveState(state, typeInfo).ObserveOn(Sequencer.Immediate).Subscribe(static _ => { });
        using var load = adapter.LoadState(typeInfo).ObserveOn(Sequencer.Immediate).Subscribe(static _ => { });
        using var invalidate = adapter.InvalidateState().ObserveOn(Sequencer.Immediate).Subscribe(static _ => { });

        await Assert.That(inner.LastSavedState).IsSameReferenceAs(state);
        await Assert.That(inner.LastSavedType).IsEqualTo(typeof(DummyAppState));
        await Assert.That(inner.LoadStateCallCount).IsEqualTo(1);
        await Assert.That(inner.InvalidateStateCallCount).IsEqualTo(1);
    }

    /// <summary>Verifies that DummySuspensionDriverUnsafe.InvalidateState returns a unit observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriverUnsafe_InvalidateState_ReturnsUnitObservable()
    {
        var driver = new DummySuspensionDriverUnsafe();
        var wasCalled = false;

        using var subscription = driver.InvalidateState().ObserveOn(Sequencer.Immediate).Subscribe(_ => wasCalled = true);

        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>Verifies that a host whose lifecycle signals complete does not disturb the suspend/resume setup.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_HostForwardsSignalCompletion_HandledGracefully()
    {
        var persistSubject = new Signal<IDisposable>();
        var host = new PassThroughSuspensionHost { AppState = new DummyAppState(), ShouldPersistState = persistSubject };
        var driver = new TestSuspensionDriver();

        using var setup = host.SetupDefaultSuspendResume(driver);

        await Assert.That(persistSubject.OnCompleted).ThrowsNothing();
        await Assert.That(driver.SaveStateCallCount).IsEqualTo(0);
    }

    /// <summary>Verifies that SetupDefaultSuspendResume uses an explicitly provided driver.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_WithProvidedDriver_UsesProvidedDriver()
    {
        using var host = new SuspensionHost
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
            CreateNewAppState = static () => new DummyAppState(),
        };

        var driver = new TestSuspensionDriver { StateToLoad = new DummyAppState() };

        using var disposable = host.SetupDefaultSuspendResume(driver);

        await Assert.That(disposable).IsNotNull();
    }

    /// <summary>Verifies that two hosts set up with different drivers each load and save through their own driver.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_TwoHostsWithDifferentDrivers_EachHostUsesItsOwnDriver()
    {
        var firstPersist = new Signal<IDisposable>();
        using var firstHost = new SuspensionHost
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
            ShouldPersistState = firstPersist.ObserveOn(Sequencer.Immediate),
        };
        var secondPersist = new Signal<IDisposable>();
        using var secondHost = new SuspensionHost
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
            ShouldPersistState = secondPersist.ObserveOn(Sequencer.Immediate),
        };

        var firstDriver = new TestSuspensionDriver { StateToLoad = new DummyAppState() };
        var secondDriver = new TestSuspensionDriver { StateToLoad = new DummyAppState() };

        using var firstSetup = firstHost.SetupDefaultSuspendResume(firstDriver);
        using var secondSetup = secondHost.SetupDefaultSuspendResume(secondDriver);

        var firstState = firstHost.GetAppState<DummyAppState>();
        var secondState = secondHost.GetAppState<DummyAppState>();
        firstPersist.OnNext(Scope.Empty);
        secondPersist.OnNext(Scope.Empty);

        await Assert.That(firstState).IsSameReferenceAs(firstDriver.StateToLoad);
        await Assert.That(secondState).IsSameReferenceAs(secondDriver.StateToLoad);
        await Assert.That(firstDriver.LoadStateCallCount).IsEqualTo(1);
        await Assert.That(secondDriver.LoadStateCallCount).IsEqualTo(1);
        await Assert.That(firstDriver.SaveStateCallCount).IsEqualTo(1);
        await Assert.That(secondDriver.SaveStateCallCount).IsEqualTo(1);
        await Assert.That(firstDriver.LastSavedState).IsSameReferenceAs(firstDriver.StateToLoad);
        await Assert.That(secondDriver.LastSavedState).IsSameReferenceAs(secondDriver.StateToLoad);
    }

    /// <summary>Verifies that a synchronous exception from a driver operation logs and still disposes the persist token.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_DriverThrowsSynchronously_DisposesPersistToken()
    {
        var persistSubject = new Signal<IDisposable>();
        using var host = new SuspensionHost
        {
            AppState = new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
            ShouldPersistState = persistSubject,
        };

        var driver = new TestSuspensionDriver { ThrowSynchronouslyOnSave = true };
        using var setup = host.SetupDefaultSuspendResume(driver);

        var token = new CountingDisposable();
        persistSubject.OnNext(token);

        await Assert.That(token.DisposeCount).IsEqualTo(1);
    }

    /// <summary>Verifies that an erroring driver observable logs and still disposes the persist token.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_DriverObservableErrors_DisposesPersistToken()
    {
        var persistSubject = new Signal<IDisposable>();
        using var host = new SuspensionHost
        {
            AppState = new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
            ShouldPersistState = persistSubject,
        };

        var driver = new TestSuspensionDriver { FailAsyncOnSave = true };
        using var setup = host.SetupDefaultSuspendResume(driver);

        var token = new CountingDisposable();
        persistSubject.OnNext(token);

        await Assert.That(token.DisposeCount).IsEqualTo(1);
    }

    /// <summary>Verifies that lifecycle signal errors and completions are handled without throwing.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_LifecycleSignalsErrorAndComplete_HandledGracefully()
    {
        var invalidateSubject = new Signal<RxVoid>();
        var persistSubject = new Signal<IDisposable>();
        using var host = new SuspensionHost
        {
            AppState = new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = invalidateSubject,
            ShouldPersistState = persistSubject,
        };

        var driver = new TestSuspensionDriver();
        using var setup = host.SetupDefaultSuspendResume(driver);

        await Assert.That(() =>
        {
            invalidateSubject.OnError(new InvalidOperationException("signal failure"));
            persistSubject.OnCompleted();
        }).ThrowsNothing();
    }

    /// <summary>Verifies that duplicate completion from a driver observable disposes the persist token only once.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_DriverDoubleCompletes_DisposesPersistTokenOnce()
    {
        var persistSubject = new Signal<IDisposable>();
        using var host = new SuspensionHost
        {
            AppState = new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
            ShouldPersistState = persistSubject,
        };

        var driver = new TestSuspensionDriver { DoubleCompleteOnSave = true };
        using var setup = host.SetupDefaultSuspendResume(driver);

        var token = new CountingDisposable();
        persistSubject.OnNext(token);

        await Assert.That(token.DisposeCount).IsEqualTo(1);
    }

    /// <summary>Verifies that <see cref="UnsafeSuspensionHostExtensions.NonNullCastObservable{TResult}"/> forwards errors and completion.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NonNullCastObservable_ForwardsErrorAndCompleted()
    {
        var errorSignal = new Signal<object?>();
        Exception? observedError = null;
        using (new UnsafeSuspensionHostExtensions.NonNullCastObservable<DummyAppState>(errorSignal).Subscribe(
            static _ => { },
            ex => observedError = ex))
        {
            errorSignal.OnError(new InvalidOperationException("boom"));
        }

        var completeSignal = new Signal<object?>();
        var completed = false;
        using (new UnsafeSuspensionHostExtensions.NonNullCastObservable<DummyAppState>(completeSignal).Subscribe(
            static _ => { },
            () => completed = true))
        {
            completeSignal.OnCompleted();
        }

        await Assert.That(observedError).IsTypeOf<InvalidOperationException>();
        await Assert.That(completed).IsTrue();
    }

    /// <summary>A disposable that counts how many times <see cref="Dispose"/> is invoked.</summary>
    private sealed class CountingDisposable : IDisposable
    {
        /// <summary>Gets the number of times <see cref="Dispose"/> has been called.</summary>
        public int DisposeCount { get; private set; }

        /// <inheritdoc/>
        public void Dispose() => DisposeCount++;
    }

    /// <summary>A dummy application state used for testing.</summary>
    [SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class DummyAppState;

    /// <summary>An observable that invokes <see cref="IObserver{T}.OnCompleted"/> twice to exercise idempotent cleanup.</summary>
    private sealed class DoubleCompleteObservable : IObservable<RxVoid>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<RxVoid> observer)
        {
            observer.OnNext(RxVoid.Default);
            observer.OnCompleted();
            observer.OnCompleted();
            return Scope.Empty;
        }
    }

    /// <summary>A suspension host that hands out its lifecycle signals unwrapped, so their completion reaches subscribers.</summary>
    private sealed class PassThroughSuspensionHost : ReactiveObject, ISuspensionHost
    {
        /// <inheritdoc/>
        public IObservable<RxVoid> IsLaunchingNew { get; set; } = Signal.Silent<RxVoid>();

        /// <inheritdoc/>
        public IObservable<RxVoid> IsResuming { get; set; } = Signal.Silent<RxVoid>();

        /// <inheritdoc/>
        public IObservable<RxVoid> IsUnpausing { get; set; } = Signal.Silent<RxVoid>();

        /// <inheritdoc/>
        public IObservable<IDisposable> ShouldPersistState { get; set; } = Signal.Silent<IDisposable>();

        /// <inheritdoc/>
        public IObservable<RxVoid> ShouldInvalidateState { get; set; } = Signal.Silent<RxVoid>();

        /// <inheritdoc/>
        public Func<object>? CreateNewAppState { get; set; }

        /// <inheritdoc/>
        public object? AppState { get; set; }
    }

    /// <summary>A suspension driver that implements only the source-generated metadata contract.</summary>
    private sealed class SafeOnlySuspensionDriver : ISuspensionDriver
    {
        /// <summary>Gets the last state passed to SaveState.</summary>
        public object? LastSavedState { get; private set; }

        /// <summary>Gets the type described by the metadata last passed to SaveState.</summary>
        public Type? LastSavedType { get; private set; }

        /// <summary>Gets the number of times LoadState was called.</summary>
        public int LoadStateCallCount { get; private set; }

        /// <summary>Gets the number of times InvalidateState was called.</summary>
        public int InvalidateStateCallCount { get; private set; }

        /// <inheritdoc/>
        public IObservable<RxVoid> InvalidateState()
        {
            InvalidateStateCallCount++;
            return Signal.Emit(RxVoid.Default, Sequencer.Immediate);
        }

        /// <inheritdoc/>
        public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
        {
            LoadStateCallCount++;
            return Signal.Emit<T?>(default, Sequencer.Immediate);
        }

        /// <inheritdoc/>
        public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
        {
            LastSavedState = state;
            LastSavedType = typeInfo.Type;
            return Signal.Emit(RxVoid.Default, Sequencer.Immediate);
        }
    }

    /// <summary>A test suspension driver that records calls and returns configurable results.</summary>
    private sealed class TestSuspensionDriver : ISuspensionDriverContract
    {
        /// <summary>Gets or sets a value indicating whether SaveState emits OnCompleted twice.</summary>
        public bool DoubleCompleteOnSave { get; set; }

        /// <summary>Gets or sets a value indicating whether SaveState returns a failing observable.</summary>
        public bool FailAsyncOnSave { get; set; }

        /// <summary>Gets the number of times InvalidateState was called.</summary>
        public int InvalidateStateCallCount { get; private set; }

        /// <summary>Gets the last state passed to SaveState.</summary>
        public object? LastSavedState { get; private set; }

        /// <summary>Gets the number of times LoadState was called.</summary>
        public int LoadStateCallCount { get; private set; }

        /// <summary>Gets or sets a value indicating whether LoadState yields no persisted state.</summary>
        public bool LoadsNothing { get; set; }

        /// <summary>Gets the number of times SaveState was called.</summary>
        public int SaveStateCallCount { get; private set; }

        /// <summary>Gets or sets a value indicating whether LoadState should throw.</summary>
        public bool ShouldThrowOnLoad { get; set; }

        /// <summary>Gets or sets the state to return from LoadState.</summary>
        public object? StateToLoad { get; set; }

        /// <summary>Gets or sets a value indicating whether SaveState throws synchronously.</summary>
        public bool ThrowSynchronouslyOnSave { get; set; }

        /// <inheritdoc/>
        public IObservable<RxVoid> InvalidateState()
        {
            InvalidateStateCallCount++;
            return Signal.Emit(RxVoid.Default, Sequencer.Immediate);
        }

        /// <inheritdoc/>
        [RequiresUnreferencedCode(
            "Implementations commonly use reflection-based serialization. Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
        [RequiresDynamicCode(
            "Implementations commonly use reflection-based serialization. Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
        public IObservable<object?> LoadState()
        {
            LoadStateCallCount++;
            return ShouldThrowOnLoad ? Signal.Fail<object?>(
                    new InvalidOperationException("Failed to load state"),
                    Sequencer.Immediate) : Signal.Emit(LoadsNothing ? null : StateToLoad ?? new DummyAppState(), Sequencer.Immediate);
        }

        /// <inheritdoc/>
        public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
        {
            LoadStateCallCount++;
            if (ShouldThrowOnLoad)
            {
                return Signal.Fail<T?>(
                    new InvalidOperationException("Failed to load state"),
                    Sequencer.Immediate);
            }

            // For test purposes, try to cast StateToLoad to T
            return StateToLoad is T typedState ? Signal.Emit(typedState, Sequencer.Immediate) : Signal.Emit<T?>(default, Sequencer.Immediate);
        }

        /// <inheritdoc/>
        [RequiresUnreferencedCode(
            "Implementations commonly use reflection-based serialization. Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
        [RequiresDynamicCode(
            "Implementations commonly use reflection-based serialization. Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
        public IObservable<RxVoid> SaveState<T>(T state) => SaveStateCore(state);

        /// <inheritdoc/>
        public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
        {
            _ = typeInfo;
            return SaveStateCore(state);
        }

        /// <summary>Records and completes a save request.</summary>
        /// <typeparam name="T">The state type.</typeparam>
        /// <param name="state">The state to save.</param>
        /// <returns>A completion signal.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ThrowSynchronouslyOnSave"/> is set.</exception>
        private IObservable<RxVoid> SaveStateCore<T>(T state)
        {
            SaveStateCallCount++;
            LastSavedState = state;
            if (ThrowSynchronouslyOnSave)
            {
                throw new InvalidOperationException("Failed to save state synchronously");
            }

            if (FailAsyncOnSave)
            {
                return Signal.Fail<RxVoid>(new InvalidOperationException("Failed to save state"), Sequencer.Immediate);
            }

            return DoubleCompleteOnSave
                ? new DoubleCompleteObservable()
                : Signal.Emit(RxVoid.Default, Sequencer.Immediate);
        }
    }
}
