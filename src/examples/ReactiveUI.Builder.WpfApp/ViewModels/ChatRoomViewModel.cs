// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Builder.WpfApp.Models;

namespace ReactiveUI.Builder.WpfApp.ViewModels;

/// <summary>View model for a single chat room.</summary>
public class ChatRoomViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>The chat room model whose messages this view model displays and appends to.</summary>
    private readonly ChatRoom _room;

    /// <summary>The display name of the local user, used as the sender for outgoing messages.</summary>
    private readonly string _user;

    /// <summary>Initializes a new instance of the <see cref="ChatRoomViewModel" /> class.</summary>
    /// <param name="hostScreen">The host screen.</param>
    /// <param name="room">The room.</param>
    /// <param name="user">The user.</param>
    public ChatRoomViewModel(IScreen hostScreen, ChatRoom room, string user)
    {
        ArgumentExceptionHelper.ThrowIfNull(room);
        HostScreen = hostScreen;
        UrlPathSegment = $"room/{room.Name}";
        _room = room;
        _user = user;

        var canSend =
            this.WhenAnyValue<ChatRoomViewModel, bool, string>(
                nameof(MessageText),
                txt => !string.IsNullOrWhiteSpace(txt));
        SendMessage = ReactiveCommand.Create(SendMessageImpl, canSend);

        // Observe new incoming messages via MessageBus using the room name as the contract across instances
        _ = MessageBus.Current.Listen<ChatNetworkMessage>(room.Name)
            .Where(msg => msg.InstanceId != Services.AppInstance.Id)
            .EmitIfQuiet(TimeSpan.FromMilliseconds(33))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(msg =>
            {
                _room.Messages.Add(new() { Sender = msg.Sender, Text = msg.Text, Timestamp = msg.Timestamp });
                Trace.TraceInformation($"[Room:{room.Name}] RX '{msg.Text}' from {msg.Sender}/{msg.InstanceId}");
            });
    }

    /// <inheritdoc />
    public string UrlPathSegment { get; }

    /// <inheritdoc />
    public IScreen HostScreen { get; }

    /// <summary>Gets the room name.</summary>
    public string RoomName => _room.Name;

    /// <summary>Gets the messages.</summary>
    public IReadOnlyList<ChatMessage> Messages => _room.Messages;

    /// <summary>Gets or sets the message text.</summary>
    [SuppressMessage(
        "StyleCop.CSharp.LayoutRules",
        "SA1500:Braces should not share line",
        Justification = "C# 13 field keyword with property initializer")]
    [SuppressMessage(
        "StyleCop.CSharp.LayoutRules",
        "SA1513:Closing brace should be followed by blank line",
        Justification = "C# 13 field keyword with property initializer")]
    public string MessageText
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
= string.Empty;

    /// <summary>Gets command to send a message.</summary>
    public ReactiveCommand<RxVoid, RxVoid> SendMessage { get; }

    /// <summary>Adds <see cref="MessageText"/> to the room, broadcasts it over the <see cref="MessageBus"/>, clears the input, and backs <see cref="SendMessage"/>.</summary>
    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider", Justification = "Not available all TFMs")]
    private void SendMessageImpl()
    {
        var msg = new ChatMessage
        {
            Sender = _user,
            Text = MessageText,
            Timestamp =
#if NET8_0_OR_GREATER
                TimeProvider.System.GetUtcNow(),
#else
                DateTimeOffset.Now,
#endif
        };
        _room.Messages.Add(msg);
        var networkMessage = new ChatNetworkMessage(_room.Id, _room.Name, msg.Sender, msg.Text, msg.Timestamp)
        {
            InstanceId = Services.AppInstance.Id
        };

        // Post on null contract so the network service can broadcast to other instances.
        MessageBus.Current.SendMessage(networkMessage);
        Trace.TraceInformation($"[Room:{_room.Name}] TX '{msg.Text}' from {_user}/{Services.AppInstance.Id}");

        MessageText = string.Empty;
    }
}
