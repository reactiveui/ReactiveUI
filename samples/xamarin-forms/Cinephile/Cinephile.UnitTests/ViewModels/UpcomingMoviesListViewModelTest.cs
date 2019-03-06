using System;
using Cinephile.ViewModels;
using NUnit.Framework;

namespace Cinephile.UnitTests.ViewModels
{
    [TestFixture]
    public class UpcomingMoviesListViewModelTest
    {
        private UpcomingMoviesListViewModel _target;

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void SelectedItem_null_NothingHappens()
        { }

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