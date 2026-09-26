// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>
/// Shows the overloads <see cref="StartupExamples.BuildTheRecipeBookApp"/> does not: the two-argument
/// <c>WithMainThreadScheduler</c> and the one-argument <c>WithTaskPoolScheduler</c>.
/// </summary>
public static class SchedulerExamples
{
    /// <summary>The <c>setRxApp</c> argument decides whether the scheduler also becomes RxApp's own once the app is built.</summary>
    public static void ConfigureSchedulersWithExplicitRxAppFlag()
    {
        using ModernDependencyResolver resolver = new();
        ReactiveUIBuilder builder = resolver.CreateReactiveUIBuilder();

        _ = builder
            .WithMainThreadScheduler(Sequencer.Immediate, true)
            .WithTaskPoolScheduler(TaskPoolSequencer.Default);

        Console.WriteLine(ReferenceEquals(builder.MainThreadScheduler, Sequencer.Immediate));
        Console.WriteLine(ReferenceEquals(builder.TaskpoolScheduler, TaskPoolSequencer.Default));

        // Output:
        // True
        // True
    }
}
