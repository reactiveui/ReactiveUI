// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cinephile.Core.Model
{
    public class Movie
    {
        public string PosterSmall { get; set; }
        public string PosterBig { get; set; }
        public string Overview { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IList<string> Genres { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
