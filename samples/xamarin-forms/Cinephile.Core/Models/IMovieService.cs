// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cinephile.Core.Model
{
    public interface IMovieService
    {
        IObservable<IEnumerable<Movie>> GetUpcomingMovies(int index);
    }
}
