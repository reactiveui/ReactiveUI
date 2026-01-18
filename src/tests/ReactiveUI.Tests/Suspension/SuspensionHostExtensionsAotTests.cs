// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using ReactiveUI.Tests.Utilities.SuspensionHost;

namespace ReactiveUI.Tests.Suspension;

/// <summary>
///     Tests for AOT-friendly (JsonTypeInfo-based) overloads in SuspensionHostExtensions.
/// </summary>
[NotInParallel]
[TestExecutor<SuspensionHostTestExecutor>]
public partial class SuspensionHostExtensionsAotTests
{
    [Test]
    public async Task GetAppState_Typed_ReturnsCurrentState()
    {
        var state = new TestAppState { Value = 42 };
        using var host = new SuspensionHost<TestAppState>
        {
            AppStateValue = state,
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var result = host.GetAppState();

        await Assert.That(result).IsSameReferenceAs(state);
    }

    [Test]
    public async Task GetAppState_Typed_ThrowsForNullHost()
    {
        SuspensionHost<TestAppState>? host = null;

        await Assert.That(() => host!.GetAppState())
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task GetAppState_Typed_TriggersEnsureLoadAppState()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () => new TestAppState { Value = 99 },
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        driver.StateToLoad = new TestAppState { Value = 123 };

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state = host.GetAppState();

        await Assert.That(state).IsNotNull();
        await Assert.That(state.Value).IsEqualTo(123);
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    [Test]
    public async Task ObserveAppState_Typed_EmitsCurrentValueImmediately()
    {
        var state = new TestAppState { Value = 42 };
        using var host = new SuspensionHost<TestAppState>
        {
            AppStateValue = state,
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var receivedStates = new List<TestAppState>();

        using var subscription = host.ObserveAppState()
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(receivedStates.Add);

        await Assert.That(receivedStates).Count().IsEqualTo(1);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state);
    }

    [Test]
    public async Task ObserveAppState_Typed_EmitsSubsequentChanges()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var receivedStates = new List<TestAppState>();

        using var subscription = host.ObserveAppState()
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(receivedStates.Add);

        var state1 = new TestAppState { Value = 1 };
        host.AppStateValue = state1;

        var state2 = new TestAppState { Value = 2 };
        host.AppStateValue = state2;

        await Assert.That(receivedStates).Count().IsEqualTo(2);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state1);
        await Assert.That(receivedStates[1]).IsSameReferenceAs(state2);
    }

    [Test]
    public async Task ObserveAppState_Typed_FiltersNullValues()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var receivedStates = new List<TestAppState>();

        using var subscription = host.ObserveAppState()
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(receivedStates.Add);

        host.AppStateValue = null;
        var state = new TestAppState { Value = 42 };
        host.AppStateValue = state;
        host.AppStateValue = null;

