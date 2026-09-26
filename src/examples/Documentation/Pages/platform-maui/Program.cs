// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation;
using ReactiveUI.Documentation.PlatformMaui;

ExampleApp.Start();

PageBasesExamples.ContentPageSyncsViewModelAndBindingContext();

PageBasesExamples.OtherPageAndViewBasesFollowTheSamePattern();

PageBasesExamples.WindowAndTitleBarFollowTheSamePattern();

PageBasesExamples.ReactivePageAddsBindingRoot();

PageBasesExamples.ShellFollowsTheSamePattern();

ItemViewExamples.TextItemViewShowsPrimaryAndDetailText();

ItemViewExamples.ImageItemViewAddsAPhoto();

ShellContentExamples.ContractPicksTheContentTemplate();

RoutedViewHostExamples.ConstructingWithoutAScreenThrows();

RoutedViewHostExamples.RouterAndSetTitleOnNavigateAreBindableProperties();

await RoutedViewHostExamples.SyncingPushesTheRouterStackOntoTheNavigationPage();

RoutedViewHostExamples.ResolvingAPageForAViewModel();

RoutedViewHostExamples.GenericHostResolvesWithoutReflection();

await RoutedViewHostExamples.InvalidatingRefreshesTheCurrentPagesViewModel();

ViewModelViewHostExamples.ViewModelResolvesTheRegisteredView();

ViewModelViewHostExamples.NoViewModelShowsTheDefaultContent();

ViewModelViewHostExamples.ContractPropertiesDoNotReResolveAfterConstruction();

ViewModelViewHostExamples.ContractFallbackByPassIsABindableProperty();

ViewModelViewHostExamples.ViewLocatorOverridesTheDefaultLocator();

ViewModelViewHostExamples.GenericHostTypesTheViewModelProperty();

AutoSuspendHelperExamples.LifecycleMethodsRelayToTheSuspensionHost();

AutoSuspendHelperExamples.UntimelyDemiseIsAStaticSignal();

ActivationForViewFetcherExamples.RankViewTypes();

ActivationForViewFetcherExamples.ObservingActivationNeedsARunningApp();

RegistrationsExamples.RegisterPlatformServices();

PlatformOperationsExamples.CheckOrientation();

DisableAnimationAttributeExamples.MarksAPageToSkipItsPushAnimation();

VisibilityConverterExamples.BooleanToVisibilityAppliesEachHint();

VisibilityConverterExamples.VisibilityToBooleanConvertsBack();

BuilderExamples.WithMauiConvertersRegistersTheVisibilityConverters();

BuilderExamples.WithMauiSchedulerAcceptsAnExplicitDispatcher();

BuilderExamples.WithMauiAcceptsAnExplicitDispatcher();

BuilderExamples.UseReactiveUiWithADispatcherConfiguresMaui();

BuilderExamples.UseReactiveUiWithADelegateAddsRegistrations();
