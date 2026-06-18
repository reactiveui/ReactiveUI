// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

namespace ReactiveUI;

/// <summary>A non-generic view of a batch of collection changes, exposing the add/remove counts used to detect count changes.</summary>
public interface IReactiveChangeSet : IEnumerable
{
    /// <summary>Gets the number of <see cref="ReactiveChangeReason.Add"/> changes in the set.</summary>
    int Adds { get; }

    /// <summary>Gets the number of <see cref="ReactiveChangeReason.Remove"/> changes in the set.</summary>
    int Removes { get; }
}
