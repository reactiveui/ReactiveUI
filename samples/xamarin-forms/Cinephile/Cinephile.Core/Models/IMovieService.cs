// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reactive;

namespace Cinephile.Core.Models
{
    public interface IMovieService
    {
        Unit LoadUpcomingMovies(int index);
    }
}
