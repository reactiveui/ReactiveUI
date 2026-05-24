// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Foundation;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;
using Splat;
using UIKit;

using NSAction = System.Action;

namespace ReactiveUI;

/// <summary>
/// Bridges iOS lifecycle notifications into <see cref="RxSuspension.SuspensionHost"/> so applications can persist and
/// restore state without manually wiring UIKit events.
/// </summary>
/// <typeparam name="T">The concrete <see cref="UIApplicationDelegate"/> type.</typeparam>
/// <remarks>
/// <para>
/// Instantiate <see cref="AutoSuspendHelper{T}"/> inside your <see cref="UIApplicationDelegate"/> and forward the
/// <c>FinishedLaunching</c>, <c>OnActivated</c>, and <c>DidEnterBackground</c> events to the helper. The helper updates
/// the shared <see cref="ISuspensionHost"/> observables and takes care of requesting background time when persisting
/// application state.
/// </para>
/// </remarks>
public class AutoSuspendHelper<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] T> : IEnableLogger, IDisposable
        where T : UIApplicationDelegate
{
    /// <summary>Subject that fires when the application finishes launching.</summary>
    private readonly Subject<UIApplication> _finishedLaunching = new();

    /// <summary>Subject that fires when the application becomes active.</summary>
    private readonly Subject<UIApplication> _activated = new();

    /// <summary>Subject that fires when the application enters the background.</summary>
    private readonly Subject<UIApplication> _backgrounded = new();

    /// <summary>Subject that fires when an unhandled exception signals an untimely process death.</summary>
    private readonly Subject<Unit> _untimelyDeath = new();

    /// <summary>The cached handler used to subscribe and later unsubscribe from <see cref="AppDomain.UnhandledException"/>.</summary>
    private readonly UnhandledExceptionEventHandler _unhandledExceptionHandler;

    /// <summary>Whether this instance has already been disposed.</summary>
    private bool _isDisposed;

    /// <summary>
    /// Initializes static members of the <see cref="AutoSuspendHelper{T}"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This validation runs exactly once per closed generic type and avoids repeated reflection and cache/lock overhead.
    /// </para>
    /// <para>
    /// The call uses the Type-based overload of <c>ThrowIfMethodsNotOverloaded</c> and expresses trimming requirements via
    /// <see cref="DynamicallyAccessedMembersAttribute"/> on <typeparamref name="T"/>, avoiding <c>RequiresUnreferencedCode</c>
    /// propagation.
    /// </para>
    /// </remarks>
    static AutoSuspendHelper()
    {
        Reflection.ThrowIfMethodsNotOverloaded(
            nameof(AutoSuspendHelper<>),
            typeof(T),
            nameof(FinishedLaunching),
            nameof(OnActivated),
            nameof(DidEnterBackground));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSuspendHelper{T}"/> class.
    /// </summary>
    /// <param name="appDelegate">The application delegate instance that forwards lifecycle methods.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="appDelegate"/> is <see langword="null"/>.</exception>
    public AutoSuspendHelper(T appDelegate)
    {
        ArgumentExceptionHelper.ThrowIfNull(appDelegate);

        RxSuspension.SuspensionHost.IsLaunchingNew = NeverObservable<Unit>.Instance;
        RxSuspension.SuspensionHost.IsResuming = _finishedLaunching.Select(static _ => Unit.Default);
        RxSuspension.SuspensionHost.IsUnpausing = _activated.Select(static _ => Unit.Default);

        // Keep a stable delegate instance so we can unsubscribe on Dispose.
        _unhandledExceptionHandler = (_, _) => _untimelyDeath.OnNext(Unit.Default);
        AppDomain.CurrentDomain.UnhandledException += _unhandledExceptionHandler;

        RxSuspension.SuspensionHost.ShouldInvalidateState = _untimelyDeath;

        RxSuspension.SuspensionHost.ShouldPersistState = _backgrounded.SelectMany(app =>
        {
            var taskId = app.BeginBackgroundTask(() => _untimelyDeath.OnNext(Unit.Default));

            // NB: We're being force-killed, signal invalidate instead.
            if (taskId == UIApplication.BackgroundTaskInvalid)
            {
                _untimelyDeath.OnNext(Unit.Default);
                return EmptyObservable<IDisposable>.Instance;
            }

            return Observable.Return(Disposable.Create(() => app.EndBackgroundTask(taskId)));
        });
    }

    /// <summary>
    /// Gets the launch options captured from the most recent call to <see cref="FinishedLaunching"/>.
    /// Keys are converted to strings and values are stringified for convenience when hydrating state.
    /// </summary>
    public IDictionary<string, string>? LaunchOptions { get; private set; }

    /// <summary>
    /// Notifies the helper that <see cref="UIApplicationDelegate.FinishedLaunching(UIApplication, NSDictionary)"/> was
    /// invoked so it can propagate the <see cref="ISuspensionHost.IsResuming"/> observable.
    /// </summary>
    /// <param name="application">The application instance.</param>
    /// <param name="launchOptions">The launch options dictionary.</param>
    public void FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        ThrowIfDisposed();

        // Preserve original behavior: if launchOptions is null, expose an empty dictionary.
        if (launchOptions is null)
        {
            LaunchOptions = new Dictionary<string, string>(0);
        }
        else
        {
            // Avoid LINQ allocations; keep behavior broadly equivalent to ToDictionary.
            var keys = launchOptions.Keys;
            var dict = new Dictionary<string, string>(keys.Length);

            for (int i = 0; i < keys.Length; i++)
            {
                var k = keys[i];
                var keyString = k?.ToString() ?? string.Empty;

                // NSDictionary keys are unique by contract.
                dict[keyString] = launchOptions[k]?.ToString() ?? string.Empty;
            }

            LaunchOptions = dict;
        }

        // NB: This is run in-context (i.e. not scheduled), so by the time this
        // statement returns, UIWindow should be created already.
        _finishedLaunching.OnNext(application);
    }

    /// <summary>
    /// Notifies the helper that <see cref="UIApplicationDelegate.OnActivated(UIApplication)"/> occurred.
    /// </summary>
    /// <param name="application">The application instance.</param>
    public void OnActivated(UIApplication application)
    {
        ThrowIfDisposed();
        _activated.OnNext(application);
    }

    /// <summary>
    /// Notifies the helper that <see cref="UIApplicationDelegate.DidEnterBackground(UIApplication)"/> was raised so that
    /// persistence can begin.
    /// </summary>
    /// <param name="application">The application instance.</param>
    public void DidEnterBackground(UIApplication application)
    {
        ThrowIfDisposed();
        _backgrounded.OnNext(application);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases managed resources held by the helper.
    /// </summary>
    /// <param name="isDisposing">Whether to release managed resources.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        if (!isDisposing)
        {
            return;
        }

        // Unsubscribe first to avoid keeping the helper alive.
        AppDomain.CurrentDomain.UnhandledException -= _unhandledExceptionHandler;

        // Dispose subjects to release downstream subscriptions deterministically.
        _activated.Dispose();
        _backgrounded.Dispose();
        _finishedLaunching.Dispose();
        _untimelyDeath.Dispose();
    }

    /// <summary>Throws if this helper has already been disposed.</summary>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed()
    {
        if (!_isDisposed)
        {
            return;
        }

        throw new ObjectDisposedException(nameof(AutoSuspendHelper<>));
    }
}
