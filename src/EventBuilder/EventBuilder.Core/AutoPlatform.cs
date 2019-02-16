// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace EventBuilder.Core
{
    /// <summary>
    /// The event builder platform.
    /// </summary>
    public enum AutoPlatform
    {
        /// <summary>
        /// Android platform.
        /// </summary>
        Android,

#pragma warning disable SA1300 // Element should begin with upper-case letter
        /// <summary>
        /// iOS platform.
        /// </summary>
        iOS,
#pragma warning restore SA1300 // Element should begin with upper-case letter

        /// <summary>
        /// Mac platform.
        /// </summary>
        Mac,

        /// <summary>
        /// Tizen platform.
        /// </summary>
        Tizen4,

        /// <summary>
        /// WPF platform.
        /// </summary>
        WPF,

        /// <summary>
        /// Xamarin Forms platform.
        /// </summary>
        XamForms,

        /// <summary>
        /// UWP platform.
        /// </summary>
        UWP,

        /// <summary>
        /// Win Forms platform.
        /// </summary>
        Winforms,

        /// <summary>
        /// TV OS platform.
        /// </summary>
        TVOS,

        /// <summary>
        /// Xamarin Essentials platform.
        /// </summary>
        Essentials
    }
}
