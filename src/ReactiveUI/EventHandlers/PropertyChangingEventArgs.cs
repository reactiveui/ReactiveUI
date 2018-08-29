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

    public class PropertyChangingEventArgs : EventArgs
    {
        public PropertyChangingEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; protected set; }
    }
}
