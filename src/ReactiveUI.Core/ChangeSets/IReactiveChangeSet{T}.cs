// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>A batch of collection changes produced by the <see cref="ChangeSetExtensions"/> change-set observers.</summary>
/// <typeparam name="T">The collection item type.</typeparam>
[SuppressMessage(
    "Design",
    "SST2320:An interface inherits two interfaces that declare the same member",
    Justification = "Re-declaring GetEnumerator with 'new' breaks implementers whose value-type enumerator cannot satisfy an IEnumerator<T>-returning member (CS0738).")]
public interface IReactiveChangeSet<T> : IReactiveChangeSet, IReadOnlyList<ReactiveChange<T>>;
