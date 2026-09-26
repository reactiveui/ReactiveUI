// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation;
using ReactiveUI.Documentation.Collections;

ExampleApp.Start();

ChangeSetExamples.ObserveInventoryAsChangeSet();

ChangeSetExamples.ObserveACustomCatalogAsChangeSet();

ChangeSetExamples.WatchForCountChangingBatches();

ChangeSetExamples.DetectCountChangingBatches();

ChangeSetExamples.TestTheChangeHandlerDirectly();

ChangeSetExamples.CompareChangesForEquality();

ChangeSetExamples.BuildAChangeSetByHand();

CollectionChangedExamples.ObserveRawCollectionChangedEvents();

CollectionChangedExamples.CompareNotificationsForEquality();

AutoPersistExamples.TrackAddsAndRemovesOnAnObservableCollection();

AutoPersistExamples.TrackAddsAndRemovesOnAReadOnlyView();

AutoPersistExamples.TrackAddsAndRemovesFromAChangeSetStream();

AutoPersistExamples.TrackAddsAndRemovesOnACustomCatalog();

ComparerExamples.RankLeaderboardByScoreThenName();

ComparerExamples.RankLeaderboardIgnoringNameCase();

ComparerExamples.RankLeaderboardByNameThenHighestScore();

ComparerExamples.RankLeaderboardCaseInsensitiveNameThenHighestScore();

ComparerExamples.ChooseALeaderboardSortAtRuntime();

ComparerExamples.BuildComparerForAnEmptyLeaderboard();
