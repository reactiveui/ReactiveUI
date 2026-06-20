// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// Exposes a single storage slot on a reactive object so <see cref="IReactiveObjectExtensions"/> can keep that
/// object's notification state (change/changing/exception subjects, suppression counters) on the instance itself
/// rather than in a process-wide <see cref="System.Runtime.CompilerServices.ConditionalWeakTable{TKey, TValue}"/>.
/// This removes a per-notification table lookup and the associated allocations on the hot path.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ReactiveObject"/> and the platform UI base types implement this (explicitly, so it stays off their
/// public surface). Hand-rolled <see cref="IReactiveObject"/> implementers are not required to: those without a
/// slot transparently fall back to the table, so this is a performance opt-in and not a breaking requirement.
/// </para>
/// <para>
/// The slot holds an opaque framework-internal object; consumers must not read or write it directly. A reference is
/// returned so the framework can initialize it atomically on first access.
/// </para>
/// </remarks>
public interface IReactiveObjectStateSlot
{
    /// <summary>
    /// Returns a reference to this object's single reactive notification-state slot. The framework uses the
    /// reference to lazily and atomically initialize the slot; the stored value is framework-internal and opaque.
    /// </summary>
    /// <returns>A reference to the backing field that stores this object's reactive notification state.</returns>
    ref object? GetReactiveStateSlot();
}
