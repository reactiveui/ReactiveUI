// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the ISuspensionHost interface.
/// </summary>
public static class SuspensionHostExtensions
{
    /// <summary>
    /// Func used to load app state exactly once.
    /// </summary>
    private static Func<IObservable<Unit>>? ensureLoadAppStateFunc;

    /// <summary>
    /// Supsension driver reference field to prevent introducing breaking change.
    /// </summary>
    private static ISuspensionDriver? suspensionDriver;

    /// <summary>
    /// Get the current App State of a class derived from ISuspensionHost.
    /// </summary>
    /// <typeparam name="T">The app state type.</typeparam>
    /// <param name="item">The suspension host.</param>
    /// <returns>The app state.</returns>
    public static T GetAppState<T>(this ISuspensionHost item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        Interlocked.Exchange(ref ensureLoadAppStateFunc, null)?.Invoke();

        return (T)item.AppState!;
    }

    /// <summary>
    /// Observe changes to the AppState of a class derived from ISuspensionHost.
    /// </summary>
    /// <typeparam name="T">The observable type.</typeparam>
    /// <param name="item">The suspension host.</param>
    /// <returns>An observable of the app state.</returns>
    public static IObservable<T> ObserveAppState<T>(this ISuspensionHost item)
        where T : class
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        return item.WhenAny(suspensionHost => suspensionHost.AppState, observedChange => observedChange.Value)
                   .WhereNotNull()
                   .Cast<T>();
    }

    /// <summary>
    /// Setup our suspension driver for a class derived off ISuspensionHost interface.
    /// This will make your suspension host respond to suspend and resume requests.
    /// </summary>
    /// <param name="item">The suspension host.</param>
    /// <param name="driver">The suspension driver.</param>
    /// <returns>A disposable which will stop responding to Suspend and Resume requests.</returns>
    public static IDisposable SetupDefaultSuspendResume(this ISuspensionHost item, ISuspensionDriver? driver = null)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        var ret = new CompositeDisposable();
        suspensionDriver ??= driver ?? Locator.Current.GetService<ISuspensionDriver>();

        if (suspensionDriver is null)
        {
            item.Log().Error("Could not find a valid driver and therefore cannot setup Suspend/Resume.");
            return Disposable.Empty;
        }

        ensureLoadAppStateFunc = () => EnsureLoadAppState(item, suspensionDriver);

        ret.Add(item.ShouldInvalidateState
                    .SelectMany(_ => suspensionDriver.InvalidateState())
                    .LoggedCatch(item, Observables.Unit, "Tried to invalidate app state")
                    .Subscribe(_ => item.Log().Info("Invalidated app state")));

        ret.Add(item.ShouldPersistState
                    .SelectMany(x => suspensionDriver.SaveState(item.AppState!).Finally(x.Dispose))
                    .LoggedCatch(item, Observables.Unit, "Tried to persist app state")
                    .Subscribe(_ => item.Log().Info("Persisted application state")));

        ret.Add(item.IsResuming.Merge(item.IsLaunchingNew)
                    .Do(_ => Interlocked.Exchange(ref ensureLoadAppStateFunc, null)?.Invoke())
                    .Subscribe());

        return ret;
    }

    /// <summary>
    /// Ensures one time app state load from storage.
    /// </summary>
    /// <param name="item">The suspension host.</param>
    /// <param name="driver">The suspension driver.</param>
    /// <returns>A completed observable.</returns>
    private static IObservable<Unit> EnsureLoadAppState(this ISuspensionHost item, ISuspensionDriver? driver = null)
    {
        if (item.AppState is not null)
        {
            return Observable.Return(Unit.Default);
        }

        suspensionDriver ??= driver ?? Locator.Current.GetService<ISuspensionDriver>();

        if (suspensionDriver is null)
        {
            item.Log().Error("Could not find a valid driver and therefore cannot load app state.");
            return Observable.Return(Unit.Default);
        }

        try
        {
            item.AppState = suspensionDriver.LoadState().Wait();
        }
        catch (Exception ex)
        {
            item.Log().Warn(ex, "Failed to restore app state from storage, creating from scratch");
            item.AppState = item.CreateNewAppState?.Invoke();
        }

        return Observable.Return(Unit.Default);
    }
}
