// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>Internal helpers for observable composition used by the WhenAny mixins.</summary>
internal static class ObservableExtensions
{
    /// <summary>Provides EmptyIfNull extension members for <see cref="IObservable{T}"/>.</summary>
    /// <typeparam name="T">The type of the observable's values.</typeparam>
    /// <param name="this">The source observable, which may be null.</param>
    extension<T>(IObservable<T> @this)
    {
        /// <summary>Returns the source observable, or an empty observable when it is null.</summary>
        /// <returns>The source observable, or an empty observable when the source is null.</returns>
        public IObservable<T> EmptyIfNull() =>
            @this ?? Signal.None<T>();
    }
}
