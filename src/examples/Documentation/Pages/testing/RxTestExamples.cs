// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using ReactiveUI.Testing;

namespace ReactiveUI.Documentation.Testing;

/// <summary>
/// Shows <see cref="RxTest.AppBuilderTestAsync(Func{Task})"/> directly. It serializes app-builder tests behind a
/// global gate, resets the builder before and after the test body, and fails the test if it runs too long.
/// <see cref="AppBuilderTestExamples"/> shows the same behavior through <see cref="AppBuilderTestBase"/>.
/// </summary>
public static class RxTestExamples
{
    /// <summary>Resets the builder, runs the test body, then resets it again, with the default 60 second timeout.</summary>
    /// <returns>A task that completes once the test body has run.</returns>
    public static Task RunWithTheDefaultTimeout()
    {
        Task test = RxTest.AppBuilderTestAsync(static () =>
        {
            IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder().WithMainThreadScheduler(Sequencer.Immediate);
            _ = builder.WithCoreServices().BuildApp();

            using GradeCalculatorViewModel viewModel = new StudentBuilder().WithName("Ada Lovelace").WithGrade(92).Build();
            Console.WriteLine(viewModel.StudentName);

            return Task.CompletedTask;
        });

        return test;

        // Output:
        // Ada Lovelace
    }

    /// <summary><c>AppBuilderTestAsync(Func&lt;Task&gt;, int)</c> takes an explicit timeout, in milliseconds, for both the gate and the test body.</summary>
    /// <returns>A task that completes once the test body has run.</returns>
    public static Task RunWithAnExplicitTimeout()
    {
        Task test = RxTest.AppBuilderTestAsync(
            static () =>
            {
                IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder().WithMainThreadScheduler(Sequencer.Immediate);
                _ = builder.WithCoreServices().BuildApp();

                using GradeCalculatorViewModel viewModel = new StudentBuilder().WithName("Grace Hopper").WithGrade(88).Build();
                Console.WriteLine(viewModel.Grades.Count);

                return Task.CompletedTask;
            },
            5_000);

        return test;

        // Output:
        // 1
    }
}
