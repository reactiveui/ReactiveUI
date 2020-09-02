﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI
{
    /// <summary>
    /// Implement this interface for ViewModels that can be navigated to.
    /// </summary>
    public interface IRoutableViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets a string token representing the current ViewModel, such as 'login' or 'user'.
        /// </summary>
        string? UrlPathSegment { get; }

        /// <summary>
        /// Gets the IScreen that this ViewModel is currently being shown in. This
        /// is usually passed into the ViewModel in the Constructor and saved
        /// as a ReadOnly Property.
        /// </summary>
        IScreen HostScreen { get; }
    }
}
