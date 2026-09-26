// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Reflection;

/// <summary>
/// Shows the view-side <c>WhenActivated</c> overloads that discover a view's ViewModel with an expression-based
/// <c>WhenAnyValue</c>, which needs reflection. Passing a ViewModel-change signal explicitly — as
/// <c>TodoListView</c> does with <c>this.WhenAnyValue(x =&gt; x.ViewModel)</c> — is the trim- and AOT-safe
/// alternative to every overload shown here.
/// </summary>
public static class WhenActivatedReflectionExamples
{
    /// <summary>The no-block overload activates only the ViewModel it discovers on the view itself.</summary>
    public static void ActivateWithoutABlock()
    {
        using MusicPlayerViewModel viewModel = new("Africa");
        using MusicPlayerScreen screen = new() { ViewModel = viewModel };

        using IDisposable subscription = screen.WhenActivated();

        screen.Show();
        Console.WriteLine(viewModel.IsPlaying);
        screen.Hide();
        Console.WriteLine(viewModel.IsPlaying);

        // Output:
        // True
        // False
    }

    /// <summary>The function-block style returns the disposables to keep; with no view given, the screen discovers its own ViewModel.</summary>
    public static void ActivateWithAFunctionBlock()
    {
        using MusicPlayerViewModel viewModel = new("Bohemian Rhapsody");
        using MusicPlayerScreen screen = new() { ViewModel = viewModel, NowPlayingText = viewModel.Title };

        using IDisposable subscription = screen.WhenActivated(
            static () => [new ActionDisposable(static () => Console.WriteLine("Stopped"))]);

        screen.Show();
        Console.WriteLine(screen.NowPlayingText);
        Console.WriteLine(viewModel.IsPlaying);
        screen.Hide();

        // Output:
        // Bohemian Rhapsody
        // True
        // Stopped
    }

    /// <summary>The callback style passes a delegate that registers each disposable one at a time.</summary>
    public static void ActivateWithACallbackBlock()
    {
        using MusicPlayerViewModel viewModel = new("Africa");
        using MusicPlayerScreen screen = new() { ViewModel = viewModel };

        using IDisposable subscription = screen.WhenActivated(
            static register => register(new ActionDisposable(static () => Console.WriteLine("Stopped"))));

        screen.Show();
        Console.WriteLine(viewModel.IsPlaying);
        screen.Hide();

        // Output:
        // True
        // Stopped
    }

    /// <summary>The disposables-container style adds each disposable to the container the activator hands the block.</summary>
    public static void ActivateWithADisposablesContainer()
    {
        using MusicPlayerViewModel viewModel = new("Clocks");
        using MusicPlayerScreen screen = new() { ViewModel = viewModel };

        using IDisposable subscription = screen.WhenActivated(
            static disposables => disposables.Add(new ActionDisposable(static () => Console.WriteLine("Stopped"))));

        screen.Show();
        Console.WriteLine(viewModel.IsPlaying);
        screen.Hide();

        // Output:
        // True
        // Stopped
    }

    /// <summary>
    /// Every block style also takes an explicit <see cref="IViewFor"/>, for a code-behind that raises activation but
    /// is not itself a view — a templated container that activates a data-bound content control it does not own.
    /// </summary>
    public static void ActivateAViewGivenExplicitly()
    {
        using MusicPlayerViewModel functionViewModel = new("Yellow");
        using MusicPlayerScreen functionView = new() { ViewModel = functionViewModel };
        using MusicPlayerCodeBehind functionCodeBehind = new();
        using IDisposable functionSubscription = functionCodeBehind.WhenActivated(
            static () => [new ActionDisposable(static () => Console.WriteLine("Function block stopped"))],
            functionView);

        using MusicPlayerViewModel callbackViewModel = new("Fix You");
        using MusicPlayerScreen callbackView = new() { ViewModel = callbackViewModel };
        using MusicPlayerCodeBehind callbackCodeBehind = new();
        using IDisposable callbackSubscription = callbackCodeBehind.WhenActivated(
            static register => register(new ActionDisposable(static () => Console.WriteLine("Callback block stopped"))),
            callbackView);

        using MusicPlayerViewModel containerViewModel = new("Adventure of a Lifetime");
        using MusicPlayerScreen containerView = new() { ViewModel = containerViewModel };
        using MusicPlayerCodeBehind containerCodeBehind = new();
        using IDisposable containerSubscription = containerCodeBehind.WhenActivated(
            static disposables => disposables.Add(new ActionDisposable(static () => Console.WriteLine("Container block stopped"))),
            containerView);

        functionCodeBehind.Show();
        callbackCodeBehind.Show();
        containerCodeBehind.Show();

        Console.WriteLine(functionViewModel.IsPlaying);
        Console.WriteLine(callbackViewModel.IsPlaying);
        Console.WriteLine(containerViewModel.IsPlaying);

        functionCodeBehind.Hide();
        callbackCodeBehind.Hide();
        containerCodeBehind.Hide();

        // Output:
        // True
        // True
        // True
        // Function block stopped
        // Callback block stopped
        // Container block stopped
    }
}
