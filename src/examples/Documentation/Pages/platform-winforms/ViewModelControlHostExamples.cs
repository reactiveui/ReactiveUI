// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// Shows the static <see cref="ViewModelControlHost.DefaultCacheViewsEnabled"/> switch. The rest of
/// <see cref="ViewModelControlHost"/>'s members are shown by <see cref="LibraryForm"/>'s member card host.
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
}
