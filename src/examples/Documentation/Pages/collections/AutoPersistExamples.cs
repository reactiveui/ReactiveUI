// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace ReactiveUI.Documentation.Collections;

/// <summary>Shows <c>ActOnEveryObject</c>, which calls a method for every object already in a collection and for every
/// one added or removed afterwards.</summary>
public static class AutoPersistExamples
{
    /// <summary><c>ActOnEveryObject</c> reports the items already in the collection first, then every later add and remove.</summary>
    public static void TrackAddsAndRemovesOnAnObservableCollection()
    {
        ObservableCollection<Product> inventory = [new Product("Kettle", 4)];
        List<string> log = [];
        using IDisposable subscription = inventory.ActOnEveryObject(
            product => log.Add($"add {product.Name}"),
            product => log.Add($"remove {product.Name}"));

        inventory.Add(new Product("Toaster", 2));
        inventory.RemoveAt(0);

        foreach (string entry in log)
        {
            Console.WriteLine(entry);
        }

        // Output:
        // add Kettle
        // add Toaster
        // remove Kettle
    }

    /// <summary><c>ActOnEveryObject</c> also works on the <see cref="ReadOnlyObservableCollection{T}"/> a view model exposes to a view.</summary>
    public static void TrackAddsAndRemovesOnAReadOnlyView()
    {
        ObservableCollection<Product> inventory = [new Product("Kettle", 4)];
        ReadOnlyObservableCollection<Product> readOnlyInventory = new(inventory);
        List<string> log = [];
        using IDisposable subscription = readOnlyInventory.ActOnEveryObject(
            product => log.Add($"add {product.Name}"),
            product => log.Add($"remove {product.Name}"));

        inventory.Add(new Product("Toaster", 2));

        foreach (string entry in log)
        {
            Console.WriteLine(entry);
        }

        // Output:
        // add Kettle
        // add Toaster
    }

    /// <summary><c>ActOnEveryObject</c> also subscribes directly to a change-set stream produced by <c>ToReactiveChangeSet</c>.</summary>
    public static void TrackAddsAndRemovesFromAChangeSetStream()
    {
        ObservableCollection<Product> inventory = [new Product("Kettle", 4)];
        List<string> log = [];
        using IDisposable subscription = inventory.ToReactiveChangeSet().ActOnEveryObject(
            product => log.Add($"add {product.Name}"),
            product => log.Add($"remove {product.Name}"));

        inventory.Add(new Product("Toaster", 2));

        foreach (string entry in log)
        {
            Console.WriteLine(entry);
        }

        // Output:
        // add Kettle
        // add Toaster
    }

    /// <summary>A collection that only raises <c>CollectionChanged</c> itself also works with <c>ActOnEveryObject</c>.</summary>
    public static void TrackAddsAndRemovesOnACustomCatalog()
    {
        ShopCatalog catalog = new();
        catalog.Stock(new Product("Kettle", 4));

        List<string> log = [];
        using IDisposable subscription = catalog.ActOnEveryObject<Product, ShopCatalog>(
            product => log.Add($"add {product.Name}"),
            product => log.Add($"remove {product.Name}"));

        catalog.Stock(new Product("Toaster", 2));
        catalog.SellOut(catalog.First(static product => product.Name == "Kettle"));

        foreach (string entry in log)
        {
            Console.WriteLine(entry);
        }

        // Output:
        // add Kettle
        // add Toaster
        // remove Kettle
    }
}
