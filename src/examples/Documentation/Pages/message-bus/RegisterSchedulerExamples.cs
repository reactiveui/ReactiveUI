// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Messaging;

/// <summary>Shows <c>RegisterScheduler</c> choosing which sequencer delivers a message type's notifications.</summary>
public static class RegisterSchedulerExamples
{
    /// <summary>Without <c>RegisterScheduler</c> the bus delivers on <see cref="Sequencer.CurrentThread"/>, so the handler has already run by the time <c>SendMessage</c> returns.</summary>
    public static void DeliverOnTheCallingThreadByDefault()
    {
        MessageBus office = new MessageBus();
        bool handled = false;
        using IDisposable subscription = office.Listen<Announcement>().Subscribe(_ => handled = true);

        office.SendMessage(new Announcement("Sports day is cancelled"));

        Console.WriteLine(handled);

        // Output:
        // True
    }

    /// <summary><c>RegisterScheduler</c> with <see cref="Sequencer.Default"/> posts delivery to the thread pool, so a
    /// screen's handler runs on another thread instead of the one that called <c>SendMessage</c>.</summary>
    public static void RegisterSchedulerDeliversOnAnotherThread()
    {
        MessageBus office = new MessageBus();
        office.RegisterScheduler<Announcement>(Sequencer.Default);

        int callingThreadId = Environment.CurrentManagedThreadId;
        using ManualResetEventSlim delivered = new ManualResetEventSlim();
        bool deliveredOnAnotherThread = false;

        using IDisposable subscription = office.Listen<Announcement>().Subscribe(_ =>
        {
            deliveredOnAnotherThread = Environment.CurrentManagedThreadId != callingThreadId;
            delivered.Set();
        });

        office.SendMessage(new Announcement("Sports day is cancelled"));
        delivered.Wait();

        Console.WriteLine(deliveredOnAnotherThread);

        // Output:
        // True
    }

    /// <summary>The contract overload registers a sequencer for one contract only; a year group left unregistered keeps the default, synchronous delivery.</summary>
    public static void RegisterSchedulerPerYearGroup()
    {
        MessageBus office = new MessageBus();
        office.RegisterScheduler<Announcement>(Sequencer.Default, "Year7");

        int callingThreadId = Environment.CurrentManagedThreadId;
        using ManualResetEventSlim year7Delivered = new ManualResetEventSlim();
        bool year7OnAnotherThread = false;
        bool year8Handled = false;

        using IDisposable year7Subscription = office.Listen<Announcement>("Year7").Subscribe(_ =>
        {
            year7OnAnotherThread = Environment.CurrentManagedThreadId != callingThreadId;
            year7Delivered.Set();
        });
        using IDisposable year8Subscription = office.Listen<Announcement>("Year8").Subscribe(_ => year8Handled = true);

        office.SendMessage(new Announcement("Sports day is cancelled"), "Year7");
        year7Delivered.Wait();

        office.SendMessage(new Announcement("Sports day is cancelled"), "Year8");

        Console.WriteLine(year7OnAnotherThread);
        Console.WriteLine(year8Handled);

        // Output:
        // True
        // True
    }
}
