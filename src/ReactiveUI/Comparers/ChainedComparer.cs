// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace ReactiveUI
{
    internal sealed class ChainedComparer<T> : IComparer<T>
    {
        private readonly IComparer<T>? _parent;
        private readonly Comparison<T> _inner;

        public ChainedComparer(IComparer<T>? parent, Comparison<T> comparison)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _inner = comparison ?? throw new ArgumentNullException(nameof(comparison));
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            int parentResult = _parent?.Compare(x, y) ?? 0;

            return parentResult != 0 ? parentResult : _inner(x, y);
        }
    }
}
