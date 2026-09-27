// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using ReactiveUI.Builder;
using ReactiveUI.Documentation;
using ReactiveUI.Documentation.ViewLocation;

ExampleApp.Start(static builder => builder
    .WithViewModule<GitHubViewModule>()
    .WithRegistration(static resolver => resolver.RegisterConstant<IActivationForViewFetcher>(new VendorDialogActivationFetcher())));

ViewLocationExamples.FindTheViewForAViewModel();

ViewLocationExamples.PickAViewByContract();

ViewLocationExamples.MapViewsInAModule();

ViewLocationExamples.WrapTheViewLocator();

SchoolTimetableExamples.MapDayAndWeekViewsAndResolveThemWithoutAnInstance();

SchoolTimetableExamples.MapATeacherViewAtSignInAndUnmapAtSignOut();

SchoolTimetableExamples.MapViewsWithABuilderIncludingFromTheServiceLocator();

SchoolTimetableExamples.BuildAViewLocatorNotFoundException();

ExtendingIViewForExamples.ConfirmABookReturnThroughAVendorDialog();

ExtendingIViewForExamples.LocateTheVendorDialogLikeAnyOtherView();
