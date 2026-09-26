// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.GitHub;

namespace ReactiveUI.Documentation.GettingStarted;

/// <summary>Shows <see cref="RepositoryDetailsViewModel.OpenPage"/>: a command created with <c>ReactiveCommand.Create</c>.</summary>
public static class RepositoryDetailsExamples
{
    /// <summary>Executing <see cref="RepositoryDetailsViewModel.OpenPage"/> runs the action it was created with.</summary>
    public static void ExecuteTheOpenPageCommand()
    {
        Repository repository = new("reactiveui/ReactiveUI", "An advanced, composable, functional reactive MVVM framework", 8400);
        using RepositoryDetailsViewModel details = new(repository);

        using IDisposable execution = details.OpenPage.Execute().Subscribe();

        Console.WriteLine(details.FullName);

        // Output:
        // Opening https://github.com/reactiveui/ReactiveUI
        // reactiveui/ReactiveUI
    }

    /// <summary>The view binds the title and description one way, and the open button to the command with <c>BindCommand</c>.</summary>
    public static void BindTheDetailsView()
    {
        Repository repository = new("reactiveui/refit", "The automatic type-safe REST library for .NET", 8900);
        using RepositoryDetailsViewModel details = new(repository);
        RepositoryDetailsView view = new() { ViewModel = details };

        using IReactiveBinding<RepositoryDetailsView, string> titleBinding = view.OneWayBind(details, x => x.FullName, v => v.TitleLabel.Text);
        using IReactiveBinding<RepositoryDetailsView, string> descriptionBinding = view.OneWayBind(details, x => x.Description, v => v.DescriptionLabel.Text);
        using IDisposable commandBinding = view.BindCommand(details, x => x.OpenPage, v => v.OpenButton);

        Console.WriteLine(view.TitleLabel.Text);
        view.OpenButton.PerformClick();

        // Output:
        // reactiveui/refit
        // Opening https://github.com/reactiveui/refit
    }
}
