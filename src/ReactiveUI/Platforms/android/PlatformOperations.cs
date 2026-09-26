// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Android.Content;
using Android.Hardware.Display;
using Android.Runtime;
using Android.Views;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Returns the current orientation of the device on Android.</summary>
public class PlatformOperations : IPlatformOperations
{
    /// <inheritdoc/>
    /// <remarks>
    /// Reads the rotation of the default display through <see cref="DisplayManager"/>, which works from the
    /// application context. <c>GetSystemService</c> returns an untyped Java peer, so the result is converted
    /// with <c>JavaCast</c>; a C# type test on that peer always fails.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? GetOrientation() =>
        Application.Context.GetSystemService(Context.DisplayService)
            .JavaCast<DisplayManager>()?
            .GetDisplay(Display.DefaultDisplay)?
            .Rotation.ToString();
}
