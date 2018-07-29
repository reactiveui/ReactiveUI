// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Cinephile.Core.Rest;
using Cinephile.Core.Rest.Dtos.Movies;
using Splat;

namespace Cinephile.Core.Model
{
    public class MovieService : IMovieService
    {
        public const int PageSize = 20;
        const string Language = "en-US";

        const string BaseUrl = "http://image.tmdb.org/t/p/";
        const string SmallPosterSize = "w185";
        const string BigPosterSize = "w500";


        private const string apiKey = "1f54bd990f1cdfb230adb312546d765d";
        private IApiService movieApiService;
        private ICache movieCache;

        public MovieService(IApiService apiService = null, ICache cache = null)
        {
            movieApiService = apiService ?? Locator.Current.GetService<IApiService>();
            movieCache = cache ?? Locator.Current.GetService<ICache>();
        }

        public IObservable<IEnumerable<Movie>> GetUpcomingMovies(int index)
        {
            return
                movieCache
                    .GetAndFetchLatest($"upcoming_movies_{index}", () => FetchUpcomingMovies(index));
        }

        IObservable<IEnumerable<Movie>> FetchUpcomingMovies(int index)
        {
            int page = (int)Math.Ceiling(index / (double)PageSize) + 1;

            return Observable
                .CombineLatest(
                    movieApiService
                        .UserInitiated
                        .FetchUpcomingMovies(apiKey, page, Language),
                    movieApiService
                        .UserInitiated
                        .FetchGenres(apiKey, Language),
                    (movies, genres) =>
                    {
                        return movies
                                .Results
                                .Select(movieDto => MapDtoToModel(genres, movieDto));
                    });
        }

        Movie MapDtoToModel(GenresDto genres, MovieResult movieDto)
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
                ReleaseDate = DateTime.Parse(movieDto.ReleaseDate, new CultureInfo(Language)),
                Overview = movieDto.Overview
            };
        }
    }
}