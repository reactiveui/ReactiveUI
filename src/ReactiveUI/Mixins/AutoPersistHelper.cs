// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.CompilerServices;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI.Builder;

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
    private static readonly ConditionalWeakTable<Type, PersistMetadata> PersistMetadataByType = new();

    /// <summary>
    /// Initializes static members of the <see cref="AutoPersistHelper"/> class.
    /// </summary>
    static AutoPersistHelper() => RxAppBuilder.EnsureInitialized();

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
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] T>(
        this T @this,
        Func<T, IObservable<Unit>> doPersist,
        TimeSpan? interval = null)
            where T : IReactiveObject
        => @this.AutoPersist(doPersist, Observable<Unit>.Never, interval);

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
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] T,
        TDontCare>(
        this T @this,
        Func<T, IObservable<Unit>> doPersist,
        IObservable<TDontCare> manualSaveSignal,
        TimeSpan? interval = null)
            where T : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(doPersist);
        ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);

        interval ??= TimeSpan.FromSeconds(3.0);

        // Fast path: if T is the actual runtime type, use per-closed-generic cache (no CWT lookup).
        // Slow path: preserve historical semantics by reflecting over the runtime type.
        var runtimeType = @this.GetType();
        var metadata = runtimeType == typeof(T)
            ? PersistMetadataHolder<T>.Metadata
            : GetMetadataForUnknownRuntimeType(runtimeType);

        if (!metadata.HasDataContract)
        {
            throw new ArgumentException("AutoPersist can only be applied to objects with [DataContract]");
        }

        var persistablePropertyNames = metadata.PersistablePropertyNames;

        var saveHint =
            @this.GetChangedObservable()
                 .Where(x => x.PropertyName is not null && persistablePropertyNames.Contains(x.PropertyName))
                 .Select(static _ => Unit.Default)
                 .Merge(manualSaveSignal.Select(static _ => Unit.Default));

        var autoSaver =
            saveHint
                .Throttle(interval.Value, RxSchedulers.TaskpoolScheduler)
                .SelectMany(_ => doPersist(@this))
                .Publish();

        // NB: This rigamarole is to prevent the initialization of a class
        // from triggering a save.
        var ret = new SingleAssignmentDisposable();
        RxSchedulers.MainThreadScheduler.Schedule(() =>
        {
            if (ret.IsDisposed)
            {
                return;
            }

            ret.Disposable = autoSaver.Connect();
        });

        return ret;
    }

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
        TimeSpan? interval = null)
            where TItem : IReactiveObject
        => AutoPersistCollection(@this, doPersist, Observable<Unit>.Never, metadata, interval);

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
        TimeSpan? interval = null)
            where TItem : IReactiveObject
        => AutoPersistCollection<TItem, ObservableCollection<TItem>, TDontCare>(@this, doPersist, manualSaveSignal, metadata, interval);

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
        TimeSpan? interval = null)
            where TItem : IReactiveObject
            where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(doPersist);
        ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);
        ArgumentExceptionHelper.ThrowIfNull(metadata);

        if (!metadata.HasDataContract)
        {
            throw new ArgumentException("AutoPersist can only be applied to objects with [DataContract]", nameof(metadata));
        }

        var disposerList = new Dictionary<TItem, IDisposable>();

        var subscription = @this.ActOnEveryObject<TItem, TCollection>(
            onAdd: x =>
            {
                if (disposerList.ContainsKey(x))
                {
                    return;
                }

                disposerList[x] = x.AutoPersist(doPersist, manualSaveSignal, metadata, interval);
            },
            onRemove: x =>
            {
                if (!disposerList.TryGetValue(x, out var d))
                {
                    return;
                }

                d.Dispose();
                disposerList.Remove(x);
            });

        return Disposable.Create(() =>
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
        TimeSpan? interval = null)
            where TItem : IReactiveObject
        => AutoPersistCollection<TItem, ReadOnlyObservableCollection<TItem>, TDontCare>(@this, doPersist, manualSaveSignal, metadata, interval);

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
        TimeSpan? interval = null)
            where TItem : IReactiveObject
            where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(doPersist);
        ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);
        ArgumentExceptionHelper.ThrowIfNull(metadataProvider);

        var disposerList = new Dictionary<TItem, IDisposable>();

        var subscription = @this.ActOnEveryObject<TItem, TCollection>(
            onAdd: x =>
            {
                if (disposerList.ContainsKey(x))
                {
                    return;
                }

                // Non-RUC path: caller provides metadata explicitly.
                var metadata = metadataProvider(x);
                disposerList[x] = x.AutoPersist(doPersist, manualSaveSignal, metadata, interval);
            },
            onRemove: x =>
            {
                if (!disposerList.TryGetValue(x, out var d))
                {
                    return;
                }

                d.Dispose();
                disposerList.Remove(x);
            });

        return Disposable.Create(() =>
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
    public static Func<TItem, AutoPersistMetadata> CreateMetadataProvider<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] TItem>()
            where TItem : IReactiveObject
    {
        var metadata = CreateMetadata<TItem>();
        return _ => metadata;
    }

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
        TimeSpan? interval = null)
            where T : IReactiveObject
        => @this.AutoPersist(doPersist, Observable<Unit>.Never, metadata, interval);

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
        TimeSpan? interval = null)
            where T : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(doPersist);
        ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);
        ArgumentExceptionHelper.ThrowIfNull(metadata);

        if (!metadata.HasDataContract)
        {
            throw new ArgumentException("AutoPersist can only be applied to objects with [DataContract]", nameof(metadata));
        }

        interval ??= TimeSpan.FromSeconds(3.0);

        var persistablePropertyNames = metadata.PersistablePropertyNames;

        var saveHint =
            @this.GetChangedObservable()
                 .Where(x => x.PropertyName is not null && persistablePropertyNames.Contains(x.PropertyName))
                 .Select(static _ => Unit.Default)
                 .Merge(manualSaveSignal.Select(static _ => Unit.Default));

        var autoSaver =
            saveHint
                .Throttle(interval.Value, RxSchedulers.TaskpoolScheduler)
                .SelectMany(_ => doPersist(@this))
                .Publish();

        var ret = new SingleAssignmentDisposable();
        RxSchedulers.MainThreadScheduler.Schedule(() =>
        {
            if (ret.IsDisposed)
            {
                return;
            }

            ret.Disposable = autoSaver.Connect();
        });

        return ret;
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
    public static AutoPersistMetadata CreateMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] T>()
            where T : IReactiveObject
        => PersistMetadataHolder<T>.Metadata.Public;

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
        TimeSpan? interval = null)
            where TItem : IReactiveObject
        => AutoPersistCollection(@this, doPersist, Observable<Unit>.Never, interval);

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
        TimeSpan? interval = null)
            where TItem : IReactiveObject
        => AutoPersistCollection<TItem, ObservableCollection<TItem>, TDontCare>(@this, doPersist, manualSaveSignal, interval);

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
        TimeSpan? interval = null)
            where TItem : IReactiveObject
        => AutoPersistCollection<TItem, ReadOnlyObservableCollection<TItem>, TDontCare>(@this, doPersist, manualSaveSignal, interval);

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
        TimeSpan? interval = null)
            where TItem : IReactiveObject
            where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(doPersist);
        ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);

        // Dictionary is used to preserve prior semantics: per-item disposable tracked by item instance.
        var disposerList = new Dictionary<TItem, IDisposable>();

        var subscription = @this.ActOnEveryObject<TItem, TCollection>(
            onAdd: x =>
            {
                if (disposerList.TryGetValue(x, out _))
                {
                    return;
                }

                disposerList[x] = x.AutoPersist(doPersist, manualSaveSignal, interval);
            },
            onRemove: x =>
            {
                if (!disposerList.TryGetValue(x, out var d))
                {
                    return;
                }

                d.Dispose();
                disposerList.Remove(x);
            });

        return Disposable.Create(() =>
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

        // ToObservableChangeSet will emit existing items when first subscribed, so we don't need to manually iterate them
        var changedDisposable = ActOnEveryObject(collection.ToObservableChangeSet<TCollection, TItem>(), onAdd, onRemove);

        return Disposable.Create(() =>
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
        this IObservable<IChangeSet<TItem>> @this,
        Action<TItem> onAdd,
        Action<TItem> onRemove)
            where TItem : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(@this);
        ArgumentExceptionHelper.ThrowIfNull(onAdd);
        ArgumentExceptionHelper.ThrowIfNull(onRemove);

        return @this.Subscribe(changeSet =>
        {
            foreach (var change in changeSet)
            {
                switch (change.Reason)
                {
                    case ListChangeReason.Refresh:
                        // Preserve original ordering: remove all, then add all.
                        foreach (var item in change.Range)
                        {
                            onRemove(item);
                        }

                        foreach (var item in change.Range)
                        {
                            onAdd(item);
                        }

                        break;

                    case ListChangeReason.Clear:
                        foreach (var item in change.Range)
                        {
                            onRemove(item);
                        }

                        break;

                    case ListChangeReason.Add:
                        onAdd(change.Item.Current);
                        break;

                    case ListChangeReason.AddRange:
                        foreach (var item in change.Range)
                        {
                            onAdd(item);
                        }

                        break;

                    case ListChangeReason.Replace:
                        onRemove(change.Item.Previous.Value);
                        onAdd(change.Item.Current);
                        break;

                    case ListChangeReason.Remove:
                        onRemove(change.Item.Current);
                        break;

                    case ListChangeReason.RemoveRange:
                        foreach (var item in change.Range)
                        {
                            onRemove(item);
                        }

                        break;
                }
            }
        });
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
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] T>
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
            Public = new AutoPersistMetadata(hasDataContract, persistablePropertyNames);
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
        internal static PersistMetadata Create(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
            Type type)
        {
            // Preserve original semantics: [DataContract] is checked via GetCustomAttributes(..., inherit: true).
            var hasDataContract = type.GetCustomAttributes(typeof(DataContractAttribute), inherit: true).Length > 0;

            // Preserve original semantics: consider DeclaredProperties only (not inherited properties).
            // Use reflection flags directly to avoid GetTypeInfo() overhead.
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            HashSet<string>? set = null;

            for (var i = 0; i < properties.Length; i++)
            {
                var p = properties[i];
                if (!HasDataMemberAttribute(p))
                {
                    continue;
                }

                set ??= new HashSet<string>(StringComparer.Ordinal);
                set.Add(p.Name);
            }

            set ??= new HashSet<string>(StringComparer.Ordinal);
            return new PersistMetadata(hasDataContract, set);
        }

        /// <summary>
        /// Determines whether a property is annotated with <c>[DataMember]</c>.
        /// </summary>
        /// <param name="property">The property to inspect.</param>
        /// <returns><see langword="true"/> if the property is annotated; otherwise <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasDataMemberAttribute(PropertyInfo property)
        {
            // Avoid LINQ allocations; use IsDefined which is efficient for the common case.
            // DataMemberAttribute is not inherited by default, but preserve inherit=true for parity.
            return property.IsDefined(typeof(DataMemberAttribute), inherit: true);
        }
    }
}
