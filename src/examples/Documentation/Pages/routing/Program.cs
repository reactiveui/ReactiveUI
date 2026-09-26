// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation;
using ReactiveUI.Documentation.Routing;

ExampleApp.Start();

await RoutingExamples.NavigateAndGoBack();

await RoutingExamples.EnableTheBackButton();

await RoutingExamples.WatchWhetherYouCanGoBack();

await RoutingExamples.WatchTheStack();

await RoutingExamples.ShowTheViewForTheCurrentPage();

await RoutingExamples.ResetTheStack();

await RoutingExamples.AwaitNavigationOnABackgroundSequencer();

RoutingExamples.DeliverNavigationImmediately();

await RoutingExamples.TellAPushFromAPop();

await RoutingExamples.WatchEachAddAndRemove();

await RoutingExamples.TrackPagesEnteringAndLeavingTheStack();

await RoutingExamples.FindAPageOfAGivenType();

await RoutingExamples.ReadTheTopOfTheStack();

await CookingTimerExamples.StartAndStopWorkOnAPage();

await CookingTimerExamples.ObserveArrivalAndDeparture();

await CookingTimerExamples.ObserveAPageThatIsOnTheStackTwice();
