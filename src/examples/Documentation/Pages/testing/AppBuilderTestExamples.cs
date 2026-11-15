// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using ReactiveUI.Testing;

namespace ReactiveUI.Documentation.Testing;

/// <summary>
/// Deriving from <see cref="AppBuilderTestBase"/> gives a test fixture the protected <c>RunAppBuilderTestAsync</c>
/// helpers, which reset the ReactiveUI builder before and after the test body and serialize it against every other
/// app-builder test in the process. <see cref="GradeCalculatorAppBuilderTests"/> is such a fixture; the methods here
/// create one and show what each helper prints.
/// </summary>
public static class AppBuilderTestExamples
{
    /// <summary><c>RunAppBuilderTestAsync(Action)</c> wraps a synchronous test body.</summary>
    /// <returns>A task that completes once the test body has run.</returns>
    public static Task BuildAStudentSynchronously()
    {
        GradeCalculatorAppBuilderTests fixture = new("Katherine Johnson");
        Task test = fixture.BuildAStudentSynchronously();

        return test;

        // Output:
        // Katherine Johnson
        // 92
    }

    /// <summary><c>RunAppBuilderTestAsync(Func&lt;Task&gt;)</c> wraps an asynchronous test body.</summary>
    /// <returns>A task that completes once the test body has run.</returns>
    public static Task RecordAGradeAsynchronously()
    {
        GradeCalculatorAppBuilderTests fixture = new("Rosalind Franklin");
        Task test = fixture.RecordAGradeAsynchronously();

        return test;

        // Output:
        // Rosalind Franklin
        // 84
    }

    /// <summary>A test fixture that derives from <see cref="AppBuilderTestBase"/> to reach its protected helpers.</summary>
    /// <param name="studentName">The student whose grades the fixture exercises.</param>
    private sealed class GradeCalculatorAppBuilderTests(string studentName) : AppBuilderTestBase
    {
        /// <summary>The student whose grades the fixture exercises.</summary>
        private readonly string _studentName = studentName;

        /// <summary>Builds a student and prints its name and average inside a reset app-builder context.</summary>
        /// <returns>A task that completes once the test body has run.</returns>
        public Task BuildAStudentSynchronously() =>
            RunAppBuilderTestAsync(() =>
            {
                IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder().WithMainThreadScheduler(Sequencer.Immediate);
                _ = builder.WithCoreServices().BuildApp();

                using GradeCalculatorViewModel viewModel = new StudentBuilder()
                    .WithName(_studentName)
                    .WithGrades([95, 89])
                    .Build();

                Console.WriteLine(viewModel.StudentName);
                Console.WriteLine(viewModel.Average);
            });

        /// <summary>Records a grade and prints it inside a reset app-builder context.</summary>
        /// <returns>A task that completes once the test body has run.</returns>
        public Task RecordAGradeAsynchronously() =>
            RunAppBuilderTestAsync(async () =>
            {
                IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder().WithMainThreadScheduler(Sequencer.Immediate);
                _ = builder.WithCoreServices().BuildApp();

                using GradeCalculatorViewModel viewModel = new StudentBuilder().WithName(_studentName).Build();

                GradeRecorded recorded = await viewModel.RecordGrade.Execute(84);
                Console.WriteLine(recorded.StudentName);
                Console.WriteLine(recorded.Grade);
            });
    }
}
