// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;
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
    public class ActivatingReactiveContentPageTests
    {
        /// <summary>
        /// Tests to make sure that views generally activate.
        /// </summary>
        [Fact]
        public void ActivatingReactiveContentPageTest()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            locator.Register<IViewFor<ShellViewModel>>(() => new ShellView());
            locator.Register<IViewFor<ContentPageViewModel>>(() => new ContentPageView());
            locator.Register<IViewFor<TabbedPageViewModel>>(() => new TabbedPageView());
            locator.Register<IViewFor<CarouselPageViewModel>>(() => new CarouselPageView());
            locator.Register<IViewFor<FlyOutPageViewModel>>(() => new FlyoutPageView());

            MockForms.Init();
            var app = new App();
            var main = app.MainPage as AppShell;

            Assert.Equal(1, main!.ViewModel!.IsActiveCount);
            Assert.Equal(1, main.IsActiveCount);

            var vm = new ContentPageViewModel();
            var fixture = new ContentPageView
            {
                ViewModel = vm
            };

            // Activate
            main.Navigation.PushAsync(fixture);
            Assert.Equal(1, fixture.ViewModel.IsActiveCount);
            Assert.Equal(1, fixture.IsActiveCount);

            // Deactivate
            Shell.Current.GoToAsync("..");
            fixture.ViewModel = null;
            Assert.Equal(0, vm.IsActiveCount);
            Assert.Equal(0, fixture.IsActiveCount);

            var vm1 = new TabbedPageViewModel();
            var fixture1 = new TabbedPageView
            {
                ViewModel = vm1
            };

            // Activate
            main.Navigation.PushAsync(fixture1);
            Assert.Equal(1, fixture1.ViewModel.IsActiveCount);
            Assert.Equal(1, fixture1.IsActiveCount);

            // Deactivate
            Shell.Current.GoToAsync("..");
            fixture1.ViewModel = null;
            Assert.Equal(0, vm1.IsActiveCount);
            Assert.Equal(0, fixture1.IsActiveCount);

            var vm3 = new FlyOutPageViewModel();
            var fixture3 = new FlyoutPageView
            {
                ViewModel = vm3
            };

            // Activate
            main.Navigation.PushAsync(fixture3);
            Assert.Equal(1, fixture3.ViewModel!.IsActiveCount);
            Assert.Equal(1, fixture3.IsActiveCount);

            // Deactivate
            Shell.Current.GoToAsync("..");
            fixture3.ViewModel = null;
            Assert.Equal(0, vm3.IsActiveCount);
            Assert.Equal(0, fixture3.IsActiveCount);

            var vm4 = new CarouselPageViewModel();
            var fixture4 = new CarouselPageView
            {
                ViewModel = vm4
            };

            // Activate
            main.Navigation.PushAsync(fixture4);
            Assert.Equal(1, fixture4.ViewModel!.IsActiveCount);
            Assert.Equal(1, fixture4.IsActiveCount);

            // Deactivate
            Shell.Current.GoToAsync("..");
            fixture4.ViewModel = null;
            Assert.Equal(0, vm4.IsActiveCount);
            Assert.Equal(0, fixture4.IsActiveCount);

            // remember to kill the app
            app.Quit();
        }
    }
}
