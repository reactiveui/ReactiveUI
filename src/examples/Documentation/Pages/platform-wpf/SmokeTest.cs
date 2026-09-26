// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// Drives the grade book without a person at the keyboard, so a headless run on a real Windows machine can confirm
/// the app actually works. Started with the command-line argument <c>--smoke</c>.
/// </summary>
public static class SmokeTest
{
    /// <summary>Opens the main window, navigates through it, edits a grade, then closes it and exits.</summary>
    public static void Run()
    {
        MainWindow window = new();
        window.Show();
        PumpDispatcher();

        AppShell shell = window.ViewModel!;
        CourseListViewModel courseList = (CourseListViewModel)shell.Router.NavigationStack[0];
        Console.WriteLine($"Courses loaded: {courseList.Courses.Count}");
        Console.WriteLine($"Students listed: {courseList.Students.Count}");

        Student student = courseList.Students[0];
        courseList.SelectedStudent = student;
        PumpDispatcher();
        Console.WriteLine($"Selected in the summary panel: {courseList.Summary?.Student.Name}");

        _ = WaitForResult(courseList.OpenStudent.Execute(student));

        StudentDetailViewModel detail = (StudentDetailViewModel)shell.Router.NavigationStack[^1];
        Console.WriteLine($"Navigated to: {detail.UrlPathSegment}");

        detail.Grade = 97;
        PumpDispatcher();
        Console.WriteLine($"Typed grade: {detail.Grade}");

        _ = WaitForResult(detail.GoBack.Execute());
        Console.WriteLine($"Saved grade for {student.Name}: {student.Grade}");
        Console.WriteLine($"Back on: {shell.Router.NavigationStack[^1].UrlPathSegment}");

        Console.WriteLine("Closing window.");
        window.Close();

        Environment.Exit(0);
    }

    /// <summary>
    /// Waits for a command's result by pumping the dispatcher rather than blocking the thread. Commands deliver
    /// their result through the WPF main-thread scheduler, which posts back to this same dispatcher; blocking the
    /// thread while waiting for the result would deadlock, because nothing would be left running to process that
    /// post.
    /// </summary>
    /// <typeparam name="T">The type of the command's result.</typeparam>
    /// <param name="source">The command execution to wait for.</param>
    /// <returns>The value the command produced.</returns>
    private static T WaitForResult<T>(IObservable<T> source)
    {
        ResultBox<T> box = new();
        using IDisposable subscription = source.Subscribe(box.SetValue);

        while (!box.HasValue)
        {
            PumpDispatcher();
        }

        return box.Value!;
    }

    /// <summary>
    /// Runs the dispatcher queue until it is idle, the way a person clicking through the app would let WPF catch up
    /// between actions. Without this, bindings, transitions and the visual tree <c>BindWithValidation</c> relies on
    /// would not have run yet when the next line of this method reads their result.
    /// </summary>
    private static void PumpDispatcher()
    {
        DispatcherFrame frame = new();
        _ = Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new DispatcherOperationCallback(ExitFrame),
            frame);
        Dispatcher.PushFrame(frame);
    }

    /// <summary>Stops the dispatcher frame started by <see cref="PumpDispatcher"/>.</summary>
    /// <param name="frame">The frame to stop.</param>
    /// <returns>Always <see langword="null"/>; required by <see cref="DispatcherOperationCallback"/>.</returns>
    private static object? ExitFrame(object frame)
    {
        ((DispatcherFrame)frame).Continue = false;
        return null;
    }

    /// <summary>Holds the single value a subscription in <see cref="WaitForResult{T}"/> is waiting for.</summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    private sealed class ResultBox<T>
    {
        /// <summary>Gets a value indicating whether <see cref="SetValue"/> has been called.</summary>
        public bool HasValue { get; private set; }

        /// <summary>Gets the value <see cref="SetValue"/> was called with.</summary>
        public T? Value { get; private set; }

        /// <summary>Records the value.</summary>
        /// <param name="value">The value to record.</param>
        public void SetValue(T value)
        {
            Value = value;
            HasValue = true;
        }
    }
}
