// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Tests.Utilities.SuspensionHost;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Suspension;

#if REACTIVE_SHIM
using ISuspensionDriverContract = ReactiveUI.Reactive.ISuspensionDriver;
#else
using ISuspensionDriverContract = ReactiveUI.ISuspensionDriver;
#endif

/// <summary>Tests for AOT-friendly (JsonTypeInfo-based) overloads in SuspensionHostExtensions.</summary>
[NotInParallel]
[TestExecutor<SuspensionHostTestExecutor>]
public partial class SuspensionHostExtensionsAotTests
{
    /// <summary>The sample state value used to seed the host.</summary>
    private const int SampleStateValue = 42;

    /// <summary>The state value produced by the create-state factory.</summary>
    private const int FactoryStateValue = 99;

    /// <summary>The state value emitted to simulate a loaded state.</summary>
    private const int LoadedStateValue = 123;

    /// <summary>The state value emitted to simulate a created state.</summary>
    private const int CreatedStateValue = 999;

    /// <summary>The second state value used in emission tests.</summary>
    private const int SecondStateValue = 2;

    /// <summary>The expected number of emissions observed in tests.</summary>
    private const int ExpectedEmissionCount = 2;

    /// <summary>Verifies the typed GetAppState returns the host's current state.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppState_Typed_ReturnsCurrentState()
    {
        var state = new TestAppState { Value = SampleStateValue };
        using var host = new SuspensionHost<TestAppState>
        {
            AppStateValue = state,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var result = host.GetAppState();

        await Assert.That(result).IsSameReferenceAs(state);
    }

    /// <summary>Verifies the typed GetAppState throws when the host is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppState_Typed_ThrowsForNullHost()
    {
        SuspensionHost<TestAppState>? host = null;

        await Assert.That(() => host!.GetAppState())
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies the typed GetAppState triggers loading of the app state from the driver.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppState_Typed_TriggersEnsureLoadAppState()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () => new() { Value = FactoryStateValue },
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState> { StateToLoad = new() { Value = LoadedStateValue } };

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state = host.GetAppState();

        await Assert.That(state).IsNotNull();
        await Assert.That(state.Value).IsEqualTo(LoadedStateValue);
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    /// <summary>Verifies the typed ObserveAppState emits the current value immediately on subscription.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppState_Typed_WhenNoPersistedState_CreatesAndStoresNewAppState()
    {
        var createdState = new TestAppState { Value = 99 };
        var createNewAppStateCallCount = 0;
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () =>
            {
                createNewAppStateCallCount++;
                return createdState;
            },
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state = host.GetAppState();

        await Assert.That(state).IsSameReferenceAs(createdState);
        await Assert.That(host.AppStateValue).IsSameReferenceAs(createdState);
        await Assert.That(createNewAppStateCallCount).IsEqualTo(1);
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    /// <summary>Verifies that observing the typed app state emits the current value immediately on subscription.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppState_Typed_EmitsCurrentValueImmediately()
    {
        var state = new TestAppState { Value = SampleStateValue };
        using var host = new SuspensionHost<TestAppState>
        {
            AppStateValue = state,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var receivedStates = new List<TestAppState>();

        using var subscription = host.ObserveAppState().ObserveOn(Sequencer.Immediate).Subscribe(receivedStates.Add);

        await Assert.That(receivedStates).Count().IsEqualTo(1);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state);
    }

    /// <summary>Verifies the typed ObserveAppState emits subsequent app state changes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppState_Typed_EmitsSubsequentChanges()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var receivedStates = new List<TestAppState>();

        using var subscription = host.ObserveAppState().ObserveOn(Sequencer.Immediate).Subscribe(receivedStates.Add);

        var state1 = new TestAppState { Value = 1 };
        host.AppStateValue = state1;

        var state2 = new TestAppState { Value = SecondStateValue };
        host.AppStateValue = state2;

        await Assert.That(receivedStates).Count().IsEqualTo(ExpectedEmissionCount);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state1);
        await Assert.That(receivedStates[1]).IsSameReferenceAs(state2);
    }

    /// <summary>Verifies the typed ObserveAppState filters out null values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppState_Typed_FiltersNullValues()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var receivedStates = new List<TestAppState>();

        using var subscription = host.ObserveAppState().ObserveOn(Sequencer.Immediate).Subscribe(receivedStates.Add);

        host.AppStateValue = null;
        var state = new TestAppState { Value = SampleStateValue };
        host.AppStateValue = state;
        host.AppStateValue = null;

        await Assert.That(receivedStates).Count().IsEqualTo(1);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state);
    }

    /// <summary>Verifies the typed ObserveAppState throws when the host is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppState_Typed_ThrowsForNullHost()
    {
        SuspensionHost<TestAppState>? host = null;

        await Assert.That(() => host!.ObserveAppState())
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies the typed SetupDefaultSuspendResume throws when the host is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ThrowsForNullHost()
    {
        SuspensionHost<TestAppState>? host = null;

        await Assert.That(() => host!.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies the typed SetupDefaultSuspendResume throws when the JsonTypeInfo is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ThrowsForNullTypeInfo()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        await Assert.That(() => host.SetupDefaultSuspendResume(null!))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies the typed SetupDefaultSuspendResume uses the explicitly provided driver.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_WithProvidedDriver_UsesProvidedDriver()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>(),
            CreateNewAppStateTyped = () => new()
        };

        var driver = new TestSuspensionDriver<TestAppState> { StateToLoad = new() { Value = SampleStateValue } };

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        await Assert.That(disposable).IsNotNull();
    }

    /// <summary>Verifies a ShouldPersistState signal causes the typed setup to save state via the driver.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ShouldPersistState_CallsDriverSaveState()
    {
        var appState = new TestAppState { Value = FactoryStateValue };
        using var host = new SuspensionHost<TestAppState>
        {
            AppStateValue = appState,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        var persistSubject = new Signal<IDisposable>();
        host.ShouldPersistState = persistSubject.ObserveOn(Sequencer.Immediate);

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var token = Scope.Empty;
        persistSubject.OnNext(token);

        await Assert.That(driver.SaveStateCallCount).IsEqualTo(1);
        await Assert.That(driver.LastSavedState).IsSameReferenceAs(appState);
    }

    /// <summary>Verifies a ShouldInvalidateState signal causes the typed setup to invalidate state via the driver.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ShouldPersistCreatedState_WhenNoPersistedStateAndPersistOccursBeforeGetAppState()
    {
        var createdState = new TestAppState { Value = 321 };
        var createNewAppStateCallCount = 0;
        var persistTokenDisposed = false;
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () =>
            {
                createNewAppStateCallCount++;
                return createdState;
            },
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        var persistSubject = new Signal<IDisposable>();
        host.ShouldPersistState = persistSubject.ObserveOn(Sequencer.Immediate);

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);
        var persistToken = Scope.Create(() => persistTokenDisposed = true);

        persistSubject.OnNext(persistToken);

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
        await Assert.That(createNewAppStateCallCount).IsEqualTo(1);
        await Assert.That(host.AppStateValue).IsSameReferenceAs(createdState);
        await Assert.That(driver.SaveStateCallCount).IsEqualTo(1);
        await Assert.That(driver.LastSavedState).IsSameReferenceAs(createdState);
        await Assert.That(persistTokenDisposed).IsTrue();
    }

    /// <summary>Verifies that the typed default suspend/resume persists created state when the launch signal was raised before setup.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ShouldPersistCreatedState_WhenLaunchSignalWasRaisedBeforeSetup()
    {
        var createdState = new TestAppState { Value = 654 };
        var createNewAppStateCallCount = 0;
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () =>
            {
                createNewAppStateCallCount++;
                return createdState;
            },
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var launchSubject = new Signal<RxVoid>();
        var resumeSubject = new Signal<RxVoid>();
        var persistSubject = new Signal<IDisposable>();
        host.IsLaunchingNew = launchSubject.ObserveOn(Sequencer.Immediate);
        host.IsResuming = resumeSubject.ObserveOn(Sequencer.Immediate);
        host.ShouldPersistState = persistSubject.ObserveOn(Sequencer.Immediate);

        launchSubject.OnNext(RxVoid.Default);

        var driver = new TestSuspensionDriver<TestAppState>();
        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        persistSubject.OnNext(Scope.Empty);

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
        await Assert.That(createNewAppStateCallCount).IsEqualTo(1);
        await Assert.That(host.AppStateValue).IsSameReferenceAs(createdState);
        await Assert.That(driver.SaveStateCallCount).IsEqualTo(1);
        await Assert.That(driver.LastSavedState).IsSameReferenceAs(createdState);
    }

    /// <summary>
    ///     Verifies that the typed default suspend/resume persists loaded state when persist occurs before the app state is fetched.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ShouldPersistLoadedState_WhenPersistOccursBeforeGetAppState()
    {
        var loadedState = new TestAppState { Value = 987 };
        var createNewAppStateCallCount = 0;
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () =>
            {
                createNewAppStateCallCount++;
                return new();
            },
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState> { StateToLoad = loadedState };
        var persistSubject = new Signal<IDisposable>();
        host.ShouldPersistState = persistSubject.ObserveOn(Sequencer.Immediate);

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        persistSubject.OnNext(Scope.Empty);

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
        await Assert.That(createNewAppStateCallCount).IsEqualTo(0);
        await Assert.That(host.AppStateValue).IsSameReferenceAs(loadedState);
        await Assert.That(driver.SaveStateCallCount).IsEqualTo(1);
        await Assert.That(driver.LastSavedState).IsSameReferenceAs(loadedState);
    }

    /// <summary>Verifies that the typed default suspend/resume disposes the persist token after saving.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ShouldDisposePersistTokenAfterSave()
    {
        var appState = new TestAppState { Value = 111 };
        var persistTokenDisposed = false;
        using var host = new SuspensionHost<TestAppState>
        {
            AppStateValue = appState,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        var persistSubject = new Signal<IDisposable>();
        host.ShouldPersistState = persistSubject.ObserveOn(Sequencer.Immediate);

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);
        var persistToken = Scope.Create(() => persistTokenDisposed = true);

        persistSubject.OnNext(persistToken);

        await Assert.That(driver.SaveStateCallCount).IsEqualTo(1);
        await Assert.That(persistTokenDisposed).IsTrue();
    }

    /// <summary>Verifies that the typed default suspend/resume invalidate-state path calls the driver's invalidate-state method.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ShouldInvalidateState_CallsDriverInvalidateState()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        var invalidateSubject = new Signal<RxVoid>();
        host.ShouldInvalidateState = invalidateSubject.ObserveOn(Sequencer.Immediate);

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        invalidateSubject.OnNext(RxVoid.Default);

        await Assert.That(driver.InvalidateStateCallCount).IsEqualTo(1);
    }

    /// <summary>Verifies an IsLaunchingNew signal triggers loading of the app state in the typed setup.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_IsLaunchingNew_TriggersStateLoad()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () => new(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState> { StateToLoad = new() { Value = LoadedStateValue } };

        var launchSubject = new Signal<RxVoid>();
        var resumeSubject = new Signal<RxVoid>();

        host.IsLaunchingNew = launchSubject.ObserveOn(Sequencer.Immediate);
        host.IsResuming = resumeSubject.ObserveOn(Sequencer.Immediate);

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        launchSubject.OnNext(RxVoid.Default);

        await Assert.That(host.AppStateValue).IsNotNull();
        await Assert.That(host.AppStateValue!.Value).IsEqualTo(LoadedStateValue);
    }

    /// <summary>Verifies the typed setup logs an error and returns an empty disposable when no driver is available.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_WithNullDriver_LogsErrorAndReturnsEmptyDisposable()
    {
        using var host = new SuspensionHost<TestAppState>
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
            var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState);

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

    /// <summary>Verifies the typed EnsureLoadAppState does not load when the host already has state.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_Typed_WithExistingState_DoesNotLoad()
    {
        var existingState = new TestAppState { Value = FactoryStateValue };
        using var host = new SuspensionHost<TestAppState>
        {
            AppStateValue = existingState,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state = host.GetAppState();

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(0);
        await Assert.That(state).IsSameReferenceAs(existingState);
    }

    /// <summary>Verifies the typed EnsureLoadAppState creates new app state when loading throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_Typed_LoadStateThrows_CreatesNewAppState()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () => new() { Value = CreatedStateValue },
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState> { ShouldThrowOnLoad = true };

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state = host.GetAppState();

        await Assert.That(state).IsNotNull();
        await Assert.That(state.Value).IsEqualTo(CreatedStateValue);
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    /// <summary>Verifies the typed EnsureLoadAppState leaves state null when there is no factory and loading throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_Typed_WithNullCreateNewAppState_SetsStateToNull()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = null,
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState> { ShouldThrowOnLoad = true };

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state = host.GetAppState();

        await Assert.That((object?)state).IsNull();
    }

    /// <summary>Verifies the typed EnsureLoadAppState logs an error and leaves state null when the driver becomes null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_Typed_DriverBecomesNull_LogsErrorAndStateRemainsNull()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () => new(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState> { StateToLoad = new() { Value = SampleStateValue } };

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var previousDrivers = Splat.Locator.Current.GetServices<ISuspensionDriver>().ToList();
        Splat.Locator.CurrentMutable.UnregisterAll<ISuspensionDriver>();
        try
        {
            SuspensionHostExtensions.SuspensionDriver = null;

            var state = host.GetAppState();

            await Assert.That((object?)state).IsNull();
        }
        finally
        {
            foreach (var previousDriver in previousDrivers)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousDriver);
            }
        }
    }

    /// <summary>Verifies the typed EnsureLoadAppState loads the state only once across repeated calls.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EnsureLoadAppState_Typed_OnlyLoadsOnce()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () => new(),
            IsLaunchingNew = Signal.Silent<RxVoid>(),
            IsResuming = Signal.Silent<RxVoid>(),
            ShouldPersistState = Signal.Silent<IDisposable>(),
            ShouldInvalidateState = Signal.Silent<RxVoid>()
        };

        var driver = new TestSuspensionDriver<TestAppState> { StateToLoad = new() { Value = SampleStateValue } };

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state1 = host.GetAppState();
        var state2 = host.GetAppState();

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
        await Assert.That(state1).IsSameReferenceAs(state2);
    }

    /// <summary>A simple application state used by the typed suspension host tests.</summary>
    private sealed class TestAppState
    {
        /// <summary>Gets or sets an arbitrary value used to assert state identity.</summary>
        public int Value { get; set; }
    }

    /// <summary>Source-generated JSON serializer context for <see cref="TestAppState" />.</summary>
    [JsonSerializable(typeof(TestAppState))]
    private sealed partial class TestAppStateContext : JsonSerializerContext;

    /// <summary>A fake <see cref="ISuspensionDriver" /> that records calls for assertions.</summary>
    /// <typeparam name="T">The application state type.</typeparam>
    private sealed class TestSuspensionDriver<T> : ISuspensionDriverContract
        where T : class
    {
        /// <summary>Gets the number of times InvalidateState was called.</summary>
        public int InvalidateStateCallCount { get; private set; }

        /// <summary>Gets the last state passed to SaveState.</summary>
        public T? LastSavedState { get; private set; }

        /// <summary>Gets the number of times LoadState was called.</summary>
        public int LoadStateCallCount { get; private set; }

        /// <summary>Gets the number of times SaveState was called.</summary>
        public int SaveStateCallCount { get; private set; }

        /// <summary>Gets or sets a value indicating whether LoadState should throw.</summary>
        public bool ShouldThrowOnLoad { get; set; }

        /// <summary>Gets or sets the state returned by LoadState.</summary>
        public T? StateToLoad { get; set; }

        /// <inheritdoc/>
        public IObservable<RxVoid> InvalidateState()
        {
            InvalidateStateCallCount++;
            return Signal.Emit(RxVoid.Default, Sequencer.Immediate);
        }

        /// <inheritdoc/>
        [RequiresUnreferencedCode("Reflection-based serialization")]
        [RequiresDynamicCode("Reflection-based serialization")]
        public IObservable<object?> LoadState()
        {
            LoadStateCallCount++;
            return ShouldThrowOnLoad ? Signal.Fail<object?>(
                    new InvalidOperationException("Failed to load state"),
                    Sequencer.Immediate) : Signal.Emit((object?)StateToLoad, Sequencer.Immediate);
        }

        /// <inheritdoc/>
        public IObservable<TState?> LoadState<TState>(JsonTypeInfo<TState> typeInfo)
        {
            LoadStateCallCount++;
            if (ShouldThrowOnLoad)
            {
                return Signal.Fail<TState?>(
                    new InvalidOperationException("Failed to load state"),
                    Sequencer.Immediate);
            }

            return StateToLoad is TState typedState ? Signal.Emit<TState?>(typedState, Sequencer.Immediate) : Signal.Emit<TState?>(default, Sequencer.Immediate);
        }

        /// <inheritdoc/>
        [RequiresUnreferencedCode("Reflection-based serialization")]
        [RequiresDynamicCode("Reflection-based serialization")]
        public IObservable<RxVoid> SaveState<TState>(TState state)
        {
            SaveStateCallCount++;
            if (state is T typedState)
            {
                LastSavedState = typedState;
            }

            return Signal.Emit(RxVoid.Default, Sequencer.Immediate);
        }

        /// <inheritdoc/>
        public IObservable<RxVoid> SaveState<TState>(TState state, JsonTypeInfo<TState> typeInfo)
        {
            SaveStateCallCount++;
            if (state is T typedState)
            {
                LastSavedState = typedState;
            }

            return Signal.Emit(RxVoid.Default, Sequencer.Immediate);
        }
    }
}
