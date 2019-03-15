// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Cinephile.Core.Models;
using Cinephile.Core.Rest;
using Cinephile.Core.Rest.Dtos.Movies;
using DynamicData;
using Moq;
using NUnit.Framework;

namespace Cinephile.UnitTests.Model
{
    /// <summary>
    /// Tests associated with the movie service.
    /// </summary>
    [TestFixture]
    public class MovieServiceTest
    {
        private MovieDto _movieDto;
        private GenresDto _genresDto;
        private DateTime _dateTimeNow;

        /// <summary>
        /// Sets up details for the tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _dateTimeNow = DateTime.Now;

            _movieDto = new MovieDto()
            {
                Dates = new MovieDates()
                {
                    Maximum = _dateTimeNow.ToString(CultureInfo.InvariantCulture),
                    Minimum = _dateTimeNow.ToString(CultureInfo.InvariantCulture)
                },
                Page = 1,
                TotalPages = 1,
                TotalResults = 20
            };

            var movies = new List<MovieResult>();
            for (int i = 0; i < 20; i++)
            {
                movies.Add(new MovieResult
                {
                    Id = i,
                    GenreIds = new List<int>() { 1, 2 },
                    Overview = $"Overview {i}",
                    PosterPath = "PosterPath/",
                    ReleaseDate = _dateTimeNow.ToString(CultureInfo.InvariantCulture),
                    Title = "Title"
                });
            }

            _movieDto.Results = movies;

            _genresDto = new GenresDto()
            {
                Genres = new List<GenreDto>()
                {
                    new GenreDto
                    {
                        Id = 1,
                        Name = "Genre1"
                    },

                    new GenreDto
                    {
                        Id = 2,
                        Name = "Genre2"
                    }
                }
            };
        }

        /// <summary>
        /// Makes sure that upcoming movies returns movies appropriately.
        /// </summary>
        [Test]
        public void GetUpcomingMovies_Zero_20Movies()
        {
            var cacheMock = new Mock<ICache>();

            cacheMock
                .Setup(cache => cache.GetAndFetchLatest(It.IsAny<string>(), It.IsAny<Func<IObservable<IEnumerable<Movie>>>>()))
                .Returns((string _, Func<IObservable<IEnumerable<Movie>>> arg2) => arg2());

            var apiServiceMock = new Mock<IApiService>();
            apiServiceMock
                .Setup(api => api.UserInitiated.FetchUpcomingMovies(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Observable.Return(_movieDto));

            apiServiceMock
                .Setup(api => api.UserInitiated.FetchGenres(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Observable.Return(_genresDto));

            var target = new MovieService(apiServiceMock.Object, cacheMock.Object);

            target
                .UpcomingMovies
                .Connect()
                .Bind(out ReadOnlyObservableCollection<Movie> actual)
                .Subscribe();

            target.LoadUpcomingMovies(0).Subscribe();

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Count(), Is.EqualTo(20));
            Assert.That(actual.Select(m => m.Overview.Length), Has.All.GreaterThan(0));
            Assert.That(actual.Select(m => m.PosterBig.Length), Has.All.GreaterThan(0));
            Assert.That(actual.Select(m => m.PosterSmall.Length), Has.All.GreaterThan(0));

            Assert.AreEqual("Genre1", actual.First().Genres[0]);
            Assert.AreEqual("Genre2", actual.First().Genres[1]);
        }
    }
}
