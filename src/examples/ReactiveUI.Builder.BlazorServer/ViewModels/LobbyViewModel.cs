// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Builder.BlazorServer.Models;

namespace ReactiveUI.Builder.BlazorServer.ViewModels;

/// <summary>The lobby view model which lists rooms and allows creating/joining rooms.</summary>
public class LobbyViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>The message bus contract used for room events.</summary>
    private const string RoomsKey = "__rooms__";

    /// <summary>The quiet window, in milliseconds, applied before publishing a batched rooms-changed signal.</summary>
    private const int RoomsChangedQuietWindowMilliseconds = 50;

    /// <summary>The delay, in milliseconds, before broadcasting the initial peer sync request after activation.</summary>
    private const int SyncRequestDelayMilliseconds = 500;

    /// <summary>Backing helper indicating whether room creation is disabled.</summary>
    private readonly ObservableAsPropertyHelper<bool> _createRoomDisabledHelper;

    /// <summary>Backing helper indicating whether room deletion is disabled.</summary>
    private readonly ObservableAsPropertyHelper<bool> _deleteRoomDisabledHelper;

    /// <summary>Backing helper exposing the current list of rooms.</summary>
    private readonly ObservableAsPropertyHelper<IReadOnlyList<ChatRoom>> _rooms;

    /// <summary>Initializes a new instance of the <see cref="LobbyViewModel"/> class.</summary>
    /// <param name="hostScreen">The host screen.</param>
    [SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "Canonical ObservableAsPropertyHelper and WhenAnyObservable setup needs 'this' in the constructor; the single-threaded Blazor circuit never exposes the half-built instance.")]
    public LobbyViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        UrlPathSegment = "lobby";

        var canDelete = this.WhenAnyValue(nameof(SelectedChatRoom), static (ChatRoom? room) => room is not null);

        var canCreate =
            this.WhenAnyValue<LobbyViewModel, bool, string>(nameof(RoomName), static rn => !string.IsNullOrWhiteSpace(rn));
        CreateRoom = ReactiveCommand.Create(CreateRoomImpl, canCreate);

        _createRoomDisabledHelper = canCreate
            .ToProperty(this, x => x.CreateRoomDisabled);
        _deleteRoomDisabledHelper = canDelete
            .ToProperty(this, x => x.DeleteRoomDisabled);

        DeleteRoom = ReactiveCommand.Create<ChatRoom>(DeleteRoomImpl);

        JoinRoom = ReactiveCommand.CreateFromTask<ChatRoom>(async room =>
        {
            ArgumentNullException.ThrowIfNull(room);
            await HostScreen.Router.Navigate.Execute(new ChatRoomViewModel(HostScreen, room, DisplayName));
        });

        // Local changes
        var localRoomsChanged = MessageBus.Current.Listen<ChatStateChanged>().Select(static _ => RxVoid.Default);

        // Remote changes and sync (ignore own events)
        var remoteRoomsChanged = MessageBus.Current
            .Listen<RoomEventMessage>(RoomsKey)
            .Where(static m => m.InstanceId != Services.AppInstance.Id)
            .Do(HandleRemoteRoomEvent)
            .Select(static _ => RxVoid.Default);

        RoomsChanged = Signal.Emit(RxVoid.Default)
            .Concat(Signal.Blend(localRoomsChanged, remoteRoomsChanged)
                .EmitIfQuiet(TimeSpan.FromMilliseconds(RoomsChangedQuietWindowMilliseconds), RxSchedulers.TaskpoolScheduler));

        _ = this.WhenAnyObservable(x => x.RoomsChanged)
            .Select(static _ => (IReadOnlyList<ChatRoom>)[.. GetState().Rooms])
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .ToProperty(this, nameof(Rooms), out _rooms);

        // Request a snapshot from peers shortly after activation
        _ = RxSchedulers.MainThreadScheduler.Schedule(RxVoid.Default, TimeSpan.FromMilliseconds(SyncRequestDelayMilliseconds), static (_, _) =>
        {
            var req = new RoomEventMessage(Services.RoomEventKind.SyncRequest, string.Empty)
            {
                InstanceId = Services.AppInstance.Id
            };
            Trace.TraceInformation("[Lobby] Broadcasting SyncRequest");
            MessageBus.Current.SendMessage(req, RoomsKey);
            return EmptyDisposable.Instance;
        });
    }

    /// <inheritdoc />
    public string UrlPathSegment { get; }

    /// <inheritdoc />
    public IScreen HostScreen { get; }

    /// <summary>Gets or sets the chat room associated with the current context.</summary>
    public ChatRoom? SelectedChatRoom
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the display name for the current user.</summary>
    [SuppressMessage(
        "StyleCop.CSharp.LayoutRules",
        "SA1500:Braces should not share line",
        Justification = "C# 13 field keyword with property initializer")]
    [SuppressMessage(
        "StyleCop.CSharp.LayoutRules",
        "SA1513:Closing brace should be followed by blank line",
        Justification = "C# 13 field keyword with property initializer")]
    public string DisplayName
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
= Environment.MachineName;

    /// <summary>Gets or sets the new room name.</summary>
    [SuppressMessage(
        "StyleCop.CSharp.LayoutRules",
        "SA1500:Braces should not share line",
        Justification = "C# 13 field keyword with property initializer")]
    [SuppressMessage(
        "StyleCop.CSharp.LayoutRules",
        "SA1513:Closing brace should be followed by blank line",
        Justification = "C# 13 field keyword with property initializer")]
    public string RoomName
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
= string.Empty;

    /// <summary>Gets the current list of rooms.</summary>
    public IReadOnlyList<ChatRoom> Rooms => _rooms.Value;

    /// <summary>Gets an observable signaling when the rooms change.</summary>
    public IObservable<RxVoid> RoomsChanged { get; }

    /// <summary>Gets the command which creates a new room.</summary>
    public ReactiveCommand<RxVoid, RxVoid> CreateRoom { get; }

    /// <summary>Gets the command which deletes a room.</summary>
    public ReactiveCommand<ChatRoom, RxVoid> DeleteRoom { get; }

    /// <summary>Gets the command which joins an existing room.</summary>
    public ReactiveCommand<ChatRoom, RxVoid> JoinRoom { get; }

    /// <summary>Gets a value indicating whether flag showing whether room can be created or not.</summary>
    public bool CreateRoomDisabled => !_createRoomDisabledHelper.Value;

    /// <summary>Gets a value indicating whether flag showing whether room can be deleted or not.</summary>
    public bool DeleteRoomDisabled => !_deleteRoomDisabledHelper.Value;

    /// <summary>Gets the current application chat state.</summary>
    /// <returns>The current <see cref="ChatState"/>.</returns>
    private static ChatState GetState() => RxSuspension.SuspensionHost.GetAppState<ChatState>();

    /// <summary>Handles an incoming remote room event by answering sync requests or applying room changes.</summary>
    /// <param name="evt">The remote room event.</param>
    private static void HandleRemoteRoomEvent(RoomEventMessage evt)
    {
        Trace.TraceInformation($"[Lobby] Room evt {evt.Kind} name='{evt.RoomName}' from={evt.InstanceId}");
        switch (evt.Kind)
        {
            case Services.RoomEventKind.SyncRequest:
                {
                    // Respond with our snapshot of room names
                    var snapshot = GetState().Rooms.ConvertAll(static r => r.Name);
                    var response = new RoomEventMessage(Services.RoomEventKind.Add, string.Empty)
                    {
                        Snapshot = snapshot,
                        InstanceId = Services.AppInstance.Id
                    };
                    MessageBus.Current.SendMessage(response, RoomsKey);
                    break;
                }

            default:
                {
                    ApplyRoomEvent(evt);
                    break;
                }
        }
    }

    /// <summary>Applies an incoming room event to the local state.</summary>
    /// <param name="evt">The room event to apply.</param>
    private static void ApplyRoomEvent(RoomEventMessage evt)
    {
        var state = GetState();

        if (evt.Snapshot is not null)
        {
            // Apply snapshot
            foreach (var name in evt.Snapshot)
            {
                if (!state.Rooms.Exists(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase)))
                {
                    state.Rooms.Add(new() { Name = name });
                }
            }

            return;
        }

        switch (evt.Kind)
        {
            case Services.RoomEventKind.Add:
                {
                    if (!state.Rooms.Exists(r => string.Equals(r.Name, evt.RoomName, StringComparison.OrdinalIgnoreCase)))
                    {
                        state.Rooms.Add(new() { Name = evt.RoomName });
                    }

                    break;
                }

            case Services.RoomEventKind.Remove:
                {
                    _ = state.Rooms.RemoveAll(r => string.Equals(r.Name, evt.RoomName, StringComparison.OrdinalIgnoreCase));
                    break;
                }

            default:
                {
                    // SyncRequest carries no room mutation; it is answered in HandleRemoteRoomEvent.
                    break;
                }
        }
    }

    /// <summary>Deletes the specified room and broadcasts the removal to peers.</summary>
    /// <param name="room">The room to delete.</param>
    private static void DeleteRoomImpl(ChatRoom room)
    {
        var state = GetState();
        if (!state.Rooms.Remove(room))
        {
            return;
        }

        var evt = new RoomEventMessage(Services.RoomEventKind.Remove, room.Name)
        {
            InstanceId = Services.AppInstance.Id
        };
        MessageBus.Current.SendMessage(evt, RoomsKey);
        MessageBus.Current.SendMessage(new ChatStateChanged());
        Trace.TraceInformation($"[Lobby] Deleted room '{room.Name}'");
    }

    /// <summary>Creates a new room from the current room name and broadcasts it to peers.</summary>
    private void CreateRoomImpl()
    {
        var name = RoomName.Trim();
        var state = GetState();
        ChatRoom? existing = null;
        foreach (var room in state.Rooms)
        {
            if (string.Equals(room.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                existing = room;
                break;
            }
        }

        if (existing is null)
        {
            var room = new ChatRoom { Name = name };
            state.Rooms.Add(room);

            // Broadcast room add to peers
            var evt = new RoomEventMessage(Services.RoomEventKind.Add, room.Name)
            {
                InstanceId = Services.AppInstance.Id
            };
            MessageBus.Current.SendMessage(evt, RoomsKey);
            Trace.TraceInformation($"[Lobby] Created room '{room.Name}'");
        }

        MessageBus.Current.SendMessage(new ChatStateChanged());
        RoomName = string.Empty;
    }
}
