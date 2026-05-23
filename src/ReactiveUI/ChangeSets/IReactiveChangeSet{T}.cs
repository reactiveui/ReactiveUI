// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// A batch of collection changes produced by the <see cref="ChangeSetExtensions"/> change-set observers.
/// </summary>
/// <typeparam name="T">The collection item type.</typeparam>
public interface IReactiveChangeSet<T> : IReactiveChangeSet, IReadOnlyList<ReactiveChange<T>>;
