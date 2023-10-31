// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Object = Java.Lang.Object;

namespace ReactiveUI;

internal static class ObjectExtension
{
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

    public static Object? ToJavaObject<TObject>(this TObject value)
    {
        if (value is null)
        {
            return null;
        }

        return new JavaHolder(value);
    }
}
