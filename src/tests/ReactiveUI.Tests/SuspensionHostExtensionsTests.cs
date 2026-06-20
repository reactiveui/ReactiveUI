// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Tests.Utilities.SuspensionHost;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

#if REACTIVE_SHIM
using ISuspensionDriverContract = ReactiveUI.Reactive.ISuspensionDriver;
#else
using ISuspensionDriverContract = ReactiveUI.ISuspensionDriver;
#endif

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
        var driver = new DummySuspensionDriver();
        object? result = null;

        using var subscription = driver.LoadState().ObserveOn(Sequencer.Immediate).Subscribe(state => result = state);

        await Assert.That(result).IsNull();
    }

    /// <summary>Verifies that DummySuspensionDriver.SaveState returns a unit observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_SaveState_ReturnsUnitObservable()
    {
        var driver = new DummySuspensionDriver();
        var wasCalled = false;

        using var subscription = driver.SaveState(new DummyAppState()).ObserveOn(Sequencer.Immediate).Subscribe(_ => wasCalled = true);

        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>Verifies that EnsureLoadAppState with null driver after initially having one logs error and AppState remains null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_DriverBecomesNull_LogsErrorAndAppStateRemainsNull()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = () => new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver { StateToLoad = new DummyAppState() };

        // Set up with a driver
        using var disposable = host.SetupDefaultSuspendResume(driver);

        // Clear both the static driver AND service locator to force the null branch in EnsureLoadAppState
        var previousDrivers = Splat.Locator.Current.GetServices<ISuspensionDriver>().ToList();
        Splat.Locator.CurrentMutable.UnregisterAll<ISuspensionDriver>();
        try
        {
            SuspensionHostExtensions.SuspensionDriver = null;

            // Now call GetAppState which should trigger EnsureLoadAppState
            // It should hit the null driver branch and log error, leaving AppState null
            var state = host.GetAppState<DummyAppState>();

            // State should be null since driver became null and couldn't load
            await Assert.That(state).IsNull();
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
            CreateNewAppState = () => new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
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
            ShouldInvalidateState = Signal.Silent<RxVoid>()
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
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver { ShouldThrowOnLoad = true };

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(state).IsNull();
    }

    /// <summary>Verifies that GetAppState only triggers a state load once.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppState_OnlyLoadsOnce()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = () => new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
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
            CreateNewAppState = () => new DummyAppState(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
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
        .That(() => ((ISuspensionHost)null!).GetAppState<DummyAppState>()).Throws<ArgumentNullException>();

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

    /// <summary>Verifies that observing AppState does not throw <see cref="InvalidCastException" />.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task ObserveAppStateDoesNotThrowInvalidCastException()
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
        .That(() => ((ISuspensionHost)null!).ObserveAppState<DummyAppState>()).Throws<ArgumentNullException>();

    /// <summary>Verifies that SetupDefaultSuspendResume triggers a state load when launching new or resuming.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_IsResumingOrIsLaunchingNew_TriggersStateLoad()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = () => new DummyAppState(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
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
        using var host = new SuspensionHost
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>()
        };

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
        using var host = new SuspensionHost
        {
            AppState = appState,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver();
        var persistSubject = new Signal<IDisposable>();
        host.ShouldPersistState = persistSubject.ObserveOn(Sequencer.Immediate);

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var token = Scope.Empty;
        persistSubject.OnNext(token);

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
            ShouldInvalidateState = Signal.Silent<RxVoid>()
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
            CreateNewAppState = () => new DummyAppState()
        };

        var driver = new TestSuspensionDriver { StateToLoad = new DummyAppState() };

        using var disposable = host.SetupDefaultSuspendResume(driver);

        await Assert.That(disposable).IsNotNull();
    }

    /// <summary>A dummy application state used for testing.</summary>
    [SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class DummyAppState;

    /// <summary>A test suspension driver that records calls and returns configurable results.</summary>
    private sealed class TestSuspensionDriver : ISuspensionDriverContract
    {
        /// <summary>Gets the number of times InvalidateState was called.</summary>
        public int InvalidateStateCallCount { get; private set; }

        /// <summary>Gets the last state passed to SaveState.</summary>
        public object? LastSavedState { get; private set; }

        /// <summary>Gets the number of times LoadState was called.</summary>
        public int LoadStateCallCount { get; private set; }

        /// <summary>Gets the number of times SaveState was called.</summary>
        public int SaveStateCallCount { get; private set; }

        /// <summary>Gets or sets a value indicating whether LoadState should throw.</summary>
        public bool ShouldThrowOnLoad { get; set; }

        /// <summary>Gets or sets the state to return from LoadState.</summary>
        public object? StateToLoad { get; set; }

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
            if (ShouldThrowOnLoad)
            {
                return Signal.Fail<object?>(
                    new InvalidOperationException("Failed to load state"),
                    Sequencer.Immediate);
            }

            return Signal.Emit(StateToLoad ?? new DummyAppState(), Sequencer.Immediate);
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
            if (StateToLoad is T typedState)
            {
                return Signal.Emit(typedState, Sequencer.Immediate);
            }

            return Signal.Emit<T?>(default, Sequencer.Immediate);
        }

        /// <inheritdoc/>
        [RequiresUnreferencedCode(
            "Implementations commonly use reflection-based serialization. Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
        [RequiresDynamicCode(
            "Implementations commonly use reflection-based serialization. Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
        public IObservable<RxVoid> SaveState<T>(T state)
        {
            SaveStateCallCount++;
            LastSavedState = state;
            return Signal.Emit(RxVoid.Default, Sequencer.Immediate);
        }

        /// <inheritdoc/>
        public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
        {
            SaveStateCallCount++;
            LastSavedState = state;
            return Signal.Emit(RxVoid.Default, Sequencer.Immediate);
        }
    }
}
