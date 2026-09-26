// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using ReactiveUI.Documentation.RoutingDynamicData;

// This page does not use ExampleApp.Start(), because it opts out of the shared Common apps (see routing-dynamicdata.csproj);
// it registers ReactiveUI's services itself, the way ExampleApp does for every other page.
IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder().WithMainThreadScheduler(Sequencer.Immediate);

_ = builder.WithCoreServices().BuildApp();

await NavigationChangeSetExamples.ObserveNavigationAsDynamicDataChangeSet();

await NavigationChangeSetExamples.ConvertNavigationChangesDirectly();

await NavigationChangeSetExamples.FilterGenericNavigationChangesByCount();

await NavigationChangeSetExamples.FilterNonGenericNavigationChangesByCount();

await NavigationChangeSetExamples.TrackPagesEnteringAndLeavingTheStack();
