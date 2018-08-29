// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;

namespace ReactiveUI
{
    /// <summary>
    /// Notifies when a collection is changing.
    /// </summary>
    public interface INotifyCollectionChanging
    {
        /// <summary>
        /// An event for when a collection is changing. Used for getting values
        /// before the change.
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanging;
    }
}