// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Winforms;
#else
namespace ReactiveUI.Winforms;
#endif

/// <summary>A converter that can handle setting values on a <see cref="Panel"/> control for binding.</summary>
public class PanelSetMethodBindingConverter : ISetMethodBindingConverter
{
    /// <summary>The affinity returned when the source can be bound to a control collection.</summary>
    private const int ControlCollectionAffinity = 10;

    /// <inheritdoc />
    public int GetAffinityForObjects(Type? fromType, Type? toType)
    {
        if (toType != typeof(Control.ControlCollection) || fromType is null)
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

        if (newValue is not IEnumerable<Control> newValueEnumerable)
        {
            throw new ArgumentException($"newValue must be {nameof(newValue)}", nameof(newValue));
        }

        var targetCollection = (Control.ControlCollection)toTarget;

        targetCollection.Owner.SuspendLayout();

        targetCollection.Clear();
        targetCollection.AddRange([.. newValueEnumerable]);

        targetCollection.Owner.ResumeLayout();

        return targetCollection;
    }
}
