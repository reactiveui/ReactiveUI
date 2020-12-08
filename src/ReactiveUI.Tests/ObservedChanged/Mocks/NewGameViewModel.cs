// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
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
    public class NewGameViewModel : ReactiveObject
    {
        private string? _newPlayerName;

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

        public ObservableCollectionExtended<string> Players { get; }

        public ReactiveCommand<Unit, Unit> AddPlayer { get; }

        public ReactiveCommand<string, Unit> RemovePlayer { get; }

        public ReactiveCommand<Unit, Unit> StartGame { get; }

        public ReactiveCommand<Unit, Unit> RandomizeOrder { get; }

        public string? NewPlayerName
        {
            get => _newPlayerName;
            set => this.RaiseAndSetIfChanged(ref _newPlayerName, value);
        }
    }
}
