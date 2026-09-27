// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>
/// A display board that posts each announcement it receives directly, by implementing <see cref="IObserver{T}"/>
/// itself. It only needs a command that produces <c>string</c> results, not any particular command subclass.
/// </summary>
public sealed class DeskAnnouncementBoard : IObserver<string>
{
    /// <inheritdoc/>
    public void OnNext(string value) => Console.WriteLine($"Board: {value}");

    /// <inheritdoc/>
    public void OnError(Exception error) => Console.WriteLine($"Board offline: {error.Message}");

    /// <inheritdoc/>
    public void OnCompleted() => Console.WriteLine("Board cleared");
}
