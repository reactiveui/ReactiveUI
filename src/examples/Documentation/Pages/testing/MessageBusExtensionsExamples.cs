// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;

namespace ReactiveUI.Documentation.Testing;

/// <summary>Shows how a test isolates <see cref="MessageBus.Current"/> so one test's messages never reach another.</summary>
public static class MessageBusExtensionsExamples
{
    /// <summary><c>WithMessageBus</c> installs a message bus until the returned token is disposed, then restores the previous one.</summary>
    public static void SwapTheMessageBusManually()
    {
        MessageBus isolatedBus = new();
        IMessageBus originalBus = MessageBus.Current;

        using (isolatedBus.WithMessageBus())
        {
            Console.WriteLine(ReferenceEquals(MessageBus.Current, isolatedBus));
        }

        Console.WriteLine(ReferenceEquals(MessageBus.Current, originalBus));

        // Output:
        // True
        // True
    }

    /// <summary>The <c>Action</c> overload of <c>With</c> runs a side effect against an isolated message bus.</summary>
    public static void RecordAGradeOnAnIsolatedBus()
    {
        MessageBus isolatedBus = new();
        List<GradeRecorded> received = [];

        isolatedBus.With(() =>
        {
            using IDisposable subscription = MessageBus.Current.Listen<GradeRecorded>().Subscribe(received.Add);
            MessageBus.Current.SendMessage(new GradeRecorded("Ada Lovelace", 92));
        });

        Console.WriteLine(received.Count);
        Console.WriteLine(received[0].Grade);

        // Output:
        // 1
        // 92
    }

    /// <summary>
    /// The <c>Func&lt;TRet&gt;</c> overload of <c>With</c> runs a function against an isolated message bus and returns
    /// its result once the previous bus is restored.
    /// </summary>
    public static void ComputeAverageOnAnIsolatedBus()
    {
        MessageBus isolatedBus = new();

        double average = isolatedBus.With(static () =>
        {
            using GradeCalculatorViewModel viewModel = new StudentBuilder()
                .WithName("Grace Hopper")
                .WithGrades([88, 92, 79])
                .Build();

            MessageBus.Current.SendMessage(new GradeRecorded(viewModel.StudentName, viewModel.Grades[0]));
            return viewModel.Average;
        });

        Console.WriteLine(average);

        // Output:
        // 86.33333333333333
    }
}
