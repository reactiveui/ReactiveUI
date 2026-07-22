// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Helper extension method class associated with the AutoPersist related functionality.</summary>
public static partial class AutoPersistHelperMixins
{
    /// <summary>The default debounce interval, in seconds, applied when no explicit AutoPersist interval is supplied.</summary>
    private const double DefaultAutoPersistIntervalSeconds = 3.0;

    /// <summary>Stores per-runtime-type persistence metadata computed via reflection.</summary>
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

    /// <summary>Initializes static members of the <see cref="AutoPersistHelperMixins"/> class.</summary>
    static AutoPersistHelperMixins() => RxAppBuilder.EnsureInitialized();

    /// <summary>Provides ActOnEveryObject extension members for reactive change-set streams.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The observable change set to watch for changes.</param>
    extension<TItem>(IObservable<IReactiveChangeSet<TItem>> @this)
        where TItem : IReactiveObject
    {
        /// <summary>
        /// Call methods <paramref name="onAdd"/> and <paramref name="onRemove"/> whenever an object is added or
        /// removed from a collection. This method correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <param name="onAdd">A method to be called when an object is added to the collection.</param>
        /// <param name="onRemove">A method to be called when an object is removed from the collection.</param>
        /// <returns>A disposable that deactivates this behavior.</returns>
        public IDisposable ActOnEveryObject(
            Action<TItem> onAdd,
            Action<TItem> onRemove)
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
    }

    /// <summary>Provides AutoPersistCollection and ActOnEveryObject extension members for observable collections.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    extension<TItem>(ObservableCollection<TItem> @this)
        where TItem : IReactiveObject
    {
        /// <summary>Applies AutoPersistence to all objects in a collection using explicit persistence metadata.</summary>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        public IDisposable AutoPersistCollection(
            Func<TItem, IObservable<RxVoid>> doPersist,
            AutoPersistMetadata metadata)
            => @this.AutoPersistCollection(doPersist, metadata, interval: null);

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection using explicit persistence metadata.
        /// This overload performs no runtime reflection and is suitable for trimming/AOT scenarios.
        /// </summary>
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
        public IDisposable AutoPersistCollection(
            Func<TItem, IObservable<RxVoid>> doPersist,
            AutoPersistMetadata metadata,
            TimeSpan? interval)
            => AutoPersistCollection(@this, doPersist, Signal.Silent<RxVoid>(), metadata, interval);

