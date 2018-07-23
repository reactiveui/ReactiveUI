// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Cinephile.Core.Model;
using ReactiveUI;

namespace Cinephile.ViewModels
{
    public class UpcomingMoviesCellViewModel : ViewModelBase
    {
        public Movie Movie
        {
            get
            {
                return this.movie;
            }
        }

        public string Title
        {
            get
            {
                return this.movie.Title;
            }
        }

        public string PosterPath
        {
            get
            {
                return this.movie.PosterSmall;
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

        private Movie movie;

        public UpcomingMoviesCellViewModel(Movie movie, IScreen hostScreen = null) : base(hostScreen)
        {
            this.movie = movie;
        }
    }
}
