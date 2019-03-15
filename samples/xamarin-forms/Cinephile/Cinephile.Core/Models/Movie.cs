// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cinephile.Core.Models
{
    /// <summary>
    /// Represents a movie that we want to show.
    /// </summary>
    public class Movie : IEquatable<Movie>
    {
        /// <summary>
        /// Gets or sets a url to the small poster image.
        /// </summary>
        public string PosterSmall { get; set; }

        /// <summary>
        /// Gets or sets a url to the big poster image.
        /// </summary>
        public string PosterBig { get; set; }

        /// <summary>
        /// Gets or sets a overview about the movie.
        /// </summary>
        public string Overview { get; set; }

        /// <summary>
        /// Gets or sets the date time of the release.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets a list of genres that this movie belongs to.
        /// </summary>
        [SuppressMessage("Design", "CA2227: Change to be read-only by removing the property setter.", Justification = "Used in DTO object.")]
        public IList<string> Genres { get; set; }

        /// <summary>
        /// Gets or sets a identifier for the movie.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the movie.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Determines if this movie is equal to another <see cref="Movie"/> instance.
        /// </summary>
        /// <param name="other">The other movie to compare.</param>
        /// <returns>If this movie instance is equal to the other movie instance.</returns>
        public bool Equals(Movie other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var equals = Id == other.Id
                && PosterSmall == other.PosterSmall
                && PosterBig == other.PosterBig
                && Overview == other.Overview
                && ReleaseDate == other.ReleaseDate
                && Title == other.Title;

            return equals;
        }

        /// <summary>
        /// Determines if this movie is equal to another <see cref="Movie"/> instance.
        /// </summary>
        /// <param name="obj">The other movie to compare.</param>
        /// <returns>If this movie instance is equal to the other movie instance.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Movie)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 745094178;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(PosterSmall);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(PosterBig);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Overview);
            hashCode = (hashCode * -1521134295) + ReleaseDate.GetHashCode();
            hashCode = (hashCode * -1521134295) + Id.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Title);
            return hashCode;
        }
    }
}
