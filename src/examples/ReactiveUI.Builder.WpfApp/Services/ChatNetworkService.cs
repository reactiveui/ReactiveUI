// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using ReactiveUI.Builder.WpfApp.Models;

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>A simple UDP-based network relay to share chat messages and room events between app instances.</summary>
public sealed class ChatNetworkService : IDisposable
{
    /// <summary>The MessageBus contract used for room add/remove/sync events.</summary>
    private const string RoomsContract = "__rooms__";

    /// <summary>The UDP port that all instances send to and listen on.</summary>
    private const int Port = 54_545;

    /// <summary>The IPv4 local multicast address used to reach other instances on the same machine/network.</summary>
    private static readonly IPAddress MulticastAddress = IPAddress.Parse("239.255.0.1");

    /// <summary>The UDP client used to send outgoing packets.</summary>
    private readonly UdpClient _udp; // sender

    /// <summary>The multicast endpoint that outgoing packets are addressed to.</summary>
    private readonly IPEndPoint _sendEndpoint;

    /// <summary>Signals the background receive loop to stop when the service is disposed.</summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>Initializes a new instance of the <see cref="ChatNetworkService"/> class.</summary>
    public ChatNetworkService()
    {
        _sendEndpoint = new(MulticastAddress, Port);
        _udp = new(AddressFamily.InterNetwork);

        try
        {
            // Enable multicast loopback so we can also receive our messages (we filter locally using InstanceId)
            _udp.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
            _udp.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
        }
        catch (SocketException ex)
        {
            // Multicast options are best-effort; continue without them if the platform rejects them.
            Trace.TraceWarning($"[Net] Failed to configure multicast socket options: {ex.Message}");
        }

        // Outgoing chat messages (default contract) - only send messages originating from this instance
        _ = MessageBus.Current.Listen<ChatNetworkMessage>()
            .Where(static m => m.InstanceId == AppInstance.Id)
            .ObserveOn(RxSchedulers.TaskpoolScheduler)
            .Subscribe(Send);

        // Outgoing room events - only send messages originating from this instance
        _ = MessageBus.Current.Listen<RoomEventMessage>(RoomsContract)
            .Where(static m => m.InstanceId == AppInstance.Id)
            .ObserveOn(RxSchedulers.TaskpoolScheduler)
            .Subscribe(Send);

        Trace.TraceInformation("[Net] ChatNetworkService initialized.");
    }

    /// <summary>Starts the background receive loop.</summary>
    public void Start()
    {
        Trace.TraceInformation("[Net] Starting receive loop...");
        _ = Task.Run(ReceiveLoop, _cts.Token);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _cts.Cancel();
        _udp.Dispose();
        _cts.Dispose();
        Trace.TraceInformation("[Net] Disposed.");
    }

    /// <summary>Configures and binds the supplied listener to the multicast group used by the service.</summary>
    /// <param name="listener">The UDP client to configure and bind.</param>
    /// <returns><see langword="true"/> if the listener started successfully; otherwise <see langword="false"/>.</returns>
    private static bool TryStartListener(UdpClient listener)
    {
        try
        {
            // Allow multiple processes to bind the same UDP port
            listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.ExclusiveAddressUse = false;
            listener.Client.Bind(new IPEndPoint(IPAddress.Any, Port));

            // Join multicast group on default interface
            listener.JoinMulticastGroup(MulticastAddress);
            Trace.TraceInformation($"[Net] Listening on {MulticastAddress}:{Port}");
            return true;
        }
        catch (Exception ex)
        {
            Trace.TraceInformation($"[Net] Failed to start listener: {ex.Message}");
            return false;
        }
    }

    /// <summary>Deserializes a received packet and routes it to the appropriate handler.</summary>
    /// <param name="buffer">The raw bytes of the received packet.</param>
    private static void DispatchPacket(byte[] buffer)
    {
        using var doc = JsonDocument.Parse(buffer);
        var root = doc.RootElement;
        var isRoomEvent = root.TryGetProperty("Kind", out _) || root.TryGetProperty("Snapshot", out _);

        if (isRoomEvent)
        {
            HandleRoomEvent(buffer);
        }
        else
        {
            HandleChatMessage(buffer);
        }
    }

    /// <summary>Deserializes and republishes a room event, ignoring packets that originated from this instance.</summary>
    /// <param name="buffer">The raw bytes of the received room-event packet.</param>
    private static void HandleRoomEvent(byte[] buffer)
    {
        var evt = JsonSerializer.Deserialize<RoomEventMessage>(buffer);
        if (evt is null || evt.InstanceId == AppInstance.Id)
        {
            // null payload or our own looped-back packet
            return;
        }

        Trace.TraceInformation($"[Net] RX RoomEvent {evt.Kind} name='{evt.RoomName}' from={evt.InstanceId}");
        MessageBus.Current.SendMessage(evt, RoomsContract);
    }

    /// <summary>Deserializes and republishes a chat message, ignoring packets that originated from this instance.</summary>
    /// <param name="buffer">The raw bytes of the received chat packet.</param>
    private static void HandleChatMessage(byte[] buffer)
    {
        var chat = JsonSerializer.Deserialize<ChatNetworkMessage>(buffer);
        if (chat is null || chat.InstanceId == AppInstance.Id)
        {
            // null payload or our own looped-back packet
            return;
        }

        Trace.TraceInformation(
            $"[Net] RX Chat '{chat.Text}' in '{chat.RoomName}' from={chat.Sender}/{chat.InstanceId}");
        MessageBus.Current.SendMessage(chat, chat.RoomName);
    }

    /// <summary>
    /// Continuously receives UDP packets, deserializes them into chat or room messages, and republishes
    /// them on the local <see cref="MessageBus"/> until cancellation is requested.
    /// </summary>
    /// <returns>A task that completes when the receive loop ends.</returns>
    private async Task ReceiveLoop()
    {
        using var listener = new UdpClient(AddressFamily.InterNetwork);
        if (!TryStartListener(listener))
        {
            return;
        }

        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var result = await listener.ReceiveAsync().ConfigureAwait(false);
                DispatchPacket(result.Buffer);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"[Net] RX error: {ex.Message}");
            }
        }
    }

    /// <summary>Serializes the supplied message to JSON and broadcasts it to the multicast endpoint.</summary>
    /// <param name="message">The chat or room message to broadcast.</param>
    private void Send(object message)
    {
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType());
            _ = _udp.Send(bytes, bytes.Length, _sendEndpoint);
            switch (message)
            {
                case ChatNetworkMessage c:
                    {
                        Trace.TraceInformation($"[Net] TX Chat '{c.Text}' in '{c.RoomName}' from={c.Sender}/{c.InstanceId}");
                        break;
                    }

                case RoomEventMessage r:
                    {
                        Trace.TraceInformation($"[Net] TX RoomEvent {r.Kind} name='{r.RoomName}' from={r.InstanceId}");
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            Trace.TraceInformation($"[Net] TX error: {ex.Message}");
        }
    }
}
