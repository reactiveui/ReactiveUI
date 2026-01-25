// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Object = Java.Lang.Object;

namespace ReactiveUI;

/// <summary>
/// Provides extension methods for converting between .NET objects and their Java object representations.
/// </summary>
/// <remarks>These methods facilitate interoperability between .NET and Java environments by enabling conversion
/// of objects to and from Java-compatible forms. The class is intended for internal use within interop scenarios and is
/// not designed for general-purpose object conversion.</remarks>
internal static class ObjectExtension
{
    /// <summary>
    /// Converts a Java.Lang.Object, previously created with .ToJavaObject(), back to its corresponding .NET object of
    /// the specified type.
    /// </summary>
    /// <remarks>This method is intended for use with objects that were originally converted from .NET to Java
    /// using .ToJavaObject(). Attempting to convert other Java.Lang.Object instances may result in an
    /// exception.</remarks>
    /// <typeparam name="TObject">The type of the .NET object to return.</typeparam>
    /// <param name="value">The Java.Lang.Object instance to convert. Must have been created using .ToJavaObject().</param>
    /// <returns>The .NET object of type TObject represented by the specified Java.Lang.Object, or the default value of TObject
    /// if value is null.</returns>
    /// <exception cref="InvalidOperationException">Thrown if value is not a Java.Lang.Object created with .ToJavaObject().</exception>
    public static TObject ToNetObject<TObject>(this Object value)
    {
        if (value is null)
        {
            return default!;
        }

        if (value is not JavaHolder)
        {
            throw new InvalidOperationException("Unable to convert to .NET object. Only Java.Lang.Object created with .ToJavaObject() can be converted.");
        }

        return (TObject)((JavaHolder)value).Instance;
    }

    /// <summary>
    /// Converts the specified value to a Java-compatible object representation.
    /// </summary>
    /// <typeparam name="TObject">The type of the value to convert.</typeparam>
    /// <param name="value">The value to convert to a Java object. Can be null.</param>
    /// <returns>A Java-compatible object that represents the specified value, or null if the value is null.</returns>
    public static Object? ToJavaObject<TObject>(this TObject value)
    {
        if (value is null)
        {
            return null;
        }

        return new JavaHolder(value);
    }
}
