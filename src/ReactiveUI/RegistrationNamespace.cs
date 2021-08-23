// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI
{
    /// <summary>
    /// Platforms or other registration namespaces for the dependency resolver to consider when initializing.
    /// </summary>
    public enum RegistrationNamespace
    {
        /// <summary>No platform to register.</summary>
        None = 0,

        /// <summary>
        /// Xamarin Forms.
        /// </summary>
        XamForms,

        /// <summary>
        /// Windows Forms.
        /// </summary>
        Winforms,

        /// <summary>
        /// WPF.
        /// </summary>
        Wpf,

        /// <summary>
        /// Uno.
        /// </summary>
        Uno,

        /// <summary>
        /// Blazor.
        /// </summary>
        Blazor,

        /// <summary>
        /// Drawing.
        /// </summary>
        Drawing,

        /// <summary>
        /// Avalonia.
        /// </summary>
        Avalonia,

        /// <summary>
        /// Maui.
        /// </summary>
        Maui,

        /// <summary>
        /// Uwp.
        /// </summary>
        Uwp,

        /// <summary>
        /// WinUI.
        /// </summary>
        WinUI,
    }
}
