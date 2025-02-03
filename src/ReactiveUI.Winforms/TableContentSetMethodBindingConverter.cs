// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Winforms;

/// <summary>
/// A binding set converter which will convert from a Table.
/// </summary>
public class TableContentSetMethodBindingConverter : ISetMethodBindingConverter
{
    /// <inheritdoc />
    public int GetAffinityForObjects(Type? fromType, Type? toType) =>
        toType != typeof(TableLayoutControlCollection)
            ? 0
            : fromType?.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>) && x.GetGenericArguments()[0].IsSubclassOf(typeof(Control))) ?? false
                ? 15
                : 0;

    /// <inheritdoc />
    public object PerformSet(object? toTarget, object? newValue, object?[]? arguments)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(toTarget);
#else
        if (toTarget is null)
        {
            throw new ArgumentNullException(nameof(toTarget));
        }
#endif

        if (toTarget is not TableLayoutControlCollection targetCollection)
        {
            throw new ArgumentException($"{nameof(toTarget)} must be of type {nameof(TableLayoutControlCollection)}", nameof(toTarget));
        }

        if (newValue is not IEnumerable<Control> newValueEnumerable)
        {
            throw new ArgumentException($"newValue must be {nameof(newValue)}", nameof(newValue));
        }

        targetCollection.Container.SuspendLayout();

        targetCollection.Clear();

        targetCollection.AddRange(newValueEnumerable.ToArray());

        targetCollection.Container.ResumeLayout();
        return targetCollection;
    }
}
