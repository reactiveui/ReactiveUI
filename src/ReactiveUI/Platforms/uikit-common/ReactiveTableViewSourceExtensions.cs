// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using Foundation;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;
using UIKit;

namespace ReactiveUI;

/// <summary>
/// Extension methods for <see cref="ReactiveTableViewSource{TSource}"/>.
/// </summary>
public static class ReactiveTableViewSourceExtensions
{
    /// <summary>
    /// <para>Extension method that binds an observable of a list of table
    /// sections as the source of a <see cref="UITableView"/>.</para>
    /// <para>If your <see cref="IReadOnlyList{T}"/> is also an instance of
    /// <see cref="INotifyCollectionChanged"/>, then this method
    /// will silently update the bindings whenever it changes as well.
    /// Otherwise, it will just log a message.</para>
    /// </summary>
    /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
    /// <param name="sectionsObservable">Sections observable.</param>
    /// <param name="tableView">Table view.</param>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<IReadOnlyList<TableSectionInformation<TSource, TCell>>> sectionsObservable,
        UITableView tableView)
        where TCell : UITableViewCell =>
        sectionsObservable.BindTo(tableView, initSource: null);

    /// <summary>
    /// <para>Extension method that binds an observable of a list of table
    /// sections as the source of a <see cref="UITableView"/>.</para>
    /// <para>If your <see cref="IReadOnlyList{T}"/> is also an instance of
    /// <see cref="INotifyCollectionChanged"/>, then this method
    /// will silently update the bindings whenever it changes as well.
    /// Otherwise, it will just log a message.</para>
    /// </summary>
    /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
    /// <param name="sectionsObservable">Sections observable.</param>
    /// <param name="tableView">Table view.</param>
    /// <param name="initSource">Optionally initializes some property of
    /// the <see cref="ReactiveTableViewSource{TSource}"/>.</param>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<IReadOnlyList<TableSectionInformation<TSource, TCell>>> sectionsObservable,
        UITableView tableView,
        Func<ReactiveTableViewSource<TSource>, IDisposable>? initSource)
        where TCell : UITableViewCell
    {
        ArgumentExceptionHelper.ThrowIfNull(sectionsObservable);
        ArgumentExceptionHelper.ThrowIfNull(tableView);

        var source = new ReactiveTableViewSource<TSource>(tableView);
        if (initSource is not null)
        {
            initSource(source);
        }

        var bind = sectionsObservable.BindTo(source, static x => x.Data);
        tableView.Source = source;

        return new CompositeDisposable(bind, source);
    }

    /// <summary>
    /// Extension method that binds an observable of a collection
    /// as the source of a <see cref="UITableView"/>.
    /// </summary>
    /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
    /// <param name="sourceObservable">Source collection observable.</param>
    /// <param name="tableView">Table view.</param>
    /// <param name="cellKey">Cell key.</param>
    /// <param name="sizeHint">Size hint.</param>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameters",
        Justification = "Type parameters are part of the public binding API and specified at the call site.")]
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<INotifyCollectionChanged> sourceObservable,
        UITableView tableView,
        NSString cellKey,
        float sizeHint)
        where TCell : UITableViewCell =>
        sourceObservable.BindTo<TSource, TCell>(tableView, cellKey, sizeHint, initializeCellAction: null);

    /// <summary>
    /// Extension method that binds an observable of a collection
    /// as the source of a <see cref="UITableView"/>.
    /// </summary>
    /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
    /// <param name="sourceObservable">Source collection observable.</param>
    /// <param name="tableView">Table view.</param>
    /// <param name="cellKey">Cell key.</param>
    /// <param name="sizeHint">Size hint.</param>
    /// <param name="initializeCellAction">Initialize cell action.</param>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameters",
        Justification = "Type parameters are part of the public binding API and specified at the call site.")]
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<INotifyCollectionChanged> sourceObservable,
        UITableView tableView,
        NSString cellKey,
        float sizeHint,
        Action<TCell>? initializeCellAction)
        where TCell : UITableViewCell =>
        sourceObservable.BindTo<TSource, TCell>(tableView, cellKey, sizeHint, initializeCellAction, initSource: null);

    /// <summary>
    /// Extension method that binds an observable of a collection
    /// as the source of a <see cref="UITableView"/>.
    /// </summary>
    /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
    /// <param name="sourceObservable">Source collection observable.</param>
    /// <param name="tableView">Table view.</param>
    /// <param name="cellKey">Cell key.</param>
    /// <param name="sizeHint">Size hint.</param>
    /// <param name="initializeCellAction">Initialize cell action.</param>
    /// <param name="initSource">Optionally initializes some property of
    /// the <see cref="ReactiveTableViewSource{TSource}"/>.</param>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<INotifyCollectionChanged> sourceObservable,
        UITableView tableView,
        NSString cellKey,
        float sizeHint,
        Action<TCell>? initializeCellAction,
        Func<ReactiveTableViewSource<TSource>, IDisposable>? initSource)
        where TCell : UITableViewCell =>
        new SelectObservable<INotifyCollectionChanged, TableSectionInformation<TSource, TCell>[]>(
                sourceObservable,
                src =>
                [
                    new TableSectionInformation<TSource, TCell>(
                                                                src,
                                                                cellKey,
                                                                sizeHint,
                                                                initializeCellAction),
                ])
            .BindTo(tableView, initSource);

    /// <summary>
    /// Extension method that binds an observable of a collection
    /// as the source of a <see cref="UITableView"/>.  Also registers
    /// the given class with an unspecified cellKey (you should probably
    /// not specify any other cellKeys).
    /// </summary>
    /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
    /// <param name="sourceObservable">Source collection observable.</param>
    /// <param name="tableView">Table view.</param>
    /// <param name="sizeHint">Size hint.</param>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameters",
        Justification = "Type parameters are part of the public binding API and specified at the call site.")]
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<INotifyCollectionChanged> sourceObservable,
        UITableView tableView,
        float sizeHint)
        where TCell : UITableViewCell =>
        sourceObservable.BindTo<TSource, TCell>(tableView, sizeHint, initializeCellAction: null);

    /// <summary>
    /// Extension method that binds an observable of a collection
    /// as the source of a <see cref="UITableView"/>.  Also registers
    /// the given class with an unspecified cellKey (you should probably
    /// not specify any other cellKeys).
    /// </summary>
    /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
    /// <param name="sourceObservable">Source collection observable.</param>
    /// <param name="tableView">Table view.</param>
    /// <param name="sizeHint">Size hint.</param>
    /// <param name="initializeCellAction">Initialize cell action.</param>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameters",
        Justification = "Type parameters are part of the public binding API and specified at the call site.")]
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<INotifyCollectionChanged> sourceObservable,
        UITableView tableView,
        float sizeHint,
        Action<TCell>? initializeCellAction)
        where TCell : UITableViewCell =>
        sourceObservable.BindTo<TSource, TCell>(tableView, sizeHint, initializeCellAction, initSource: null);

    /// <summary>
    /// Extension method that binds an observable of a collection
    /// as the source of a <see cref="UITableView"/>.  Also registers
    /// the given class with an unspecified cellKey (you should probably
    /// not specify any other cellKeys).
    /// </summary>
    /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
    /// <param name="sourceObservable">Source collection observable.</param>
    /// <param name="tableView">Table view.</param>
    /// <param name="sizeHint">Size hint.</param>
    /// <param name="initializeCellAction">Initialize cell action.</param>
    /// <param name="initSource">Optionally initializes some property of
    /// the <see cref="ReactiveTableViewSource{TSource}"/>.</param>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
    public static IDisposable BindTo<TSource, TCell>(
        this IObservable<INotifyCollectionChanged> sourceObservable,
        UITableView tableView,
        float sizeHint,
        Action<TCell>? initializeCellAction,
        Func<ReactiveTableViewSource<TSource>, IDisposable>? initSource)
        where TCell : UITableViewCell
    {
        ArgumentExceptionHelper.ThrowIfNull(tableView);

        var type = typeof(TCell);
        var cellKey = new NSString(type.ToString());
        tableView.RegisterClassForCellReuse(type, new NSString(cellKey));

        return sourceObservable
            .BindTo(tableView, cellKey, sizeHint, initializeCellAction, initSource);
    }
}
