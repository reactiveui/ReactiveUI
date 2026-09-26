// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>A suspension driver that keeps the journal in memory, standing in for one that writes to disk.</summary>
[System.Diagnostics.DebuggerDisplay("InMemorySuspensionDriver")]
public sealed class InMemorySuspensionDriver : ISuspensionDriver
{
    /// <summary>The last state that was saved.</summary>
    private object? _state;

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Reflection-based overload kept only to satisfy the interface; this driver never uses reflection.")]
    [RequiresDynamicCode("Reflection-based overload kept only to satisfy the interface; this driver never uses reflection.")]
    public IObservable<RxVoid> SaveState<T>(T state)
    {
        _state = state;
        return Signal.Emit(RxVoid.Default);
    }

    /// <inheritdoc/>
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
    {
        _state = state;
        return Signal.Emit(RxVoid.Default);
    }

    /// <inheritdoc/>
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) =>
        Signal.Emit(_state is T loaded ? loaded : default);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Reflection-based overload kept only to satisfy the interface; this driver never uses reflection.")]
    [RequiresDynamicCode("Reflection-based overload kept only to satisfy the interface; this driver never uses reflection.")]
    public IObservable<object?> LoadState() => Signal.Emit(_state);

    /// <inheritdoc/>
    public IObservable<RxVoid> InvalidateState()
    {
        _state = null;
        return Signal.Emit(RxVoid.Default);
    }
}
