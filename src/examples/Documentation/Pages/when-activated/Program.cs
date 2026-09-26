// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using ReactiveUI.Documentation;
using ReactiveUI.Documentation.WhenActivated;

ExampleApp.Start(static builder => builder.WithRegistration(
    static resolver => resolver.RegisterConstant<IActivationForViewFetcher>(new LegacyPanelActivationFetcher())));

WhenActivatedExamples.ActivateAViewModel();

WhenActivatedExamples.ActivateAView();

await WhenActivatedExamples.ReactivateToRefresh();

ScoreBoardActivationExamples.UpdateTheTitleWithAFunctionBlock();

ScoreBoardActivationExamples.UpdateTheTitleWithADisposablesContainer();

ScoreBoardActivationExamples.ActivateOnlyTheContentViewModel();

ScoreBoardActivationExamples.InspectTheBuiltInActivationFetcher();

ScoreBoardViewModelExamples.ActivateAndDeactivateWithARefCount();

ScoreBoardViewModelExamples.ForceDeactivateRegardlessOfRefCount();

ScoreBoardViewModelExamples.LoadHistoryOnActivation();

ManualActivationExamples.ForceATileActiveFromTheDashboard();

ManualActivationExamples.ActivateALegacyControlThroughACustomFetcher();
