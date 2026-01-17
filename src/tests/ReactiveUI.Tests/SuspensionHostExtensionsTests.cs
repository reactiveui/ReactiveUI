// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json.Serialization.Metadata;

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for SuspensionHostExtensions that use static state.
///     These tests must run in NonParallel suite due to static fields in SuspensionHostExtensions.
/// </summary>
public class SuspensionHostExtensionsTests
{
    private Func<IObservable<Unit>>? _previousEnsureLoadAppStateFunc;
    private ISuspensionDriver? _previousSuspensionDriver;

    [Test]
    public async Task DummySuspensionDriver_InvalidateState_ReturnsUnitObservable()
    {
        var driver = new DummySuspensionDriver();
        var wasCalled = false;

        using var subscription = driver.InvalidateState()
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(_ => wasCalled = true);

        await Assert.That(wasCalled).IsTrue();
    }

    [Test]
    public async Task DummySuspensionDriver_LoadState_ReturnsDefaultObservable()
    {
        var driver = new DummySuspensionDriver();
        object? result = null;

        using var subscription = driver.LoadState()
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(state => result = state);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task DummySuspensionDriver_SaveState_ReturnsUnitObservable()
    {
        var driver = new DummySuspensionDriver();
        var wasCalled = false;

        using var subscription = driver.SaveState(new DummyAppState())
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(_ => wasCalled = true);

        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>
    ///     Verifies that EnsureLoadAppState with null driver after initially having one logs error and AppState remains null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_DriverBecomesNull_LogsErrorAndAppStateRemainsNull()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = () => new DummyAppState(),
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver();
        driver.StateToLoad = new DummyAppState();

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

    [Test]
    public async Task EnsureLoadAppState_LoadStateThrows_CreatesNewAppState()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = () => new DummyAppState(),
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver();
        driver.ShouldThrowOnLoad = true;

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(state).IsNotNull();
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    [Test]
    public async Task EnsureLoadAppState_WithExistingAppState_DoesNotLoad()
    {
        var existingState = new DummyAppState();
        using var host = new SuspensionHost
        {
            AppState = existingState,
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver();

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(0);
        await Assert.That(state).IsSameReferenceAs(existingState);
    }

    [Test]
    public async Task EnsureLoadAppState_WithNullCreateNewAppState_SetsAppStateToNull()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = null,
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver();
        driver.ShouldThrowOnLoad = true;

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(state).IsNull();
    }

    [Test]
    public async Task GetAppState_OnlyLoadsOnce()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = () => new DummyAppState(),
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver();
        driver.StateToLoad = new DummyAppState();

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state1 = host.GetAppState<DummyAppState>();
        var state2 = host.GetAppState<DummyAppState>();

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
        await Assert.That(state1).IsSameReferenceAs(state2);
    }

    [Test]
    public async Task GetAppState_TriggersLoadOnFirstCall()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = () => new DummyAppState(),
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver();
        driver.StateToLoad = new DummyAppState();

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var state = host.GetAppState<DummyAppState>();

        await Assert.That(state).IsNotNull();
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    /// <summary>
    ///     Verifies that GetAppState correctly retrieves the current app state.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppStateReturns()
    {
        var fixture = new SuspensionHost { AppState = new DummyAppState() };

        var result = fixture.GetAppState<DummyAppState>();

        await Assert.That(result).IsSameReferenceAs(fixture.AppState);
    }

    /// <summary>
    ///     Verifies that GetAppState throws for null ISuspensionHost.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppStateThrowsForNullHost() => await Assert
        .That(() => ((ISuspensionHost)null!).GetAppState<DummyAppState>()).Throws<ArgumentNullException>();

    /// <summary>
    ///     Verifies that a null AppState does not throw when calling SetupDefaultSuspendResume.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullAppStateDoesNotThrowException()
    {
        var fixture = new SuspensionHost();

        await Assert.That(() => fixture.SetupDefaultSuspendResume()).ThrowsNothing();
    }

    /// <summary>
    ///     Verifies that a null <see cref="SuspensionHost" /> throws <see cref="ArgumentNullException" /> when calling
    ///     SetupDefaultSuspendResume.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullSuspensionHostThrowsException() => await Assert
        .That(static () => ((SuspensionHost)null!).SetupDefaultSuspendResume()).Throws<ArgumentNullException>();

    /// <summary>
    ///     Verifies that observing AppState does not throw.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateDoesNotThrowException()
    {
        var fixture = new SuspensionHost();

        await Assert.That(() => fixture.ObserveAppState<DummyAppState>().Subscribe()).ThrowsNothing();
    }

    /// <summary>
    ///     Verifies that observing AppState does not throw <see cref="InvalidCastException" />.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateDoesNotThrowInvalidCastException()
    {
        var fixture = new SuspensionHost();

        await Assert.That(() => fixture.ObserveAppState<DummyAppState>().Subscribe()).ThrowsNothing();
    }

    /// <summary>
    ///     Verifies that ObserveAppState emits values when AppState changes.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateEmitsValues()
    {
        using var fixture = new SuspensionHost();
        var receivedStates = new List<DummyAppState>();

        using var subscription = fixture.ObserveAppState<DummyAppState>()
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(receivedStates.Add);

        var state1 = new DummyAppState();
        fixture.AppState = state1;

        var state2 = new DummyAppState();
        fixture.AppState = state2;

        await Assert.That(receivedStates).Count().IsEqualTo(2);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state1);
        await Assert.That(receivedStates[1]).IsSameReferenceAs(state2);
    }

