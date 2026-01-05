// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI;

/// <summary>
/// Thread-safe registry for typed binding converters using a lock-free snapshot pattern.
/// </summary>
/// <remarks>
/// <para>
/// This registry uses a copy-on-write snapshot pattern optimized for read-heavy workloads:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <strong>Reads:</strong> Lock-free via a volatile read of the snapshot reference.
/// Multiple readers can access the registry concurrently without contention.
/// </description></item>
/// <item><description>
/// <strong>Writes:</strong> Serialized under a lock. Writes clone the affected dictionary entry,
/// mutate the clone, and publish a new snapshot atomically.
/// </description></item>
/// <item><description>
/// <strong>Selection:</strong> Converters are grouped by (FromType, ToType) pair.
/// When multiple converters match, the one with the highest affinity (&gt; 0) is selected.
/// </description></item>
/// </list>
/// <para>
/// This design prioritizes performance for the common case: converters are registered once at
/// application startup, then looked up many times during binding operations.
/// </para>
/// </remarks>
public sealed class BindingTypeConverterRegistry
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
    /// Stores all registered converters grouped by (FromType, ToType) pair.
    /// </summary>
    /// <remarks>
    /// This is a copy-on-write snapshot to allow lock-free reads:
    /// writers publish a new <see cref="Snapshot"/> instance via assignment under <see cref="_gate"/>;
    /// readers use a volatile read without locking.
    /// </remarks>
    private Snapshot? _snapshot;

    /// <summary>
    /// Registers a typed binding converter.
    /// </summary>
    /// <param name="converter">The converter to register. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="converter"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// Converters are grouped by their (FromType, ToType) pair. Multiple converters can be
    /// registered for the same type pair; when retrieved, the converter with the highest
    /// affinity (returned by <see cref="IBindingTypeConverter.GetAffinityForObjects"/>) will be selected.
    /// </para>
    /// <para>
    /// This method is thread-safe but serialized (only one registration can occur at a time).
    /// Reads can proceed concurrently with writes.
    /// </para>
    /// </remarks>
    public void Register(IBindingTypeConverter converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(converter);

        var key = (converter.FromType, converter.ToType);

        lock (_gate)
        {
            var snap = _snapshot ?? new Snapshot(new Dictionary<(Type fromType, Type toType), List<IBindingTypeConverter>>(16));

            // Copy-on-write update: clone the dictionary shallowly
            var newDict = CloneRegistryShallow(snap.ConvertersByTypePair);

            if (!newDict.TryGetValue(key, out var list))
            {
                list = new List<IBindingTypeConverter>(4);
            }
            else
            {
                // Copy-on-write at the list level: clone before mutating
                list = [.. list];
            }

            list.Add(converter);
            newDict[key] = list;

            // Publish the new snapshot (atomic via reference assignment)
            _snapshot = new Snapshot(newDict);
        }
    }

    /// <summary>
    /// Attempts to retrieve the best converter for the specified type pair.
    /// </summary>
    /// <param name="fromType">The source type to convert from.</param>
    /// <param name="toType">The target type to convert to.</param>
    /// <returns>
    /// The converter with the highest affinity for the type pair, or <see langword="null"/> if no converter is registered.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="fromType"/> or <paramref name="toType"/> is null.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method is lock-free and can be called concurrently from multiple threads.
    /// It returns the converter with the highest affinity (&gt; 0) for the exact type pair.
    /// </para>
    /// <para>
    /// If multiple converters are registered for the same type pair, the selection is based
    /// on affinity returned by <see cref="IBindingTypeConverter.GetAffinityForObjects"/>:
    /// the converter with the highest score wins.
    /// </para>
    /// </remarks>
    public IBindingTypeConverter? TryGetConverter(Type fromType, Type toType)
    {
        ArgumentExceptionHelper.ThrowIfNull(fromType);
        ArgumentExceptionHelper.ThrowIfNull(toType);

        var snap = Volatile.Read(ref _snapshot);
        if (snap is null)
        {
            return null;
        }

        if (!snap.ConvertersByTypePair.TryGetValue((fromType, toType), out var list))
        {
            return null;
        }

        // Find the converter with the highest affinity
        IBindingTypeConverter? best = null;
        var bestScore = -1;

        for (var i = 0; i < list.Count; i++)
        {
            var converter = list[i];
            var score = converter.GetAffinityForObjects();
            if (score > bestScore && score > 0)
            {
                bestScore = score;
                best = converter;
            }
        }

        return best;
    }

    /// <summary>
    /// Returns all registered converters.
    /// </summary>
    /// <returns>
    /// A sequence of all converters currently registered in the registry.
    /// Returns an empty sequence if no converters are registered.
    /// </returns>
    /// <remarks>
    /// This method is lock-free and returns a snapshot of all converters at the time of the call.
    /// The returned sequence is safe to enumerate even if concurrent registrations occur.
    /// </remarks>
    public IEnumerable<IBindingTypeConverter> GetAllConverters()
    {
        var snap = Volatile.Read(ref _snapshot);
        if (snap is null)
        {
            return [];
        }

        // Flatten all lists into a single enumerable
        // Use a list to avoid lazy evaluation issues with concurrent modifications
        var result = new List<IBindingTypeConverter>();
        foreach (var kvp in snap.ConvertersByTypePair)
        {
            result.AddRange(kvp.Value);
        }

        return result;
    }

    /// <summary>
    /// Creates a shallow clone of a registry dictionary.
    /// </summary>
    /// <param name="source">The source dictionary to clone.</param>
    /// <returns>
    /// A new dictionary instance containing the same keys and list references as <paramref name="source"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Lists are cloned only when mutated (copy-on-write at the list level).
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Dictionary<(Type fromType, Type toType), List<IBindingTypeConverter>> CloneRegistryShallow(
        Dictionary<(Type fromType, Type toType), List<IBindingTypeConverter>> source)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);

        var clone = new Dictionary<(Type fromType, Type toType), List<IBindingTypeConverter>>(source.Count);
        foreach (var kvp in source)
        {
            clone[kvp.Key] = kvp.Value;
        }

        return clone;
    }

    /// <summary>
    /// A copy-on-write snapshot of the converter registry.
    /// </summary>
    /// <param name="ConvertersByTypePair">
    /// Dictionary of converters grouped by (FromType, ToType) pair.
    /// </param>
    /// <remarks>
    /// This record enables lock-free reads: readers access an immutable reference to the dictionary,
    /// while writers publish a new snapshot after applying mutations.
    /// </remarks>
    private sealed record Snapshot(
        Dictionary<(Type fromType, Type toType), List<IBindingTypeConverter>> ConvertersByTypePair);
}
