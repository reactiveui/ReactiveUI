// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI
{
    /// <summary>
    /// Instead of using System events will allow for external classes to
    /// manually Activate the View object.
    /// </summary>
    internal interface ICanForceManualActivation
    {
        /// <summary>
        /// Activates the view object.
        /// </summary>
        /// <param name="activate">If we are activating or not.</param>
        void Activate(bool activate);
    }
}
