// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>Writes a bound ingredient name by appending it to a list instead of assigning a property.</summary>
[System.Diagnostics.DebuggerDisplay("IngredientSetConverter")]
public sealed class IngredientSetConverter : ISetMethodBindingConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type? fromType, Type? toType) =>
        fromType == typeof(string) && toType == typeof(List<string>) ? 10 : 0;

    /// <inheritdoc/>
    public object? PerformSet(object? toTarget, object? newValue, object?[]? arguments)
    {
        if (toTarget is List<string> ingredients && newValue is string ingredient)
        {
            ingredients.Add(ingredient);
        }

        return toTarget;
    }
}
