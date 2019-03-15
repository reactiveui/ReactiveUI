// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cinephile.Core.Rest.Dtos.Movies
{
    /// <summary>
    /// Information about a movie.
    /// </summary>
    public class MovieDto
    {
        /// <summary>
        /// Gets or sets the page index.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets a list of movie results.
        /// </summary>
        [SuppressMessage("Design", "CA2227: Change to be read-only by removing the property setter.", Justification = "Used in DTO object.")]
        public IList<MovieResult> Results { get; set; }

        /// <summary>
        /// Gets or sets date information about the movies.
        /// </summary>
        public MovieDates Dates { get; set; }

        /// <summary>
        /// Gets or sets the total number of available pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total number of results.
        /// </summary>
        public int TotalResults { get; set; }
    }
}
