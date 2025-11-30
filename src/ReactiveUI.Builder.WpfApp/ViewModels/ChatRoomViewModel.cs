// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI.Builder.WpfApp.Models;

namespace ReactiveUI.Builder.WpfApp.ViewModels;

/// <summary>
/// View model for a single chat room.
/// </summary>
public class ChatRoomViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ChatRoom _room;
    private readonly string _user;
    private string _messageText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatRoomViewModel" /> class.
    /// </summary>
    /// <param name="hostScreen">The host screen.</param>
    /// <param name="room">The room.</param>
    /// <param name="user">The user.</param>
    public ChatRoomViewModel(IScreen hostScreen, ChatRoom room, string user)
    {
        ArgumentNullException.ThrowIfNull(room);
        HostScreen = hostScreen;
        UrlPathSegment = $"room/{room.Name}";
        _room = room;
        _user = user;

        var canSend = this.WhenAnyValue<ChatRoomViewModel, bool, string>(nameof(MessageText), txt => !string.IsNullOrWhiteSpace(txt));
        SendMessage = ReactiveCommand.Create(SendMessageImpl, canSend);

        // Observe new incoming messages via MessageBus using the room name as the contract across instances
        MessageBus.Current.Listen<ChatNetworkMessage>(contract: room.Name)
            .Where(msg => msg.InstanceId != Services.AppInstance.Id)
            .Throttle(TimeSpan.FromMilliseconds(33))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(msg =>
            {
                _room.Messages.Add(new ChatMessage { Sender = msg.Sender, Text = msg.Text, Timestamp = msg.Timestamp });
                Trace.WriteLine($"[Room:{room.Name}] RX '{msg.Text}' from {msg.Sender}/{msg.InstanceId}");
            });
    }

    /// <inheritdoc />
    public string UrlPathSegment { get; }

    /// <inheritdoc />
    public IScreen HostScreen { get; }

    /// <summary>
    /// Gets the room name.
    /// </summary>
    public string RoomName => _room.Name;

    /// <summary>
    /// Gets the messages.
    /// </summary>
    public IReadOnlyList<ChatMessage> Messages => _room.Messages;

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    public string MessageText
    {
        get => _messageText;
        set => this.RaiseAndSetIfChanged(ref _messageText, value);
    }

    /// <summary>
    /// Gets command to send a message.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SendMessage { get; }

    private void SendMessageImpl()
    {
        var msg = new ChatMessage { Sender = _user, Text = MessageText, Timestamp = DateTimeOffset.Now };
        _room.Messages.Add(msg);
        var networkMessage = new ChatNetworkMessage(_room.Id, _room.Name, msg.Sender, msg.Text, msg.Timestamp)
        {
            InstanceId = Services.AppInstance.Id
        };

        // Post on null contract so the network service can broadcast to other instances.
        MessageBus.Current.SendMessage(networkMessage);
        Trace.WriteLine($"[Room:{_room.Name}] TX '{msg.Text}' from {_user}/{Services.AppInstance.Id}");

        MessageText = string.Empty;
    }
}
