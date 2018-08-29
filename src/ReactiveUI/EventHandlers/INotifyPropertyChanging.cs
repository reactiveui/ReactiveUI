// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;

namespace ReactiveUI
{
    /// <summary>
    /// Classes which implement this interface will notify
    /// external users when a property is changing.
    /// </summary>
    public interface INotifyPropertyChanging
    {
        /// <summary>
        /// An event that is triggered before a property's value is going to change.
        /// </summary>
        event PropertyChangingEventHandler PropertyChanging;
    }
}