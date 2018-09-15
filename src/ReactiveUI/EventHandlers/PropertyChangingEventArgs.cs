// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;

namespace ReactiveUI
{
    public delegate void PropertyChangingEventHandler(
        object sender,
        PropertyChangingEventArgs e);

    /// <summary>
    /// The arguments for the PropertyChanging event.
    /// </summary>
    public class PropertyChangingEventArgs : EventArgs
    {
        public PropertyChangingEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the name of the property that is changing.
        /// </summary>
        public string PropertyName { get; protected set; }
    }
}
