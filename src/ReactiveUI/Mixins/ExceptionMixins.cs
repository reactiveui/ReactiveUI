// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace System;

internal static class ExceptionMixins
{
    public static void ArgumentNullExceptionThrowIfNull<T>(this T? value, string name)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name);
        }
    }
}