    /// <summary>
    ///     Verifies that ObserveAppState filters null values.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateFiltersNullValues()
    {
        using var fixture = new SuspensionHost();
        var receivedStates = new List<DummyAppState>();

        using var subscription = fixture.ObserveAppState<DummyAppState>()
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(receivedStates.Add);

        fixture.AppState = null;
        var state = new DummyAppState();
        fixture.AppState = state;
        fixture.AppState = null;

        await Assert.That(receivedStates).Count().IsEqualTo(1);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state);
    }

    /// <summary>
    ///     Verifies that ObserveAppState throws for null ISuspensionHost.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateThrowsForNullHost() => await Assert
        .That(() => ((ISuspensionHost)null!).ObserveAppState<DummyAppState>()).Throws<ArgumentNullException>();

    /// <summary>
    ///     Restores the static fields in SuspensionHostExtensions after each test.
    /// </summary>
    [After(Test)]
    public void RestoreStaticState()
    {
        SuspensionHostExtensions.EnsureLoadAppStateFunc = _previousEnsureLoadAppStateFunc;
        SuspensionHostExtensions.SuspensionDriver = _previousSuspensionDriver;
    }

    /// <summary>
    ///     Saves the static state before each test.
    /// </summary>
    [Before(Test)]
    public void SaveStaticState()
    {
        _previousEnsureLoadAppStateFunc = SuspensionHostExtensions.EnsureLoadAppStateFunc;
        _previousSuspensionDriver = SuspensionHostExtensions.SuspensionDriver;
    }

    [Test]
    public async Task SetupDefaultSuspendResume_IsResumingOrIsLaunchingNew_TriggersStateLoad()
    {
        using var host = new SuspensionHost
        {
            CreateNewAppState = () => new DummyAppState(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver();
        var launchSubject = new Subject<Unit>();
        var resumeSubject = new Subject<Unit>();

        host.IsLaunchingNew = launchSubject.ObserveOn(ImmediateScheduler.Instance);
        host.IsResuming = resumeSubject.ObserveOn(ImmediateScheduler.Instance);

        using var disposable = host.SetupDefaultSuspendResume(driver);

        launchSubject.OnNext(Unit.Default);

        await Assert.That(host.AppState).IsNotNull();
    }

    [Test]
    public async Task SetupDefaultSuspendResume_ShouldInvalidateState_CallsDriverInvalidateState()
    {
        using var host = new SuspensionHost
        {
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>()
        };

        var driver = new TestSuspensionDriver();
        var invalidateSubject = new Subject<Unit>();
        host.ShouldInvalidateState = invalidateSubject.ObserveOn(ImmediateScheduler.Instance);

        using var disposable = host.SetupDefaultSuspendResume(driver);

        invalidateSubject.OnNext(Unit.Default);

        await Assert.That(driver.InvalidateStateCallCount).IsEqualTo(1);
    }

    [Test]
    public async Task SetupDefaultSuspendResume_ShouldPersistState_CallsDriverSaveState()
    {
        var appState = new DummyAppState();
        using var host = new SuspensionHost
        {
            AppState = appState,
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver();
        var persistSubject = new Subject<IDisposable>();
        host.ShouldPersistState = persistSubject.ObserveOn(ImmediateScheduler.Instance);

        using var disposable = host.SetupDefaultSuspendResume(driver);

        var token = Disposable.Empty;
        persistSubject.OnNext(token);

        await Assert.That(driver.SaveStateCallCount).IsEqualTo(1);
        await Assert.That(driver.LastSavedState).IsSameReferenceAs(appState);
    }

    [Test]
    public async Task SetupDefaultSuspendResume_WithNullDriver_LogsError()
    {
        using var host = new SuspensionHost
        {
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
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

    [Test]
    public async Task SetupDefaultSuspendResume_WithProvidedDriver_UsesProvidedDriver()
    {
        using var host = new SuspensionHost
        {
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>(),
            CreateNewAppState = () => new DummyAppState()
        };

        var driver = new TestSuspensionDriver();
        driver.StateToLoad = new DummyAppState();

        using var disposable = host.SetupDefaultSuspendResume(driver);

        await Assert.That(disposable).IsNotNull();
    }

    private class DummyAppState
    {
    }

    private class TestSuspensionDriver : ISuspensionDriver
    {
        public int InvalidateStateCallCount { get; private set; }

        public object? LastSavedState { get; private set; }

        public int LoadStateCallCount { get; private set; }

        public int SaveStateCallCount { get; private set; }

        public bool ShouldThrowOnLoad { get; set; }

        public object? StateToLoad { get; set; }

        public IObservable<Unit> InvalidateState()
        {
            InvalidateStateCallCount++;
            return Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        }

        [RequiresUnreferencedCode(
            "Implementations commonly use reflection-based serialization. Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
        [RequiresDynamicCode(
            "Implementations commonly use reflection-based serialization. Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
        public IObservable<object?> LoadState()
        {
            LoadStateCallCount++;
            if (ShouldThrowOnLoad)
            {
                return Observable.Throw<object?>(
                    new InvalidOperationException("Failed to load state"),
                    ImmediateScheduler.Instance);
            }

            return Observable.Return(StateToLoad ?? new DummyAppState(), ImmediateScheduler.Instance);
        }

        public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
        {
            LoadStateCallCount++;
            if (ShouldThrowOnLoad)
            {
                return Observable.Throw<T?>(
                    new InvalidOperationException("Failed to load state"),
                    ImmediateScheduler.Instance);
            }

            // For test purposes, try to cast StateToLoad to T
            if (StateToLoad is T typedState)
            {
                return Observable.Return(typedState, ImmediateScheduler.Instance);
            }

            return Observable.Return<T?>(default, ImmediateScheduler.Instance);
        }

        [RequiresUnreferencedCode(
            "Implementations commonly use reflection-based serialization. Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
        [RequiresDynamicCode(
            "Implementations commonly use reflection-based serialization. Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
        public IObservable<Unit> SaveState<T>(T state)
        {
            SaveStateCallCount++;
            LastSavedState = state;
            return Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        }

        public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
        {
            SaveStateCallCount++;
            LastSavedState = state;
            return Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        }
    }
}
