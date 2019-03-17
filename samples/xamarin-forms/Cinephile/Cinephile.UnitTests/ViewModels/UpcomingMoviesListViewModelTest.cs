// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Cinephile.Core.Models;
using Cinephile.ViewModels;
using DynamicData;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using ReactiveUI;

namespace Cinephile.UnitTests.ViewModels
{
    /// <summary>
    /// Tests the upcoming movies list view model.
    /// </summary>
    [TestFixture]
    public class UpcomingMoviesListViewModelTest
    {
        private UpcomingMoviesListViewModel _target;
        private TestScheduler _scheduler;
        private IScreen _screen;
        private Mock<IMovieService> _movieService;
        private SourceCache<Movie, int> _moviesSourceCache;
        private AlertViewModel _alertOutput;

        /// <summary>
        /// Sets up the test conditions.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _scheduler = new TestScheduler();

            var routingState = new RoutingState();
            var screenMock = new Mock<IScreen>() { DefaultValue = DefaultValue.Mock };
            screenMock.SetupGet(x => x.Router).Returns(routingState);
            _screen = screenMock.Object;

            _movieService = new Mock<IMovieService>() { DefaultValue = DefaultValue.Mock };
            _movieService.Setup(x => x.LoadUpcomingMovies(It.Is<int>(y => y >= 0 && y < 100))).Returns(() => Observable.Return(Unit.Default));
            _movieService.Setup(x => x.LoadUpcomingMovies(It.Is<int>(y => y >= 100))).Returns(() => throw new Exception("Boom!"));

            var movies = new List<Movie>();
            for (int i = 0; i < 20; i++)
            {
                movies.Add(new Movie
                {
                    Id = i,
                    Overview = $"Overview {i}",
                    PosterBig = "PosterPath/",
                    PosterSmall = "PosterPath/",
                    ReleaseDate = DateTime.Now,
                    Title = $"Title {i}"
                });
            }

            _moviesSourceCache = new SourceCache<Movie, int>(x => x.Id);
            _moviesSourceCache.AddOrUpdate(movies);
            _movieService.Setup(x => x.UpcomingMovies).Returns(_moviesSourceCache);

            _target = new UpcomingMoviesListViewModel(_scheduler, _scheduler, _movieService.Object, _screen);

            _alertOutput = null;
            _target.ShowAlert.RegisterHandler(handler =>
            {
                _alertOutput = handler.Input;
                handler.SetOutput(Unit.Default);
            });

            _screen.Router.NavigateAndReset.Execute(_target);
        }

        /// <summary>
        /// Test to make sure when the selected item is null that the current view model is also null.
        /// </summary>
        [Test]
        public void SelectedItem_null_NothingHappens()
        {
            _target.SelectedItem = null;

            Assert.AreEqual(_screen.Router.GetCurrentViewModel().GetType(), typeof(UpcomingMoviesListViewModel));
        }

        /// <summary>
        /// Test to make sure that when there is a valid value the navigate happens.
        /// </summary>
        [Test]
        public void SelectedItem_ValidCell_Navigate()
        {
            Observable.Return(0).InvokeCommand(_target.LoadMovies);
            _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

            _target.SelectedItem = _target.Movies.First();

            Assert.AreEqual(_screen.Router.GetCurrentViewModel().GetType(), typeof(MovieDetailViewModel));
        }

        /// <summary>
        /// Test to make sure we load more when we hit the defined threshold.
        /// </summary>
        [Test]
        public void ItemAppearing_Items_OnlyLoadMoreWhenAboveThreshold()
        {
            Observable.Return(0).InvokeCommand(_target.LoadMovies);
            _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

            _target.ItemAppearing = _target.Movies.ElementAt(0);
            _target.ItemAppearing = _target.Movies.ElementAt(5);
            _target.ItemAppearing = _target.Movies.ElementAt(11);
            _target.ItemAppearing = _target.Movies.ElementAt(15);

            _movieService.Verify(x => x.LoadUpcomingMovies(It.IsAny<int>()), Times.Once);
        }

        /// <summary>
        /// Test to make sure we load upcoming movies.
        /// </summary>
        [Test]
        public void LoadMovies_Zero_LoadUpcomingMoviesInvokedWithZeroAndIsRefreshingUpdates()
        {
            Observable.Return(0).InvokeCommand(_target.LoadMovies);
            _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

            _movieService.Verify(x => x.LoadUpcomingMovies(It.Is<int>(y => y == 0)), Times.Once);
        }

        /// <summary>
        /// Test to make sure that we show an alert interaction if a exception is thrown.
        /// </summary>
        [Test]
        public void LoadMovies_ExceptionHappens_ShowAlertHandle()
        {
            Observable.Return(101).ObserveOn(_scheduler).InvokeCommand(_target.LoadMovies);
            _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

            Assert.AreEqual("Boom!", _alertOutput.Description);
        }
    }
}
