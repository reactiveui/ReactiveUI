// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;

namespace ReactiveUI.Tests.Mocks;

/// <summary>
///     A bind view model.
/// </summary>
/// <seealso cref="ReactiveObject" />
public class InteractionBindViewModel : ReactiveObject
{
    private Interaction<string, bool> _interaction1;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InteractionBindViewModel" /> class.
    /// </summary>
    // Use ImmediateScheduler so Handle completes synchronously and cannot deadlock on an ambient
    // CurrentThreadScheduler trampoline left active by another test on the shared (non-parallel) test thread.
    public InteractionBindViewModel() => _interaction1 = new(ImmediateScheduler.Instance);

    /// <summary>
    ///     Gets or sets the interaction1.
    /// </summary>
    public Interaction<string, bool> Interaction1
    {
        get => _interaction1;
        set => this.RaiseAndSetIfChanged(ref _interaction1, value);
    }
}
