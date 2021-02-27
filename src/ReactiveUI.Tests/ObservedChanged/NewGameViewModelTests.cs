// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests for a sample game.
    /// </summary>
    public class NewGameViewModelTests
    {
        private readonly NewGameViewModel _viewmodel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewGameViewModelTests"/> class.
        /// </summary>
        public NewGameViewModelTests() => _viewmodel = new NewGameViewModel();

        /// <summary>
        /// Tests that determines whether this instance [can add up to seven players].
        /// </summary>
        [Fact]
        public void CanAddUpToSevenPlayers()
        {
            foreach (var i in Enumerable.Range(1, 7))
            {
                _viewmodel.NewPlayerName = "Player" + i;
                _viewmodel.AddPlayer.Execute().Subscribe();
                Assert.Equal(i, _viewmodel.Players.Count);
            }
        }
    }
}
