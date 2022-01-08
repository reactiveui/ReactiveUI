// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.XamForms;
using ReactiveUI.XamForms.Tests.Activation;
using ReactiveUI.XamForms.Tests.Activation.Mocks;
using Splat;
using Xamarin.Forms;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests for activating views.
    /// </summary>
    public class ActivatingReactivePagesTests : IClassFixture<ApplicationFixture<App>>
    {
        private ApplicationFixture<App> _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatingReactivePagesTests"/> class.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        public ActivatingReactivePagesTests(ApplicationFixture<App> fixture)
        {
            _fixture = fixture;
            Locator.CurrentMutable.Register(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            Locator.CurrentMutable.Register<IViewFor<ShellViewModel>>(() => new ShellView());
            Locator.CurrentMutable.Register<IViewFor<ContentPageViewModel>>(() => new ContentPageView());
            Locator.CurrentMutable.Register<IViewFor<TabbedPageViewModel>>(() => new TabbedPageView());
            Locator.CurrentMutable.Register<IViewFor<CarouselPageViewModel>>(() => new CarouselPageView());
            Locator.CurrentMutable.Register<IViewFor<FlyOutPageViewModel>>(() => new FlyoutPageView());
            _fixture.ActivateApp(new App());
        }

        /// <summary>
        /// Tests to make sure that views generally activate.
        /// </summary>
        [Fact]
        public void ActivatingReactiveShellTest()
        {
            var main = _fixture.AppMock!.MainPage as AppShell;

            Assert.Equal(1, main!.ViewModel!.IsActiveCount);
            Assert.Equal(1, main.IsActiveCount);
        }

        /// <summary>
        /// Tests to make sure that views generally activate.
        /// </summary>
        [Fact]
        public void ActivatingReactiveContentPageTest()
        {
            var vm = new ContentPageViewModel();
            var fixture = new ContentPageView
            {
                ViewModel = vm
            };

            // Activate
            Shell.Current.Navigation.PushAsync(fixture);
            Assert.Equal(1, fixture.ViewModel.IsActiveCount);
            Assert.Equal(1, fixture.IsActiveCount);

            // Deactivate
            Shell.Current.GoToAsync("..");
            fixture.ViewModel = null;
            Assert.Equal(0, vm.IsActiveCount);
            Assert.Equal(0, fixture.IsActiveCount);
        }

        /// <summary>
        /// Tests to make sure that views generally activate.
        /// </summary>
        [Fact]
        public void ActivatingReactiveTabbedPageTest()
        {
            var vm1 = new TabbedPageViewModel();
            var fixture1 = new TabbedPageView
            {
                ViewModel = vm1
            };

            // Activate
            Shell.Current.Navigation.PushAsync(fixture1);
            Assert.Equal(1, fixture1.ViewModel.IsActiveCount);
            Assert.Equal(1, fixture1.IsActiveCount);

            // Deactivate
            Shell.Current.GoToAsync("..");
            fixture1.ViewModel = null;
            Assert.Equal(0, vm1.IsActiveCount);
            Assert.Equal(0, fixture1.IsActiveCount);
        }

        /// <summary>
        /// Tests to make sure that views generally activate.
        /// </summary>
        [Fact]
        public void ActivatingReactiveFlyoutPageTest()
        {
            var vm3 = new FlyOutPageViewModel();
            var fixture3 = new FlyoutPageView
            {
                ViewModel = vm3
            };

            // Activate
            Shell.Current.Navigation.PushAsync(fixture3);
            Assert.Equal(1, fixture3.ViewModel!.IsActiveCount);
            Assert.Equal(1, fixture3.IsActiveCount);

            // Deactivate
            Shell.Current.GoToAsync("..");
            fixture3.ViewModel = null;
            Assert.Equal(0, vm3.IsActiveCount);
            Assert.Equal(0, fixture3.IsActiveCount);
        }

        /// <summary>
        /// Tests to make sure that views generally activate.
        /// </summary>
        [Fact]
        public void ActivatingReactiveCarouselPageTest()
        {
            var vm4 = new CarouselPageViewModel();
            var fixture4 = new CarouselPageView
            {
                ViewModel = vm4
            };

            // Activate
            Shell.Current.Navigation.PushAsync(fixture4);
            Assert.Equal(1, fixture4.ViewModel!.IsActiveCount);
            Assert.Equal(1, fixture4.IsActiveCount);

            // Deactivate
            Shell.Current.GoToAsync("..");
            fixture4.ViewModel = null;
            Assert.Equal(0, vm4.IsActiveCount);
            Assert.Equal(0, fixture4.IsActiveCount);
        }
    }
}
