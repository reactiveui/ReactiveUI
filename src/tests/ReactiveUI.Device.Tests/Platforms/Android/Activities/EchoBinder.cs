// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.OS;

namespace ReactiveUI.Device.Tests;

/// <summary>The binder <see cref="EchoService"/> returns; a local client receives this same instance.</summary>
public class EchoBinder : Binder
{
    /// <summary>Returns the text it was given, so a test can call through the bound service.</summary>
    /// <param name="text">The text to echo.</param>
    /// <returns>The same text.</returns>
    public string Echo(string text) => text;
}