        /// <summary>Applies AutoPersistence to all objects in a collection using explicit persistence metadata.</summary>
        /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            AutoPersistMetadata metadata)
            => @this.AutoPersistCollection(doPersist, manualSaveSignal, metadata, interval: null);

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection using explicit persistence metadata.
        /// This overload performs no runtime reflection and is suitable for trimming/AOT scenarios.
        /// </summary>
        /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
        /// <param name="interval">The interval to save the object on.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="metadata"/> indicates the object is not annotated with <c>[DataContract]</c>.
        /// </exception>
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            AutoPersistMetadata metadata,
            TimeSpan? interval)
            => AutoPersistCollection<TItem, ObservableCollection<TItem>, TDontCare>(
                @this,
                doPersist,
                manualSaveSignal,
                metadata,
                interval);

        /// <summary>Applies AutoPersistence to all objects in a collection.</summary>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
        [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
        public IDisposable AutoPersistCollection(
            Func<TItem, IObservable<RxVoid>> doPersist)
            => @this.AutoPersistCollection(doPersist, interval: null);

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection. Items that are
        /// no longer in the collection won't be persisted anymore.
        /// </summary>
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
        public IDisposable AutoPersistCollection(
            Func<TItem, IObservable<RxVoid>> doPersist,
            TimeSpan? interval)
            => AutoPersistCollection(@this, doPersist, Signal.Silent<RxVoid>(), interval);

        /// <summary>Applies AutoPersistence to all objects in a collection.</summary>
        /// <typeparam name="TDontCare">The return signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
        [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal)
            => @this.AutoPersistCollection(doPersist, manualSaveSignal, interval: null);

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection. Items that are
        /// no longer in the collection won't be persisted anymore.
        /// </summary>
        /// <typeparam name="TDontCare">The return signal type.</typeparam>
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
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            TimeSpan? interval)
            => AutoPersistCollection<TItem, ObservableCollection<TItem>, TDontCare>(
                @this,
                doPersist,
                manualSaveSignal,
                interval);

        /// <summary>
        /// Call methods <paramref name="onAdd"/> and <paramref name="onRemove"/> whenever an object is added or
        /// removed from a collection. This method correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <param name="onAdd">A method to be called when an object is added to the collection.</param>
        /// <param name="onRemove">A method to be called when an object is removed from the collection.</param>
        /// <returns>A disposable that deactivates this behavior.</returns>
        public IDisposable ActOnEveryObject(
            Action<TItem> onAdd,
            Action<TItem> onRemove)
            => ActOnEveryObject<TItem, ObservableCollection<TItem>>(@this, onAdd, onRemove);
    }

    /// <summary>Provides AutoPersistCollection and ActOnEveryObject extension members for read-only observable collections.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    extension<TItem>(ReadOnlyObservableCollection<TItem> @this)
        where TItem : IReactiveObject
    {
        /// <summary>Applies AutoPersistence to all objects in a read-only collection using explicit persistence metadata.</summary>
        /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            AutoPersistMetadata metadata)
            => @this.AutoPersistCollection(doPersist, manualSaveSignal, metadata, interval: null);

        /// <summary>
        /// Apply AutoPersistence to all objects in a read-only collection using explicit persistence metadata.
        /// This overload performs no runtime reflection and is suitable for trimming/AOT scenarios.
        /// </summary>
        /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
        /// <param name="interval">The interval to save the object on.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="metadata"/> indicates the object is not annotated with <c>[DataContract]</c>.
        /// </exception>
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            AutoPersistMetadata metadata,
            TimeSpan? interval)
            => AutoPersistCollection<TItem, ReadOnlyObservableCollection<TItem>, TDontCare>(
                @this,
                doPersist,
                manualSaveSignal,
                metadata,
                interval);

        /// <summary>Applies AutoPersistence to all objects in a read-only collection.</summary>
        /// <typeparam name="TDontCare">The signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
        [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal)
            => @this.AutoPersistCollection(doPersist, manualSaveSignal, interval: null);

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection. Items that are
        /// no longer in the collection won't be persisted anymore.
        /// </summary>
        /// <typeparam name="TDontCare">The signal type.</typeparam>
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
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            TimeSpan? interval)
            => AutoPersistCollection<TItem, ReadOnlyObservableCollection<TItem>, TDontCare>(
                @this,
                doPersist,
                manualSaveSignal,
                interval);

        /// <summary>
        /// Call methods <paramref name="onAdd"/> and <paramref name="onRemove"/> whenever an object is added or
        /// removed from a collection. This method correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <param name="onAdd">A method to be called when an object is added to the collection.</param>
        /// <param name="onRemove">A method to be called when an object is removed from the collection.</param>
        /// <returns>A disposable that deactivates this behavior.</returns>
        public IDisposable ActOnEveryObject(
            Action<TItem> onAdd,
            Action<TItem> onRemove)
            => ActOnEveryObject<TItem, ReadOnlyObservableCollection<TItem>>(@this, onAdd, onRemove);
    }

    /// <summary>Provides AutoPersist extension members for reactive objects, reflecting over the runtime type when required.</summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    extension<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
    T>(T @this)
        where T : IReactiveObject
    {
        /// <summary>AutoPersist automatically calls a method whenever the object changes.</summary>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
        [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
        public IDisposable AutoPersist(
            Func<T, IObservable<RxVoid>> doPersist)
            => @this.AutoPersist(doPersist, interval: null);

        /// <summary>
        /// AutoPersist allows you to automatically call a method when an object
        /// has changed, throttling on a certain interval. Note that this object
        /// must mark its persistent properties via the [DataMember] attribute.
        /// Changes to properties not marked with DataMember will not trigger the
        /// object to be saved.
        /// </summary>
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
        public IDisposable AutoPersist(
            Func<T, IObservable<RxVoid>> doPersist,
            TimeSpan? interval)
            => @this.AutoPersist(doPersist, Signal.Silent<RxVoid>(), interval);

        /// <summary>AutoPersist automatically calls a method whenever the object changes or a manual save is signalled.</summary>
        /// <typeparam name="TDontCare">The save signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
        [RequiresDynamicCode("AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.")]
        public IDisposable AutoPersist<TDontCare>(
            Func<T, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal)
            => @this.AutoPersist(doPersist, manualSaveSignal, interval: null);

        /// <summary>
        /// AutoPersist allows you to automatically call a method when an object
        /// has changed, throttling on a certain interval. Note that this object
        /// must mark its persistent properties via the [DataMember] attribute.
        /// Changes to properties not marked with DataMember will not trigger the
        /// object to be saved.
        /// </summary>
        /// <typeparam name="TDontCare">The save signal type.</typeparam>
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
        public IDisposable AutoPersist<TDontCare>(
            Func<T, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            TimeSpan? interval)
        {
            ArgumentExceptionHelper.ThrowIfNull(@this);
            ArgumentExceptionHelper.ThrowIfNull(doPersist);
            ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);

            interval ??= TimeSpan.FromSeconds(DefaultAutoPersistIntervalSeconds);

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
            _ = RxSchedulers.MainThreadScheduler.Schedule(
                (ret, source: @this, doPersist, persistablePropertyNames, manualSaveSignal, interval: interval.Value),
                static (_, state) =>
                {
                    if (state.ret.IsDisposed)
                    {
                        return EmptyDisposable.Instance;
                    }

                    state.ret.Disposable = new AutoPersistDriver<T, TDontCare>(
                        state.source,
                        state.doPersist,
                        state.persistablePropertyNames,
                        state.manualSaveSignal,
                        state.interval,
                        RxSchedulers.TaskpoolScheduler);
                    return EmptyDisposable.Instance;
                });

            return ret;
        }
    }
}
