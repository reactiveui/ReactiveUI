// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using DynamicData;

namespace Cinephile.Core.Models
{
    /// <summary>
    /// This service will provide data about movies.
    /// </summary>
    public interface IMovieService
    {
        /// <summary>
        /// Gets an observable which contains upcoming movies.
        /// </summary>
        IObservableCache<Movie, int> UpcomingMovies { get; }

        /// <summary>
        /// Loads the upcoming movies.
        /// </summary>
        /// <param name="index">The paging index.</param>
        /// <returns>An observable which signals when the update is complete.</returns>
        IObservable<Unit> LoadUpcomingMovies(int index);
    }
}
