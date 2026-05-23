// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Common affinity scores shared by binding type converters, command binders, property
/// observation factories and view activation fetchers. A higher value indicates a stronger
/// match; zero means the candidate does not apply.
/// </summary>
internal static class BindingAffinity
{
    /// <summary>The affinity returned by the built-in value and string type converters.</summary>
    public const int DefaultInternalTypeConverter = 2;

    /// <summary>The affinity for binding to a type's conventional default event.</summary>
    public const int DefaultEvent = 3;

    /// <summary>The affinity for an explicit or interface-based match, such as INotifyPropertyChanged or a named event.</summary>
    public const int Explicit = 5;

    /// <summary>The affinity for a strong, exact-type match, such as IReactiveObject or ICanActivate.</summary>
    public const int ExactType = 10;
}
