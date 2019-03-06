// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Cinephile.Core.Models;
using ReactiveUI;

namespace Cinephile.ViewModels
{
    public class UpcomingMoviesCellViewModel : ViewModelBase
    {
        public Movie Movie { get; }

        public string Title => Movie.Title;

        public string PosterPath => Movie.PosterSmall;

        public string Genres => string.Join(", ", Movie.Genres);

        public string ReleaseDate => Movie.ReleaseDate.ToString("D");

        public UpcomingMoviesCellViewModel(Movie movie, IScreen hostScreen = null) : base(movie.Title, hostScreen: hostScreen)
        {
            Movie = movie;
        }
    }
}
