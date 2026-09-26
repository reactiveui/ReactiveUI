// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.Documentation.ReactiveObjects;

/// <summary>Shows the <c>Changing</c>/<c>Changed</c> observables, <c>ThrownExceptions</c>, and notification control on <see cref="ReactiveObject"/>.</summary>
public static class ChangeNotificationExamples
{
    /// <summary><c>Changing</c> fires before a property changes and <c>Changed</c> fires after; both name the sender and the property.</summary>
    public static void ObserveChangingAndChanged()
    {
        Student student = new() { Name = "Ada Lovelace" };
        List<string> events = [];
        using IDisposable changingSubscription = student.Changing.Subscribe(args => events.Add($"changing {args.PropertyName} on {((Student)args.Sender).Name}"));
        using IDisposable changedSubscription = student.Changed.Subscribe(args => events.Add($"changed {args.PropertyName} on {((Student)args.Sender).Name}"));

        student.Course = "Mathematics";

        Console.WriteLine(string.Join(" -> ", events));

        // Output:
        // changing Course on Ada Lovelace -> changed Course on Ada Lovelace
    }

    /// <summary>An exception thrown by a <c>Changed</c> subscriber is caught and forwarded to <c>ThrownExceptions</c> instead of crashing the setter.</summary>
    public static void ObserveThrownExceptions()
    {
        Student student = new() { Name = "Ada Lovelace", Course = "Mathematics" };
        List<Exception> errors = [];
        using IDisposable errorSubscription = student.ThrownExceptions.Subscribe(errors.Add);
        using IDisposable changeSubscription = student.Changed.Subscribe(
            static _ => throw new InvalidOperationException("Course change must go through the registrar."));

        student.Course = "Physics";

        Console.WriteLine(errors.Count);
        Console.WriteLine(errors[0].Message);

        // Output:
        // 1
        // Course change must go through the registrar.
    }

    /// <summary><c>SuppressChangeNotifications</c> stops both events and observables until the disposable is disposed; <c>AreChangeNotificationsEnabled</c> reports the current state.</summary>
    public static void SuppressAndCheckEnabled()
    {
        Student student = new() { Name = "Ada Lovelace", Course = "Mathematics" };
        List<string> changed = [];
        using IDisposable subscription = student.Changed.Subscribe(args => changed.Add(args.PropertyName ?? string.Empty));

        Console.WriteLine(student.AreChangeNotificationsEnabled());

        using (student.SuppressChangeNotifications())
        {
            Console.WriteLine(student.AreChangeNotificationsEnabled());
            student.Course = "Physics";
        }

        Console.WriteLine(student.AreChangeNotificationsEnabled());
        Console.WriteLine(changed.Count);

        // Output:
        // True
        // False
        // True
        // 0
    }

    /// <summary><c>DelayChangeNotifications</c> still raises every change once the disposable is disposed, collapsed to the last change per property.</summary>
    public static void DelayNotificationsDuringABatchEdit()
    {
        Student student = new() { Name = "Ada Lovelace", Course = "Mathematics" };
        List<string> changed = [];
        using IDisposable subscription = student.Changed.Subscribe(args => changed.Add(args.PropertyName ?? string.Empty));

        using (student.DelayChangeNotifications())
        {
            student.Course = "Physics";
            student.Course = "Chemistry";
            Console.WriteLine(changed.Count);
        }

        Console.WriteLine(changed.Count);
        Console.WriteLine(student.Course);

        // Output:
        // 0
        // 1
        // Chemistry
    }

    /// <summary>
    /// <c>IReactiveObject.RaisePropertyChanging</c> and <c>RaisePropertyChanged</c> only raise the classic
    /// <see cref="INotifyPropertyChanged"/> events; they skip the <c>Changing</c>/<c>Changed</c> observables and the
    /// suppression check. Prefer <c>RaiseAndSetIfChanged</c> or <c>RaisePropertyChanged(string)</c> unless you are
    /// implementing <see cref="IReactiveObject"/> yourself.
    /// </summary>
    public static void RaiseTheClassicEventsDirectly()
    {
        Student student = new() { Name = "Ada Lovelace" };
        int classicChanging = 0;
        int classicChanged = 0;
        int observableChanged = 0;
        student.PropertyChanging += (_, _) => classicChanging++;
        student.PropertyChanged += (_, _) => classicChanged++;
        using IDisposable subscription = student.Changed.Subscribe(_ => observableChanged++);

        IReactiveObject rawStudent = student;
        rawStudent.RaisePropertyChanging(new PropertyChangingEventArgs(nameof(Student.Course)));
        rawStudent.RaisePropertyChanged(new PropertyChangedEventArgs(nameof(Student.Course)));

        Console.WriteLine(classicChanging);
        Console.WriteLine(classicChanged);
        Console.WriteLine(observableChanged);

        // Output:
        // 1
        // 1
        // 0
    }
}
