// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components.Web;
using ReactiveUI.Blazor;
using ReactiveUI.Builder.BlazorServer.Models;
using ReactiveUI.Builder.BlazorServer.ViewModels;

namespace ReactiveUI.Builder.BlazorServer.Views;

/// <summary>Lobby (rooms listing) view.</summary>
public partial class LobbyView : ReactiveComponentBase<LobbyViewModel>
{
    /// <summary>Handles the create-room button click by executing the view model's create command.</summary>
    /// <param name="args">The mouse event arguments raised by the button click.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    private async Task OnCreateRoomClicked(MouseEventArgs args)
    {
        if (ViewModel is null)
        {
            throw new ArgumentNullException(nameof(ViewModel));
        }

        await ViewModel.CreateRoom.Execute();
    }

    /// <summary>Handles the delete button click by executing the view model's delete command for the selected room.</summary>
    /// <param name="args">The mouse event arguments raised by the button click.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
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

    /// <summary>Handles a single click on a room by marking it as the currently selected room.</summary>
    /// <param name="room">The room that was clicked.</param>
    private void OnRoomClicked(ChatRoom room)
    {
        if (ViewModel is null)
        {
            throw new ArgumentNullException(nameof(ViewModel));
        }

        ViewModel.SelectedChatRoom = room;
    }

    /// <summary>Handles a double click on a room by executing the view model's join command to enter it.</summary>
    /// <param name="room">The room that was double clicked.</param>
    /// <returns>A task that represents the asynchronous join operation.</returns>
    private async Task OnRoomDoubleClicked(ChatRoom room)
    {
        if (ViewModel is null)
        {
            throw new ArgumentNullException(nameof(ViewModel));
        }

        await ViewModel.JoinRoom.Execute(room);
    }
}
