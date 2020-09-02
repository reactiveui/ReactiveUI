﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Xunit;

namespace ReactiveUI.Tests
{
    public class NewGameViewModelTests
    {
        private readonly NewGameViewModel _viewmodel;

        public NewGameViewModelTests()
        {
            _viewmodel = new NewGameViewModel();
        }

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
