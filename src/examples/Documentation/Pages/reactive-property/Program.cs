// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation;
using ReactiveUI.Documentation.ReactiveProperty;

ExampleApp.Start();

ConstructionExamples.CreateWithDefaultConstructor();

ConstructionExamples.CreateWithInitialValue();

ConstructionExamples.CreateWithFactoryMethods();

ConstructionExamples.SubscribeWithAWitness();

ConstructionExamples.RefreshForcesReEmission();

ConstructionExamples.OverrideDisposeForCleanup();

await SubscriptionBehaviorExamples.ConstructWithoutAnExplicitScheduler();

SubscriptionBehaviorExamples.SkipCurrentValueOnSubscribe();

SubscriptionBehaviorExamples.AllowDuplicateValues();

SyncValidationExamples.ValidateWithAStringMessage();

SyncValidationExamples.ValidateWithMultipleErrors();

SyncValidationExamples.GetErrorsReturnsCurrentErrors();

SyncValidationExamples.EventArgsAreCachedInstances();

ObservableValidationExamples.ValidateAStreamWithAStringMessage();

ObservableValidationExamples.ValidateAStreamWithMultipleErrors();

ObservableValidationExamples.ObserveErrorStreams();

await AsyncValidationExamples.ValidateAsynchronouslyWithAStringMessage();

await AsyncValidationExamples.ValidateAsynchronouslyWithMultipleErrors();

IReactivePropertyExamples.UseThroughTheInterface();

ReactivePropertyMixinsExamples.ObserveValidationErrorsAsAString();
