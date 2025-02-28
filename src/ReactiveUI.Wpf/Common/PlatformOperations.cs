// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if HAS_UNO
namespace ReactiveUI.Uno
#else
namespace ReactiveUI;
#endif

/// <summary>
/// Returns the current orientation of the device on Windows.
/// </summary>
public class PlatformOperations : IPlatformOperations
{
    /// <inheritdoc/>
    public string? GetOrientation()
    {
#if NETFX_CORE || HAS_UNO
        try
        {
            return Windows.Graphics.Display.DisplayInformation.GetForCurrentView().CurrentOrientation.ToString();
        }
        catch (Exception)
        {
            return null;
        }
#else
#pragma warning disable IDE0022 // Use expression body for method
        return null;
#pragma warning restore IDE0022 // Use expression body for method
#endif
    }
}
