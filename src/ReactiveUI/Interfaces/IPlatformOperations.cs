// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Additional details implemented by the different ReactiveUI platform projects.
/// </summary>
public interface IPlatformOperations
{
    /// <summary>
    /// Gets a descriptor that describes (if applicable) the orientation
    /// of the screen.
    /// </summary>
    /// <returns>The device orientation descriptor.</returns>
    string? GetOrientation();
}