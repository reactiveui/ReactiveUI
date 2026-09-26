// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// Shows <see cref="Winforms.ActivationForViewFetcher"/>, which turns a WinForms control's handle and visibility
/// events into the activation signal <c>WhenActivated</c> relies on. <see cref="Winforms.Registrations"/> already
/// registers this fetcher for every view in the app; this example builds one directly to show its own two members.
/// Qualified as <c>Winforms.ActivationForViewFetcher</c>: this code's own namespace nests under <c>ReactiveUI</c>,
/// and other platforms' fetchers of the same short name live directly under <c>ReactiveUI</c>, so the unqualified
/// name is not safe here.
/// </summary>
public static class ActivationForViewFetcherExamples
{
    /// <summary>A <see cref="Control"/> gets the strongest affinity; an unrelated type gets none.</summary>
    public static void RankViewTypes()
    {
        Winforms.ActivationForViewFetcher fetcher = new();

        Console.WriteLine(fetcher.GetAffinityForView(typeof(MemberCardView)));
        Console.WriteLine(fetcher.GetAffinityForView(typeof(string)));

        // Output:
        // 10
        // 0
    }

    /// <summary>Creating the control's window handle raises the activation signal.</summary>
    public static void ObserveActivation()
    {
        Winforms.ActivationForViewFetcher fetcher = new();
        using MemberCardView view = new();
        List<bool> states = [];

        using IDisposable subscription = fetcher.GetActivationForView(view).Subscribe(states.Add);

        _ = view.Handle;

        Console.WriteLine(states.Contains(true));

        // Output:
        // True
    }
}
