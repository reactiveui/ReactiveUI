// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ReactiveUI.Documentation.Collections;

/// <summary>Shows the raw <see cref="INotifyCollectionChanged.CollectionChanged"/> event as a stream of notifications.</summary>
public static class CollectionChangedExamples
{
    /// <summary><c>ObserveCollectionChanges</c> forwards each raw <c>CollectionChanged</c> event, carrying the sender and its event arguments.</summary>
    public static void ObserveRawCollectionChangedEvents()
    {
        ObservableCollection<Product> inventory = [new Product("Kettle", 4)];
        List<CollectionChanged> events = [];
        using IDisposable subscription = inventory.ObserveCollectionChanges().Subscribe(events.Add);

        inventory.Add(new Product("Toaster", 2));
        inventory.RemoveAt(0);

        foreach (CollectionChanged notification in events)
        {
            Console.WriteLine($"{notification.EventArgs.Action}, sender is inventory: {ReferenceEquals(notification.Sender, inventory)}");
        }

        // Output:
        // Add, sender is inventory: True
        // Remove, sender is inventory: True
    }

    /// <summary>Two notifications built from the same sender and event arguments are equal, and hash the same.</summary>
    public static void CompareNotificationsForEquality()
    {
        ObservableCollection<Product> inventory = [];
        NotifyCollectionChangedEventArgs sharedArgs = new(NotifyCollectionChangedAction.Reset);

        CollectionChanged first = new(inventory, sharedArgs);
        CollectionChanged second = new(inventory, sharedArgs);
        CollectionChanged third = new(inventory, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        Console.WriteLine(first == second);
        Console.WriteLine(first.Equals(second));
        Console.WriteLine(first.Equals((object)second));
        Console.WriteLine(first != third);
        Console.WriteLine(first.GetHashCode() == second.GetHashCode());
        Console.WriteLine(first.Sender is ObservableCollection<Product>);
        Console.WriteLine(first.EventArgs.Action);

        // Output:
        // True
        // True
        // True
        // True
        // True
        // True
        // Reset
    }
}
