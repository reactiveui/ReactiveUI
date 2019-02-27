// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Cinephile.Core.Models;
using ReactiveUI;

namespace Cinephile.ViewModels
{
    public class UpcomingMoviesCellViewModel : ViewModelBase
    {
        public Movie Movie { get; }

        public string Title
        {
            get
            {
                return Movie.Title;
            }
        }

        public string PosterPath
        {
            get
            {
                return Movie.PosterSmall;
            }
        }

        public string Genres
        {
            get
            {
                return string.Join(", ", Movie.Genres);
			}
        }

        public string ReleaseDate
        {
            get
            {
                return Movie.ReleaseDate.ToString("D");
            }
        }

        public UpcomingMoviesCellViewModel(Movie movie, IScreen hostScreen = null) : base(hostScreen)
        {
            Movie = movie;
        }
    }
}
