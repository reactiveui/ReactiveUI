// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>
/// Shows the three ways to give the builder a message bus. <see cref="StartupExamples.BuildTheRecipeBookApp"/>
/// uses the third, passing an instance the rest of the app already holds a reference to.
/// </summary>
public static class MessageBusExamples
{
    /// <summary>
    /// <c>WithMessageBus()</c> and <c>WithMessageBus(configure)</c> both build a new bus when the app is built;
    /// <c>WithMessageBus(bus)</c> takes one the caller already made, which is why it is the one this page's app uses.
    /// </summary>
    public static void ChooseHowTheAppGetsItsMessageBus()
    {
        using ModernDependencyResolver resolver = new();
        MessageBus kitchenEvents = new();
        List<string> announcements = [];
        using IDisposable subscription = kitchenEvents.Listen<string>().Subscribe(announcements.Add);

        _ = resolver.CreateReactiveUIBuilder()
            .WithMessageBus()
            .WithMessageBus(static bus => bus.RegisterScheduler<string>(Sequencer.Immediate))
            .WithMessageBus(kitchenEvents);

        kitchenEvents.SendMessage("New recipe: Tomato Soup");

        Console.WriteLine(announcements[0]);

        // Output:
        // New recipe: Tomato Soup
    }
}
