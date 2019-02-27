﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Cinephile.Core.Rest.Dtos.ImageConfigurations;
using Cinephile.Core.Rest.Dtos.Movies;
using Refit;

namespace Cinephile.Core.Rest
{
    [Headers("Content-Type: application/json")]
    public interface IRestApiClient
    {
        [Get("/movie/upcoming?api_key={apiKey}&language={language}&page={page}&sort_by=release_date")]
        IObservable<MovieDto> FetchUpcomingMovies(string apiKey, int page, string language);

        [Get("/configuration?api_key={apiKey}")]
        IObservable<ImageConfigurationDto> FetchImageConfiguration(string apiKey);

        [Get("/genre/movie/list?api_key={apiKey}&language={language}")]
        IObservable<GenresDto> FetchGenres(string apiKey, string language);
   }
}
