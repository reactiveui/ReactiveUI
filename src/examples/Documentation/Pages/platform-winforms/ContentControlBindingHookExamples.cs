// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// Shows <see cref="ContentControlBindingHook"/>. Registering one lets custom logic run before a binding executes;
/// this one always lets the binding proceed, which is what a WinForms app gets unless it registers its own hook.
/// </summary>
public static class ContentControlBindingHookExamples
{
    /// <summary>The hook returns true no matter what it is asked about, so it never blocks a binding.</summary>
    public static void AlwaysAllowsTheBindingToProceed()
    {
        ContentControlBindingHook hook = new();

        bool allowed = hook.ExecuteHook(
            source: null,
            target: new object(),
            getCurrentViewModelProperties: static () => [],
            getCurrentViewProperties: static () => [],
            direction: BindingDirection.OneWay);

        Console.WriteLine(allowed);

        // Output:
        // True
    }
}
