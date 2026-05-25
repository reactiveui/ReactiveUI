// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI.Builder.BlazorServer.Models;

namespace ReactiveUI.Builder.BlazorServer.ViewModels;

/// <summary>
/// View model for a single chat room.
/// </summary>
public class ChatRoomViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>
    /// Backing helper indicating whether a message can be sent.
    /// </summary>
    private readonly ObservableAsPropertyHelper<bool> _canSendPropertyHelper;

    /// <summary>
    /// The chat room backing this view model.
    /// </summary>
    private readonly ChatRoom _room;

    /// <summary>
    /// The current user's display name.
    /// </summary>
    private readonly string _user;

    /// <summary>
    /// The identifier of the sending instance, used to filter out echoed messages.
    /// </summary>
    private readonly Guid _senderInstanceId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatRoomViewModel" /> class.
    /// </summary>
    /// <param name="hostScreen">The host screen.</param>
    /// <param name="room">The room.</param>
    /// <param name="user">The user.</param>
    [SuppressMessage("Reliability", "S3366:Don't expose 'this' in constructors", Justification = "OAPH/WhenAny initialization requires 'this'; single-threaded sample.")]
    public ChatRoomViewModel(IScreen hostScreen, ChatRoom room, string user)
    {
        ArgumentNullException.ThrowIfNull(room);
        HostScreen = hostScreen;
        UrlPathSegment = $"room/{room.Name}";
        _room = room;
        _user = user;

        _senderInstanceId = (hostScreen as AppBootstrapper)?.CircuitId
                            ?? throw new InvalidOperationException("Expected HostScreen to be AppBootstrapper.");

        var canSend =
            this.WhenAnyValue<ChatRoomViewModel, bool, string>(
                nameof(MessageText),
                txt => !string.IsNullOrWhiteSpace(txt));
        SendMessage = ReactiveCommand.Create(SendMessageImpl, canSend);

        NavigateBack = ReactiveCommand.CreateFromObservable(
            () => hostScreen.Router.NavigateBack.Execute().Select(static _ => Unit.Default));

        _canSendPropertyHelper = canSend.ToProperty(this, nameof(SendMessage));

        // Observe new incoming messages via MessageBus using the room name as the contract across instances
        MessageBus.Current.Listen<ChatNetworkMessage>(room.Name)
            .Throttle(TimeSpan.FromMilliseconds(33))
            .Where(x => x.InstanceId != _senderInstanceId)
            .Subscribe(msg =>
            {
                // Since we share the room, message is already added there, so we just need to notify the UI
                this.RaisePropertyChanged(nameof(Messages));
                Trace.TraceInformation($"[Room:{room.Name}] RX '{msg.Text}' from {msg.Sender}/{msg.InstanceId}");
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

    /// <summary>
    /// Gets command to send a message.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SendMessage { get; }

    /// <summary>
    /// Gets a value indicating whether sending operations are currently disabled.
    /// </summary>
    public bool CanSendDisabled => !_canSendPropertyHelper.Value;

    /// <summary>
    /// Gets command to navigate back.
    /// </summary>
    public ReactiveCommand<Unit, Unit> NavigateBack { get; }

    /// <summary>
    /// Sends the current message text to the room and broadcasts it to peers.
    /// </summary>
    private void SendMessageImpl()
    {
        var msg = new ChatMessage { Sender = _user, Text = MessageText, Timestamp = TimeProvider.System.GetUtcNow() };
        _room.Messages.Add(msg);
        var networkMessage = new ChatNetworkMessage(_room.Id, _room.Name, msg.Sender, msg.Text, msg.Timestamp)
        {
            InstanceId = _senderInstanceId
        };

        MessageBus.Current.SendMessage(networkMessage, RoomName);
        Trace.TraceInformation($"[Room:{_room.Name}] TX '{msg.Text}' from {_user}/{Services.AppInstance.Id}");

        MessageText = string.Empty;
    }
}
