// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;

namespace ReactiveUI
{
    /// <summary>
    /// The arguments for the PropertyChanging event.
    /// </summary>
    public class PropertyChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangingEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property that is changing.</param>
        public PropertyChangingEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the name of the property that is changing.
        /// </summary>
        public string PropertyName { get; protected set; }
    }
}
