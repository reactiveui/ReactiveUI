// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>Holds the tracks the library module knows about.</summary>
public interface ITrackLibrary
{
    /// <summary>Gets the titles of the tracks in the library.</summary>
    IReadOnlyList<string> Titles { get; }
}
