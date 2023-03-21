// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData.Binding;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// A sample view model that implements a game.
    /// </summary>
    /// <seealso cref="ReactiveUI.ReactiveObject" />
    public class NewGameViewModel : ReactiveObject
    {
        private string? _newPlayerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewGameViewModel"/> class.
        /// </summary>
        public NewGameViewModel()
        {
            Players = new ObservableCollectionExtended<string>();

            var canStart = Players.ToObservableChangeSet().CountChanged().Select(_ => Players.Count >= 3);
            StartGame = ReactiveCommand.Create(() => { }, canStart);
            RandomizeOrder = ReactiveCommand.Create(
                                                    () =>
                                                    {
                                                        using (Players.SuspendNotifications())
                                                        {
                                                            var r = new Random();
                                                            var newOrder = Players.OrderBy(x => r.NextDouble()).ToList();
                                                            Players.Clear();
                                                            Players.AddRange(newOrder);
                                                        }
                                                    },
                                                    canStart);

            RemovePlayer = ReactiveCommand.Create<string>(player => Players.Remove(player));
            var canAddPlayer = this.WhenAnyValue(
                                                 x => x.Players.Count,
                                                 x => x.NewPlayerName,
                                                 (count, newPlayerName) => count < 7 && !string.IsNullOrWhiteSpace(newPlayerName) && !Players.Contains(newPlayerName!));
            AddPlayer = ReactiveCommand.Create(
                                               () =>
                                               {
                                                   if (NewPlayerName is null)
                                                   {
                                                       throw new InvalidOperationException("NewPlayerName is null");
                                                   }

                                                   Players.Add(NewPlayerName.Trim());
                                                   NewPlayerName = string.Empty;
                                               },
                                               canAddPlayer);
        }

        /// <summary>
        /// Gets the players collection.
        /// </summary>
        public ObservableCollectionExtended<string> Players { get; }

        /// <summary>
        /// Gets the add player command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddPlayer { get; }

        /// <summary>
        /// Gets the remove player command.
        /// </summary>
        public ReactiveCommand<string, Unit> RemovePlayer { get; }

        /// <summary>
        /// Gets the start game command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> StartGame { get; }

        /// <summary>
        /// Gets the randomize order command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> RandomizeOrder { get; }

        /// <summary>
        /// Gets or sets the new player name.
        /// </summary>
        public string? NewPlayerName
        {
            get => _newPlayerName;
            set => this.RaiseAndSetIfChanged(ref _newPlayerName, value);
        }
    }
}
