// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Cinephile.Core.Model;
using Cinephile.Core.Rest;
using Cinephile.Core.Rest.Dtos.Movies;
using Moq;
using NUnit.Framework;

namespace Cinephile.UnitTests.Model
{
    [TestFixture]
    public class MovieServiceTest
    {
        MovieDto movieDto;
        GenresDto genresDto;
        DateTime dateTimeNow;

        [SetUp]
        public void Setup()
        {
            dateTimeNow = DateTime.Now;

            movieDto = new MovieDto()
            {
                Dates = new MovieDates()
                {
                    Maximum = dateTimeNow.ToString(),
                    Minimum = dateTimeNow.ToString()
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
                    ReleaseDate = dateTimeNow.ToString(),
                    Title = "Title"
                });
            }

            movieDto.Results = movies;

            genresDto = new GenresDto()
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

        [Test]
        public void GetUpcomingMovies_Zero_20Movies()
        {
            var cacheMock = new Mock<ICache>();
            cacheMock
                .Setup(cache => cache.GetAndFetchLatest(It.IsAny<string>(), It.IsAny<Func<IObservable<IEnumerable<Movie>>>>()))
                .Returns((string arg1, Func<IObservable<IEnumerable<Movie>>> arg2) => arg2());

            var apiServiceMock = new Mock<IApiService>();
            apiServiceMock
                .Setup(api => api.UserInitiated.FetchUpcomingMovies(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Observable.Return(movieDto));

            apiServiceMock
                .Setup(api => api.UserInitiated.FetchGenres(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Observable.Return(genresDto));

            var sut = new MovieService(apiServiceMock.Object, cacheMock.Object);

            IEnumerable<Movie> actual = null;

            sut
                .GetUpcomingMovies(0)
                .Subscribe(movies => actual = movies);

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
