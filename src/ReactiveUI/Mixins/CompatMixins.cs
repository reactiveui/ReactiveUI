﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveUI
{
    internal static class CompatMixins
    {
        internal static void ForEach<T>(this IEnumerable<T> @this, Action<T> block)
        {
            foreach (var v in @this)
            {
                block(v);
            }
        }

        internal static IEnumerable<T> SkipLast<T>(this IEnumerable<T> @this, int count)
        {
            return @this.Take(@this.Count() - count);
        }
    }

    // according to spouliot, this is just a string match, and will cause the
    // linker to be ok with everything.
}