        await Assert.That(receivedStates).Count().IsEqualTo(1);
        await Assert.That(receivedStates[0]).IsSameReferenceAs(state);
    }

    [Test]
    public async Task ObserveAppState_Typed_ThrowsForNullHost()
    {
        SuspensionHost<TestAppState>? host = null;

        await Assert.That(() => host!.ObserveAppState())
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ThrowsForNullHost()
    {
        SuspensionHost<TestAppState>? host = null;

        await Assert.That(() => host!.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ThrowsForNullTypeInfo()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        await Assert.That(() => host.SetupDefaultSuspendResume(null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SetupDefaultSuspendResume_Typed_WithProvidedDriver_UsesProvidedDriver()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>(),
            CreateNewAppStateTyped = () => new TestAppState()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        driver.StateToLoad = new TestAppState { Value = 42 };

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        await Assert.That(disposable).IsNotNull();
    }

    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ShouldPersistState_CallsDriverSaveState()
    {
        var appState = new TestAppState { Value = 99 };
        using var host = new SuspensionHost<TestAppState>
        {
            AppStateValue = appState,
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        var persistSubject = new Subject<IDisposable>();
        host.ShouldPersistState = persistSubject.ObserveOn(ImmediateScheduler.Instance);

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var token = Disposable.Empty;
        persistSubject.OnNext(token);

        await Assert.That(driver.SaveStateCallCount).IsEqualTo(1);
        await Assert.That(driver.LastSavedState).IsSameReferenceAs(appState);
    }

    [Test]
    public async Task SetupDefaultSuspendResume_Typed_ShouldInvalidateState_CallsDriverInvalidateState()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        var invalidateSubject = new Subject<Unit>();
        host.ShouldInvalidateState = invalidateSubject.ObserveOn(ImmediateScheduler.Instance);

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        invalidateSubject.OnNext(Unit.Default);

        await Assert.That(driver.InvalidateStateCallCount).IsEqualTo(1);
    }

    [Test]
    public async Task SetupDefaultSuspendResume_Typed_IsLaunchingNew_TriggersStateLoad()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () => new TestAppState(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        driver.StateToLoad = new TestAppState { Value = 123 };

        var launchSubject = new Subject<Unit>();
        var resumeSubject = new Subject<Unit>();

        host.IsLaunchingNew = launchSubject.ObserveOn(ImmediateScheduler.Instance);
        host.IsResuming = resumeSubject.ObserveOn(ImmediateScheduler.Instance);

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        launchSubject.OnNext(Unit.Default);

        await Assert.That(host.AppStateValue).IsNotNull();
        await Assert.That(host.AppStateValue!.Value).IsEqualTo(123);
    }

    [Test]
    public async Task SetupDefaultSuspendResume_Typed_WithNullDriver_LogsErrorAndReturnsEmptyDisposable()
    {
        using var host = new SuspensionHost<TestAppState>
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

    [Test]
    public async Task EnsureLoadAppState_Typed_WithExistingState_DoesNotLoad()
    {
        var existingState = new TestAppState { Value = 99 };
        using var host = new SuspensionHost<TestAppState>
        {
            AppStateValue = existingState,
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state = host.GetAppState();

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(0);
        await Assert.That(state).IsSameReferenceAs(existingState);
    }

    [Test]
    public async Task EnsureLoadAppState_Typed_LoadStateThrows_CreatesNewAppState()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () => new TestAppState { Value = 999 },
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        driver.ShouldThrowOnLoad = true;

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state = host.GetAppState();

        await Assert.That(state).IsNotNull();
        await Assert.That(state.Value).IsEqualTo(999);
        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
    }

    [Test]
    public async Task EnsureLoadAppState_Typed_WithNullCreateNewAppState_SetsStateToNull()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = null,
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        driver.ShouldThrowOnLoad = true;

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state = host.GetAppState();

        await Assert.That((object?)state).IsNull();
    }

    [Test]
    public async Task EnsureLoadAppState_Typed_DriverBecomesNull_LogsErrorAndStateRemainsNull()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () => new TestAppState(),
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        driver.StateToLoad = new TestAppState { Value = 42 };

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

    [Test]
    public async Task EnsureLoadAppState_Typed_OnlyLoadsOnce()
    {
        using var host = new SuspensionHost<TestAppState>
        {
            CreateNewAppStateTyped = () => new TestAppState(),
            IsLaunchingNew = Observable.Never<Unit>(),
            IsResuming = Observable.Never<Unit>(),
            ShouldPersistState = Observable.Never<IDisposable>(),
            ShouldInvalidateState = Observable.Never<Unit>()
        };

        var driver = new TestSuspensionDriver<TestAppState>();
        driver.StateToLoad = new TestAppState { Value = 42 };

        using var disposable = host.SetupDefaultSuspendResume(TestAppStateContext.Default.TestAppState, driver);

        var state1 = host.GetAppState();
        var state2 = host.GetAppState();

        await Assert.That(driver.LoadStateCallCount).IsEqualTo(1);
        await Assert.That(state1).IsSameReferenceAs(state2);
    }

    private class TestAppState
    {
        public int Value { get; set; }
    }

    [JsonSerializable(typeof(TestAppState))]
    private partial class TestAppStateContext : JsonSerializerContext
    {
    }

    private class TestSuspensionDriver<T> : ISuspensionDriver
        where T : class
    {
        public int InvalidateStateCallCount { get; private set; }

        public T? LastSavedState { get; private set; }

        public int LoadStateCallCount { get; private set; }

        public int SaveStateCallCount { get; private set; }

        public bool ShouldThrowOnLoad { get; set; }

        public T? StateToLoad { get; set; }

        public IObservable<Unit> InvalidateState()
        {
            InvalidateStateCallCount++;
            return Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        }

        [RequiresUnreferencedCode("Reflection-based serialization")]
        [RequiresDynamicCode("Reflection-based serialization")]
        public IObservable<object?> LoadState()
        {
            LoadStateCallCount++;
            if (ShouldThrowOnLoad)
            {
                return Observable.Throw<object?>(
                    new InvalidOperationException("Failed to load state"),
                    ImmediateScheduler.Instance);
            }

            return Observable.Return((object?)StateToLoad, ImmediateScheduler.Instance);
        }

        public IObservable<TState?> LoadState<TState>(JsonTypeInfo<TState> typeInfo)
        {
            LoadStateCallCount++;
            if (ShouldThrowOnLoad)
            {
                return Observable.Throw<TState?>(
                    new InvalidOperationException("Failed to load state"),
                    ImmediateScheduler.Instance);
            }

            if (StateToLoad is TState typedState)
            {
                return Observable.Return<TState?>(typedState, ImmediateScheduler.Instance);
            }

            return Observable.Return<TState?>(default, ImmediateScheduler.Instance);
        }

        [RequiresUnreferencedCode("Reflection-based serialization")]
        [RequiresDynamicCode("Reflection-based serialization")]
        public IObservable<Unit> SaveState<TState>(TState state)
        {
            SaveStateCallCount++;
            if (state is T typedState)
            {
                LastSavedState = typedState;
            }

            return Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        }

        public IObservable<Unit> SaveState<TState>(TState state, JsonTypeInfo<TState> typeInfo)
        {
            SaveStateCallCount++;
            if (state is T typedState)
            {
                LastSavedState = typedState;
            }

            return Observable.Return(Unit.Default, ImmediateScheduler.Instance);
        }
    }
}
