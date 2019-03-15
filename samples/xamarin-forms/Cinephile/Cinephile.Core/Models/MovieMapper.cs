// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Linq;
using Cinephile.Core.Rest.Dtos.Movies;

namespace Cinephile.Core.Models
{
    /// <summary>
    /// Maps the data transfer objects (DTO) to Movie instances.
    /// </summary>
    public static class MovieMapper
    {
        private const string BaseUrl = "http://image.tmdb.org/t/p/";
        private const string SmallPosterSize = "w185";
        private const string BigPosterSize = "w500";

        /// <summary>
        /// Converts the DTO to their movie instances.
        /// </summary>
        /// <param name="genres">Gets the available movie genres.</param>
        /// <param name="movieDto">Gets the movie DTO instances.</param>
        /// <param name="language">Gets the language.</param>
        /// <returns>The mapped Movie instance.</returns>
        public static Movie ToModel(GenresDto genres, MovieResult movieDto, string language)
        {
            return new Movie
            {
                Id = movieDto.Id,
                Title = movieDto.Title,
                PosterSmall = string
                    .Concat(
                        BaseUrl,
                        SmallPosterSize,
                        movieDto.PosterPath),
                PosterBig = string
                    .Concat(
                        BaseUrl,
                        BigPosterSize,
                        movieDto.PosterPath),
                Genres = genres.Genres.Where(g => movieDto.GenreIds.Contains(g.Id)).Select(j => j.Name).ToList(),
                ReleaseDate = DateTime.Parse(movieDto.ReleaseDate, new CultureInfo(language)),
                Overview = movieDto.Overview
            };
        }
    }
}
