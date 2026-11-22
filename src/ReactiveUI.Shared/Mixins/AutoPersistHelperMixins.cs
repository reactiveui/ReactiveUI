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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection(
            Func<TItem, IObservable<RxVoid>> doPersist,
            AutoPersistMetadata metadata) =>
            @this.AutoPersistCollection(doPersist, metadata, interval: null);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection(
            Func<TItem, IObservable<RxVoid>> doPersist,
            AutoPersistMetadata metadata,
            TimeSpan? interval) =>
            AutoPersistCollection(@this, doPersist, Signal.Silent<RxVoid>(), metadata, interval);

        /// <summary>Applies AutoPersistence to all objects in a collection using explicit persistence metadata.</summary>
        /// <typeparam name="TDontCare">The manual save signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <param name="metadata">The persistence metadata that determines which properties trigger persistence.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            AutoPersistMetadata metadata) =>
            @this.AutoPersistCollection(doPersist, manualSaveSignal, metadata, interval: null);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            AutoPersistMetadata metadata,
            TimeSpan? interval) =>
            AutoPersistCollection<TItem, ObservableCollection<TItem>, TDontCare>(
                @this,
                doPersist,
                manualSaveSignal,
                metadata,
                interval);

        /// <summary>
        /// Call methods <paramref name="onAdd"/> and <paramref name="onRemove"/> whenever an object is added or
        /// removed from a collection. This method correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <param name="onAdd">A method to be called when an object is added to the collection.</param>
        /// <param name="onRemove">A method to be called when an object is removed from the collection.</param>
        /// <returns>A disposable that deactivates this behavior.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable ActOnEveryObject(
            Action<TItem> onAdd,
            Action<TItem> onRemove) =>
            ActOnEveryObject<TItem, ObservableCollection<TItem>>(@this, onAdd, onRemove);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            AutoPersistMetadata metadata) =>
            @this.AutoPersistCollection(doPersist, manualSaveSignal, metadata, interval: null);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            AutoPersistMetadata metadata,
            TimeSpan? interval) =>
            AutoPersistCollection<TItem, ReadOnlyObservableCollection<TItem>, TDontCare>(
                @this,
                doPersist,
                manualSaveSignal,
                metadata,
                interval);

        /// <summary>
        /// Call methods <paramref name="onAdd"/> and <paramref name="onRemove"/> whenever an object is added or
        /// removed from a collection. This method correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <param name="onAdd">A method to be called when an object is added to the collection.</param>
        /// <param name="onRemove">A method to be called when an object is removed from the collection.</param>
        /// <returns>A disposable that deactivates this behavior.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable ActOnEveryObject(
            Action<TItem> onAdd,
            Action<TItem> onRemove) =>
            ActOnEveryObject<TItem, ReadOnlyObservableCollection<TItem>>(@this, onAdd, onRemove);
    }
}
