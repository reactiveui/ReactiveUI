// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
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
/// <summary>Loads and saves state to persistent storage under the platform Application Support directory.</summary>
/// <remarks>
/// <para>
/// Uses source-generated System.Text.Json via overloads accepting <see cref="JsonTypeInfo{T}"/>.
/// For reflection-based serialization, use <c>AppSupportJsonSuspensionDriverUnsafe</c> from <c>ReactiveUI.Unsafe</c>.
/// </para>
/// <para>
/// The persisted file name is <c>state.dat</c>.
/// </para>
/// </remarks>
[DebuggerDisplay("AppSupportJsonSuspensionDriver")]
public sealed class AppSupportJsonSuspensionDriver : ISuspensionDriver
{
    /// <summary>The default subdirectory used beneath Application Support.</summary>
    private const string DefaultSubDirectory = "Data";

    /// <summary>The persisted state file name.</summary>
    private const string StateFileName = "state.dat";

    /// <summary>Lazily computed directory path used to reduce repeated Foundation calls and filesystem checks.</summary>
    private readonly Lazy<string> _appDirectory;

    /// <summary>Initializes a new instance of the <see cref="AppSupportJsonSuspensionDriver"/> class using the default <c>Data</c> subdirectory beneath Application Support.</summary>
    public AppSupportJsonSuspensionDriver()
        : this(DefaultSubDirectory)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AppSupportJsonSuspensionDriver"/> class.</summary>
    /// <param name="subDirectory">
    /// The application-specific subdirectory beneath Application Support to store state.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="subDirectory"/> is <see langword="null"/>.</exception>
    public AppSupportJsonSuspensionDriver(string subDirectory)
    {
        ArgumentExceptionHelper.ThrowIfNull(subDirectory);

        _appDirectory = new(
            () => AppleAppDirectoryHelper.CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory, subDirectory),
            isThreadSafe: true);
    }

    /// <inheritdoc />
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
    {
        ArgumentExceptionHelper.ThrowIfNull(typeInfo);

        try
        {
            var path = GetStatePath();
            using var stream = File.OpenRead(path);

            var result = JsonSerializer.Deserialize(stream, typeInfo);
            return Signal.Emit(result);
        }
        catch (Exception ex)
        {
            return Signal.Fail<T?>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
    {
        ArgumentExceptionHelper.ThrowIfNull(typeInfo);

        try
        {
            var path = GetStatePath();
            using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);

            JsonSerializer.Serialize(stream, state, typeInfo);
            return ImmutableReturnRxVoidSignal.Instance;
        }
        catch (Exception ex)
        {
            return Signal.Fail<RxVoid>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<RxVoid> InvalidateState()
    {
        try
        {
            var path = GetStatePath();
            File.Delete(path);

            return ImmutableReturnRxVoidSignal.Instance;
        }
        catch (Exception ex)
        {
            return Signal.Fail<RxVoid>(ex);
        }
    }

    /// <summary>Computes the full path to the persisted state file.</summary>
    /// <returns>The absolute file path.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetStatePath() =>
        Path.Combine(_appDirectory.Value, StateFileName);
}
