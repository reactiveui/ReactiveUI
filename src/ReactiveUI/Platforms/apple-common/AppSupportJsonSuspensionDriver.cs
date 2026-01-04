// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using Foundation;

namespace ReactiveUI;

/// <summary>
/// Loads and saves state to persistent storage under the platform Application Support directory.
/// </summary>
/// <remarks>
/// <para>
/// This driver supports two serialization modes:
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// Source-generated System.Text.Json via overloads accepting <see cref="JsonTypeInfo{T}"/>. This is trimming/AOT-friendly.
/// </description>
/// </item>
/// <item>
/// <description>
/// Reflection-based System.Text.Json via <see cref="ISuspensionDriver"/> interface methods. These are marked with
/// <see cref="RequiresUnreferencedCodeAttribute"/> and <see cref="RequiresDynamicCodeAttribute"/>.
/// </description>
/// </item>
/// </list>
/// <para>
/// The persisted file name is <c>state.dat</c>.
/// </para>
/// </remarks>
public sealed class AppSupportJsonSuspensionDriver : ISuspensionDriver
{
    /// <summary>
    /// The default subdirectory used beneath Application Support.
    /// </summary>
    private const string DefaultSubDirectory = "Data";

    /// <summary>
    /// The persisted state file name.
    /// </summary>
    private const string StateFileName = "state.dat";

    /// <summary>
    /// Lazily computed directory path used to reduce repeated Foundation calls and filesystem checks.
    /// </summary>
    private readonly Lazy<string> _appDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppSupportJsonSuspensionDriver"/> class.
    /// </summary>
    /// <param name="subDirectory">
    /// The application-specific subdirectory beneath Application Support to store state. Defaults to <c>Data</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="subDirectory"/> is <see langword="null"/>.</exception>
    public AppSupportJsonSuspensionDriver(string subDirectory = DefaultSubDirectory)
    {
        ArgumentNullException.ThrowIfNull(subDirectory);

        _appDirectory = new Lazy<string>(
            () => CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory, subDirectory),
            isThreadSafe: true);
    }

    /// <inheritdoc />
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);

        try
        {
            var path = GetStatePath();
            using var stream = File.OpenRead(path);

            var result = JsonSerializer.Deserialize(stream, typeInfo);
            return Observable.Return(result);
        }
        catch (Exception ex)
        {
            return Observable.Throw<T?>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);

        try
        {
            var path = GetStatePath();
            using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);

            JsonSerializer.Serialize(stream, state, typeInfo);
            return Observables.Unit;
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection-based System.Text.Json serialization for 'object'. Prefer LoadState<T>(JsonTypeInfo<T>) for trimming/AOT.")]
    [RequiresDynamicCode("Uses reflection-based System.Text.Json serialization for 'object'. Prefer LoadState<T>(JsonTypeInfo<T>) for trimming/AOT.")]
    public IObservable<object?> LoadState()
    {
        try
        {
            var path = GetStatePath();
            using var stream = File.OpenRead(path);

            // Reflection-based: object deserialization typically requires metadata at runtime.
            var result = JsonSerializer.Deserialize<object>(stream);
            return Observable.Return(result);
        }
        catch (Exception ex)
        {
            return Observable.Throw<object?>(ex);
        }
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection-based System.Text.Json serialization for generic T. Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming/AOT.")]
    [RequiresDynamicCode("Uses reflection-based System.Text.Json serialization for generic T. Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming/AOT.")]
    public IObservable<Unit> SaveState<T>(T state)
    {
        try
        {
            var path = GetStatePath();
            using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);

            JsonSerializer.Serialize(stream, state);
            return Observables.Unit;
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<Unit> InvalidateState()
    {
        try
        {
            var path = GetStatePath();
            File.Delete(path);

            return Observables.Unit;
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }

    /// <summary>
    /// Creates (if necessary) and returns the application storage directory beneath the specified special folder.
    /// </summary>
    /// <param name="targetDir">The platform search path directory.</param>
    /// <param name="subDir">The application-specific subdirectory name.</param>
    /// <returns>The fully qualified directory path.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the platform path cannot be resolved or the bundle identifier is unavailable.
    /// </exception>
    private static string CreateAppDirectory(NSSearchPathDirectory targetDir, string subDir)
    {
        // Allocate NSFileManager only once per driver instance via Lazy. Kept local and simple.
        var fm = new NSFileManager();

        var url = fm.GetUrl(targetDir, NSSearchPathDomain.All, null, true, out _);
        if (url is null)
        {
            throw new InvalidOperationException("Unable to resolve platform application support directory.");
        }

        var bundleId = NSBundle.MainBundle?.BundleIdentifier;
        if (string.IsNullOrEmpty(bundleId))
        {
            throw new InvalidOperationException("Unable to resolve application bundle identifier.");
        }

        var basePath = url.RelativePath;
        if (string.IsNullOrEmpty(basePath))
        {
            throw new InvalidOperationException("Resolved application support path was empty.");
        }

        var path = Path.Combine(basePath, bundleId, subDir);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    /// <summary>
    /// Computes the full path to the persisted state file.
    /// </summary>
    /// <returns>The absolute file path.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetStatePath()
        => Path.Combine(_appDirectory.Value, StateFileName);
}
