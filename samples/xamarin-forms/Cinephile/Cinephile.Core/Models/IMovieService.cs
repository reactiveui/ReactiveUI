// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive;
using DynamicData;

namespace Cinephile.Core.Models
{
    public interface IMovieService
    {
        IObservableCache<Movie, int> UpcomingMovies { get; }
        IObservable<Unit> LoadUpcomingMovies(int index);
    }
}
