// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation;
using ReactiveUI.Documentation.Testing;

ExampleApp.Start();

BuilderExamples.SetASingleField();

BuilderExamples.AddGradesOneAtATimeOrAllAtOnce();

BuilderExamples.SetCourseCreditsThreeWays();

SchedulerExtensionsExamples.SwapSchedulersForABlock();

SchedulerExtensionsExamples.ComputeAverageUnderATestScheduler();

SchedulerExtensionsExamples.RecordAGradeUnderATestScheduler();

await SchedulerExtensionsExamples.ComputeAverageAsyncUnderATestScheduler();

await SchedulerExtensionsExamples.RecordAGradeAsyncUnderATestScheduler();

MessageBusExtensionsExamples.SwapTheMessageBusManually();

MessageBusExtensionsExamples.RecordAGradeOnAnIsolatedBus();

MessageBusExtensionsExamples.ComputeAverageOnAnIsolatedBus();

await TestSequencerExamples.StepAGradeRecorderPhaseByPhase();

// The app-builder test helpers reset the ReactiveUI builder before and after their own test body, so they run last.
await RxTestExamples.RunWithTheDefaultTimeout();

await RxTestExamples.RunWithAnExplicitTimeout();

await AppBuilderTestExamples.BuildAStudentSynchronously();

await AppBuilderTestExamples.RecordAGradeAsynchronously();
