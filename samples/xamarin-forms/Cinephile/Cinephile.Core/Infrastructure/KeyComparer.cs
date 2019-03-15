// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Cinephile.Core.Infrastructure
{
    internal class KeyComparer<TObject, TKey> : IEqualityComparer<KeyValuePair<TKey, TObject>>
    {
        public bool Equals(KeyValuePair<TKey, TObject> x, KeyValuePair<TKey, TObject> y)
        {
            return x.Key.Equals(y.Key);
        }

        public int GetHashCode(KeyValuePair<TKey, TObject> obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}
