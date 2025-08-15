// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using System.Text.Json;

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

        // Outgoing chat messages (default contract)
        MessageBus.Current.Listen<ChatNetworkMessage>()
            .Subscribe(Send);

        // Outgoing room events
        MessageBus.Current.Listen<RoomEventMessage>(contract: RoomsContract)
            .Subscribe(Send);
    }

    /// <summary>
    /// Starts the background receive loop.
    /// </summary>
    public void Start() => Task.Run(ReceiveLoop, _cts.Token);

    /// <inheritdoc />
    public void Dispose()
    {
        _cts.Cancel();
        _udp.Dispose();
        _cts.Dispose();
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
        }
        catch
        {
            return;
        }

        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var result = await listener.ReceiveAsync(_cts.Token).ConfigureAwait(false);
                var buffer = result.Buffer;

                // Inspect JSON for known properties to determine message type
                using var doc = JsonDocument.Parse(buffer);
                var root = doc.RootElement;
                var isRoomEvent = root.TryGetProperty("Kind", out _) || root.TryGetProperty("Snapshot", out _);

                if (isRoomEvent)
                {
                    var evt = JsonSerializer.Deserialize<RoomEventMessage>(buffer);
                    if (evt is not null)
                    {
                        MessageBus.Current.SendMessage(evt, contract: RoomsContract);
                    }

                    continue;
                }

                // Otherwise treat as chat message
                var chat = JsonSerializer.Deserialize<ChatNetworkMessage>(buffer);
                if (chat is not null)
                {
                    MessageBus.Current.SendMessage(chat, contract: chat.RoomName);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                // ignore malformed input
            }
        }
    }

    private void Send(object message)
    {
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType());
            _udp.Send(bytes, bytes.Length, _sendEndpoint);
        }
        catch
        {
            // ignore
        }
    }
}
