// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using Cinephile.Core.Rest.Dtos.Movies;

namespace Cinephile.Core.Models
{
    public static class MovieMapper
    {
        const string BaseUrl = "http://image.tmdb.org/t/p/";
        const string SmallPosterSize = "w185";
        const string BigPosterSize = "w500";

        public static Movie ToModel(GenresDto genres, MovieResult movieDto, string language)
        {
            return new Movie()
            {
                Id = movieDto.Id,
                Title = movieDto.Title,
                PosterSmall = string
                    .Concat(BaseUrl,
                        SmallPosterSize,
                        movieDto.PosterPath),
                PosterBig = string
                    .Concat(BaseUrl,
                        BigPosterSize,
                        movieDto.PosterPath),
                Genres = genres.Genres.Where(g => movieDto.GenreIds.Contains(g.Id)).Select(j => j.Name).ToList(),
                ReleaseDate = DateTime.Parse(movieDto.ReleaseDate, new CultureInfo(language)),
                Overview = movieDto.Overview
            };
        }
    }
}
