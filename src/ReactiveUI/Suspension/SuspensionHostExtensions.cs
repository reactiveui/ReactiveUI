// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the ISuspensionHost interface.
/// </summary>
/// <remarks>
/// <para>
/// These helpers provide strongly-typed access to the current application state and wire up the
/// <see cref="ISuspensionDriver"/> responsible for persisting it. They are typically invoked from platform bootstrap
/// classes after registering an <c>AutoSuspendHelper</c>.
/// </para>
/// </remarks>
public static class SuspensionHostExtensions
{
    /// <summary>
    /// Func used to load app state exactly once. Backing field for testing purposes.
    /// </summary>
    private static Func<IObservable<Unit>>? _ensureLoadAppStateFunc;

    /// <summary>
    /// Suspension driver reference field. Backing field for testing purposes.
    /// </summary>
    private static ISuspensionDriver? _suspensionDriver;

    /// <summary>
    /// Gets or sets the ensure load app state function. Internal for testing purposes only.
    /// </summary>
    [SuppressMessage("Roslynator", "RCS1085:Use auto-implemented property", Justification = "Need explicit backing field for Interlocked.Exchange")]
    internal static Func<IObservable<Unit>>? EnsureLoadAppStateFunc
    {
        get => _ensureLoadAppStateFunc;
        set => _ensureLoadAppStateFunc = value;
    }

    /// <summary>
    /// Gets or sets the suspension driver. Internal for testing purposes only.
    /// </summary>
    [SuppressMessage("Roslynator", "RCS1085:Use auto-implemented property", Justification = "Need explicit backing field for Interlocked.Exchange")]
    internal static ISuspensionDriver? SuspensionDriver
    {
        get => _suspensionDriver;
        set => _suspensionDriver = value;
    }

    /// <summary>
    /// Get the current App State of a class derived from ISuspensionHost.
    /// </summary>
    /// <typeparam name="T">The app state type.</typeparam>
    /// <param name="item">The suspension host.</param>
    /// <returns>The app state.</returns>
    /// <remarks>
    /// Calling this method triggers a one-time load via <see cref="ISuspensionDriver.LoadState"/> if the state has not
    /// yet been materialized, ensuring late subscribers still receive persisted data.
    /// </remarks>
    public static T GetAppState<T>(this ISuspensionHost item)
    {
        item.ArgumentNullExceptionThrowIfNull(nameof(item));

        Interlocked.Exchange(ref _ensureLoadAppStateFunc, null)?.Invoke();

        return (T)item.AppState!;
    }

    /// <summary>
    /// Observe changes to the AppState of a class derived from ISuspensionHost.
    /// </summary>
    /// <typeparam name="T">The observable type.</typeparam>
    /// <param name="item">The suspension host.</param>
    /// <returns>An observable of the app state.</returns>
    /// <remarks>
    /// Emits the current value immediately (if available) and every subsequent assignment so downstream components can
    /// react to hot reloads or state restoration.
    /// </remarks>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("ObserveAppState uses WhenAny which requires dynamic code generation for expression tree analysis")]
    [RequiresUnreferencedCode("ObserveAppState uses WhenAny which may reference members that could be trimmed")]
#endif
    public static IObservable<T> ObserveAppState<T>(this ISuspensionHost item)
        where T : class
    {
        item.ArgumentNullExceptionThrowIfNull(nameof(item));

        return item.WhenAny<ISuspensionHost, object?, object?>(nameof(item.AppState), static observedChange => observedChange.Value)
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
    /// <remarks>
    /// <para>
    /// Registers handlers for <see cref="ISuspensionHost.ShouldPersistState"/>, <see cref="ISuspensionHost.ShouldInvalidateState"/>,
    /// and resume notifications, delegating serialization to the provided <paramref name="driver"/> (or a resolved
    /// instance from <see cref="AppLocator"/>).
    /// </para>
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// <![CDATA[
    /// RxApp.SuspensionHost.CreateNewAppState = () => new ShellState();
    /// RxApp.SuspensionHost.SetupDefaultSuspendResume(new FileSuspensionDriver(FileSystem.AppDataDirectory));
    /// ]]>
    /// </code>
    /// </example>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("SetupDefaultSuspendResume uses ISuspensionDriver which may require dynamic code generation for serialization")]
    [RequiresUnreferencedCode("SetupDefaultSuspendResume uses ISuspensionDriver which may require unreferenced code for serialization")]
#endif
    public static IDisposable SetupDefaultSuspendResume(this ISuspensionHost item, ISuspensionDriver? driver = null)
    {
        item.ArgumentNullExceptionThrowIfNull(nameof(item));

        var ret = new CompositeDisposable();
        _suspensionDriver ??= driver ?? AppLocator.Current.GetService<ISuspensionDriver>();

        if (_suspensionDriver is null)
        {
            item.Log().Error("Could not find a valid driver and therefore cannot setup Suspend/Resume.");
            return Disposable.Empty;
        }

        _ensureLoadAppStateFunc = () => EnsureLoadAppState(item, _suspensionDriver);

        ret.Add(item.ShouldInvalidateState
                    .SelectMany(_ => _suspensionDriver.InvalidateState())
                    .LoggedCatch(item, Observables.Unit, "Tried to invalidate app state")
                    .Subscribe(_ => item.Log().Info("Invalidated app state")));

        ret.Add(item.ShouldPersistState
                    .SelectMany(x => _suspensionDriver.SaveState(item.AppState!).Finally(x.Dispose))
                    .LoggedCatch(item, Observables.Unit, "Tried to persist app state")
                    .Subscribe(_ => item.Log().Info("Persisted application state")));

        ret.Add(item.IsResuming.Merge(item.IsLaunchingNew)
                    .Do(_ => Interlocked.Exchange(ref _ensureLoadAppStateFunc, null)?.Invoke())
                    .Subscribe());

        return ret;
    }

    /// <summary>
    /// Ensures one time app state load from storage.
    /// </summary>
    /// <param name="item">The suspension host.</param>
    /// <param name="driver">The suspension driver.</param>
    /// <returns>A completed observable.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("EnsureLoadAppState uses ISuspensionDriver.LoadState which may require dynamic code generation for serialization")]
    [RequiresUnreferencedCode("EnsureLoadAppState uses ISuspensionDriver.LoadState which may require unreferenced code for serialization")]
#endif
    private static IObservable<Unit> EnsureLoadAppState(this ISuspensionHost item, ISuspensionDriver? driver = null)
    {
        if (item.AppState is not null)
        {
            return Observable.Return(Unit.Default);
        }

        _suspensionDriver ??= driver ?? AppLocator.Current.GetService<ISuspensionDriver>();

        if (_suspensionDriver is null)
        {
            item.Log().Error("Could not find a valid driver and therefore cannot load app state.");
            return Observable.Return(Unit.Default);
        }

        try
        {
            item.AppState = _suspensionDriver.LoadState().Wait();
        }
        catch (Exception ex)
        {
            item.Log().Warn(ex, "Failed to restore app state from storage, creating from scratch");
            item.AppState = item.CreateNewAppState?.Invoke();
        }

        return Observable.Return(Unit.Default);
    }
}
