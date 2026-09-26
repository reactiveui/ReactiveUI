// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Messaging;

/// <summary>Shows creating a <c>MessageBus</c>, sending and listening for messages, and checking whether a type is registered.</summary>
public static class MessageBusExamples
{
    /// <summary>A fresh <c>MessageBus</c> keeps its messages away from every other bus; useful for a self-contained feature or a test.</summary>
    public static void CreateAnIsolatedBus()
    {
        MessageBus office = new MessageBus();
        using IDisposable subscription = office.Listen<Announcement>().Subscribe(static announcement => Console.WriteLine(announcement.Text));

        office.SendMessage(new Announcement("Sports day starts at 9am"));

        // Output:
        // Sports day starts at 9am
    }

    /// <summary>A contract keeps messages for one year group away from a screen listening for the same message type with another contract.</summary>
    public static void SendPerYearGroup()
    {
        MessageBus office = new MessageBus();
        List<string> year7Notices = [];
        List<string> year8Notices = [];

        using IDisposable year7Subscription = office.Listen<ClassCancelled>("Year7").Subscribe(notice => year7Notices.Add(notice.ClassName));
        using IDisposable year8Subscription = office.Listen<ClassCancelled>("Year8").Subscribe(notice => year8Notices.Add(notice.ClassName));

        office.SendMessage(new ClassCancelled("Chemistry"), "Year7");

        Console.WriteLine(year7Notices.Count);
        Console.WriteLine(year8Notices.Count);

        // Output:
        // 1
        // 0
    }

    /// <summary><c>IsRegistered</c> reports whether a type, and optionally a contract, already has a listener or a sent message.</summary>
    public static void CheckWhetherAScreenIsListening()
    {
        MessageBus office = new MessageBus();

        Console.WriteLine(office.IsRegistered(typeof(Announcement)));

        using IDisposable subscription = office.Listen<Announcement>().Subscribe(static _ => { });

        Console.WriteLine(office.IsRegistered(typeof(Announcement)));
        Console.WriteLine(office.IsRegistered(typeof(Announcement), "Year7"));

        // Output:
        // False
        // True
        // False
    }

    /// <summary>A screen that does not hold its own bus reaches the shared one through <c>MessageBus.Current</c>; assigning it swaps the shared instance.</summary>
    public static void ShareTheAppWideBus()
    {
        IMessageBus original = MessageBus.Current;
        MessageBus.Current = new MessageBus();

        using IDisposable subscription = MessageBus.Current.Listen<Announcement>().Subscribe(static announcement => Console.WriteLine(announcement.Text));
        MessageBus.Current.SendMessage(new Announcement("Assembly starts at 9am"));

        MessageBus.Current = original;

        // Output:
        // Assembly starts at 9am
    }
}
