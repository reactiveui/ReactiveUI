// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>
/// Activation-lifecycle plumbing shared by the platform view types. Those types extend unrelated platform base
/// classes (<c>UIView</c>, <c>UIViewController</c>, <c>NSView</c>, <c>NSWindowController</c>, <c>Activity</c>,
/// <c>Fragment</c>), so the handling of their activated/deactivated signals cannot be inherited from a common
/// base and lives here instead.
/// </summary>
/// <remarks>
/// This file is linked into each platform leaf rather than shared through a project reference, so every leaf
/// compiles its own internal copy. It sits in the leaf's root namespace, so platform code in a child namespace
/// such as <c>ReactiveUI.AndroidX</c> resolves it without an extra using directive.
/// </remarks>
internal static class ActivationSignals
{
    /// <summary>Notifies subscribers that the owning view has been activated or deactivated.</summary>
    /// <param name="signal">The activation or deactivation signal to raise.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Raise(Signal<RxVoid> signal) => signal.OnNext(RxVoid.Default);

    /// <summary>
    /// Tears down a view's activation signals from its <c>Dispose(bool)</c> override. They are managed state,
    /// so they are only released on a deterministic dispose and left alone during finalization. Call this
    /// before <c>base.Dispose(disposing)</c>, so the signals are gone before the platform base class starts
    /// releasing the native peer.
    /// </summary>
    /// <param name="disposing">Whether the owner is being disposed deterministically.</param>
    /// <param name="activated">The activation signal.</param>
    /// <param name="deactivated">The deactivation signal.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DisposeWhen(bool disposing, Signal<RxVoid> activated, Signal<RxVoid> deactivated)
    {
        if (!disposing)
        {
            return;
        }

        activated.Dispose();
        deactivated.Dispose();
    }

    /// <summary>Tears down a view's activation signals along with one further signal it owns, in declaration order.</summary>
    /// <typeparam name="TResult">The element type of the additional signal.</typeparam>
    /// <param name="disposing">Whether the owner is being disposed deterministically.</param>
    /// <param name="activated">The activation signal.</param>
    /// <param name="deactivated">The deactivation signal.</param>
    /// <param name="additional">The additional signal owned by the same view, disposed last.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DisposeWhen<TResult>(
        bool disposing,
        Signal<RxVoid> activated,
        Signal<RxVoid> deactivated,
        Signal<TResult> additional)
    {
        if (!disposing)
        {
            return;
        }

        activated.Dispose();
        deactivated.Dispose();
        additional.Dispose();
    }

    /// <summary>
    /// Raises activation or deactivation on the main thread, backing <c>ICanForceManualActivation.Activate</c>.
    /// The caller is a platform view being activated from an arbitrary thread, so the notification is marshalled
    /// through <see cref="RxSchedulers.MainThreadScheduler"/> rather than raised inline.
    /// </summary>
    /// <param name="isActivating">Whether the view is being activated rather than deactivated.</param>
    /// <param name="activated">The activation signal.</param>
    /// <param name="deactivated">The deactivation signal.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ScheduleActivation(bool isActivating, Signal<RxVoid> activated, Signal<RxVoid> deactivated) =>
        RxSchedulers.MainThreadScheduler.Schedule(
            isActivating ? activated : deactivated,
            static (_, signal) =>
            {
                signal.OnNext(RxVoid.Default);
                return EmptyDisposable.Instance;
            });
}
