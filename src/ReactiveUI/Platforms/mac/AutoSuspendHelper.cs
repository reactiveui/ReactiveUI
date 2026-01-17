// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using AppKit;

using Foundation;

namespace ReactiveUI;

/// <summary>
/// Bridges <see cref="NSApplication"/> lifecycle notifications into <see cref="RxSuspension.SuspensionHost"/> on macOS.
/// </summary>
/// <typeparam name="T">The concrete <see cref="NSApplicationDelegate"/> type.</typeparam>
/// <remarks>
/// <para>
/// Instantiate this helper inside your <see cref="NSApplicationDelegate"/> to map <c>DidFinishLaunching</c>,
/// <c>DidBecomeActive</c>, and termination callbacks to the shared <see cref="ISuspensionHost"/> streams. Pair it with
/// <see cref="SuspensionHostExtensions.SetupDefaultSuspendResume(ISuspensionHost, ISuspensionDriver?)"/> to persist
/// view model state using a platform-specific <see cref="ISuspensionDriver"/> implementation.
/// </para>
/// <para>
/// Example usage:
/// <code language="csharp">
/// <![CDATA[
/// public class AppDelegate : NSApplicationDelegate
/// {
///     private AutoSuspendHelper? _suspensionHelper;
///
///     public override void DidFinishLaunching(NSNotification notification)
///     {
///         _suspensionHelper ??= new AutoSuspendHelper(this);
///         RxSuspension.SuspensionHost.CreateNewAppState = () => new ShellState();
///         RxSuspension.SuspensionHost.SetupDefaultSuspendResume(new FileSuspensionDriver(AppStatePathProvider.Resolve()));
///         base.DidFinishLaunching(notification);
///     }
/// }
/// ]]>
/// </code>
/// </para>
/// </remarks>
public class AutoSuspendHelper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] T> : IEnableLogger, IDisposable
    where T : NSApplicationDelegate
{
    /// <summary>
    /// Emits disposables used by <see cref="ISuspensionHost.ShouldPersistState"/> subscribers to delimit persistence work.
    /// </summary>
    private readonly Subject<IDisposable> _shouldPersistState = new();

    /// <summary>
    /// Emits values to indicate the application is resuming from a prior persisted state.
    /// </summary>
    private readonly Subject<Unit> _isResuming = new();

    /// <summary>
    /// Emits values to indicate the application is becoming active again after being backgrounded/hidden.
    /// </summary>
    private readonly Subject<Unit> _isUnpausing = new();

    /// <summary>
    /// Emits values to indicate an unexpected termination, prompting state invalidation.
    /// </summary>
    private readonly Subject<Unit> _untimelyDemise = new();

    /// <summary>
    /// Cached handler so we can unsubscribe during <see cref="Dispose()"/>.
    /// </summary>
    private readonly UnhandledExceptionEventHandler _unhandledExceptionHandler;

    /// <summary>
    /// Tracks whether this instance has been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSuspendHelper{T}"/> class.
    /// </summary>
    /// <param name="appDelegate">The application delegate.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="appDelegate"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when required lifecycle methods are not declared on <paramref name="appDelegate"/>'s runtime type.
    /// </exception>
    public AutoSuspendHelper(T appDelegate)
    {
        ArgumentNullException.ThrowIfNull(appDelegate);

        // Developer-time guard. Cache the result per delegate runtime type to avoid repeated reflection.
        EnsureMethodsNotOverloadedCached();

        RxSuspension.SuspensionHost.IsLaunchingNew = Observable<Unit>.Never;
        RxSuspension.SuspensionHost.IsResuming = _isResuming;
        RxSuspension.SuspensionHost.IsUnpausing = _isUnpausing;
        RxSuspension.SuspensionHost.ShouldPersistState = _shouldPersistState;

        // Keep a stable delegate instance so we can unsubscribe on Dispose.
        _unhandledExceptionHandler = (_, _) => _untimelyDemise.OnNext(Unit.Default);
        AppDomain.CurrentDomain.UnhandledException += _unhandledExceptionHandler;

        RxSuspension.SuspensionHost.ShouldInvalidateState = _untimelyDemise;
    }

    /// <summary>
    /// Handles the application termination request.
    /// </summary>
    /// <param name="sender">The application instance requesting termination.</param>
    /// <returns>
    /// <see cref="NSApplicationTerminateReply.Later"/> to delay termination until persistence completes.
    /// </returns>
    /// <remarks>
    /// Delays OS shutdown until <see cref="ISuspensionHost.ShouldPersistState"/> subscribers finish writing
    /// <see cref="ISuspensionHost.AppState"/>, replying with <see cref="NSApplication.ReplyToApplicationShouldTerminate(bool)"/>
    /// once persistence completes.
    /// </remarks>
    public NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
    {
        ThrowIfDisposed();

        // Ensure the persist notification is emitted on the main thread, as callers typically interact with AppKit.
        RxSchedulers.MainThreadScheduler.Schedule(() =>
            _shouldPersistState.OnNext(
                Disposable.Create(() => sender.ReplyToApplicationShouldTerminate(true))));

        return NSApplicationTerminateReply.Later;
    }

    /// <summary>
    /// Notifies the helper that the application finished launching.
    /// </summary>
    /// <param name="notification">The launch notification.</param>
    /// <remarks>
    /// Signals <see cref="ISuspensionHost.IsResuming"/> so state drivers load the last persisted <see cref="ISuspensionHost.AppState"/>.
    /// </remarks>
    public void DidFinishLaunching(NSNotification notification)
    {
        ThrowIfDisposed();
        _isResuming.OnNext(Unit.Default);
    }

    /// <summary>
    /// Notifies the helper that the application resigned active state.
    /// </summary>
    /// <param name="notification">The resign-active notification.</param>
    /// <remarks>
    /// Requests an asynchronous save by emitting <see cref="Disposable.Empty"/> via <see cref="ISuspensionHost.ShouldPersistState"/>.
    /// </remarks>
    public void DidResignActive(NSNotification notification)
    {
        ThrowIfDisposed();
        _shouldPersistState.OnNext(Disposable.Empty);
    }

    /// <summary>
    /// Notifies the helper that the application became active.
    /// </summary>
    /// <param name="notification">The become-active notification.</param>
    /// <remarks>
    /// Signals <see cref="ISuspensionHost.IsUnpausing"/> so subscribers can refresh transient UI when the app regains focus.
    /// </remarks>
    public void DidBecomeActive(NSNotification notification)
    {
        ThrowIfDisposed();
        _isUnpausing.OnNext(Unit.Default);
    }

    /// <summary>
    /// Notifies the helper that the application was hidden.
    /// </summary>
    /// <param name="notification">The hide notification.</param>
    /// <remarks>
    /// Initiates a quick save when the app is hidden, mirroring the behavior of <see cref="DidResignActive"/>.
    /// </remarks>
    public void DidHide(NSNotification notification)
    {
        ThrowIfDisposed();
        _shouldPersistState.OnNext(Disposable.Empty);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases resources held by this helper.
    /// </summary>
    /// <param name="isDisposing">If <see langword="true"/>, disposes managed resources.</param>
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

        // Unsubscribe first to avoid keeping this instance alive via the AppDomain event.
        AppDomain.CurrentDomain.UnhandledException -= _unhandledExceptionHandler;

        _isResuming.Dispose();
        _isUnpausing.Dispose();
        _shouldPersistState.Dispose();
        _untimelyDemise.Dispose();
    }

    /// <summary>
    /// Performs the "methods must be implemented" guard once per application delegate runtime type.
    /// </summary>
    /// <exception cref="Exception">
    /// Thrown when required lifecycle methods are not declared on appDelegate's runtime type.
    /// </exception>
    /// <remarks>
    /// Delegates to <see cref="Reflection.ThrowIfMethodsNotOverloaded(string, object, string[])"/> on a cache miss,
    /// but avoids repeating the reflection scan for each helper construction.
    /// </remarks>
    private static void EnsureMethodsNotOverloadedCached()
    {
        var type = typeof(T);

        if (MethodForwardingValidationCache.IsValidated(type))
        {
            return;
        }

        Reflection.ThrowIfMethodsNotOverloaded(
            nameof(AutoSuspendHelper<T>),
            type,
            nameof(ApplicationShouldTerminate),
            nameof(DidFinishLaunching),
            nameof(DidResignActive),
            nameof(DidBecomeActive),
            nameof(DidHide));

        MethodForwardingValidationCache.MarkValidated(type);
    }

    /// <summary>
    /// Throws if this instance has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(AutoSuspendHelper<T>));
        }
    }

    /// <summary>
    /// Stores a process-wide cache of which delegate types have been validated for lifecycle forwarding.
    /// </summary>
    /// <remarks>
    /// Uses a single gate because this is cold-path initialization and the number of delegate types is tiny.
    /// </remarks>
    private static class MethodForwardingValidationCache
    {
#if NET9_0_OR_GREATER
        private static readonly Lock Gate = new();
#else
        private static readonly object Gate = new();
#endif
        private static readonly Dictionary<Type, byte> Validated = new();

        /// <summary>
        /// Returns whether the specified delegate type has been validated.
        /// </summary>
        /// <param name="type">The delegate runtime type.</param>
        /// <returns><see langword="true"/> if the type has been validated; otherwise <see langword="false"/>.</returns>
        public static bool IsValidated(Type type)
        {
            lock (Gate)
            {
                return Validated.ContainsKey(type);
            }
        }

        /// <summary>
        /// Marks the specified delegate type as validated.
        /// </summary>
        /// <param name="type">The delegate runtime type.</param>
        public static void MarkValidated(Type type)
        {
            lock (Gate)
            {
                Validated[type] = 0;
            }
        }
    }
}
