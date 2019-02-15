// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Cinephile.Core.Rest;
using Cinephile.Core.Rest.Dtos.Movies;
using DynamicData;
using Splat;

namespace Cinephile.Core.Models
{
    public class MovieService : IMovieService
    {
        private readonly SourceCache<Movie, int> internalSourceCache;
        public IObservableCache<Movie, int> UpcomingMovies => internalSourceCache;

        public const int PageSize = 20;

        private const string apiKey = "1f54bd990f1cdfb230adb312546d765d";
        private const string Language = "en-US";

        private IApiService movieApiService;
        private ICache movieCache;

        public MovieService(IApiService apiService = null, ICache cache = null)
        {
            movieApiService = apiService ?? Locator.Current.GetService<IApiService>();
            movieCache = cache ?? Locator.Current.GetService<ICache>();
            internalSourceCache = new SourceCache<Movie, int>(o => o.Id);
        }

        public Unit LoadUpcomingMovies(int index)
        {
            movieCache
                .GetAndFetchLatest($"upcoming_movies_{index}", () => FetchUpcomingMovies(index))
                .SelectMany(x => x)
                .Do(x => System.Diagnostics.Debug.WriteLine($"========> Movie {x.Id} - {x.Title}"))
                .Subscribe(x => internalSourceCache.AddOrUpdate(x));

            return Unit.Default;
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
                            .Select(movieDto => MovieMapper.ToModel(genres, movieDto, Language));
                    });
        }
    }
}