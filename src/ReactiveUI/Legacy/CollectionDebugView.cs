// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Legacy;

[ExcludeFromCodeCoverage]
internal sealed class CollectionDebugView<T>(ICollection<T> collection)
{
    private readonly ICollection<T> _collection = collection ?? throw new ArgumentNullException(nameof(collection), "collection is null.");

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items
    {
        get
        {
            var array = new T[_collection.Count];
            _collection.CopyTo(array, 0);
            return array;
        }
    }
}
