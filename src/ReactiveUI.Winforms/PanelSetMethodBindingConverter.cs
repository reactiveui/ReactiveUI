// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Winforms;

/// <summary>
/// A converter that can handle setting values on a <see cref="Panel"/> control for binding.
/// </summary>
public class PanelSetMethodBindingConverter : ISetMethodBindingConverter
{
    /// <inheritdoc />
    public int GetAffinityForObjects(Type? fromType, Type? toType)
    {
        if (toType != typeof(Control.ControlCollection))
        {
            return 0;
        }

        return fromType?.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>) && x.GetGenericArguments()[0].IsSubclassOf(typeof(Control))) ?? false
            ? 10
            : 0;
    }

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

        if (newValue is not IEnumerable<Control> newValueEnumerable)
        {
            throw new ArgumentException($"newValue must be {nameof(newValue)}", nameof(newValue));
        }

        var targetCollection = (Control.ControlCollection)toTarget;

        targetCollection.Owner.SuspendLayout();

        targetCollection.Clear();
        targetCollection.AddRange(newValueEnumerable.ToArray());

        targetCollection.Owner.ResumeLayout();

        return targetCollection;
    }
}
