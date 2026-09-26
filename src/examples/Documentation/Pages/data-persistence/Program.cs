// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation;
using ReactiveUI.Documentation.DataPersistence;

ExampleApp.Start();

SuspensionExamples.SingletonIsSharedAcrossTheApp();

SuspensionExamples.DriveLifecycleManually();

await SuspensionExamples.DummyDriverDoesNothing();

TypedSuspensionExamples.DriveTypedLifecycleManually();

await TypedSuspensionExamples.SetupDefaultSuspendResumeOverloads();

await NoteExamples.SavesAfterAQuietPeriod();

await NoteExamples.SavesOnManualSignal();

NoteExamples.DefaultIntervalIsThreeSeconds();

NoteExamples.ManualSaveSignalWithDefaultInterval();

CollectionPersistExamples.DefaultIntervalAppliesToEveryItem();

await CollectionPersistExamples.PersistsEachNoteAfterAQuietPeriod();

await CollectionPersistExamples.ManualSaveSignalForcesEveryItemToSave();

CollectionPersistExamples.ManualSaveSignalWithoutIntervalUsesTheDefault();

await CollectionPersistExamples.PersistsThroughAReadOnlyView();

CollectionPersistExamples.ManualSaveSignalOnAReadOnlyViewUsesTheDefaultInterval();

await CollectionPersistExamples.PersistsEveryNoteInACustomFeed();

CollectionPersistExamples.ManualSaveSignalOnACustomFeedUsesTheDefaultInterval();

await CollectionPersistExamples.PersistsUsingAMetadataProviderPerItem();

CollectionPersistExamples.MetadataProviderWithoutIntervalUsesTheDefault();
