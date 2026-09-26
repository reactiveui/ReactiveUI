// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// Shows the static <see cref="ViewModelControlHost.DefaultCacheViewsEnabled"/> switch, and the
/// <see cref="ViewModelControlHost.ViewLocator"/>, <see cref="ViewModelControlHost.ViewContractObservable"/> and
/// <see cref="ViewModelControlHost.CurrentView"/> members a host uses when a caller assigns them directly instead
/// of relying on the defaults. <see cref="LibraryForm"/>'s member card host shows the rest of
/// <see cref="ViewModelControlHost"/>'s members, driven from the loan scenario.
/// </summary>
public static class ViewModelControlHostExamples
{
    /// <summary>Every new host reads its initial <see cref="ViewModelControlHost.CacheViews"/> from this switch.</summary>
    public static void SetTheDefaultForNewHosts()
    {
        ViewModelControlHost.DefaultCacheViewsEnabled = true;

        using ViewModelControlHost host = new();

        Console.WriteLine(host.CacheViews);

        // Output:
        // True
    }

    /// <summary>
    /// Assigning <see cref="ViewModelControlHost.ViewLocator"/> and <see cref="ViewModelControlHost.ViewContractObservable"/>
    /// picks the view locator and contract a host resolves with, instead of the defaults every host starts with.
    /// Setting <see cref="ViewModelControlHost.ViewModel"/> then resolves the matching view, which
    /// <see cref="ViewModelControlHost.CurrentView"/> exposes as a plain <see cref="Control"/>.
    /// </summary>
    public static void ResolveAndReadTheCurrentView()
    {
        using ViewModelControlHost host = new()
        {
            ViewLocator = ViewLocator.GetCurrent(),
            ViewContractObservable = Signal.Emit(string.Empty),
        };

        Member member = new("M-1", "Ada");
        host.ViewModel = new MemberCardViewModel(member, LoanCount: 1);

        Console.WriteLine(host.CurrentView is MemberCardView);

        host.CurrentView?.Dispose();

        // Output:
        // True
    }
}
