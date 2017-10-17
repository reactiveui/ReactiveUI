// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using Cinephile.Core.Model;
using ReactiveUI;

namespace Cinephile.ViewModels
{
    public class MovieDetailViewModel : ViewModelBase
    {
        public string Title
        {
            get
            {
                return this.movie.Title;
            }
        }

        public string PosterSmall
        {
            get
            {
                return this.movie.PosterSmall;
            }
        }

        public string PosterBig
        {
            get
            {
                return this.movie.PosterBig;
            }
        }

        public string Genres
        {
            get
            {
                return string.Join(", ", this.movie.Genres);
            }
        }

        public string ReleaseDate
        {
            get
            {
                return this.movie.ReleaseDate.ToString("D");
            }
        }

        public string Overview
        {
            get
            {
                return this.movie.Overview;
            }
        }

        private Movie movie;

        public MovieDetailViewModel(Movie movie, IScreen hostScreen = null) : base(hostScreen)
        {
            this.movie = movie;
            UrlPathSegment = this.Title;
        }
    }
}
