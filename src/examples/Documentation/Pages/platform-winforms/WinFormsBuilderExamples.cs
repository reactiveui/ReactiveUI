// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// Shows how a WinForms app starts ReactiveUI. This chunk is about starting ReactiveUI itself, so this call
/// replaces the console pages' <c>ExampleApp.Start()</c>: it is the app's own startup, run once from <c>Main</c>.
/// </summary>
public static class WinFormsBuilderExamples
{
    /// <summary>
    /// <c>WithWinForms</c> registers the WinForms platform module and its main-thread scheduler.
    /// <c>WithWinFormsScheduler</c> sets only the scheduler, for an app that registers its platform module another
    /// way. Both point <see cref="RxSchedulers.MainThreadScheduler"/> at
    /// <see cref="WinFormsReactiveUIBuilderExtensions.WinFormsMainThreadScheduler"/>.
    /// </summary>
    public static void ConfigureWinForms()
    {
        IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder()
            .WithWinForms()
            .WithWinFormsScheduler();

        _ = builder.BuildApp();

        Console.WriteLine(ReferenceEquals(WinFormsReactiveUIBuilderExtensions.WinFormsMainThreadScheduler, RxSchedulers.MainThreadScheduler));

        // Output:
        // True
    }
}
