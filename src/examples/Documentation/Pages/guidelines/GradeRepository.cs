// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>
/// A plain class with a method a view calls directly, the way pre-ReactiveUI code wires a button to a handler by
/// naming convention instead of binding to a command.
/// </summary>
[System.Diagnostics.DebuggerDisplay("PendingGrades = {PendingGrades.Count}")]
public sealed class GradeRepository
{
    /// <summary>The grades waiting to be submitted.</summary>
    private readonly List<string> _pendingGrades = ["Ada: 92"];

    /// <summary>Gets the grades waiting to be submitted.</summary>
    public IReadOnlyList<string> PendingGrades => _pendingGrades;

    /// <summary>Submits the oldest pending grade. Nothing stops a caller from calling this when there is nothing to submit.</summary>
    public void Submit()
    {
        if (_pendingGrades.Count == 0)
        {
            Console.WriteLine("Nothing to submit, but the button let the click through anyway.");
            return;
        }

        Console.WriteLine($"Submitted {_pendingGrades[0]}");
        _pendingGrades.RemoveAt(0);
    }
}
