// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI;

/// <summary>
/// Thread-safe registry for set-method binding converters using a lock-free snapshot pattern.
/// </summary>
/// <remarks>
/// <para>
/// This registry uses a copy-on-write snapshot pattern optimized for read-heavy workloads:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <strong>Reads:</strong> Lock-free via <see cref="Volatile.Read{T}(ref T)"/> of the snapshot reference.
/// Multiple readers can access the registry concurrently without contention.
/// </description></item>
/// <item><description>
/// <strong>Writes:</strong> Serialized under a lock. Writes clone the converter list,
/// mutate the clone, and publish a new snapshot atomically.
/// </description></item>
/// <item><description>
/// <strong>Selection:</strong> Set-method converters are stored in a simple list (no type-pair grouping).
/// When looking up a converter, each converter's runtime affinity is checked via
/// <see cref="ISetMethodBindingConverter.GetAffinityForObjects(Type?, Type?)"/>.
/// The converter with the highest affinity (&gt; 0) is selected.
/// </description></item>
/// </list>
/// <para>
/// Set-method converters are used for specialized binding operations that require custom
/// set behavior, such as populating collections or handling platform-specific controls.
/// </para>
/// </remarks>
public sealed class SetMethodBindingConverterRegistry
{
#if NET9_0_OR_GREATER
    /// <summary>
    /// Synchronization primitive guarding mutations to the registry's internal state.
    /// </summary>
    /// <remarks>
    /// Protects updates to <see cref="_snapshot"/>. Reads resolve from the snapshot without locking.
    /// </remarks>
    private readonly Lock _gate = new();
#else
    /// <summary>
    /// Synchronization primitive guarding mutations to the registry's internal state.
    /// </summary>
    /// <remarks>
    /// Protects updates to <see cref="_snapshot"/>. Reads resolve from the snapshot without locking.
    /// </remarks>
    private readonly object _gate = new();
#endif

    /// <summary>
    /// Stores all registered set-method converters.
    /// </summary>
    /// <remarks>
    /// This is a copy-on-write snapshot to allow lock-free reads:
    /// writers publish a new <see cref="Snapshot"/> instance via assignment under <see cref="_gate"/>;
    /// readers use <see cref="Volatile.Read{T}(ref T)"/> without locking.
    /// </remarks>
    private Snapshot? _snapshot;

    /// <summary>
    /// Registers a set-method binding converter.
    /// </summary>
    /// <param name="converter">The converter to register. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="converter"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// Set-method converters provide specialized behavior for binding set operations.
    /// Multiple converters can be registered; when retrieved, the converter with
    /// the highest affinity for the requested type pair will be selected.
    /// </para>
    /// <para>
    /// This method is thread-safe but serialized (only one registration can occur at a time).
    /// Reads can proceed concurrently with writes.
    /// </para>
    /// </remarks>
    public void Register(ISetMethodBindingConverter converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(converter);

        lock (_gate)
        {
            var snap = _snapshot ?? new Snapshot(new List<ISetMethodBindingConverter>(8));

            // Copy-on-write update: clone the list
            var newList = new List<ISetMethodBindingConverter>(snap.Converters) { converter };

            // Publish the new snapshot (atomic via reference assignment)
            _snapshot = new Snapshot(newList);
        }
    }

    /// <summary>
    /// Attempts to retrieve the best set-method converter for the specified type pair.
    /// </summary>
    /// <param name="fromType">The source type to convert from. May be null.</param>
    /// <param name="toType">The target type to convert to. May be null.</param>
    /// <returns>
    /// The converter with the highest affinity for the type pair, or <see langword="null"/> if no converter supports the conversion.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is lock-free and can be called concurrently from multiple threads.
    /// It queries each registered set-method converter via <see cref="ISetMethodBindingConverter.GetAffinityForObjects(Type?, Type?)"/>
    /// and returns the converter with the highest affinity (&gt; 0).
    /// </para>
    /// <para>
    /// If multiple converters return the same affinity, the last registered converter wins
    /// (implementation detail based on iteration order).
    /// </para>
    /// </remarks>
    public ISetMethodBindingConverter? TryGetConverter(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? fromType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? toType)
    {
        var snap = Volatile.Read(ref _snapshot);
        if (snap is null)
        {
            return null;
        }

        // Find the converter with the highest affinity
        ISetMethodBindingConverter? best = null;
        var bestScore = -1;

        var converters = snap.Converters;
        for (var i = 0; i < converters.Count; i++)
        {
            var converter = converters[i];
            var score = converter.GetAffinityForObjects(fromType, toType);
            if (score > bestScore && score > 0)
            {
                bestScore = score;
                best = converter;
            }
        }

        return best;
    }

    /// <summary>
    /// Returns all registered set-method converters.
    /// </summary>
    /// <returns>
    /// A sequence of all set-method converters currently registered in the registry.
    /// Returns an empty sequence if no converters are registered.
    /// </returns>
    /// <remarks>
    /// This method is lock-free and returns a snapshot of all converters at the time of the call.
    /// The returned sequence is safe to enumerate even if concurrent registrations occur.
    /// </remarks>
    public IEnumerable<ISetMethodBindingConverter> GetAllConverters()
    {
        var snap = Volatile.Read(ref _snapshot);
        if (snap is null)
        {
            return [];
        }

        // Return a copy to avoid exposing internal list
        return [.. snap.Converters];
    }

    /// <summary>
    /// A copy-on-write snapshot of the set-method converter registry.
    /// </summary>
    /// <param name="Converters">
    /// List of all registered set-method converters.
    /// </param>
    /// <remarks>
    /// This record enables lock-free reads: readers access an immutable reference to the list,
    /// while writers publish a new snapshot after applying mutations.
    /// </remarks>
    private sealed record Snapshot(List<ISetMethodBindingConverter> Converters);
}
