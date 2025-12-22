// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text.Json;

using ReactiveUI.Builder.WpfApp.Models;

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>
/// A simple UDP-based network relay to share chat messages and room events between app instances.
/// </summary>
public sealed class ChatNetworkService : IDisposable
{
    private const string RoomsContract = "__rooms__";
    private const int Port = 54545;

    // IPv4 local multicast address
    private static readonly IPAddress MulticastAddress = IPAddress.Parse("239.255.0.1");

    private readonly UdpClient _udp; // sender
    private readonly IPEndPoint _sendEndpoint;
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatNetworkService"/> class.
    /// </summary>
    public ChatNetworkService()
    {
        _sendEndpoint = new IPEndPoint(MulticastAddress, Port);
        _udp = new UdpClient(AddressFamily.InterNetwork);

        try
        {
            // Enable multicast loopback so we can also receive our messages (we filter locally using InstanceId)
            _udp.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
            _udp.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
        }
        catch
        {
            // ignore
        }

        // Outgoing chat messages (default contract) - only send messages originating from this instance
        MessageBus.Current.Listen<ChatNetworkMessage>()
            .Where(static m => m.InstanceId == AppInstance.Id)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Subscribe(Send);

        // Outgoing room events - only send messages originating from this instance
        MessageBus.Current.Listen<RoomEventMessage>(contract: RoomsContract)
            .Where(static m => m.InstanceId == AppInstance.Id)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Subscribe(Send);

        Trace.WriteLine("[Net] ChatNetworkService initialized.");
    }

    /// <summary>
    /// Starts the background receive loop.
    /// </summary>
    public void Start()
    {
        Trace.WriteLine("[Net] Starting receive loop...");
        Task.Run(ReceiveLoop, _cts.Token);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _cts.Cancel();
        _udp.Dispose();
        _cts.Dispose();
        Trace.WriteLine("[Net] Disposed.");
    }

    private async Task ReceiveLoop()
    {
        using var listener = new UdpClient(AddressFamily.InterNetwork);
        try
        {
            // Allow multiple processes to bind the same UDP port
            listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.ExclusiveAddressUse = false;
            listener.Client.Bind(new IPEndPoint(IPAddress.Any, Port));

            // Join multicast group on default interface
            listener.JoinMulticastGroup(MulticastAddress);
            Trace.WriteLine($"[Net] Listening on {MulticastAddress}:{Port}");
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"[Net] Failed to start listener: {ex.Message}");
            return;
        }

        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var result = await listener.ReceiveAsync(_cts.Token).ConfigureAwait(false);
                var buffer = result.Buffer;

                using var doc = JsonDocument.Parse(buffer);
                var root = doc.RootElement;
                var isRoomEvent = root.TryGetProperty("Kind", out _) || root.TryGetProperty("Snapshot", out _);

                if (isRoomEvent)
                {
                    var evt = JsonSerializer.Deserialize<RoomEventMessage>(buffer);
                    if (evt is not null)
                    {
                        if (evt.InstanceId == AppInstance.Id)
                        {
                            // Ignore our own looped-back packet
                            continue;
                        }

                        Trace.WriteLine($"[Net] RX RoomEvent {evt.Kind} name='{evt.RoomName}' from={evt.InstanceId}");
                        MessageBus.Current.SendMessage(evt, contract: RoomsContract);
                    }

                    continue;
                }

                var chat = JsonSerializer.Deserialize<ChatNetworkMessage>(buffer);
                if (chat is not null)
                {
                    if (chat.InstanceId == AppInstance.Id)
                    {
                        // Ignore our own looped-back packet
                        continue;
                    }

                    Trace.WriteLine($"[Net] RX Chat '{chat.Text}' in '{chat.RoomName}' from={chat.Sender}/{chat.InstanceId}");
                    MessageBus.Current.SendMessage(chat, contract: chat.RoomName);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[Net] RX error: {ex.Message}");
            }
        }
    }

    private void Send(object message)
    {
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType());
            _udp.Send(bytes, bytes.Length, _sendEndpoint);
            switch (message)
            {
                case ChatNetworkMessage c:
                    Trace.WriteLine($"[Net] TX Chat '{c.Text}' in '{c.RoomName}' from={c.Sender}/{c.InstanceId}");
                    break;
                case RoomEventMessage r:
                    Trace.WriteLine($"[Net] TX RoomEvent {r.Kind} name='{r.RoomName}' from={r.InstanceId}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"[Net] TX error: {ex.Message}");
        }
    }
}
