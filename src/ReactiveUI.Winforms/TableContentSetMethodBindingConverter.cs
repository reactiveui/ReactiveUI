// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Winforms;
#else
namespace ReactiveUI.Winforms;
#endif

/// <summary>A binding set converter which will convert from a Table.</summary>
public class TableContentSetMethodBindingConverter : ISetMethodBindingConverter
{
    /// <summary>The affinity returned when the source can be bound to a control collection.</summary>
    private const int ControlCollectionAffinity = 10;

    /// <inheritdoc />
    public int GetAffinityForObjects(Type? fromType, Type? toType)
    {
        if (toType != typeof(TableLayoutControlCollection) || fromType is null)
        {
            return 0;
        }

        var implementsControlEnumerable = Array.Exists(fromType.GetInterfaces(), static x =>
            x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
            x.GetGenericArguments()[0].IsSubclassOf(typeof(Control)));

        return implementsControlEnumerable ? ControlCollectionAffinity : 0;
    }

    /// <inheritdoc />
    public object PerformSet(object? toTarget, object? newValue, object?[]? arguments)
    {
        ArgumentExceptionHelper.ThrowIfNull(toTarget);

        if (toTarget is not TableLayoutControlCollection targetCollection)
        {
            throw new ArgumentException(
                $"{nameof(toTarget)} must be of type {nameof(TableLayoutControlCollection)}",
                nameof(toTarget));
        }

        if (newValue is not IEnumerable<Control> newValueEnumerable)
        {
            throw new ArgumentException($"newValue must be {nameof(newValue)}", nameof(newValue));
        }

        targetCollection.Container.SuspendLayout();

        targetCollection.Clear();

        targetCollection.AddRange([.. newValueEnumerable]);

        targetCollection.Container.ResumeLayout();
        return targetCollection;
    }
}
