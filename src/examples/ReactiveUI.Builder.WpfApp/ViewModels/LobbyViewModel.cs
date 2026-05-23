// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.Builder.WpfApp.Models;

namespace ReactiveUI.Builder.WpfApp.ViewModels;

/// <summary>
/// The lobby view model which lists rooms and allows creating/joining rooms.
/// </summary>
public class LobbyViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>
    /// The MessageBus contract used to broadcast and listen for room events across instances.
    /// </summary>
    private const string RoomsKey = "__rooms__";

    /// <summary>
    /// Backs the <see cref="Rooms"/> output property, projecting room changes into a snapshot list.
    /// </summary>
    private readonly ObservableAsPropertyHelper<IReadOnlyList<ChatRoom>> _rooms;

    /// <summary>
    /// Initializes a new instance of the <see cref="LobbyViewModel"/> class.
    /// </summary>
    /// <param name="hostScreen">The host screen.</param>
    [SuppressMessage("Reliability", "S3366:Don't expose 'this' in constructors", Justification = "OAPH/WhenAny initialization requires 'this'; single-threaded sample.")]
    public LobbyViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        UrlPathSegment = "lobby";

        var canCreate =
            this.WhenAnyValue<LobbyViewModel, bool, string>(nameof(RoomName), rn => !string.IsNullOrWhiteSpace(rn));
        CreateRoom = ReactiveCommand.Create(CreateRoomImpl, canCreate);

        DeleteRoom = ReactiveCommand.Create<ChatRoom>(DeleteRoomImpl);

        JoinRoom = ReactiveCommand.CreateFromTask<ChatRoom>(async room =>
        {
            ArgumentExceptionHelper.ThrowIfNull(room);
            await HostScreen.Router.Navigate.Execute(new ChatRoomViewModel(HostScreen, room, DisplayName));
        });

        // Local changes
        var localRoomsChanged = MessageBus.Current.Listen<ChatStateChanged>().Select(_ => Unit.Default);

        // Remote changes and sync (ignore own events)
        var remoteRoomsChanged = MessageBus.Current
            .Listen<RoomEventMessage>(RoomsKey)
            .Where(m => m.InstanceId != Services.AppInstance.Id)
            .Do(evt =>
            {
                Trace.TraceInformation($"[Lobby] Room evt {evt.Kind} name='{evt.RoomName}' from={evt.InstanceId}");
                switch (evt.Kind)
                {
                    case Services.RoomEventKind.SyncRequest:
                        {
                            // Respond with our snapshot of room names
                            var snapshot = GetState().Rooms.ConvertAll(r => r.Name);
                            var response = new RoomEventMessage(Services.RoomEventKind.Add, string.Empty)
                            {
                                Snapshot = snapshot, InstanceId = Services.AppInstance.Id
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
            })
            .Select(_ => Unit.Default);

        RoomsChanged = localRoomsChanged.Merge(remoteRoomsChanged)
            .Throttle(TimeSpan.FromMilliseconds(50), RxSchedulers.TaskpoolScheduler);

        this.WhenAnyObservable(x => x.RoomsChanged)
            .StartWith(Unit.Default)
            .Select(_ => (IReadOnlyList<ChatRoom>)[.. GetState().Rooms])
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .ToProperty(this, nameof(Rooms), out _rooms);

        // Request a snapshot from peers shortly after activation
        RxSchedulers.MainThreadScheduler.Schedule(Unit.Default, TimeSpan.FromMilliseconds(500), (_, _) =>
        {
            var req = new RoomEventMessage(Services.RoomEventKind.SyncRequest, string.Empty)
            {
                InstanceId = Services.AppInstance.Id
            };
            Trace.TraceInformation("[Lobby] Broadcasting SyncRequest");
            MessageBus.Current.SendMessage(req, RoomsKey);
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

    /// <summary>
    /// Gets or sets the new room name.
    /// </summary>
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

    /// <summary>
    /// Gets the current persisted chat state from the ReactiveUI suspension host.
    /// </summary>
    /// <returns>The active <see cref="ChatState"/> instance.</returns>
    private static ChatState GetState() => RxSuspension.SuspensionHost.GetAppState<ChatState>();

    /// <summary>
    /// Applies a remote room event (add, remove, or snapshot) to the local chat state.
    /// </summary>
    /// <param name="evt">The room event received from another app instance.</param>
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
                    state.Rooms.RemoveAll(r => string.Equals(r.Name, evt.RoomName, StringComparison.OrdinalIgnoreCase));
                    break;
                }
        }
    }

    /// <summary>
    /// Creates a room from the current <see cref="RoomName"/> if it does not already exist, broadcasts the
    /// addition to other instances, and clears the input. Backs the <see cref="CreateRoom"/> command.
    /// </summary>
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

    /// <summary>
    /// Removes the supplied room from the local state and broadcasts the removal to other instances.
    /// Backs the <see cref="DeleteRoom"/> command.
    /// </summary>
    /// <param name="room">The room to delete.</param>
    private void DeleteRoomImpl(ChatRoom room)
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
}
