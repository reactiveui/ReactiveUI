// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Trimming/AOT-safe, metadata-based <c>AutoPersist</c> overloads.</summary>
/// <remarks>
/// These live in their own partial declaration (and a distinct <c>extension</c> block) — separate from the
/// reflection-based overloads in <c>AutoPersistHelperMixins.cs</c> — specifically so their receiver type parameter
/// does NOT carry the reflection block's <see cref="System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute"/>.
/// Merging the two blocks would propagate that trimming annotation onto these trim-safe overloads and onto every caller
/// (IL2091). Keeping them in a separate partial keeps the two contracts distinct without an analyzer suppression.
/// </remarks>
public static partial class AutoPersistHelperMixins
{
    /// <summary>Provides metadata-based AutoPersist extension members for reactive objects without runtime reflection.</summary>
    /// <typeparam name="T">The reactive object type.</typeparam>
    /// <param name="this">The reactive object to watch for changes.</param>
    extension<T>(T @this)
        where T : IReactiveObject
    {
        /// <summary>AutoPersist overload that uses explicit metadata and performs no runtime reflection.</summary>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="metadata">The persistence metadata to use for determining persistable properties.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        public IDisposable AutoPersist(
            Func<T, IObservable<RxVoid>> doPersist,
            AutoPersistMetadata metadata)
            => @this.AutoPersist(doPersist, metadata, interval: null);

        /// <summary>AutoPersist overload that performs no runtime reflection and is suitable for trimming/AOT scenarios.</summary>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="metadata">The persistence metadata to use for determining persistable properties.</param>
        /// <param name="interval">The interval to save the object on.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="metadata"/> indicates the object is not persistable.</exception>
        public IDisposable AutoPersist(
            Func<T, IObservable<RxVoid>> doPersist,
            AutoPersistMetadata metadata,
            TimeSpan? interval)
            => @this.AutoPersist(doPersist, Signal.Silent<RxVoid>(), metadata, interval);

        /// <summary>AutoPersist overload that uses explicit metadata and a manual save signal, performing no runtime reflection.</summary>
        /// <typeparam name="TDontCare">The save signal type.</typeparam>
        /// <param name="doPersist">The asynchronous method to call to save the object to disk.</param>
        /// <param name="manualSaveSignal">When invoked, the object will be saved regardless of whether it has changed.</param>
        /// <param name="metadata">The persistence metadata to use for determining persistable properties.</param>
        /// <returns>A disposable to disable automatic persistence.</returns>
        public IDisposable AutoPersist<TDontCare>(
            Func<T, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            AutoPersistMetadata metadata)
            => @this.AutoPersist(doPersist, manualSaveSignal, metadata, interval: null);

        /// <summary>AutoPersist overload that performs no runtime reflection and is suitable for trimming/AOT scenarios.</summary>
        /// <typeparam name="TDontCare">The save signal type.</typeparam>
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
        public IDisposable AutoPersist<TDontCare>(
            Func<T, IObservable<RxVoid>> doPersist,
            IObservable<TDontCare> manualSaveSignal,
            AutoPersistMetadata metadata,
            TimeSpan? interval)
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

            interval ??= TimeSpan.FromSeconds(DefaultAutoPersistIntervalSeconds);

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
