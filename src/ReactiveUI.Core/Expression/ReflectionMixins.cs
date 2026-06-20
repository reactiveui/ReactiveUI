// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI;

/// <summary>Provides extension members for reflection-related types.</summary>
public static class ReflectionMixins
{
    /// <summary>Provides IsStatic extension members for <see cref="PropertyInfo"/>.</summary>
    /// <param name="item">The property information to check.</param>
    extension(PropertyInfo item)
    {
        /// <summary>Determines if the specified property is static.</summary>
        /// <returns><see langword="true"/> if the property is static; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is <see langword="null"/>.</exception>
        public bool IsStatic()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            var method = (item.GetMethod ?? item.SetMethod)!;
            return method.IsStatic;
        }
    }
}
