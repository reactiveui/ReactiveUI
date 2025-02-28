// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI;

/// <summary>
/// Mixin associated with the DynamicData IChangeSet class.
/// </summary>
public static class ChangeSetMixin
{
    /// <summary>
    /// Is the change set associated with a count change.
    /// </summary>
    /// <param name="changeSet">The change list to evaluate.</param>
    /// <returns>If the change set is caused by the count being changed.</returns>
    public static bool HasCountChanged(this IChangeSet changeSet) // TODO: Create Test
    {
        changeSet.ArgumentNullExceptionThrowIfNull(nameof(changeSet));

        return changeSet.Adds > 0 || changeSet.Removes > 0;
    }

    /// <summary>
    /// Is the change set associated with a count change.
    /// </summary>
    /// <param name="changeSet">The change list to evaluate.</param>
    /// <returns>An observable of changes that only have count changes.</returns>
    public static IObservable<IChangeSet> CountChanged(this IObservable<IChangeSet> changeSet) => changeSet.Where(HasCountChanged); // TODO: Create Test

    /// <summary>
    /// Is the change set associated with a count change.
    /// </summary>
    /// <typeparam name="T">The change set type.</typeparam>
    /// <param name="changeSet">The change list to evaluate.</param>
    /// <returns>An observable of changes that only have count changes.</returns>
    public static IObservable<IChangeSet<T>> CountChanged<T>(this IObservable<IChangeSet<T>> changeSet)
        where T : notnull =>
        changeSet.Where(x => x.HasCountChanged()); // TODO: Create Test
}
