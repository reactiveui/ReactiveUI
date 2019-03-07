// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    [TestFixture]
    public class UpcomingMoviesListViewModelTest
    {
        private UpcomingMoviesListViewModel _target;
        private TestScheduler _scheculer;
        private Mock<IScreen> _screen;
        private Mock<IMovieService> _movieService;
        private SourceCache<Movie, int> _moviesSourceCache;

        [SetUp]
        public void Setup()
        {
            _scheculer = new TestScheduler();
            _screen = new Mock<IScreen>() { DefaultValue = DefaultValue.Mock };
            var routingState = new RoutingState();
            _screen.SetupGet(x => x.Router).Returns(routingState);
            //_screen.Setup(x => x.Router.Navigate.Execute(It.IsAny<IRoutableViewModel>()));

            _movieService = new Mock<IMovieService>() { DefaultValue = DefaultValue.Mock };
            _movieService.Setup(x => x.LoadUpcomingMovies(It.IsAny<int>())).Returns(() => Observable.Return(Unit.Default));

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

            _target = new UpcomingMoviesListViewModel(_scheculer, _scheculer, _movieService.Object, _screen.Object);

            _target.Activator.Activate();
            _scheculer.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
        }

        [Test]
        public void SelectedItem_null_NothingHappens()
        {
            _target.SelectedItem = null;
            _screen.Verify(x => x.Router.Navigate.Execute(It.IsAny<IRoutableViewModel>()), Times.Never);
        }

        [Test]
        public void SelectedItem_ValidCell_Navigate()
        { }

        [Test]
        public void ItemAppearing_Items_OnlyLoadMoreWhenAboveThreshold()
        {
            Observable.Return(0).InvokeCommand(_target.LoadMovies);
            _scheculer.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

            _target.ItemAppearing = _target.Movies.ElementAt(0);
            _target.ItemAppearing = _target.Movies.ElementAt(5);
            _target.ItemAppearing = _target.Movies.ElementAt(11);
            _target.ItemAppearing = _target.Movies.ElementAt(15);

            _movieService.Verify(x => x.LoadUpcomingMovies(It.IsAny<int>()), Times.Once);
        }


        [Test]
        public void LoadMovies_Zero_LoadUpcomingMoviesInvokedWithZeroAndIsRefreshingUpdates()
        {
            Observable.Return(0).InvokeCommand(_target.LoadMovies);
            _scheculer.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

            _movieService.Verify(x => x.LoadUpcomingMovies(It.Is<int>(y => y == 0)), Times.Once);
        }

        [Test]
        public void LoadMovies_ExceptionHappens_ShowAlertHandle()
        {
            AlertViewModel actual = null;
            _target.ShowAlert.RegisterHandler(handler =>
            {
                actual = handler.Input;
                handler.SetOutput(Unit.Default);
            });

            _movieService.Setup(x => x.LoadUpcomingMovies(It.IsAny<int>())).Returns(() => throw new Exception("Boom!"));



            Observable.Return(0).InvokeCommand(_target.LoadMovies);
            _scheculer.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);


            Assert.AreEqual("Boom!", actual.Description);
        }
    }
}