// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// Shows <see cref="ReactiveUserControlNonGeneric"/> through <see cref="LibraryNoticeView"/>, a view that needs
/// the non-generic base because the WinForms Designer cannot edit a control whose base class is generic.
/// </summary>
public static class ReactiveUserControlNonGenericExamples
{
    /// <summary><see cref="LibraryNoticeView.ViewModel"/> stores its value through the base's non-generic <see cref="IViewFor.ViewModel"/>.</summary>
    public static void SetTheViewModelThroughTheNonGenericBase()
    {
        using LibraryNoticeView view = new();

        view.ViewModel = new AnnouncementViewModel("The library closes early on Fridays.");

        Console.WriteLine(view.NoticeLabel.Text);

        // Output:
        // The library closes early on Fridays.
    }
}
