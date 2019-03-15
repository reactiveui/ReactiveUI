// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Concurrency;
using Cinephile.Core.Models;
using ReactiveUI;

namespace Cinephile.ViewModels
{
    /// <summary>
    /// A view model containing details about a movie.
    /// </summary>
    public class MovieDetailViewModel : ViewModelBase
    {
        private readonly Movie _movie;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieDetailViewModel"/> class.
        /// </summary>
        /// <param name="movie">Gets the model for the movie.</param>
        /// <param name="mainThreadScheduler">Gets the scheduler to use for the main thread.</param>
        /// <param name="taskPoolScheduler">Gets the scheduler to use for background operations.</param>
        /// <param name="hostScreen">The screen to use for routing.</param>
        public MovieDetailViewModel(Movie movie, IScheduler mainThreadScheduler = null, IScheduler taskPoolScheduler = null, IScreen hostScreen = null)
            : base(movie.Title, mainThreadScheduler, taskPoolScheduler, hostScreen)
        {
            _movie = movie;
        }

        /// <summary>
        /// Gets the title of the movie.
        /// </summary>
        public string Title => _movie.Title;

        /// <summary>
        /// Gets the URL to the small movie poster.
        /// </summary>
        public string PosterSmall => _movie.PosterSmall;

        /// <summary>
        /// Gets the URL to the big movie poster.
        /// </summary>
        public string PosterBig => _movie.PosterBig;

        /// <summary>
        /// Gets the genres of the movie.
        /// </summary>
        public string Genres => string.Join(", ", _movie.Genres);

        /// <summary>
        /// Gets the release date of the movie.
        /// </summary>
        public string ReleaseDate => _movie.ReleaseDate.ToString("D", CultureInfo.CurrentCulture);

        /// <summary>
        /// Gets an overview of the movie.
        /// </summary>
        public string Overview => _movie.Overview;
    }
}
