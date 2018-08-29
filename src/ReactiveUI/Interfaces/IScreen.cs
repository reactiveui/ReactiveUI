// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ReactiveUI
{
    /// <summary>
    /// IScreen represents any object that is hosting its own routing -
    /// usually this object is your AppViewModel or MainWindow object.
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// The Router associated with this Screen.
        /// </summary>
        RoutingState Router { get; }
    }
}
