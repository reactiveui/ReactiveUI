// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

/// <summary>A mock view model used by tests.</summary>
public class FooViewModel : ReactiveObject
{
    /// <summary>The latency, in milliseconds, modelled for each asynchronous set.</summary>
    private const int SetValueDelayMilliseconds = 10;

    /// <summary>Initializes a new instance of the <see cref="FooViewModel" /> class.</summary>
    /// <param name="foo">The foo model.</param>
    public FooViewModel(Foo foo)
    {
        Foo = foo ?? throw new ArgumentNullException(nameof(foo));

        // The per-set latency is a scheduler-native DelaySubscription (not a delay inside the awaited task): virtual-time
        // tests drive RxSchedulers.TaskpoolScheduler deterministically, whereas a thread-pool-bridged Task delay races them.
        this.WhenAnyValue(static x => x.Setpoint)
            .SelectMany(value => Signal.FromAsync(() => foo.SetValueAsync(value))
                .DelaySubscription(TimeSpan.FromMilliseconds(SetValueDelayMilliseconds), RxSchedulers.TaskpoolScheduler))
            .Subscribe();
    }

    /// <summary>Gets the foo model.</summary>
    public Foo Foo { get; }

    /// <summary>Gets or sets the setpoint.</summary>
    public int Setpoint
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
