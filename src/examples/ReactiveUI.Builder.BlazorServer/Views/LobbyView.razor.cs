// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using Microsoft.AspNetCore.Components.Web;
using ReactiveUI.Blazor;
using ReactiveUI.Builder.BlazorServer.Models;
using ReactiveUI.Builder.BlazorServer.ViewModels;

namespace ReactiveUI.Builder.BlazorServer.Views
{
    /// <summary>
    /// Lobby (rooms listing) view.
    /// </summary>
    public partial class LobbyView : ReactiveComponentBase<LobbyViewModel>
    {
        private async Task OnCreateRoomClicked(MouseEventArgs args)
        {
            if (ViewModel is null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            await ViewModel.CreateRoom.Execute();
        }

        private async Task OnDeleteSelectedClicked(MouseEventArgs args)
        {
            if (ViewModel is null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            if (ViewModel.SelectedChatRoom is null)
            {
                throw new InvalidOperationException("No chat room selected to delete.");
            }

            await ViewModel.DeleteRoom.Execute(ViewModel.SelectedChatRoom);
        }

        private void OnRoomClicked(ChatRoom room)
        {
            if (ViewModel is null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            ViewModel.SelectedChatRoom = room;
        }

        private async Task OnRoomDoubleClicked(ChatRoom room)
        {
            if (ViewModel is null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            await ViewModel.JoinRoom.Execute(room);
        }
    }
}
