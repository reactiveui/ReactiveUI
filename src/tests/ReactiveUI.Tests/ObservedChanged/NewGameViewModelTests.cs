// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.ObservedChanged.Mocks;

namespace ReactiveUI.Tests.ObservedChanged;

/// <summary>Tests for the <see cref="NewGameViewModel" />.</summary>
public class NewGameViewModelTests
{
    /// <summary>The view model under test.</summary>
    private readonly NewGameViewModel _viewmodel;

    /// <summary>Initializes a new instance of the <see cref="NewGameViewModelTests" /> class.</summary>
    public NewGameViewModelTests() => _viewmodel = new();

    /// <summary>Tests that determines whether this instance [can add up to seven players].</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanAddUpToSevenPlayers()
    {
        const int MaxPlayers = 7;
        foreach (var i in Enumerable.Range(1, MaxPlayers))
        {
            _viewmodel.NewPlayerName = "Player" + i;
            _viewmodel.AddPlayer.Execute().Subscribe();
            await Assert.That(_viewmodel.Players).Count().IsEqualTo(i);
        }
    }
}
