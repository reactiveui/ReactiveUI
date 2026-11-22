// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Foundation;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Loads and saves state under Application Support, including reflection-based overloads.</summary>
[DebuggerDisplay("AppSupportJsonSuspensionDriverUnsafe")]
public sealed class AppSupportJsonSuspensionDriverUnsafe : IUnsafeSuspensionDriver
{
    /// <summary>The default subdirectory used beneath Application Support.</summary>
    private const string DefaultSubDirectory = "Data";

    /// <summary>The persisted state file name.</summary>
    private const string StateFileName = "state.dat";

    /// <summary>The AOT-safe inner driver for typed operations.</summary>
    private readonly AppSupportJsonSuspensionDriver _inner;

    /// <summary>Lazily computed directory path used for reflection-based operations.</summary>
    private readonly Lazy<string> _appDirectory;

    /// <summary>Initializes a new instance of the <see cref="AppSupportJsonSuspensionDriverUnsafe"/> class using the default <c>Data</c> subdirectory.</summary>
    public AppSupportJsonSuspensionDriverUnsafe()
        : this(DefaultSubDirectory)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AppSupportJsonSuspensionDriverUnsafe"/> class.</summary>
    /// <param name="subDirectory">The application-specific subdirectory beneath Application Support to store state.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="subDirectory"/> is <see langword="null"/>.</exception>
    public AppSupportJsonSuspensionDriverUnsafe(string subDirectory)
    {
        ArgumentExceptionHelper.ThrowIfNull(subDirectory);

        _inner = new(subDirectory);
        _appDirectory = new(
            () => AppleAppDirectoryHelper.CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory, subDirectory),
            isThreadSafe: true);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) => _inner.LoadState(typeInfo);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection-based System.Text.Json serialization for 'object'. Prefer LoadState<T>(JsonTypeInfo<T>) for trimming/AOT.")]
    [RequiresDynamicCode("Uses reflection-based System.Text.Json serialization for 'object'. Prefer LoadState<T>(JsonTypeInfo<T>) for trimming/AOT.")]
    public IObservable<object?> LoadState()
    {
        try
        {
            var path = GetStatePath();
            using var stream = File.OpenRead(path);

            var result = JsonSerializer.Deserialize<object>(stream);
            return Signal.Emit(result);
        }
        catch (Exception ex)
        {
            return Signal.Fail<object?>(ex);
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) => _inner.SaveState(state, typeInfo);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection-based System.Text.Json serialization for generic T. Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming/AOT.")]
    [RequiresDynamicCode("Uses reflection-based System.Text.Json serialization for generic T. Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming/AOT.")]
    public IObservable<RxVoid> SaveState<T>(T state)
    {
        try
        {
            var path = GetStatePath();
            using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);

            JsonSerializer.Serialize(stream, state);
            return ImmutableReturnRxVoidSignal.Instance;
        }
        catch (Exception ex)
        {
            return Signal.Fail<RxVoid>(ex);
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<RxVoid> InvalidateState() => _inner.InvalidateState();

    /// <summary>Computes the full path to the persisted state file.</summary>
    /// <returns>The absolute file path.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetStatePath() =>
        Path.Combine(_appDirectory.Value, StateFileName);
}
