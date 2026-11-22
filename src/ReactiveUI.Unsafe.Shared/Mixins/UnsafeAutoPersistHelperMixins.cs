// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Provides reflection-based AutoPersist and AutoPersistCollection extension methods.</summary>
public static class UnsafeAutoPersistHelperMixins
{
    /// <summary>Trim/AOT diagnostic message for reflection-based <c>AutoPersist</c> overloads.</summary>
    private const string AutoPersistMessage =
        "AutoPersist may reflect over the runtime type; prefer the AutoPersistMetadata overloads for trimming/AOT.";

    /// <summary>Trim/AOT diagnostic message for reflection-based <c>AutoPersistCollection</c> overloads.</summary>
    private const string AutoPersistCollectionMessage =
        "AutoPersistCollection may reflect over runtime item types via AutoPersist; prefer the overloads that accept AutoPersistMetadata or a metadata provider.";

    /// <summary>Stores per-runtime-type persistence metadata computed via reflection.</summary>
#if NET8_0_OR_GREATER
    private static readonly ConditionalWeakTable<Type, AutoPersistHelperMixins.AutoPersistMetadata> PersistMetadataByType = [];
#else
    private static readonly ConditionalWeakTable<Type, AutoPersistHelperMixins.AutoPersistMetadata> PersistMetadataByType = new();
#endif

