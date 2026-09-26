// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using ReactiveUI.Documentation.Rxappbuilder;

// This page is about starting ReactiveUI itself, so it never calls ExampleApp.Start(). Each example below builds
// its own app on its own Splat resolver instead, so the examples stay independent of one another. Only the first
// build in a process applies, and a later one changes nothing, so BuildTheRecipeBookApp is the only example that
// calls BuildApp(), the way an app builds once at start-up.
StartupExamples.EnsureInitializedThrowsBeforeBuild();

ReactiveUIBuilder recipeBookApp = StartupExamples.BuildTheRecipeBookApp();

StartupExamples.ConfirmTheAppInitialized();

StartupExamples.ResolveRecipeBookViewsAndViewModels(recipeBookApp);

StartupExamples.ResolveRecipeBookModulesAndViews(recipeBookApp);

IsolatedBuilderExamples.ConfigureAnIsolatedBuilder();

SchedulerExamples.ConfigureSchedulersWithExplicitRxAppFlag();

ConverterExamples.ConfigureBindingConverters();

ConverterExamples.ImportConvertersFromAnotherResolver();

MessageBusExamples.ChooseHowTheAppGetsItsMessageBus();

StartupExamples.ALaterBuildChangesNothing();
