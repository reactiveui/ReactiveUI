// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveUI.Tests
{
    internal static class CompatMixins
    {
        public static void Run<T>(this IEnumerable<T> @this, Action<T> block)
        {
            foreach (var v in @this)
            {
                block(v);
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> @this, int count) => @this.Take(@this.Count() - count);
    }
}
