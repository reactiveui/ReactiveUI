// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;
using Splat;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows <see cref="ReactiveShellContent{TViewModel}"/>, the tab content a <see cref="ReactiveShell{TViewModel}"/>
/// hosts. It looks up the registered <see cref="IViewFor{TViewModel}"/> for its view model and, optionally, its
/// <see cref="ReactiveShellContent{TViewModel}.Contract"/>, and turns it into a <c>ContentTemplate</c>.
/// </summary>
public static class ShellContentExamples
{
    /// <summary>Two views are registered for the same view model, one for the default contract and one for "Compact"; each contract picks its own content template.</summary>
    public static void ContractPicksTheContentTemplate()
    {
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListPage());
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListCompactPage(), "Compact");

        RecipeListViewModel recipes = new(new RecipeBookScreen());
        ReactiveShellContent<RecipeListViewModel> defaultContent = new() { ViewModel = recipes };
        ReactiveShellContent<RecipeListViewModel> compactContent = new() { Contract = "Compact", ViewModel = recipes };

        Console.WriteLine(defaultContent.Contract ?? "(none)");
        Console.WriteLine(ReferenceEquals(defaultContent.ViewModel, recipes));
        Console.WriteLine(((IViewFor)defaultContent.ContentTemplate.CreateContent()).GetType().Name);
        Console.WriteLine(compactContent.Contract);
        Console.WriteLine(((IViewFor)compactContent.ContentTemplate.CreateContent()).GetType().Name);
        Console.WriteLine(ReactiveShellContent<RecipeListViewModel>.ContractProperty.PropertyName);
        Console.WriteLine(ReactiveShellContent<RecipeListViewModel>.ViewModelProperty.PropertyName);

        // Output:
        // (none)
        // True
        // RecipeListPage
        // Compact
        // RecipeListCompactPage
        // Contract
        // ViewModel
    }
}
