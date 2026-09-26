// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>Shows how a <see cref="ReactiveProperty{T}"/> is built, holds its value, and is disposed.</summary>
public static class ConstructionExamples
{
    /// <summary>The default constructor starts with the type's default value.</summary>
    public static void CreateWithDefaultConstructor()
    {
        ReactiveProperty<string> studentName = new();

        Console.WriteLine(studentName.Value is null);
        Console.WriteLine(studentName.IsDisposed);

        studentName.Dispose();
        Console.WriteLine(studentName.IsDisposed);

        // Output:
        // True
        // False
        // True
    }

    /// <summary>An initial value can be given directly.</summary>
    public static void CreateWithInitialValue()
    {
        ReactiveProperty<string> club = new("Chess Club");

        Console.WriteLine(club.Value);

        club.Dispose();

        // Output:
        // Chess Club
    }

    /// <summary><c>Create</c> mirrors every constructor without needing the <c>new</c> keyword.</summary>
    public static void CreateWithFactoryMethods()
    {
        ReactiveProperty<string> empty = ReactiveProperty<string>.Create();
        ReactiveProperty<string> clubName = ReactiveProperty<string>.Create("Robotics Club");
        ReactiveProperty<string> withFlags = ReactiveProperty<string>.Create("Robotics Club", skipCurrentValueOnSubscribe: false, allowDuplicateValues: true);
        ReactiveProperty<string> withScheduler = ReactiveProperty<string>.Create("Robotics Club", RxSchedulers.MainThreadScheduler, false, false);

        Console.WriteLine(empty.Value is null);
        Console.WriteLine(clubName.Value);
        Console.WriteLine(withFlags.Value);
        Console.WriteLine(withScheduler.Value);

        empty.Dispose();
        clubName.Dispose();
        withFlags.Dispose();
        withScheduler.Dispose();

        // Output:
        // True
        // Robotics Club
        // Robotics Club
        // Robotics Club
    }

    /// <summary>
    /// <c>Subscribe(IObserver{T})</c> hands notifications to a plain observer; a custom scheduler such as
    /// <see cref="RxSchedulers.MainThreadScheduler"/> delivers them in the order they happened, which keeps this
    /// console output deterministic.
    /// </summary>
    public static void SubscribeWithAWitness()
    {
        ReactiveProperty<string> club = new("Chess Club", RxSchedulers.MainThreadScheduler, false, false);
        ConsoleWitness<string?> witness = new("ChosenClub");

        IDisposable subscription = club.Subscribe(witness);
        club.Value = "Robotics Club";
        subscription.Dispose();
        club.Value = "Art Club";

        club.Dispose();

        // Output:
        // ChosenClub: Chess Club
        // ChosenClub: Robotics Club
    }

    /// <summary><c>Refresh</c> re-sends the current value even when it has not changed, unlike setting <c>Value</c> again.</summary>
    public static void RefreshForcesReEmission()
    {
        ReactiveProperty<string> club = new("Chess Club", RxSchedulers.MainThreadScheduler, false, false);
        ConsoleWitness<string?> witness = new("ChosenClub");
        IDisposable subscription = club.Subscribe(witness);

        club.Value = "Chess Club";
        club.Refresh();

        subscription.Dispose();
        club.Dispose();

        // Output:
        // ChosenClub: Chess Club
        // ChosenClub: Chess Club
    }

    /// <summary>
    /// A type that derives from <see cref="ReactiveProperty{T}"/> can override <c>Dispose(bool)</c> to release its
    /// own resources; here it prints which form field was disposed.
    /// </summary>
    public static void OverrideDisposeForCleanup()
    {
        LoggingReactiveProperty<string> studentName = new("StudentName", "Ada Lovelace");

        studentName.Dispose();
        Console.WriteLine(studentName.IsDisposed);

        // Output:
        // Disposed StudentName
        // True
    }
}
