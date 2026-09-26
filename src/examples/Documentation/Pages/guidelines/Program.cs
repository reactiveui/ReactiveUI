// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation;
using ReactiveUI.Documentation.Guidelines;

ExampleApp.Start();

IndexExamples.PreferRxAppBuilderForInitialization();

IndexExamples.PreferRxSchedulersOverTheRemovedRxApp();

await IndexExamples.PreferHandlingThrownExceptions();

await IndexExamples.PreferAwaitingAsyncWork();

CommandsExamples.AvoidWiringAClickHandlerDirectly();

await CommandsExamples.PreferBindingToACommand();

await CommandNamesExamples.PreferTheCommandSuffix();

await AsynchronousCommandsExamples.AvoidStartingAsyncWorkFromSubscribe();

await AsynchronousCommandsExamples.PreferAnAsyncCommand();

DisposeYourSubscriptionsExamples.AvoidNotDisposingTheSubscription();

DisposeYourSubscriptionsExamples.PreferDisposingWithActivation();

DisposeYourSubscriptionsExamples.NoNeedToDisposeSelfObservation();

PreferOaphOverPropertiesExamples.AvoidASettablePropertyAnyoneCanOverwrite();

PreferOaphOverPropertiesExamples.PreferAnObservableAsPropertyHelper();

await UiThreadAndSchedulersExamples.AvoidUpdatingWithoutMarshaling();

await UiThreadAndSchedulersExamples.PreferWitnessOnAtTheBoundary();

await UiThreadAndSchedulersExamples.PreferPassingTheSchedulerToTheOperation();

UseDescriptiveVariablesWithWhenAnyExamples.PreferDescriptiveParameterNames();

UseDescriptiveVariablesWithWhenAnyExamples.AvoidUnnamedParameters();

UseThisOnLeftOfWhenAnyExamples.PreferThisOnTheLeft();

UseThisOnLeftOfWhenAnyExamples.AvoidTheDependencyOnTheLeft();

UseThisOnLeftOfWhenAnyExamples.PreferDisposingEvenWithThisOnTheLeft();

await ThreadingExamples.AvoidWitnessOnAfterEveryStep();

await ThreadingExamples.PreferWitnessOnAtTheBoundary();

await ThreadingExamples.PreferLettingTheCommandMarshalItsResults();

EnableFrameworkLoggingExamples.PreferRegisteringALogger();
