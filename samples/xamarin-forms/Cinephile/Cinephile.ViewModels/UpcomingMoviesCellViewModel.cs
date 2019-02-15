// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Cinephile.Core.Models;
using ReactiveUI;

namespace Cinephile.ViewModels
{
    public class UpcomingMoviesCellViewModel : ViewModelBase, IEquatable<UpcomingMoviesCellViewModel>
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

        public bool Equals(UpcomingMoviesCellViewModel other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Movie == other.Movie;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((UpcomingMoviesCellViewModel)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = -275155498;
            hashCode = hashCode * -1521134295 + EqualityComparer<Movie>.Default.GetHashCode(Movie);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PosterPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Genres);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ReleaseDate);
            return hashCode;
        }
    }
}
