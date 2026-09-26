// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>Shows <see cref="PlatformOperations"/>, MAUI's answer to "what orientation is the device in?".</summary>
public static class PlatformOperationsExamples
{
    /// <summary>MAUI has no cross-platform orientation API of its own, so the answer is always null.</summary>
    public static void CheckOrientation()
    {
        PlatformOperations operations = new();

        Console.WriteLine(operations.GetOrientation() ?? "(null)");

        // Output:
        // (null)
    }
}
