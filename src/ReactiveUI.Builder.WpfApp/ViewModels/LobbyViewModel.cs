// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactiveUI.Builder.WpfApp.ViewModels;

/// <summary>
/// The lobby view model which lists rooms and allows creating/joining rooms.
/// </summary>
public class LobbyViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ObservableAsPropertyHelper<IReadOnlyList<ChatRoom>> _rooms;
    private readonly IScreen _hostScreen;
    private string _roomName = string.Empty;
    private string _displayName = Environment.MachineName;

    /// <summary>
    /// Initializes a new instance of the <see cref="LobbyViewModel"/> class.
    /// </summary>
    /// <param name="hostScreen">The host screen.</param>
    public LobbyViewModel(IScreen hostScreen)
    {
        _hostScreen = hostScreen;
        HostScreen = hostScreen;
        UrlPathSegment = "lobby";

        var canCreate = this.WhenAnyValue(x => x.RoomName, rn => !string.IsNullOrWhiteSpace(rn));
        CreateRoom = ReactiveCommand.Create(CreateRoomImpl, canCreate);

        DeleteRoom = ReactiveCommand.Create<ChatRoom>(DeleteRoomImpl);

        JoinRoom = ReactiveCommand.CreateFromTask<ChatRoom>(async room =>
        {
            ArgumentNullException.ThrowIfNull(room);
            await HostScreen.Router.Navigate.Execute(new ChatRoomViewModel(HostScreen, room, DisplayName));
        });

        // Local changes
        var localRoomsChanged = MessageBus.Current.Listen<ChatStateChanged>().Select(_ => Unit.Default);

        // Remote changes and sync
        var remoteRoomsChanged = MessageBus.Current
            .Listen<Services.RoomEventMessage>(contract: "__rooms__")
            .Where(m => m.InstanceId != Services.AppInstance.Id)
            .Do(evt =>
            {
                switch (evt.Kind)
                {
                    case Services.RoomEventKind.SyncRequest:
                        // Respond with our snapshot of room names
                        var snapshot = GetState().Rooms.ConvertAll(r => r.Name);
                        var response = new Services.RoomEventMessage(Services.RoomEventKind.Add, string.Empty)
                        {
                            Snapshot = snapshot,
                            InstanceId = Services.AppInstance.Id,
                        };
                        MessageBus.Current.SendMessage(response, contract: "__rooms__");
                        break;
                    default:
                        ApplyRoomEvent(evt);
                        break;
                }
            })
            .Select(_ => Unit.Default);

        RoomsChanged = localRoomsChanged.Merge(remoteRoomsChanged);

        this.WhenAnyObservable(x => x.RoomsChanged)
            .StartWith(Unit.Default)
            .Select(_ => (IReadOnlyList<ChatRoom>)[.. GetState().Rooms])
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, nameof(Rooms), out _rooms);

        // Request a snapshot from peers shortly after activation
        RxApp.MainThreadScheduler.Schedule(Unit.Default, TimeSpan.FromMilliseconds(500), (s, __) =>
        {
            var req = new Services.RoomEventMessage(Services.RoomEventKind.SyncRequest, string.Empty) { InstanceId = Services.AppInstance.Id };
            MessageBus.Current.SendMessage(req, contract: "__rooms__");
            return Disposable.Empty;
        });
    }

    /// <inheritdoc />
    public string UrlPathSegment { get; }

    /// <inheritdoc />
    public IScreen HostScreen { get; }

    /// <summary>
    /// Gets or sets the display name for the current user.
    /// </summary>
    public string DisplayName
    {
        get => _displayName;
        set => this.RaiseAndSetIfChanged(ref _displayName, value);
    }

    /// <summary>
    /// Gets or sets the new room name.
    /// </summary>
    public string RoomName
    {
        get => _roomName;
        set => this.RaiseAndSetIfChanged(ref _roomName, value);
    }

    /// <summary>
    /// Gets the current list of rooms.
    /// </summary>
    public IReadOnlyList<ChatRoom> Rooms => _rooms.Value;

    /// <summary>
    /// Gets an observable signaling when the rooms change.
    /// </summary>
    public IObservable<Unit> RoomsChanged { get; }

    /// <summary>
    /// Gets the command which creates a new room.
    /// </summary>
    public ReactiveCommand<Unit, Unit> CreateRoom { get; }

    /// <summary>
    /// Gets the command which deletes a room.
    /// </summary>
    public ReactiveCommand<ChatRoom, Unit> DeleteRoom { get; }

    /// <summary>
    /// Gets the command which joins an existing room.
    /// </summary>
    public ReactiveCommand<ChatRoom, Unit> JoinRoom { get; }

    private static ChatState GetState() => RxApp.SuspensionHost.GetAppState<ChatState>();

    private static void ApplyRoomEvent(Services.RoomEventMessage evt)
    {
        var state = GetState();

        if (evt.Snapshot is not null)
        {
            // Apply snapshot
            foreach (var name in evt.Snapshot)
            {
                if (!state.Rooms.Any(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase)))
                {
                    state.Rooms.Add(new ChatRoom { Name = name });
                }
            }

            return;
        }

        switch (evt.Kind)
        {
            case Services.RoomEventKind.Add:
                if (!state.Rooms.Any(r => string.Equals(r.Name, evt.RoomName, StringComparison.OrdinalIgnoreCase)))
                {
                    state.Rooms.Add(new ChatRoom { Name = evt.RoomName });
                }

                break;
            case Services.RoomEventKind.Remove:
                state.Rooms.RemoveAll(r => string.Equals(r.Name, evt.RoomName, StringComparison.OrdinalIgnoreCase));
                break;
        }
    }

    private void CreateRoomImpl()
    {
        var name = RoomName.Trim();
        var state = GetState();
        var existing = state.Rooms.FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
        if (existing is null)
        {
            var room = new ChatRoom { Name = name };
            state.Rooms.Add(room);

            // Broadcast room add to peers
            var evt = new Services.RoomEventMessage(Services.RoomEventKind.Add, room.Name) { InstanceId = Services.AppInstance.Id };
            MessageBus.Current.SendMessage(evt, contract: "__rooms__");
        }

        MessageBus.Current.SendMessage(new ChatStateChanged());
        RoomName = string.Empty;
    }

    private void DeleteRoomImpl(ChatRoom room)
    {
        var state = GetState();
        if (state.Rooms.Remove(room))
        {
            var evt = new Services.RoomEventMessage(Services.RoomEventKind.Remove, room.Name) { InstanceId = Services.AppInstance.Id };
            MessageBus.Current.SendMessage(evt, contract: "__rooms__");
            MessageBus.Current.SendMessage(new ChatStateChanged());
        }
    }
}
