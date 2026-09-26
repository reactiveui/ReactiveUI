// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;
using Android.OS;

namespace ReactiveUI.Device.Tests;

/// <summary>A local bound service the service-binding tests connect to.</summary>
[Service(Exported = false)]
public class EchoService : Service
{
    /// <summary>The binder handed to every client.</summary>
    private readonly EchoBinder _binder = new();

    /// <inheritdoc/>
    public override IBinder? OnBind(Intent? intent) => _binder;
}
