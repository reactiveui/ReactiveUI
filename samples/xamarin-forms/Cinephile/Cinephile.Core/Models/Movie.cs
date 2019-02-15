// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cinephile.Core.Models
{
    public class Movie : IEquatable<Movie>
    {
        public string PosterSmall { get; set; }
        public string PosterBig { get; set; }
        public string Overview { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IList<string> Genres { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }

        public bool Equals(Movie other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            var equals = Id == other.Id
                && PosterSmall == other.PosterSmall
                && PosterBig == other.PosterBig
                && Overview == other.Overview
                && ReleaseDate == other.ReleaseDate
                && Title == other.Title;

                return equals;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((Movie)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 745094178;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PosterSmall);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PosterBig);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Overview);
            hashCode = hashCode * -1521134295 + ReleaseDate.GetHashCode();
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            return hashCode;
        }
    }
}
