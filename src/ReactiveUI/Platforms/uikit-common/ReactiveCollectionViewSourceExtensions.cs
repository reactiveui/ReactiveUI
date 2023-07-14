// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;

using Foundation;

using UIKit;

namespace ReactiveUI;

/// <summary>
/// Extension methods for <see cref="ReactiveCollectionViewSource{TSource}"/>.
/// </summary>
public static class ReactiveCollectionViewSourceExtensions
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
    /// <param name="sectionsObservable">Sections observable.</param>
    /// <param name="collectionView">Collection view.</param>
    /// <param name="initSource">Optionally initializes some property of
    /// the <see cref="ReactiveCollectionViewSource{TSource}"/>.</param>
    /// <typeparam name="TSource">Type of the view source.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<IReadOnlyList<CollectionViewSectionInformation<TSource, TCell>>> sectionsObservable,
        UICollectionView collectionView,
        Func<ReactiveCollectionViewSource<TSource>, IDisposable>? initSource = null)
        where TCell : UICollectionViewCell
    {
        if (sectionsObservable is null)
        {
            throw new ArgumentNullException(nameof(sectionsObservable));
        }

        if (collectionView is null)
        {
            throw new ArgumentNullException(nameof(collectionView));
        }

        var source = new ReactiveCollectionViewSource<TSource>(collectionView);
        initSource?.Invoke(source);

        var bind = sectionsObservable.BindTo(source, x => x.Data);
        collectionView.Source = source;
        return new CompositeDisposable(bind, source);
    }

    /// <summary>
    /// Extension method that binds an observable of a collection
    /// as the source of a <see cref="UICollectionView"/>.
    /// </summary>
    /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
    /// <param name="sourceObservable">Source collection observable.</param>
    /// <param name="collectionView">Collection view.</param>
    /// <param name="cellKey">Cell key.</param>
    /// <param name="initializeCellAction">Initialize cell action.</param>
    /// <param name="initSource">Optionally initializes some property of
    /// the <see cref="ReactiveCollectionViewSource{TSource}"/>.</param>
    /// <typeparam name="TSource">Type of the source.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<INotifyCollectionChanged> sourceObservable,
        UICollectionView collectionView,
        NSString cellKey,
        Action<TCell>? initializeCellAction = null,
        Func<ReactiveCollectionViewSource<TSource>, IDisposable>? initSource = null)
        where TCell : UICollectionViewCell =>
        sourceObservable
            .Select(
                    src => new[]
                    {
                        new CollectionViewSectionInformation<TSource, TCell>(
                                                                             src,
                                                                             cellKey,
                                                                             initializeCellAction)
                    })
            .BindTo(collectionView, initSource);

    /// <summary>
    /// Extension method that binds an observable of a collection
    /// as the source of a <see cref="UICollectionView"/>.  Also registers
    /// the given class with an unspecified cellKey (you should probably
    /// not specify any other cellKeys).
    /// </summary>
    /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
    /// <param name="sourceObservable">Source collection observable.</param>
    /// <param name="collectionView">Collection view.</param>
    /// <param name="initializeCellAction">Initialize cell action.</param>
    /// <param name="initSource">Optionally initializes some property of
    /// the <see cref="ReactiveCollectionViewSource{TSource}"/>.</param>
    /// <typeparam name="TSource">Type of the source.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<INotifyCollectionChanged> sourceObservable,
        UICollectionView collectionView,
        Action<TCell>? initializeCellAction = null,
        Func<ReactiveCollectionViewSource<TSource>, IDisposable>? initSource = null)
        where TCell : UICollectionViewCell
    {
        if (collectionView is null)
        {
            throw new ArgumentNullException(nameof(collectionView));
        }

        var type = typeof(TCell);
        var cellKey = new NSString(type.ToString());
        collectionView.RegisterClassForCell(type, new NSString(cellKey));
        return sourceObservable
            .BindTo(collectionView, cellKey, initializeCellAction, initSource);
    }
}
