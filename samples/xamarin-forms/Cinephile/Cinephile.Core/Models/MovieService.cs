// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Cinephile.Core.Rest;
using DynamicData;
using Splat;

namespace Cinephile.Core.Models
{
    /// <summary>
    /// This is a service that will retrieve Movie data from a website.
    /// </summary>
    public class MovieService : IMovieService, IDisposable
    {
        private const string ApiKey = "1f54bd990f1cdfb230adb312546d765d";
        private readonly IApiService _movieApiService;
        private readonly ICache _movieCache;
        private readonly SourceCache<Movie, int> _internalSourceCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieService"/> class.
        /// </summary>
        /// <param name="apiService">The service which will communicate with the API.</param>
        /// <param name="cache">The cache where to store our retrieved instances.</param>
        public MovieService(IApiService apiService = null, ICache cache = null)
        {
            _movieApiService = apiService ?? Locator.Current.GetService<IApiService>();
            _movieCache = cache ?? Locator.Current.GetService<ICache>();
            _internalSourceCache = new SourceCache<Movie, int>(o => o.Id);
        }

        /// <summary>
        /// Gets the size of the pages.
        /// </summary>
        public int PageSize { get; } = 20;

        /// <summary>
        /// Gets a observable cache of upcoming movies.
        /// </summary>
        public IObservableCache<Movie, int> UpcomingMovies => _internalSourceCache;

        /// <summary>
        /// Gets the language we want to movie data to be in.
        /// </summary>
        protected string Language { get; } = "en-US";

        /// <summary>
        /// Loads the upcoming movies.
        /// </summary>
        /// <param name="index">The page index.</param>
        /// <returns>An observable that signals when the movie loading is complete.</returns>
        public IObservable<Unit> LoadUpcomingMovies(int index)
        {
            return _movieCache
                .GetAndFetchLatest($"upcoming_movies_{index}", () => FetchUpcomingMovies(index))
                .Select(x =>
                {
                    _internalSourceCache.Edit(innerCache => innerCache.AddOrUpdate(x));
                    return Unit.Default;
                });
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes all disposable objects contained on the object.
        /// </summary>
        /// <param name="disposing">If we are getting called from the dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _internalSourceCache?.Dispose();
            }
        }

        /// <summary>
        /// Fetch upcoming movies from the service.
        /// </summary>
        /// <param name="index">The page index.</param>
        /// <returns>An observable which signals the upcoming movies.</returns>
        private IObservable<IEnumerable<Movie>> FetchUpcomingMovies(int index)
        {
            // Uncomment this if you want to see the dialog
            //// var randomNumber = new Random(DateTime.Now.Millisecond).Next(0, 3);
            //// if (randomNumber == 1)
            ////    throw new Exception("This is a generic fake exception to show how the dialog works.");

            int page = (int)Math.Ceiling(index / (double)PageSize) + 1;

            return _movieApiService
                .UserInitiated
                .FetchUpcomingMovies(ApiKey, page, Language)
                .CombineLatest(
                    _movieApiService.UserInitiated.FetchGenres(ApiKey, Language),
                    (movies, genres) =>
                    {
                        return movies
                            .Results
                            .Select(movieDto => MovieMapper.ToModel(genres, movieDto, Language));
                    });
        }
    }
}
