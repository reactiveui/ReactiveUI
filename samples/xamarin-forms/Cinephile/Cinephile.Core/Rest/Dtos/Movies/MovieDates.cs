// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Cinephile.Core.Rest.Dtos.Movies
{
    /// <summary>
    /// Gets information about movie dates.
    /// </summary>
    public class MovieDates
    {
        /// <summary>
        /// Gets or sets the maximum movie date.
        /// </summary>
        public string Maximum { get; set; }

        /// <summary>
        /// Gets or sets the minimum movie date.
        /// </summary>
        public string Minimum { get; set; }
    }
}
