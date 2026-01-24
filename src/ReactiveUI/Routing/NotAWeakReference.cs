// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Represents a reference to an object that is always considered alive and is not subject to garbage collection
/// tracking.
/// </summary>
/// <remarks>Unlike a standard weak reference, this class always reports the target as alive and does not allow
/// the referenced object to be collected by the garbage collector. Use this class when a strong reference is required
/// but an API expects a weak reference-like interface.</remarks>
/// <param name="target">The object to reference. Cannot be null.</param>
internal class NotAWeakReference(object target)
{
    /// <summary>
    /// Gets the underlying object associated with this instance.
    /// </summary>
    public object Target { get; } = target;

    /// <summary>
    /// Gets a value indicating whether the current instance is considered alive.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1822", Justification = "Keep existing API.")]
    public bool IsAlive => true;
}
