// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Foundation;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Extension methods for <see cref="ReactiveCollectionViewSource{TSource}"/>.</summary>
public static class ReactiveCollectionViewSourceExtensions
{
    /// <summary>Provides collection-view binding extension members for an observable of a collection.</summary>
    /// <param name="sourceObservable">Source collection observable.</param>
    extension(IObservable<INotifyCollectionChanged> sourceObservable)
    {
        /// <summary>Extension method that binds an observable of a collection as the source of a <see cref="UICollectionView"/>.</summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="cellKey">Cell key.</param>
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        [SuppressMessage(
            "Design",
            "SST2307:Generic method type parameters should be inferable from the parameters",
            Justification = "Type parameters are specified at the call site; this is a type-argument-driven binding factory.")]
        public IDisposable BindTo<TSource, TCell>(
            UICollectionView collectionView,
            NSString cellKey)
            where TCell : UICollectionViewCell =>
            sourceObservable.BindTo<TSource, TCell>(collectionView, cellKey, initializeCellAction: null);

        /// <summary>Extension method that binds an observable of a collection as the source of a <see cref="UICollectionView"/>.</summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="cellKey">Cell key.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        [SuppressMessage(
            "Design",
            "SST2307:Generic method type parameters should be inferable from the parameters",
            Justification = "Type parameters are specified at the call site; this is a type-argument-driven binding factory.")]
        public IDisposable BindTo<TSource, TCell>(
            UICollectionView collectionView,
            NSString cellKey,
            Action<TCell>? initializeCellAction)
            where TCell : UICollectionViewCell =>
            sourceObservable.BindTo<TSource, TCell>(collectionView, cellKey, initializeCellAction, initSource: null);

        /// <summary>Extension method that binds an observable of a collection as the source of a <see cref="UICollectionView"/>.</summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="cellKey">Cell key.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>
        /// <param name="initSource">Optionally initializes some property of
        /// the <see cref="ReactiveCollectionViewSource{TSource}"/>.</param>
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IDisposable BindTo<TSource, TCell>(
            UICollectionView collectionView,
            NSString cellKey,
            Action<TCell>? initializeCellAction,
            Func<ReactiveCollectionViewSource<TSource>, IDisposable>? initSource)
            where TCell : UICollectionViewCell =>
            new MapSignal<INotifyCollectionChanged, CollectionViewSectionInformation<TSource, TCell>[]>(
                    sourceObservable,
                    src =>
                    [
                        new CollectionViewSectionInformation<TSource, TCell>(
                                                                             src,
                                                                             cellKey,
                                                                             initializeCellAction)
                    ])
                .BindTo(collectionView, initSource);

        /// <summary>
        /// Extension method that binds an observable of a collection
        /// as the source of a <see cref="UICollectionView"/>.  Also registers
        /// the given class with an unspecified cellKey (you should probably
        /// not specify any other cellKeys).
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="collectionView">Collection view.</param>
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        [SuppressMessage(
            "Design",
            "SST2307:Generic method type parameters should be inferable from the parameters",
            Justification = "Type parameters are specified at the call site; this is a type-argument-driven binding factory.")]
        public IDisposable BindTo<TSource, TCell>(
            UICollectionView collectionView)
            where TCell : UICollectionViewCell =>
            sourceObservable.BindTo<TSource, TCell>(collectionView, initializeCellAction: null);

        /// <summary>
        /// Extension method that binds an observable of a collection
        /// as the source of a <see cref="UICollectionView"/>.  Also registers
        /// the given class with an unspecified cellKey (you should probably
        /// not specify any other cellKeys).
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        [SuppressMessage(
            "Design",
            "SST2307:Generic method type parameters should be inferable from the parameters",
            Justification = "Type parameters are specified at the call site; this is a type-argument-driven binding factory.")]
        public IDisposable BindTo<TSource, TCell>(
            UICollectionView collectionView,
            Action<TCell>? initializeCellAction)
            where TCell : UICollectionViewCell =>
            sourceObservable.BindTo<TSource, TCell>(collectionView, initializeCellAction, initSource: null);

        /// <summary>
        /// Extension method that binds an observable of a collection
        /// as the source of a <see cref="UICollectionView"/>.  Also registers
        /// the given class with an unspecified cellKey (you should probably
        /// not specify any other cellKeys).
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>
        /// <param name="initSource">Optionally initializes some property of
        /// the <see cref="ReactiveCollectionViewSource{TSource}"/>.</param>
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IDisposable BindTo<TSource, TCell>(
            UICollectionView collectionView,
            Action<TCell>? initializeCellAction,
            Func<ReactiveCollectionViewSource<TSource>, IDisposable>? initSource)
            where TCell : UICollectionViewCell
        {
            ArgumentExceptionHelper.ThrowIfNull(collectionView);

            var type = typeof(TCell);
            var cellKey = new NSString(type.ToString());
            collectionView.RegisterClassForCell(type, new(cellKey));
            return sourceObservable
                .BindTo(collectionView, cellKey, initializeCellAction, initSource);
        }
    }

    /// <summary>Provides collection-view binding extension members for an observable of a list of collection sections.</summary>
    /// <param name="sectionsObservable">Sections observable.</param>
    /// <typeparam name="TSource">Type of the view source.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
    extension<TSource, TCell>(IObservable<IReadOnlyList<CollectionViewSectionInformation<TSource, TCell>>> sectionsObservable)
        where TCell : UICollectionViewCell
    {
        /// <summary>
        /// <para>Extension method that binds an observable of a list of collection
        /// sections as the source of a <see cref="UICollectionView"/>.</para>
        /// <para>If your <see cref="IReadOnlyList{T}"/> is also an instance of
        /// <see cref="INotifyCollectionChanged"/>, then this method
        /// will silently update the bindings whenever it changes as well.
        /// Otherwise, it will just log a message.</para>
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="collectionView">Collection view.</param>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IDisposable BindTo(UICollectionView collectionView) =>
            sectionsObservable.BindTo(collectionView, initSource: null);

        /// <summary>
        /// <para>Extension method that binds an observable of a list of collection
        /// sections as the source of a <see cref="UICollectionView"/>.</para>
        /// <para>If your <see cref="IReadOnlyList{T}"/> is also an instance of
        /// <see cref="INotifyCollectionChanged"/>, then this method
        /// will silently update the bindings whenever it changes as well.
        /// Otherwise, it will just log a message.</para>
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="initSource">Optionally initializes some property of
        /// the <see cref="ReactiveCollectionViewSource{TSource}"/>.</param>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IDisposable BindTo(
            UICollectionView collectionView,
            Func<ReactiveCollectionViewSource<TSource>, IDisposable>? initSource)
        {
            ArgumentExceptionHelper.ThrowIfNull(sectionsObservable);

            ArgumentExceptionHelper.ThrowIfNull(collectionView);

            var source = new ReactiveCollectionViewSource<TSource>(collectionView);
            initSource?.Invoke(source);

            var bind = sectionsObservable.BindTo(source, static x => x.Data);
            collectionView.Source = source;
            return new MultipleDisposable(bind, source);
        }
    }
}
