// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Cinephile.Core.Models;
using ReactiveUI;

namespace Cinephile.ViewModels
{
    public class MovieDetailViewModel : ViewModelBase
    {
        public string Title => _movie.Title;

        public string PosterSmall => _movie.PosterSmall;

        public string PosterBig => _movie.PosterBig;

        public string Genres => string.Join(", ", this._movie.Genres);

        public string ReleaseDate => _movie.ReleaseDate.ToString("D");

        public string Overview => _movie.Overview;

        private readonly Movie _movie;

        public MovieDetailViewModel(Movie movie, IScheduler mainThreadScheduler = null, IScheduler taskPoolScheduler = null, IScreen hostScreen = null) 
        : base(movie.Title, mainThreadScheduler, taskPoolScheduler, hostScreen)
        {
            _movie = movie;
        }
    }
}
