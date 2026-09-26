// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Specialized;

namespace ReactiveUI.Documentation.Collections;

/// <summary>
/// A shop's product catalog. It is not an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>, only a
/// plain list that raises <see cref="INotifyCollectionChanged.CollectionChanged"/>, the way a catalog backed by a
/// database read model might.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Count = {_products.Count}")]
public sealed class ShopCatalog : IEnumerable<Product>, INotifyCollectionChanged
{
    /// <summary>The products currently stocked.</summary>
    private readonly List<Product> _products = [];

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>Adds a product to the catalog.</summary>
    /// <param name="product">The product to stock.</param>
    public void Stock(Product product)
    {
        _products.Add(product);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, product, _products.Count - 1));
    }

    /// <summary>Removes a product from the catalog once it sells out.</summary>
    /// <param name="product">The product to remove.</param>
    public void SellOut(Product product)
    {
        int index = _products.IndexOf(product);
        _products.RemoveAt(index);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, product, index));
    }

    /// <inheritdoc/>
    public IEnumerator<Product> GetEnumerator() => _products.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
