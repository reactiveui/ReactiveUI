// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation;
using ReactiveUI.Documentation.DefaultExceptionHandler;

ExampleApp.Start();

await DefaultExceptionHandlerExamples.ReplaceTheDefaultExceptionHandler();

DefaultExceptionHandlerExamples.ConstructAnUnhandledErrorException();

LoggingExamples.LogEachNotification();

LoggingExamples.LogWithAMessageAndAStringifier();

LoggingExamples.FallBackToADefaultValueAfterLogging();

LoggingExamples.FallBackToAPreviousBalanceAfterLogging();

LoggingExamples.RecoverOnlyFromAccountServiceFailures();

WhereNotNullExamples.KeepOnlyTheSelectedAccount();
