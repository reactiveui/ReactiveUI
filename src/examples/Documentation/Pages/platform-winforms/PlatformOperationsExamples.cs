// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// Shows <see cref="Winforms.PlatformOperations"/>, the WinForms answer to "what orientation is the device in?".
/// Qualified as <c>Winforms.PlatformOperations</c>: this code's own namespace nests under <c>ReactiveUI</c>, and
/// <c>ReactiveUI</c> also carries a platform-specific <c>PlatformOperations</c> on some target frameworks, so an
/// unqualified name could silently bind to the wrong type.
/// </summary>
public static class PlatformOperationsExamples
{
    /// <summary>WinForms runs on the desktop, which has no orientation, so the answer is always null.</summary>
    public static void CheckOrientation()
    {
        Winforms.PlatformOperations operations = new();

        Console.WriteLine(operations.GetOrientation() ?? "(null)");

        // Output:
        // (null)
    }
}
