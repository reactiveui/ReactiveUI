// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace ReactiveUI.Documentation.Collections;

/// <summary>Shows how a collection's changes become a stream of <see cref="IReactiveChangeSet{T}"/> batches.</summary>
public static class ChangeSetExamples
{
    /// <summary><c>ToReactiveChangeSet</c> turns an <see cref="ObservableCollection{T}"/> into a stream of batches, one per change.</summary>
    public static void ObserveInventoryAsChangeSet()
    {
        ObservableCollection<Product> inventory = [new Product("Kettle", 4), new Product("Toaster", 2)];
        List<IReactiveChangeSet<Product>> batches = [];
        using IDisposable subscription = inventory.ToReactiveChangeSet().Subscribe(batches.Add);

        inventory.Add(new Product("Blender", 3));
        inventory.RemoveAt(0);
        inventory[0] = new Product("Air fryer", 1);
        inventory.Move(0, 1);

        foreach (IReactiveChangeSet<Product> batch in batches)
        {
            foreach (ReactiveChange<Product> change in batch)
            {
                Console.WriteLine($"{change.Reason}: {change.Current.Name}, previous={change.Previous?.Name ?? "none"}, index={change.CurrentIndex}, previousIndex={change.PreviousIndex}");
            }
        }

        // Output:
        // Add: Kettle, previous=none, index=0, previousIndex=-1
        // Add: Toaster, previous=none, index=1, previousIndex=-1
        // Add: Blender, previous=none, index=2, previousIndex=-1
        // Remove: Kettle, previous=none, index=0, previousIndex=-1
        // Replace: Air fryer, previous=Toaster, index=0, previousIndex=-1
        // Move: Air fryer, previous=none, index=1, previousIndex=0
    }

    /// <summary>A collection backed by its own <c>INotifyCollectionChanged</c> event also becomes a change-set stream, via
    /// the <c>ToReactiveChangeSet&lt;TCollection, T&gt;</c> overload.</summary>
    public static void ObserveACustomCatalogAsChangeSet()
    {
        ShopCatalog catalog = new();
        catalog.Stock(new Product("Kettle", 4));

        List<IReactiveChangeSet<Product>> batches = [];
        using IDisposable subscription = catalog.ToReactiveChangeSet<ShopCatalog, Product>().Subscribe(batches.Add);

        catalog.Stock(new Product("Toaster", 2));

        foreach (IReactiveChangeSet<Product> batch in batches)
        {
            Console.WriteLine($"{batch.Count} changes, adds={batch.Adds}, removes={batch.Removes}");
        }

        // Output:
        // 1 changes, adds=1, removes=0
        // 1 changes, adds=1, removes=0
    }

    /// <summary><c>WhenCountChanged</c> passes on only the batches that add or remove an item, skipping a batch that only replaces one.</summary>
    public static void WatchForCountChangingBatches()
    {
        ObservableCollection<Product> inventory = [new Product("Kettle", 4)];
        List<IReactiveChangeSet<Product>> countChangingBatches = [];
        using IDisposable subscription = inventory.ToReactiveChangeSet().WhenCountChanged().Subscribe(countChangingBatches.Add);

        inventory[0] = new Product("Air fryer", 1);
        inventory.Add(new Product("Toaster", 2));

        foreach (IReactiveChangeSet<Product> batch in countChangingBatches)
        {
            Console.WriteLine($"{batch.Count} changes, adds={batch.Adds}, removes={batch.Removes}");
        }

        // Output:
        // 1 changes, adds=1, removes=0
        // 1 changes, adds=1, removes=0
    }

    /// <summary><c>CountHasChanged</c> asks a single batch whether it added or removed an item.</summary>
    public static void DetectCountChangingBatches()
    {
        ObservableCollection<Product> inventory = [new Product("Kettle", 4)];
        List<IReactiveChangeSet<Product>> allBatches = [];
        using IDisposable subscription = inventory.ToReactiveChangeSet().Subscribe(allBatches.Add);

        inventory[0] = new Product("Air fryer", 1);
        inventory.Add(new Product("Toaster", 2));

        foreach (IReactiveChangeSet<Product> batch in allBatches)
        {
            Console.WriteLine(batch.CountHasChanged());
        }

        // Output:
        // True
        // False
        // True
    }

    /// <summary>A screen's change handler is tested directly by building the change it should react to, including a
    /// <see cref="ReactiveChangeReason.Refresh"/>, which the library never produces from a plain collection change.</summary>
    public static void TestTheChangeHandlerDirectly()
    {
        Product kettle = new("Kettle", 4);
        ReactiveChange<Product> refreshed = new(ReactiveChangeReason.Refresh, kettle, default, 0, -1);

        Console.WriteLine(DescribeChange(refreshed));

        // Output:
        // redraw Kettle
    }

    /// <summary>Two changes with the same reason, item and indices are equal, and hash the same.</summary>
    public static void CompareChangesForEquality()
    {
        Product kettle = new("Kettle", 4);
        ReactiveChange<Product> first = new(ReactiveChangeReason.Add, kettle, default, 0, -1);
        ReactiveChange<Product> second = new(ReactiveChangeReason.Add, kettle, default, 0, -1);
        ReactiveChange<Product> third = new(ReactiveChangeReason.Remove, kettle, default, 0, -1);

        Console.WriteLine(first.Equals(second));
        Console.WriteLine(first.Equals((object)second));
        Console.WriteLine(first.Equals(third));
        Console.WriteLine(first.GetHashCode() == second.GetHashCode());

        // Output:
        // True
        // True
        // False
        // True
    }

    /// <summary>A <see cref="ReactiveChangeSet{T}"/> is built directly for a test, the way a unit test for a handler would.</summary>
    public static void BuildAChangeSetByHand()
    {
        Product kettle = new("Kettle", 4);
        Product toaster = new("Toaster", 2);
        List<ReactiveChange<Product>> changes =
        [
            new(ReactiveChangeReason.Add, kettle, default, 0, -1),
            new(ReactiveChangeReason.Add, toaster, default, 1, -1),
        ];

        ReactiveChangeSet<Product> batch = new(changes);

        Console.WriteLine(batch.Count);
        Console.WriteLine(batch.Adds);
        Console.WriteLine(batch.Removes);
        Console.WriteLine(batch[0].Current.Name);

        foreach (ReactiveChange<Product> change in batch)
        {
            Console.WriteLine(change.Current.Name);
        }

        // Output:
        // 2
        // 2
        // 0
        // Kettle
        // Kettle
        // Toaster
    }

    /// <summary>A change from a shop's inventory is a screen might show it. Refresh has no <see cref="ObservableCollection{T}"/>
    /// equivalent, so a handler that reacts to it is tested by building the change directly.</summary>
    /// <param name="change">The change to describe.</param>
    /// <returns>What the screen would do for this change.</returns>
    private static string DescribeChange(in ReactiveChange<Product> change) => change.Reason switch
    {
        ReactiveChangeReason.Add => $"show {change.Current.Name}",
        ReactiveChangeReason.Remove => $"hide {change.Current.Name}",
        ReactiveChangeReason.Replace => $"swap {change.Previous?.Name} for {change.Current.Name}",
        ReactiveChangeReason.Move => $"reposition {change.Current.Name}",
        ReactiveChangeReason.Refresh => $"redraw {change.Current.Name}",
        _ => "unknown",
    };
}
