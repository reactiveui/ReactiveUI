// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Cinephile.Core.Rest
{
    /// <summary>
    /// A service which contains our various REST api clients.
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// Gets the rest client used for speculative checks.
        /// </summary>
        IRestApiClient Speculative { get; }

        /// <summary>
        /// Gets our rest client used by user initiated requests.
        /// </summary>
        IRestApiClient UserInitiated { get; }

        /// <summary>
        /// Gets our rest client used for background initiated requests.
        /// </summary>
        IRestApiClient Background { get; }
    }
}
