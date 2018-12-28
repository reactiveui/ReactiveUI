// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ReactiveUI
{
    /// <summary>
    /// Event handler for the property changing events.
    /// This will be called before a property value has changed.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Details about the changing property.</param>
    public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);
}
