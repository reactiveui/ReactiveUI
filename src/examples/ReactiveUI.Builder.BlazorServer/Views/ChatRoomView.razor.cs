// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using Microsoft.AspNetCore.Components.Web;
using ReactiveUI.Blazor;
using ReactiveUI.Builder.BlazorServer.ViewModels;

namespace ReactiveUI.Builder.BlazorServer.Views;

/// <summary>
/// Represents a view component for displaying and interacting with a chat room in the user interface.
/// </summary>
public partial class ChatRoomView : ReactiveComponentBase<ChatRoomViewModel>
{
    /// <summary>
    /// Handles the send button click by executing the view model's send command.
    /// </summary>
    /// <param name="args">The mouse event arguments raised by the button click.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    private async Task OnSendClicked(MouseEventArgs args)
    {
        if (ViewModel is null)
        {
            throw new ArgumentNullException(nameof(ViewModel));
        }

        await ViewModel.SendMessage.Execute();
    }

    /// <summary>
    /// Handles the back button click by executing the view model's navigate-back command.
    /// </summary>
    /// <param name="args">The mouse event arguments raised by the button click.</param>
    /// <returns>A task that represents the asynchronous navigation operation.</returns>
    private async Task OnNavigateBack(MouseEventArgs args)
    {
        if (ViewModel is null)
        {
            throw new ArgumentNullException(nameof(ViewModel));
        }

        await ViewModel.NavigateBack.Execute();
    }
}
