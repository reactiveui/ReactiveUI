// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation;
using ReactiveUI.Documentation.Reflection;

ExampleApp.Start();

IntroExamples.ExplainWhyThisProjectRunsOnTheJit();

WhenActivatedReflectionExamples.ActivateWithoutABlock();

WhenActivatedReflectionExamples.ActivateWithAFunctionBlock();

WhenActivatedReflectionExamples.ActivateWithACallbackBlock();

WhenActivatedReflectionExamples.ActivateWithADisposablesContainer();

WhenActivatedReflectionExamples.ActivateAViewGivenExplicitly();

AutoPersistReflectionExamples.PersistsASingleTrackAfterTheDefaultQuietPeriod();

await AutoPersistReflectionExamples.PersistsASingleTrackAfterAnExplicitQuietPeriod();

await AutoPersistReflectionExamples.ManualSaveSignalForcesASingleTrackToSave();

AutoPersistReflectionExamples.ManualSaveSignalWithoutIntervalUsesTheDefault();

await AutoPersistReflectionExamples.CollectionOverloadsDifferOnlyByInterval();

await AutoPersistReflectionExamples.CollectionManualSaveOverloads();

await AutoPersistReflectionExamples.ReadOnlyCollectionManualSaveOverloads();

await SuspensionReflectionExamples.SaveAndLoadThroughTheUntypedDriver();

SuspensionReflectionExamples.ReadAndObserveTheUntypedAppState();

SuspensionReflectionExamples.SetUpSuspendAndResumeWithAndWithoutAnExplicitDriver();

RegistrationReflectionExamples.ScanAnAssemblyForViews();

RegistrationReflectionExamples.RegisterViewsWhileBuildingTheApp();

ValidationReflectionExamples.RequiredAttributeFailsForAnEmptyTitle();
