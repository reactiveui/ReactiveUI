// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Cinephile.Core.Rest.Dtos.Movies
{
    /// <summary>
    /// A result about a movie.
    /// </summary>
    public class MovieResult
    {
        /// <summary>
        /// Gets or sets the poster path.
        /// </summary>
        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the movie contains adult content.
        /// </summary>
        public bool Adult { get; set; }

        /// <summary>
        /// Gets or sets a overview about the movie.
        /// </summary>
        public string Overview { get; set; }

        /// <summary>
        /// Gets or sets the release date of the movie.
        /// </summary>
        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets a list of genre id that are associated with the movie.
        /// </summary>
        [JsonProperty("genre_ids")]
        [SuppressMessage("Design", "CA2227: Change to be read-only by removing the property setter.", Justification = "Used in DTO object.")]
        public IList<int> GenreIds { get; set; }

        /// <summary>
        /// Gets or sets the ID of the movie.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the original title of the movie.
        /// </summary>
        public string OriginalTitle { get; set; }

        /// <summary>
        /// Gets or sets the original language of the movie.
        /// </summary>
        public string OriginalLanguage { get; set; }

        /// <summary>
        /// Gets or sets the title of the movie.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the path to the backdrop.
        /// </summary>
        public string BackdropPath { get; set; }

        /// <summary>
        /// Gets or sets the popularity of the movie.
        /// </summary>
        public double Popularity { get; set; }

        /// <summary>
        /// Gets or sets the number of votes about the movie.
        /// </summary>
        public int VoteCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this has a video.
        /// </summary>
        public bool Video { get; set; }

        /// <summary>
        /// Gets or sets the voting average.
        /// </summary>
        public double VoteAverage { get; set; }
    }
}