    /// <summary>Provides reflection-based AutoPersistCollection extension members for observable collections.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    extension<TItem>(ObservableCollection<TItem> @this)
        where TItem : IReactiveObject
    {
        /// <summary>Applies AutoPersistence to all objects in a collection.</summary>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistMessage)]
        [RequiresDynamicCode(AutoPersistMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection(Func<TItem, IObservable<RxVoid>> doPersist) =>
            @this.AutoPersistCollection(doPersist, interval: null);

        /// <summary>Applies AutoPersistence to all objects in a collection on the given throttle interval.</summary>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="interval">The interval to save the object on.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistCollectionMessage)]
        [RequiresDynamicCode(AutoPersistCollectionMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection(Func<TItem, IObservable<RxVoid>> doPersist, TimeSpan? interval) =>
            AutoPersistCollection(@this, doPersist, Signal.Silent<RxVoid>(), interval);

        /// <summary>Applies AutoPersistence to all objects in a collection with a manual save signal.</summary>
        /// <typeparam name="TDontCare">The return signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistMessage)]
        [RequiresDynamicCode(AutoPersistMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal) =>
            @this.AutoPersistCollection(doPersist, manualSaveSignal, interval: null);

        /// <summary>Applies AutoPersistence to all objects in a collection with a manual save signal and throttle interval.</summary>
        /// <typeparam name="TDontCare">The return signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <param name="interval">The interval to save the object on.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistCollectionMessage)]
        [RequiresDynamicCode(AutoPersistCollectionMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            TimeSpan? interval) =>
            AutoPersistCollection<TItem, ObservableCollection<TItem>, TDontCare>(@this, doPersist, manualSaveSignal, interval);
    }

    /// <summary>Provides reflection-based AutoPersistCollection extension members for read-only observable collections.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    extension<TItem>(ReadOnlyObservableCollection<TItem> @this)
        where TItem : IReactiveObject
    {
        /// <summary>Applies AutoPersistence to all objects in a read-only collection.</summary>
        /// <typeparam name="TDontCare">The signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistMessage)]
        [RequiresDynamicCode(AutoPersistMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal) =>
            @this.AutoPersistCollection(doPersist, manualSaveSignal, interval: null);

        /// <summary>Applies AutoPersistence to all objects in a read-only collection with a manual save signal and interval.</summary>
        /// <typeparam name="TDontCare">The signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <param name="interval">The interval to save the object on.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistCollectionMessage)]
        [RequiresDynamicCode(AutoPersistCollectionMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            TimeSpan? interval) =>
            AutoPersistCollection<TItem, ReadOnlyObservableCollection<TItem>, TDontCare>(@this, doPersist, manualSaveSignal, interval);
    }

    /// <summary>Provides reflection-based AutoPersist extension members for reactive objects.</summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    extension<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties
                                    | DynamicallyAccessedMemberTypes.NonPublicProperties)]
    T>(T @this)
        where T : IReactiveObject
    {
        /// <summary>AutoPersist automatically calls a method whenever the object changes.</summary>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistMessage)]
        [RequiresDynamicCode(AutoPersistMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersist(Func<T, IObservable<RxVoid>> doPersist) =>
            @this.AutoPersist(doPersist, interval: null);

        /// <summary>AutoPersist allows you to automatically call a method when an object has changed, throttling on a certain interval.</summary>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="interval">The interval to save the object on.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistMessage)]
        [RequiresDynamicCode(AutoPersistMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersist(Func<T, IObservable<RxVoid>> doPersist, TimeSpan? interval) =>
            @this.AutoPersist(doPersist, Signal.Silent<RxVoid>(), interval);

        /// <summary>AutoPersist automatically calls a method whenever the object changes or a manual save is signalled.</summary>
        /// <typeparam name="TDontCare">The save signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistMessage)]
        [RequiresDynamicCode(AutoPersistMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersist<TDontCare>(
            Func<T, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal) =>
            @this.AutoPersist(doPersist, manualSaveSignal, interval: null);

        /// <summary>AutoPersist allows you to automatically call a method when an object has changed, throttling on a certain interval.</summary>
        /// <typeparam name="TDontCare">The save signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <param name="interval">The interval to save the object on.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        /// <exception cref="ArgumentException">Thrown when the object is not annotated with <c>[DataContract]</c>.</exception>
        [RequiresUnreferencedCode(AutoPersistMessage)]
        [RequiresDynamicCode(AutoPersistMessage)]
        public IDisposable AutoPersist<TDontCare>(
            Func<T, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            TimeSpan? interval)
        {
            ArgumentExceptionHelper.ThrowIfNull(@this);
            ArgumentExceptionHelper.ThrowIfNull(doPersist);
            ArgumentExceptionHelper.ThrowIfNull(manualSaveSignal);

            var runtimeType = @this.GetType();
            var metadata = runtimeType == typeof(T)
                ? AutoPersistHelperMixins.CreateMetadata<T>()
                : GetMetadataForRuntimeType(runtimeType);

            return @this.AutoPersist(doPersist, manualSaveSignal, metadata, interval);
        }
    }

    /// <summary>Provides reflection-based AutoPersistCollection extension members for generic notify-collection-changed collections.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <param name="this">The reactive collection to watch for changes.</param>
    extension<TItem, TCollection>(TCollection @this)
        where TItem : IReactiveObject
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
    {
        /// <summary>Applies AutoPersistence to all objects in a collection.</summary>
        /// <typeparam name="TDontCare">The signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistMessage)]
        [RequiresDynamicCode(AutoPersistMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal) =>
            @this.AutoPersistCollection(doPersist, manualSaveSignal, interval: null);

        /// <summary>Applies AutoPersistence to all objects in a collection with a manual save signal and interval.</summary>
        /// <typeparam name="TDontCare">The signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <param name="interval">The interval to save the object on.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        [RequiresUnreferencedCode(AutoPersistCollectionMessage)]
        [RequiresDynamicCode(AutoPersistCollectionMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable AutoPersistCollection<TDontCare>(
            Func<TItem, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            TimeSpan? interval) =>
            @this.AutoPersistCollection(
                doPersist,
                manualSaveSignal,
                static item => GetMetadataForRuntimeType(item.GetType()),
                interval);
    }

    /// <summary>Gets persistence metadata for a runtime type that is not statically known to the trimmer.</summary>
    /// <param name="runtimeType">The runtime type.</param>
    /// <returns>The computed persistence metadata.</returns>
    [RequiresUnreferencedCode(AutoPersistMessage)]
    [RequiresDynamicCode(AutoPersistMessage)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AutoPersistHelperMixins.AutoPersistMetadata GetMetadataForRuntimeType(Type runtimeType) =>
        PersistMetadataByType.GetValue(runtimeType, CreateMetadataFromRuntimeType);

    /// <summary>Creates persistence metadata for a runtime type via reflection.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>The computed persistence metadata.</returns>
    private static AutoPersistHelperMixins.AutoPersistMetadata CreateMetadataFromRuntimeType(Type type)
    {
        var hasDataContract = type.GetCustomAttributes(typeof(DataContractAttribute), true).Length > 0;
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        HashSet<string>? set = null;
        for (var i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            if (!property.IsDefined(typeof(DataMemberAttribute), true))
            {
                continue;
            }

            set ??= [with(StringComparer.Ordinal)];
            _ = set.Add(property.Name);
        }

        set ??= [with(StringComparer.Ordinal)];
        return new(hasDataContract, set);
    }
}
