// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReactiveUI
{
    internal sealed class CollectionDebugView<T>
    {
        private readonly ICollection<T> collection;

        public CollectionDebugView(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection", "collection is null.");
            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this.collection.Count];
                this.collection.CopyTo(array, 0);
                return array;
            }
        }
    }
}