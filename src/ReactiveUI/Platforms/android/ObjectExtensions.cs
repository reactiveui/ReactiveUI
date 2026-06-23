// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Object = Java.Lang.Object;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides extension methods for converting between .NET objects and their Java object representations.</summary>
/// <remarks>These methods facilitate interoperability between .NET and Java environments by enabling conversion
/// of objects to and from Java-compatible forms. The class is intended for internal use within interop scenarios and is
/// not designed for general-purpose object conversion.</remarks>
[SuppressMessage(
    "Extensions",
    "SST1706:Avoid declaring extension members on a broad receiver type",
    Justification = "Existing public extension surface declared on object for the Android control-wireup APIs.")]
internal static class ObjectExtensions
{
    /// <summary>The Java.Lang.Object instance to convert. Must have been created using .ToJavaObject().</summary>
    /// <param name="value">The <see cref="Object"/> previously produced by .ToJavaObject() that is unwrapped back to its .NET instance.</param>
    extension(Object value)
    {
        /// <summary>
        /// Converts a Java.Lang.Object, previously created with .ToJavaObject(), back to its corresponding .NET object of
        /// the specified type.
        /// </summary>
        /// <remarks>This method is intended for use with objects that were originally converted from .NET to Java
        /// using .ToJavaObject(). Attempting to convert other Java.Lang.Object instances may result in an
        /// exception.</remarks>
        /// <typeparam name="TObject">The type of the .NET object to return.</typeparam>
        /// <returns>The .NET object of type TObject represented by the specified Java.Lang.Object, or the default value of TObject
        /// if value is null.</returns>
        /// <exception cref="InvalidOperationException">Thrown if value is not a Java.Lang.Object created with .ToJavaObject().</exception>
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Type parameter is supplied explicitly by the caller and cannot be inferred from the parameters.")]
        public TObject ToNetObject<TObject>()
        {
            if (value is null)
            {
                return default!;
            }

            if (value is not JavaHolder)
            {
                throw new InvalidOperationException(
                    "Unable to convert to .NET object. Only Java.Lang.Object created with .ToJavaObject() can be converted.");
            }

            return (TObject)((JavaHolder)value).Instance;
        }
    }

    /// <summary>The value to convert to a Java object. Can be null.</summary>
    /// <typeparam name="TObject">The type of the value to convert.</typeparam>
    /// <param name="value">The .NET value, possibly null, that is wrapped into a Java-compatible holder.</param>
    extension<TObject>(TObject value)
    {
        /// <summary>Converts the specified value to a Java-compatible object representation.</summary>
        /// <returns>A Java-compatible object that represents the specified value, or null if the value is null.</returns>
        public Object? ToJavaObject()
        {
            return value is null ? null : new JavaHolder(value);
        }
    }
}
