// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation;
using ReactiveUI.Documentation.Commands;

ExampleApp.Start();

await CommandExamples.RunSynchronousCommand();

await CommandExamples.RunAsynchronousCommand();

CommandExamples.ControlExecutability();

await CommandExamples.HandleErrors();

await CommandExamples.InvokeFromPipeline();

await CommandExamples.CombineCommands();

await CancellationExamples.CancelByDisposing();

await CancellationExamples.CancelFromAnotherCommand();

await BindCommandExamples.BindAddButton();

await BindCommandExamples.PassTheSelectedItem();

await CommandFactoryExamples.LogDeskEvents();

await CommandFactoryExamples.CountBooksOnShelf();

await CommandFactoryExamples.FlagOverdueBooks();

await CommandFactoryExamples.SearchCatalogue();

await CommandFactoryExamples.SyncCatalogueAsObservable();

await CommandFactoryExamples.BorrowBookAsObservable();

await CommandFactoryExamples.CombineEndOfDayTasks();

await CommandFactoryTaskExamples.ReturnAllBooksTask();

await CommandFactoryTaskExamples.ReturnAllBooksWithCountTask();

await CommandFactoryTaskExamples.RefreshLocalCacheTask();

await CommandFactoryTaskExamples.SyncCentralCatalogueTask();

await CommandFactoryTaskExamples.SendRenewalReceiptTask();

await CommandFactoryTaskExamples.RenewLoanTask();

await CommandFactoryTaskExamples.SendRenewalReceiptCancellableTask();

await CommandFactoryTaskExamples.BorrowBookCancellableTask();

await CommandFactoryBackgroundExamples.LogDeskEventsInBackground();

await CommandFactoryBackgroundExamples.FlagOverdueBooksInBackground();

await CommandFactoryBackgroundExamples.CountBooksOnShelfInBackground();

await CommandFactoryBackgroundExamples.SearchCatalogueInBackground();

await CommandTypeExamples.DeriveWithResultObservable();

await CommandTypeExamples.DeriveWithCancelCallback();

await CommandTypeExamples.ObserveThrownExceptions();

await CommandTypeExamples.DeriveFromCommandBase();

await CommandTypeExamples.DeriveCombinedCommand();

await CommandTypeExamples.ExposeCommandThroughInterface();

await CommandTypeExamples.AcceptAnyCommand();

SwitchSubscribeExamples.TrackSwappedProgressStream();

SwitchSubscribeExamples.TrackSwappedProgressStreamWithHandlers();

SwitchSubscribeExamples.TrackSwappedSessionProgress();

SwitchSubscribeExamples.TrackSwappedSessionProgressWithHandlers();

SwitchSubscribeExamples.SelectSwappedSessionProgress();

await SwitchSubscribeExamples.TrackSwappedCommandResults();

await SwitchSubscribeExamples.TrackSwappedCommandResultsWithHandlers();

await SwitchSubscribeExamples.TrackSwappedCommandIsExecuting();

await SwitchSubscribeExamples.TrackSwappedCommandIsExecutingWithHandlers();

await SwitchSubscribeExamples.SelectSwappedCommandIsExecuting();
