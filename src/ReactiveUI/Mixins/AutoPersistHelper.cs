// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ReactiveUI.Builder;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// Helper extension method class associated with the AutoPersist related functionality.
/// </summary>
public static class AutoPersistHelper
{
    /// <summary>
    /// Stores per-runtime-type persistence metadata computed via reflection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This cache is intentionally non-evicting for correctness and predictability. The number of distinct reactive
    /// object runtime types in a typical application is small and stable; MRU eviction introduces churn and can
    /// re-trigger expensive reflection.
    /// </para>
    /// <para>
    /// This cache is used only when callers use the legacy reflection-based overloads and
    /// the generic type does not match the runtime type of the instance.
    /// </para>
    /// </remarks>
#if NET8_0_OR_GREATER
    private static readonly ConditionalWeakTable<Type, PersistMetadata> PersistMetadataByType = [];
#else
    private static readonly ConditionalWeakTable<Type, PersistMetadata> PersistMetadataByType = new();
#endif

    /// <summary>
    /// Initializes static members of the <see cref="AutoPersistHelper"/> class.
    /// </summary>
    static AutoPersistHelper() => RxAppBuilder.EnsureInitialized();

    /// <summary>
    /// AutoPersist automatically calls a method whenever the object changes.
    /// </summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    public static IDisposable AutoPersist<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        T>(
        this T @this,
        Func<T, IObservable<Unit>> doPersist)
        where T : IReactiveObject
        => @this.AutoPersist(doPersist, interval: null);

    /// <summary>
    /// AutoPersist allows you to automatically call a method when an object
    /// has changed, throttling on a certain interval. Note that this object
    /// must mark its persistent properties via the [DataMember] attribute.
    /// Changes to properties not marked with DataMember will not trigger the
    /// object to be saved.
    /// </summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="interval">
    /// The interval to save the object on. Note that if an object is constantly changing,
    /// it is possible that it will never be saved.
    /// </param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <remarks>
    /// <para>
    /// This overload preserves historical behavior by reflecting over the runtime type when it differs from
    /// <typeparamref name="T"/>. This behavior is trimming/AOT-unsafe unless the application explicitly preserves the
    /// required members and attribute metadata.
    /// </para>
    /// <para>
    /// For trimming/AOT-friendly behavior, prefer the overloads that accept <see cref="AutoPersistMetadata"/>.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode(
        "AutoPersist may reflect over the runtime type when it differs from T. In trimmed/AOT builds, required property/attribute metadata " +
        "may be removed unless explicitly preserved. Prefer the overloads that accept AutoPersistMetadata to avoid runtime reflection.")]
    [RequiresDynamicCode(
        "AutoPersist may reflect over the runtime type when it differs from T. In trimmed/AOT builds, required property/attribute metadata " +
        "may be removed unless explicitly preserved. Prefer the overloads that accept AutoPersistMetadata to avoid runtime reflection.")]
    public static IDisposable AutoPersist<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        T>(
        this T @this,
        Func<T, IObservable<Unit>> doPersist,
        TimeSpan? interval)
        where T : IReactiveObject
        => @this.AutoPersist(doPersist, NeverObservable<Unit>.Instance, interval);

    /// <summary>
    /// AutoPersist automatically calls a method whenever the object changes or a manual save is signalled.
    /// </summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <typeparam name="TDontCare">The save signal type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    public static IDisposable AutoPersist<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        T,
        TDontCare>(
        this T @this,
        Func<T, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal)
        where T : IReactiveObject
        => @this.AutoPersist(doPersist, manualSaveSignal, interval: null);

    /// <summary>
    /// AutoPersist allows you to automatically call a method when an object
    /// has changed, throttling on a certain interval. Note that this object
    /// must mark its persistent properties via the [DataMember] attribute.
    /// Changes to properties not marked with DataMember will not trigger the
    /// object to be saved.
    /// </summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <typeparam name="TDontCare">The save signal type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">
    /// When invoked, the object will be saved regardless of whether it has changed.
    /// </param>
    /// <param name="interval">
    /// The interval to save the object on. Note that if an object is constantly changing,
    /// it is possible that it will never be saved.
    /// </param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <exception cref="ArgumentException">Thrown when the object is not annotated with <c>[DataContract]</c>.</exception>
    /// <remarks>
    /// <para>
    /// This overload preserves historical behavior by reflecting over the runtime type when it differs from
    /// <typeparamref name="T"/>. This behavior is trimming/AOT-unsafe unless the application explicitly preserves the
    /// required members and attribute metadata.
    /// </para>
    /// <para>
    /// For trimming/AOT-friendly behavior, prefer the overloads that accept <see cref="AutoPersistMetadata"/>.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode(
        "AutoPersist may reflect over the runtime type when it differs from T. In trimmed/AOT builds, required property/attribute metadata " +
        "may be removed unless explicitly preserved. Prefer the overloads that accept AutoPersistMetadata to avoid runtime reflection.")]
    [RequiresDynamicCode(
        "AutoPersist may reflect over the runtime type when it differs from T. In trimmed/AOT builds, required property/attribute metadata " +
        "may be removed unless explicitly preserved. Prefer the overloads that accept AutoPersistMetadata to avoid runtime reflection.")]
    public static IDisposable AutoPersist<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        T,
        TDontCare>(
        this T @this,
        Func<T, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        TimeSpan? interval)
        where T : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(doPersist);
        ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);

        interval ??= TimeSpan.FromSeconds(3.0);

        var runtimeType = @this.GetType();
        var metadata = runtimeType == typeof(T)
            ? PersistMetadataHolder<T>.Metadata
            : GetMetadataForUnknownRuntimeType(runtimeType);

        if (!metadata.HasDataContract)
        {
            throw new ArgumentException("AutoPersist can only be applied to objects with [DataContract]");
        }

        var persistablePropertyNames = metadata.PersistablePropertyNames;

        var ret = new OnceDisposable();
        RxSchedulers.MainThreadScheduler.Schedule(() =>
        {
            if (ret.IsDisposed)
            {
                return;
            }

            ret.Disposable = new AutoPersistDriver<T, TDontCare>(
                @this,
                doPersist,
                persistablePropertyNames,
                manualSaveSignal,
                interval.Value,
                RxSchedulers.TaskpoolScheduler);
        });

        return ret;
    }

    /// <summary>
    /// AutoPersist overload that uses explicit metadata and performs no runtime reflection.
    /// </summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="metadata">The persistence metadata to use for determining persistable properties.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    public static IDisposable AutoPersist<T>(
        this T @this,
        Func<T, IObservable<Unit>> doPersist,
        AutoPersistMetadata metadata)
        where T : IReactiveObject
        => @this.AutoPersist(doPersist, metadata, interval: null);

    /// <summary>
    /// AutoPersist overload that performs no runtime reflection and is suitable for trimming/AOT scenarios.
    /// </summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="metadata">The persistence metadata to use for determining persistable properties.</param>
    /// <param name="interval">The interval to save the object on.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="metadata"/> indicates the object is not persistable.</exception>
    public static IDisposable AutoPersist<T>(
        this T @this,
        Func<T, IObservable<Unit>> doPersist,
        AutoPersistMetadata metadata,
        TimeSpan? interval)
        where T : IReactiveObject
        => @this.AutoPersist(doPersist, NeverObservable<Unit>.Instance, metadata, interval);

    /// <summary>
    /// AutoPersist overload that uses explicit metadata and a manual save signal, performing no runtime reflection.
    /// </summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <typeparam name="TDontCare">The save signal type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <param name="metadata">The persistence metadata to use for determining persistable properties.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    public static IDisposable AutoPersist<T, TDontCare>(
        this T @this,
        Func<T, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        AutoPersistMetadata metadata)
        where T : IReactiveObject
        => @this.AutoPersist(doPersist, manualSaveSignal, metadata, interval: null);

    /// <summary>
    /// AutoPersist overload that performs no runtime reflection and is suitable for trimming/AOT scenarios.
    /// </summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <typeparam name="TDontCare">The save signal type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">
    /// When invoked, the object will be saved regardless of whether it has changed.
    /// </param>
    /// <param name="metadata">The persistence metadata to use for determining persistable properties.</param>
    /// <param name="interval">
    /// The interval to save the object on. Note that if an object is constantly changing,
    /// it is possible that it will never be saved.
    /// </param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="metadata"/> indicates the object is not persistable.</exception>
    public static IDisposable AutoPersist<T, TDontCare>(
        this T @this,
        Func<T, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        AutoPersistMetadata metadata,
        TimeSpan? interval)
        where T : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(doPersist);
        ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);
        ArgumentExceptionHelper.ThrowIfNull(metadata);

        if (!metadata.HasDataContract)
        {
            throw new ArgumentException(
                "AutoPersist can only be applied to objects with [DataContract]",
                nameof(metadata));
        }

        interval ??= TimeSpan.FromSeconds(3.0);

        var persistablePropertyNames = metadata.PersistablePropertyNames;

        var ret = new OnceDisposable();
        RxSchedulers.MainThreadScheduler.Schedule(() =>
        {
            if (ret.IsDisposed)
            {
                return;
            }

            ret.Disposable = new AutoPersistDriver<T, TDontCare>(
                @this,
                doPersist,
                persistablePropertyNames,
                manualSaveSignal,
                interval.Value,
                RxSchedulers.TaskpoolScheduler);
        });

        return ret;
    }

    /// <summary>
    /// Applies AutoPersistence to all objects in a collection using explicit persistence metadata.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    public static IDisposable AutoPersistCollection<TItem>(
        this ObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        AutoPersistMetadata metadata)
        where TItem : IReactiveObject
        => @this.AutoPersistCollection(doPersist, metadata, interval: null);

    /// <summary>
    /// Apply AutoPersistence to all objects in a collection using explicit persistence metadata.
    /// This overload performs no runtime reflection and is suitable for trimming/AOT scenarios.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
    /// <param name="interval">
    /// The interval to save the object on. Note that if an object is constantly changing,
    /// it is possible that it will never be saved.
    /// </param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="metadata"/> indicates the object is not annotated with <c>[DataContract]</c>.
    /// </exception>
    public static IDisposable AutoPersistCollection<TItem>(
        this ObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        AutoPersistMetadata metadata,
        TimeSpan? interval)
        where TItem : IReactiveObject
        => AutoPersistCollection(@this, doPersist, NeverObservable<Unit>.Instance, metadata, interval);

    /// <summary>
    /// Applies AutoPersistence to all objects in a collection using explicit persistence metadata.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    public static IDisposable AutoPersistCollection<TItem, TDontCare>(
        this ObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        AutoPersistMetadata metadata)
        where TItem : IReactiveObject
        => @this.AutoPersistCollection(doPersist, manualSaveSignal, metadata, interval: null);

    /// <summary>
    /// Apply AutoPersistence to all objects in a collection using explicit persistence metadata.
    /// This overload performs no runtime reflection and is suitable for trimming/AOT scenarios.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
    /// <param name="interval">The interval to save the object on.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="metadata"/> indicates the object is not annotated with <c>[DataContract]</c>.
    /// </exception>
    public static IDisposable AutoPersistCollection<TItem, TDontCare>(
        this ObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        AutoPersistMetadata metadata,
        TimeSpan? interval)
        where TItem : IReactiveObject
        => AutoPersistCollection<TItem, ObservableCollection<TItem>, TDontCare>(
            @this,
            doPersist,
            manualSaveSignal,
            metadata,
            interval);

    /// <summary>
    /// Applies AutoPersistence to all objects in a collection using explicit persistence metadata.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    public static IDisposable AutoPersistCollection<TItem, TCollection, TDontCare>(
        this TCollection @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        AutoPersistMetadata metadata)
        where TItem : IReactiveObject
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
        => @this.AutoPersistCollection(doPersist, manualSaveSignal, metadata, interval: null);

    /// <summary>
    /// Apply AutoPersistence to all objects in a collection using explicit persistence metadata.
    /// This overload performs no runtime reflection and is suitable for trimming/AOT scenarios.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
    /// <param name="interval">The interval to save the object on.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="metadata"/> indicates the object is not annotated with <c>[DataContract]</c>.
    /// </exception>
    public static IDisposable AutoPersistCollection<TItem, TCollection, TDontCare>(
        this TCollection @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        AutoPersistMetadata metadata,
        TimeSpan? interval)
        where TItem : IReactiveObject
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(doPersist);
        ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);
        ArgumentExceptionHelper.ThrowIfNull(metadata);

        if (!metadata.HasDataContract)
        {
            throw new ArgumentException(
                "AutoPersist can only be applied to objects with [DataContract]",
                nameof(metadata));
        }

        Dictionary<TItem, IDisposable> disposerList = [];

        var subscription = @this.ActOnEveryObject<TItem, TCollection>(
            x =>
            {
                if (disposerList.ContainsKey(x))
                {
                    return;
                }

                disposerList[x] = x.AutoPersist(doPersist, manualSaveSignal, metadata, interval);
            },
            x =>
            {
                if (!disposerList.TryGetValue(x, out var d))
                {
                    return;
                }

                d.Dispose();
                disposerList.Remove(x);
            });

        return new ActionDisposable(() =>
        {
            subscription.Dispose();

            foreach (var kvp in disposerList)
            {
                kvp.Value.Dispose();
            }

            disposerList.Clear();
        });
    }

    /// <summary>
    /// Applies AutoPersistence to all objects in a read-only collection using explicit persistence metadata.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    public static IDisposable AutoPersistCollection<TItem, TDontCare>(
        this ReadOnlyObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        AutoPersistMetadata metadata)
        where TItem : IReactiveObject
        => @this.AutoPersistCollection(doPersist, manualSaveSignal, metadata, interval: null);

    /// <summary>
    /// Apply AutoPersistence to all objects in a read-only collection using explicit persistence metadata.
    /// This overload performs no runtime reflection and is suitable for trimming/AOT scenarios.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
    /// <param name="interval">The interval to save the object on.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="metadata"/> indicates the object is not annotated with <c>[DataContract]</c>.
    /// </exception>
    public static IDisposable AutoPersistCollection<TItem, TDontCare>(
        this ReadOnlyObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        AutoPersistMetadata metadata,
        TimeSpan? interval)
        where TItem : IReactiveObject
        => AutoPersistCollection<TItem, ReadOnlyObservableCollection<TItem>, TDontCare>(
            @this,
            doPersist,
            manualSaveSignal,
            metadata,
            interval);

    /// <summary>
    /// Applies AutoPersistence to all objects in a collection using a metadata provider.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <param name="metadataProvider">A function that returns the persistence metadata to use for a specific item instance.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    public static IDisposable AutoPersistCollection<TItem, TCollection, TDontCare>(
        this TCollection @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        Func<TItem, AutoPersistMetadata> metadataProvider)
        where TItem : IReactiveObject
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
        => @this.AutoPersistCollection(doPersist, manualSaveSignal, metadataProvider, interval: null);

    /// <summary>
    /// Apply AutoPersistence to all objects in a collection using a metadata provider.
    /// This overload performs no runtime reflection and is suitable for trimming/AOT scenarios,
    /// including polymorphic collections.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <param name="metadataProvider">
    /// A function that returns the persistence metadata to use for the specific item instance.
    /// </param>
    /// <param name="interval">The interval to save the object on.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="metadataProvider"/> is <see langword="null"/>.
    /// </exception>
    public static IDisposable AutoPersistCollection<TItem, TCollection, TDontCare>(
        this TCollection @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        Func<TItem, AutoPersistMetadata> metadataProvider,
        TimeSpan? interval)
        where TItem : IReactiveObject
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(doPersist);
        ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);
        ArgumentExceptionHelper.ThrowIfNull(metadataProvider);

        Dictionary<TItem, IDisposable> disposerList = [];

        var subscription = @this.ActOnEveryObject<TItem, TCollection>(
            x =>
            {
                if (disposerList.ContainsKey(x))
                {
                    return;
                }

                var metadata = metadataProvider(x);
                disposerList[x] = x.AutoPersist(doPersist, manualSaveSignal, metadata, interval);
            },
            x =>
            {
                if (!disposerList.TryGetValue(x, out var d))
                {
                    return;
                }

                d.Dispose();
                disposerList.Remove(x);
            });

        return new ActionDisposable(() =>
        {
            subscription.Dispose();

            foreach (var kvp in disposerList)
            {
                kvp.Value.Dispose();
            }

            disposerList.Clear();
        });
    }

    /// <summary>
    /// Applies AutoPersistence to all objects in a collection.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    public static IDisposable AutoPersistCollection<TItem>(
        this ObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist)
        where TItem : IReactiveObject
        => @this.AutoPersistCollection(doPersist, interval: null);

    /// <summary>
    /// Apply AutoPersistence to all objects in a collection. Items that are
    /// no longer in the collection won't be persisted anymore.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="interval">
    /// The interval to save the object on. Note that if an object is constantly changing,
    /// it is possible that it will never be saved.
    /// </param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <remarks>
    /// This overload preserves historical behavior by delegating to the reflection-based AutoPersist pipeline for each item.
    /// In trimming/AOT scenarios, required property/attribute metadata may be removed unless explicitly preserved.
    /// Prefer the overloads that accept <see cref="AutoPersistMetadata"/> (or a metadata provider) to avoid runtime reflection.
    /// </remarks>
    [RequiresUnreferencedCode(
        "AutoPersistCollection may reflect over runtime item types via AutoPersist when generic type parameters do not match item runtime types. " +
        "In trimmed/AOT builds, required property/attribute metadata may be removed unless explicitly preserved. " +
        "Prefer the overloads that accept AutoPersistMetadata or a metadata provider to avoid runtime reflection.")]
    [RequiresDynamicCode(
        "AutoPersistCollection may reflect over runtime item types via AutoPersist when generic type parameters do not match item runtime types. " +
        "In trimmed/AOT builds, required property/attribute metadata may be removed unless explicitly preserved. " +
        "Prefer the overloads that accept AutoPersistMetadata or a metadata provider to avoid runtime reflection.")]
    public static IDisposable AutoPersistCollection<TItem>(
        this ObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        TimeSpan? interval)
        where TItem : IReactiveObject
        => AutoPersistCollection(@this, doPersist, NeverObservable<Unit>.Instance, interval);

    /// <summary>
    /// Applies AutoPersistence to all objects in a collection.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TDontCare">The return signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    public static IDisposable AutoPersistCollection<TItem, TDontCare>(
        this ObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal)
        where TItem : IReactiveObject
        => @this.AutoPersistCollection(doPersist, manualSaveSignal, interval: null);

    /// <summary>
    /// Apply AutoPersistence to all objects in a collection. Items that are
    /// no longer in the collection won't be persisted anymore.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TDontCare">The return signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">
    /// When invoked, the object will be saved regardless of whether it has changed.
    /// </param>
    /// <param name="interval">
    /// The interval to save the object on. Note that if an object is constantly changing,
    /// it is possible that it will never be saved.
    /// </param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <remarks>
    /// This overload preserves historical behavior by delegating to the reflection-based AutoPersist pipeline for each item.
    /// In trimming/AOT scenarios, required property/attribute metadata may be removed unless explicitly preserved.
    /// Prefer the overloads that accept <see cref="AutoPersistMetadata"/> (or a metadata provider) to avoid runtime reflection.
    /// </remarks>
    [RequiresUnreferencedCode(
        "AutoPersistCollection may reflect over runtime item types via AutoPersist when generic type parameters do not match item runtime types. " +
        "In trimmed/AOT builds, required property/attribute metadata may be removed unless explicitly preserved. " +
        "Prefer the overloads that accept AutoPersistMetadata or a metadata provider to avoid runtime reflection.")]
    [RequiresDynamicCode(
        "AutoPersistCollection may reflect over runtime item types via AutoPersist when generic type parameters do not match item runtime types. " +
        "In trimmed/AOT builds, required property/attribute metadata may be removed unless explicitly preserved. " +
        "Prefer the overloads that accept AutoPersistMetadata or a metadata provider to avoid runtime reflection.")]
    public static IDisposable AutoPersistCollection<TItem, TDontCare>(
        this ObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        TimeSpan? interval)
        where TItem : IReactiveObject
        => AutoPersistCollection<TItem, ObservableCollection<TItem>, TDontCare>(
            @this,
            doPersist,
            manualSaveSignal,
            interval);

    /// <summary>
    /// Applies AutoPersistence to all objects in a read-only collection.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TDontCare">The signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    public static IDisposable AutoPersistCollection<TItem, TDontCare>(
        this ReadOnlyObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal)
        where TItem : IReactiveObject
        => @this.AutoPersistCollection(doPersist, manualSaveSignal, interval: null);

    /// <summary>
    /// Apply AutoPersistence to all objects in a collection. Items that are
    /// no longer in the collection won't be persisted anymore.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TDontCare">The signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">
    /// When invoked, the object will be saved regardless of whether it has changed.
    /// </param>
    /// <param name="interval">
    /// The interval to save the object on. Note that if an object is constantly changing,
    /// it is possible that it will never be saved.
    /// </param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    /// <remarks>
    /// This overload preserves historical behavior by delegating to the reflection-based AutoPersist pipeline for each item.
    /// In trimming/AOT scenarios, required property/attribute metadata may be removed unless explicitly preserved.
    /// Prefer the overloads that accept <see cref="AutoPersistMetadata"/> (or a metadata provider) to avoid runtime reflection.
    /// </remarks>
    [RequiresUnreferencedCode(
        "AutoPersistCollection may reflect over runtime item types via AutoPersist when generic type parameters do not match item runtime types. " +
        "In trimmed/AOT builds, required property/attribute metadata may be removed unless explicitly preserved. " +
        "Prefer the overloads that accept AutoPersistMetadata or a metadata provider to avoid runtime reflection.")]
    [RequiresDynamicCode(
        "AutoPersistCollection may reflect over runtime item types via AutoPersist when generic type parameters do not match item runtime types. " +
        "In trimmed/AOT builds, required property/attribute metadata may be removed unless explicitly preserved. " +
        "Prefer the overloads that accept AutoPersistMetadata or a metadata provider to avoid runtime reflection.")]
    public static IDisposable AutoPersistCollection<TItem, TDontCare>(
        this ReadOnlyObservableCollection<TItem> @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        TimeSpan? interval)
        where TItem : IReactiveObject
        => AutoPersistCollection<TItem, ReadOnlyObservableCollection<TItem>, TDontCare>(
            @this,
            doPersist,
            manualSaveSignal,
            interval);

    /// <summary>
    /// Applies AutoPersistence to all objects in a collection.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TDontCare">The signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
    public static IDisposable AutoPersistCollection<TItem, TCollection, TDontCare>(
        this TCollection @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal)
        where TItem : IReactiveObject
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
        => @this.AutoPersistCollection(doPersist, manualSaveSignal, interval: null);

    /// <summary>
    /// Apply AutoPersistence to all objects in a collection. Items that are
    /// no longer in the collection won't be persisted anymore.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TDontCare">The signal type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
    /// <param name="manualSaveSignal">
    /// When invoked, the object will be saved regardless of whether it has changed.
    /// </param>
    /// <param name="interval">
    /// The interval to save the object on. Note that if an object is constantly changing,
    /// it is possible that it will never be saved.
    /// </param>
    /// <returns>A disposable to disable automatic persistence.</returns>
    [RequiresUnreferencedCode(
        "AutoPersistCollection may reflect over runtime item types via AutoPersist when generic type parameters do not match item runtime types. " +
        "In trimmed/AOT builds, required property/attribute metadata may be removed unless explicitly preserved. " +
        "Prefer the overloads that accept AutoPersistMetadata or a metadata provider to avoid runtime reflection.")]
    [RequiresDynamicCode(
        "AutoPersistCollection may reflect over runtime item types via AutoPersist when generic type parameters do not match item runtime types. " +
        "In trimmed/AOT builds, required property/attribute metadata may be removed unless explicitly preserved. " +
        "Prefer the overloads that accept AutoPersistMetadata or a metadata provider to avoid runtime reflection.")]
    public static IDisposable AutoPersistCollection<TItem, TCollection, TDontCare>(
        this TCollection @this,
        Func<TItem, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        TimeSpan? interval)
        where TItem : IReactiveObject
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(doPersist);
        ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);

        Dictionary<TItem, IDisposable> disposerList = [];

        var subscription = @this.ActOnEveryObject<TItem, TCollection>(
            x =>
            {
                if (disposerList.TryGetValue(x, out _))
                {
                    return;
                }

                disposerList[x] = x.AutoPersist(doPersist, manualSaveSignal, interval);
            },
            x =>
            {
                if (!disposerList.TryGetValue(x, out var d))
                {
                    return;
                }

                d.Dispose();
                disposerList.Remove(x);
            });

        return new ActionDisposable(() =>
        {
            subscription.Dispose();

            foreach (var kvp in disposerList)
            {
                kvp.Value.Dispose();
            }

            disposerList.Clear();
        });
    }

    /// <summary>
    /// Creates a metadata provider for homogeneous collections where <typeparamref name="TItem"/> is the concrete runtime type.
    /// This helper performs no runtime reflection and is suitable for trimming/AOT scenarios.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <returns>A function returning metadata for <typeparamref name="TItem"/>.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public static Func<TItem, AutoPersistMetadata> CreateMetadataProvider<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        TItem>()
        where TItem : IReactiveObject
    {
        var metadata = CreateMetadata<TItem>();
        return _ => metadata;
    }

    /// <summary>
    /// Creates trimming/AOT-friendly persistence metadata for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type to analyze for <c>[DataContract]</c> and <c>[DataMember]</c>.
    /// </typeparam>
    /// <returns>The computed persistence metadata.</returns>
    /// <remarks>
    /// This method is analyzable by the trimmer due to the
    /// <see cref="DynamicallyAccessedMembersAttribute"/> on <typeparamref name="T"/>
    /// and uses no runtime type discovery.
    /// </remarks>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public static AutoPersistMetadata CreateMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        T>()
        where T : IReactiveObject
        => PersistMetadataHolder<T>.Metadata.Public;

    /// <summary>
    /// Call methods <paramref name="onAdd"/> and <paramref name="onRemove"/> whenever an object is added or
    /// removed from a collection. This method correctly handles both when
    /// a collection is initialized, as well as when the collection is Reset.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="onAdd">A method to be called when an object is added to the collection.</param>
    /// <param name="onRemove">A method to be called when an object is removed from the collection.</param>
    /// <returns>A disposable that deactivates this behavior.</returns>
    public static IDisposable ActOnEveryObject<TItem>(
        this ObservableCollection<TItem> @this,
        Action<TItem> onAdd,
        Action<TItem> onRemove)
        where TItem : IReactiveObject
        => ActOnEveryObject<TItem, ObservableCollection<TItem>>(@this, onAdd, onRemove);

    /// <summary>
    /// Call methods <paramref name="onAdd"/> and <paramref name="onRemove"/> whenever an object is added or
    /// removed from a collection. This method correctly handles both when
    /// a collection is initialized, as well as when the collection is Reset.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    /// <param name="onAdd">A method to be called when an object is added to the collection.</param>
    /// <param name="onRemove">A method to be called when an object is removed from the collection.</param>
    /// <returns>A disposable that deactivates this behavior.</returns>
    public static IDisposable ActOnEveryObject<TItem>(
        this ReadOnlyObservableCollection<TItem> @this,
        Action<TItem> onAdd,
        Action<TItem> onRemove)
        where TItem : IReactiveObject
        => ActOnEveryObject<TItem, ReadOnlyObservableCollection<TItem>>(@this, onAdd, onRemove);

    /// <summary>
    /// Call methods <paramref name="onAdd"/> and <paramref name="onRemove"/> whenever an object is added or
    /// removed from a collection. This method correctly handles both when
    /// a collection is initialized, as well as when the collection is Reset.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <param name="collection">The reactive collection to watch for changes.</param>
    /// <param name="onAdd">A method to be called when an object is added to the collection.</param>
    /// <param name="onRemove">A method to be called when an object is removed from the collection.</param>
    /// <returns>A disposable that deactivates this behavior.</returns>
    public static IDisposable ActOnEveryObject<TItem, TCollection>(
        this TCollection collection,
        Action<TItem> onAdd,
        Action<TItem> onRemove)
        where TItem : IReactiveObject
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
    {
        ArgumentExceptionHelper.ThrowIfNull(onAdd);
        ArgumentExceptionHelper.ThrowIfNull(onRemove);
        ArgumentExceptionHelper.ThrowIfNull(collection);

        var changedDisposable =
            ActOnEveryObject(collection.ToReactiveChangeSet<TCollection, TItem>(), onAdd, onRemove);

        return new ActionDisposable(() =>
        {
            changedDisposable.Dispose();

            foreach (var v in collection)
            {
                onRemove(v);
            }
        });
    }

    /// <summary>
    /// Call methods <paramref name="onAdd"/> and <paramref name="onRemove"/> whenever an object is added or
    /// removed from a collection. This method correctly handles both when
    /// a collection is initialized, as well as when the collection is Reset.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The observable change set to watch for changes.</param>
    /// <param name="onAdd">A method to be called when an object is added to the collection.</param>
    /// <param name="onRemove">A method to be called when an object is removed from the collection.</param>
    /// <returns>A disposable that deactivates this behavior.</returns>
    public static IDisposable ActOnEveryObject<TItem>(
        this IObservable<IReactiveChangeSet<TItem>> @this,
        Action<TItem> onAdd,
        Action<TItem> onRemove)
        where TItem : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(onAdd);
        ArgumentExceptionHelper.ThrowIfNull(onRemove);

        return @this.Subscribe(new DelegateObserver<IReactiveChangeSet<TItem>>(changeSet =>
        {
            foreach (var change in changeSet)
            {
                ApplyChange(change, onAdd, onRemove);
            }
        }));
    }

    /// <summary>
    /// Applies a single change-set entry by invoking the add or remove callback for the affected items.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="change">The change to apply.</param>
    /// <param name="onAdd">The callback invoked for added items.</param>
    /// <param name="onRemove">The callback invoked for removed items.</param>
    private static void ApplyChange<TItem>(ReactiveChange<TItem> change, Action<TItem> onAdd, Action<TItem> onRemove)
        where TItem : IReactiveObject
    {
        switch (change.Reason)
        {
            case ReactiveChangeReason.Add:
            {
                onAdd(change.Current);
                break;
            }

            case ReactiveChangeReason.Remove:
            {
                onRemove(change.Current);
                break;
            }

            case ReactiveChangeReason.Replace:
            {
                if (change.Previous is { } previous)
                {
                    onRemove(previous);
                }

                onAdd(change.Current);
                break;
            }

            case ReactiveChangeReason.Refresh:
            {
                onRemove(change.Current);
                onAdd(change.Current);
                break;
            }
        }
    }

    /// <summary>
    /// Gets metadata for a runtime type that is not statically known to the trimmer.
    /// </summary>
    /// <param name="runtimeType">The runtime type.</param>
    /// <returns>The computed persistence metadata.</returns>
    /// <remarks>
    /// This path is trimming/AOT unsafe unless the application explicitly preserves the required members
    /// (properties and related attribute metadata) for <paramref name="runtimeType"/>.
    /// </remarks>
    [RequiresUnreferencedCode(
        "AutoPersist reflects over the runtime type. In trimmed/AOT builds, required property/attribute metadata may be removed " +
        "unless explicitly preserved. Prefer CreateMetadata<T>() and the overloads that accept AutoPersistMetadata.")]
    [RequiresDynamicCode(
        "AutoPersist reflects over the runtime type. In trimmed/AOT builds, required property/attribute metadata may be removed " +
        "unless explicitly preserved. Prefer CreateMetadata<T>() and the overloads that accept AutoPersistMetadata.")]
    private static PersistMetadata GetMetadataForUnknownRuntimeType(Type runtimeType)
        => PersistMetadataByType.GetValue(runtimeType, static t => PersistMetadata.Create(t));

    /// <summary>
    /// Public-facing persistence metadata for AutoPersist.
    /// </summary>
    /// <remarks>
    /// This type exists so callers can provide persistence metadata explicitly in trimming/AOT scenarios,
    /// avoiding runtime reflection over unknown types.
    /// </remarks>
    public sealed class AutoPersistMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoPersistMetadata"/> class.
        /// </summary>
        /// <param name="hasDataContract">Whether the type is annotated with <c>[DataContract]</c>.</param>
        /// <param name="persistablePropertyNames">The set of property names annotated with <c>[DataMember]</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="persistablePropertyNames"/> is <see langword="null"/>.
        /// </exception>
        public AutoPersistMetadata(bool hasDataContract, ISet<string> persistablePropertyNames)
        {
            ArgumentExceptionHelper.ThrowIfNull(persistablePropertyNames);

            HasDataContract = hasDataContract;
            PersistablePropertyNames = persistablePropertyNames;
        }

        /// <summary>
        /// Gets a value indicating whether the target type is annotated with <c>[DataContract]</c>.
        /// </summary>
        public bool HasDataContract { get; }

        /// <summary>
        /// Gets the set of property names annotated with <c>[DataMember]</c>.
        /// </summary>
        public ISet<string> PersistablePropertyNames { get; }
    }

    /// <summary>
    /// Holds precomputed metadata for a closed generic <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type for which persistence metadata is computed.
    /// </typeparam>
    private static class PersistMetadataHolder<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        T>
        where T : IReactiveObject
    {
        /// <summary>
        /// Gets the computed persistence metadata for <typeparamref name="T"/>.
        /// </summary>
        internal static readonly PersistMetadata Metadata = PersistMetadata.Create(typeof(T));
    }

    /// <summary>
    /// Immutable persistence metadata for a given type.
    /// </summary>
    private sealed record PersistMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistMetadata"/> class.
        /// </summary>
        /// <param name="hasDataContract">Whether the type is annotated with <c>[DataContract]</c>.</param>
        /// <param name="persistablePropertyNames">The set of property names annotated with <c>[DataMember]</c>.</param>
        private PersistMetadata(bool hasDataContract, HashSet<string> persistablePropertyNames)
        {
            HasDataContract = hasDataContract;
            PersistablePropertyNames = persistablePropertyNames;
            Public = new(hasDataContract, persistablePropertyNames);
        }

        /// <summary>
        /// Gets a value indicating whether the target type is annotated with <c>[DataContract]</c>.
        /// </summary>
        internal bool HasDataContract { get; }

        /// <summary>
        /// Gets the set of property names annotated with <c>[DataMember]</c>.
        /// </summary>
        internal HashSet<string> PersistablePropertyNames { get; }

        /// <summary>
        /// Gets a public metadata wrapper for callers.
        /// </summary>
        internal AutoPersistMetadata Public { get; }

        /// <summary>
        /// Creates persistence metadata for a statically-known or explicitly-preserved type.
        /// </summary>
        /// <param name="type">The type to analyze.</param>
        /// <returns>The computed persistence metadata.</returns>
        [SuppressMessage(
            "Security Hotspot",
            "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
            Justification = "AutoPersist inspects non-public [DataMember] properties to mirror DataContract serialization semantics.")]
        internal static PersistMetadata Create(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                        DynamicallyAccessedMemberTypes.NonPublicProperties)]
            Type type)
        {
            var hasDataContract = type.GetCustomAttributes(typeof(DataContractAttribute), true).Length > 0;

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                                BindingFlags.DeclaredOnly);

            HashSet<string>? set = null;

            for (var i = 0; i < properties.Length; i++)
            {
                var p = properties[i];
                if (!HasDataMemberAttribute(p))
                {
                    continue;
                }

                set ??= new(StringComparer.Ordinal);
                set.Add(p.Name);
            }

            set ??= new(StringComparer.Ordinal);
            return new(hasDataContract, set);
        }

        /// <summary>
        /// Determines whether a property is annotated with <c>[DataMember]</c>.
        /// </summary>
        /// <param name="property">The property to inspect.</param>
        /// <returns><see langword="true"/> if the property is annotated; otherwise <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasDataMemberAttribute(PropertyInfo property) => property.IsDefined(typeof(DataMemberAttribute), true);
    }

    /// <summary>
    /// Watches a reactive object for persistable property changes and manual save signals, debounces them by a fixed
    /// interval, and runs the persist operation. A single allocation-light sink with no intermediate observable
    /// operators (replaces the prior <c>Where</c>/<c>Select</c>/<c>Merge</c>/<c>Throttle</c>/<c>SelectMany</c>/<c>Publish</c> chain).
    /// </summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
    private sealed class AutoPersistDriver<T, TDontCare> : IDisposable
        where T : IReactiveObject
    {
        /// <summary>Guards the timer, the persist subscription, and the disposed flag.</summary>
        #if NET9_0_OR_GREATER
        private readonly Lock _gate = new();
        #else
        private readonly object _gate = new();
        #endif

        /// <summary>The object being persisted.</summary>
        private readonly T _target;

        /// <summary>Runs the persist operation for the target.</summary>
        private readonly Func<T, IObservable<Unit>> _doPersist;

        /// <summary>The property names whose changes trigger a save.</summary>
        private readonly ISet<string> _persistableNames;

        /// <summary>The quiet interval after which a save runs (debounce).</summary>
        private readonly TimeSpan _interval;

        /// <summary>The scheduler on which the debounce interval is measured and the save runs.</summary>
        private readonly IScheduler _scheduler;

        /// <summary>Holds the pending debounced save; assigning a new one cancels the prior pending save (debounce).</summary>
        private readonly SwapDisposable _debounce = new();

        /// <summary>The subscription to the target's property-change stream.</summary>
        private readonly IDisposable _changeSubscription;

        /// <summary>The subscription to the manual save signal.</summary>
        private readonly IDisposable _manualSubscription;

        /// <summary>The current in-flight persist subscription, if any.</summary>
        private IDisposable? _persistSubscription;

        /// <summary>Whether the driver has been disposed.</summary>
        private bool _disposed;

        /// <summary>Initializes a new instance of the <see cref="AutoPersistDriver{T, TDontCare}"/> class and starts watching.</summary>
        /// <param name="target">The object being persisted.</param>
        /// <param name="doPersist">Runs the persist operation for the target.</param>
        /// <param name="persistableNames">The property names whose changes trigger a save.</param>
        /// <param name="manualSaveSignal">A signal that forces a save regardless of changes.</param>
        /// <param name="interval">The quiet interval after which a save runs.</param>
        /// <param name="scheduler">The scheduler on which the debounce interval is measured and the save runs.</param>
        public AutoPersistDriver(
            T target,
            Func<T, IObservable<Unit>> doPersist,
            ISet<string> persistableNames,
            IObservable<TDontCare> manualSaveSignal,
            TimeSpan interval,
            IScheduler scheduler)
        {
            _target = target;
            _doPersist = doPersist;
            _persistableNames = persistableNames;
            _interval = interval;
            _scheduler = scheduler;
            _changeSubscription = target.GetChangedObservable()
                .Subscribe(new DelegateObserver<IReactivePropertyChangedEventArgs<T>>(OnPropertyChanged));
            _manualSubscription = manualSaveSignal
                .Subscribe(new DelegateObserver<TDontCare>(_ => RequestSave()));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
            }

            _changeSubscription.Dispose();
            _manualSubscription.Dispose();
            _debounce.Dispose();
            _persistSubscription?.Dispose();
        }

        /// <summary>Requests a save when a persistable property changes.</summary>
        /// <param name="args">The property-change arguments.</param>
        private void OnPropertyChanged(IReactivePropertyChangedEventArgs<T> args)
        {
            if (args.PropertyName is null || !_persistableNames.Contains(args.PropertyName))
            {
                return;
            }

            RequestSave();
        }

        /// <summary>(Re)starts the debounce timer so a save runs after the quiet interval.</summary>
        private void RequestSave()
        {
            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }

                // Debounce: assigning a new scheduled save cancels (disposes) any prior pending one. A single reused
                // SwapDisposable slot keeps this allocation-light versus an operator pipeline.
                _debounce.Disposable = _scheduler.Schedule(_interval, RunPersist);
            }
        }

        /// <summary>Runs the persist operation when the debounce interval elapses.</summary>
        private void RunPersist()
        {
            IObservable<Unit> persist;
            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }

                persist = _doPersist(_target);
            }

            var subscription = persist.Subscribe(new DelegateObserver<Unit>(static _ => { }));
            lock (_gate)
            {
                if (_disposed)
                {
                    subscription.Dispose();
                    return;
                }

                _persistSubscription?.Dispose();
                _persistSubscription = subscription;
            }
        }
    }
}
