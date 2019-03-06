// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Cinephile.Core.Models;
using Cinephile.ViewModels;
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

        [SetUp]
        public void Setup()
        {
            _scheculer = new TestScheduler();
            _screen = new Mock<IScreen>() { DefaultValue = DefaultValue.Mock };
            _screen.Setup(x => x.Router.Navigate.Execute(It.IsAny<IRoutableViewModel>()));
            _movieService = new Mock<IMovieService>() { DefaultValue = DefaultValue.Mock };

            _target = new UpcomingMoviesListViewModel(_scheculer, _scheculer, _movieService.Object, _screen.Object);

            _target.Activator.Activate();
            _scheculer.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
        }

        [Test]
        public void SelectedItem_null_NothingHappens()
        {
            _target.SelectedItem = null;
            _screen.Verify(x => x.Router.Navigate.Execute(It.IsAny<IRoutableViewModel>()), Times.Once);
        }

        [Test]
        public void SelectedItem_ValidCell_Navigate()
        { }

        [Test]
        public void ItemAppearing_LowerThanThreshold_NothingHappens()
        { }

        [Test]
        public void ItemAppearing_GreaterOrEnqualsThanThreshold_LoadMovies()
        { }


        [Test]
        public void LoadMovies_Zero_LoadUpcomingMoviesInvokedWithZeroAndIsRefreshingUpdates()
        { }

        [Test]
        public void LoadMovies_ExceptionHappens_ShowAlertHandle()
        { }
    }
}