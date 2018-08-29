// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ReactiveUI
{
    /// <summary>
    /// Additional details implemented by the different ReactiveUI platform projects.
    /// </summary>
    internal interface IPlatformOperations
    {
        /// <summary>
        /// Gets a descriptor that describes (if applicable) the orientation
        /// of the screen.
        /// </summary>
        /// <returns>The orientation of the screen if supported.</returns>
        string GetOrientation();
    }
}
