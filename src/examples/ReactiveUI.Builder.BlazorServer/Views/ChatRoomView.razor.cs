// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.BlazorServer.ViewModels;

namespace ReactiveUI.Builder.BlazorServer.Views
{
    /// <summary>
    /// Represents a view component for displaying and interacting with a chat room in the user interface.
    /// </summary>
    public partial class ChatRoomView : ReactiveComponentBase<ChatRoomViewModel>
    {
        private async Task OnSendClicked(MouseEventArgs args)
        {
            if (ViewModel is null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            await ViewModel.SendMessage.Execute();
        }

        private async Task OnNavigateBack(MouseEventArgs args)
        {
            if (ViewModel is null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            await ViewModel.NavigateBack.Execute();
        }
    }
}
