// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Routing;

/// <summary>
/// The page that times a recipe while it cooks. It starts the timer when it becomes the current page and stops it
/// when the user leaves, using <c>WhenNavigatedTo</c>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class CookingTimerPage : ReactiveObject, IRoutableViewModel, IDisposable
{
    /// <summary>The subscription that starts and stops the timer as the page gains and loses focus.</summary>
    private readonly IDisposable _navigationSubscription;

    /// <summary>Initializes a new instance of the <see cref="CookingTimerPage"/> class.</summary>
    /// <param name="hostScreen">The window the page is shown in.</param>
    /// <param name="recipe">The recipe being timed.</param>
    public CookingTimerPage(IScreen hostScreen, Recipe recipe)
    {
        HostScreen = hostScreen;
        Recipe = recipe;
        _navigationSubscription = this.WhenNavigatedTo(StartTimer);
    }

    /// <inheritdoc/>
    public string UrlPathSegment => $"recipes/{Recipe.Id}/timer";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the recipe being timed.</summary>
    public Recipe Recipe { get; }

    /// <summary>Gets the command that advances the timer by one second. A real timer would call this from a clock.</summary>
    public ReactiveCommand<RxVoid, RxVoid> Tick { get; } = ReactiveCommand.Create(static () => { });

    /// <summary>Gets the number of seconds the timer has counted while the page had focus.</summary>
    public int SecondsElapsed { get; private set; }

    /// <inheritdoc/>
    public void Dispose() => _navigationSubscription.Dispose();

    /// <summary>Starts the timer; the returned disposable stops it when the page loses focus.</summary>
    /// <returns>A disposable that stops the timer.</returns>
    private IDisposable StartTimer()
    {
        Console.WriteLine($"Timer started for {Recipe.Name}");
        IDisposable subscription = Tick.Subscribe(_ =>
        {
            SecondsElapsed++;
            Console.WriteLine($"{Recipe.Name}: {SecondsElapsed}s");
        });

        return new ActionDisposable(() =>
        {
            subscription.Dispose();
            Console.WriteLine($"Timer stopped for {Recipe.Name}");
        });
    }
}
