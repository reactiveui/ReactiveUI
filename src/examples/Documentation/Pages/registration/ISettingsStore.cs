// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>Holds the app's settings for as long as the app runs.</summary>
public interface ISettingsStore
{
    /// <summary>Gets or sets the volume, from 0 to 100.</summary>
    int Volume { get; set; }
}
