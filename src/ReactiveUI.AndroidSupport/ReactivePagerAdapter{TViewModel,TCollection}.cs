// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.AndroidSupport;

/// <summary>
/// ReactivePagerAdapter is a PagerAdapter that will interface with a
/// Observable change set, in a similar fashion to ReactiveTableViewSource.
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
/// <typeparam name="TCollection">The type of collection.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ReactivePagerAdapter{TViewModel, TCollection}"/> class.
/// </remarks>
/// <param name="collection">The collection to page.</param>
/// <param name="viewCreator">The function which will create the view.</param>
/// <param name="viewInitializer">A action which will initialize the view.</param>
public class ReactivePagerAdapter<TViewModel, TCollection>(
    TCollection collection,
    Func<TViewModel, ViewGroup, View> viewCreator,
    Action<TViewModel, View>? viewInitializer = null) : ReactivePagerAdapter<TViewModel>(collection.ToObservableChangeSet<TCollection, TViewModel>(), viewCreator, viewInitializer)
    where TViewModel : class
    where TCollection : INotifyCollectionChanged, IEnumerable<TViewModel>
{
}
