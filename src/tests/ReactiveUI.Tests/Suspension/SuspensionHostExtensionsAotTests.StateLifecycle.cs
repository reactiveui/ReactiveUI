// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Suspension;

/// <summary>State persist/load lifecycle tests for the AOT suspension-host extensions.</summary>
public partial class SuspensionHostExtensionsAotTests
{
    /// <summary>
    ///     Verifies that the typed default suspend/resume persists loaded state when persist occurs before the app state is fetched.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ShouldPersistLoadedState_WhenPersistOccursBeforeGetAppState()
    {
        const int LoadedStateValueForPersistBeforeGetAppState = 987;

        var loadedState = new TestAppState { Value = LoadedStateValueForPersistBeforeGetAppState };
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
    [SuppressMessage(
        "Performance",
        "PSH1011:Use the 'Create' overload with a state argument so this lambda does not capture",
        Justification = "the captured value is test-local setup; the allocation is irrelevant in a unit test.")]
    public async Task SetupDefaultSuspendResume_Typed_ShouldDisposePersistTokenAfterSave()
    {
        const int AppStateValueForDisposePersistToken = 111;

        var appState = new TestAppState { Value = AppStateValueForDisposePersistToken };
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
            CreateNewAppStateTyped = static () => new(),
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
            CreateNewAppStateTyped = static () => new() { Value = CreatedStateValue },
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
            CreateNewAppStateTyped = static () => new(),
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
            CreateNewAppStateTyped = static () => new(),
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
}
