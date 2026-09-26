// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>Plays one track at a time. A fresh engine is cheap, so nothing about it needs to be shared.</summary>
public interface IPlaybackEngine
{
    /// <summary>Gets the number that tells one engine instance apart from another in the examples.</summary>
    int InstanceId { get; }
}
