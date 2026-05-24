// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks <see cref="MessageBus"/> throughput: sending messages to a live listener and cold listener subscription.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class MessageBusBenchmarks
{
    /// <summary>The number of messages sent per benchmark invocation.</summary>
    private const int MessageCount = 10_000;

    /// <summary>Sink for the listened messages.</summary>
    private readonly NoopObserver<int> _sink = new();

    /// <summary>The message bus under test.</summary>
    private MessageBus _bus = null!;

    /// <summary>The standing listener subscription.</summary>
    private IDisposable _subscription = null!;

    /// <summary>Creates a message bus and a standing listener for the message type.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _bus = new MessageBus();
        _subscription = _bus.Listen<int>().Subscribe(_sink);
    }

    /// <summary>Disposes the standing listener.</summary>
    [GlobalCleanup]
    public void Cleanup() => _subscription.Dispose();

    /// <summary>Measures repeated message delivery to the standing listener.</summary>
    [Benchmark]
    public void SendMessage()
    {
        for (var i = 0; i < MessageCount; i++)
        {
            _bus.SendMessage(i);
        }
    }

    /// <summary>Measures a cold listen + dispose of a listener subscription.</summary>
    [Benchmark]
    public void ListenSubscribe()
    {
        using var subscription = _bus.Listen<int>().Subscribe(_sink);
    }
}
