// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Cinephile.Core.Models;
using Cinephile.Core.Rest;
using Cinephile.ViewModels;
using Cinephile.Views;
using ReactiveUI;
using ReactiveUI.XamForms;
using Splat;
using Xamarin.Forms;

namespace Cinephile
{
    /// <summary>
    /// The app bootstrapper which is used to register everything with the Splat service locator.
    /// It is also the central location for the RoutingState used for routing between views.
    /// </summary>
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
        /// </summary>
        public AppBootstrapper()
        {
            Router = new RoutingState();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
            Locator.CurrentMutable.Register(() => new UpcomingMoviesListView(), typeof(IViewFor<UpcomingMoviesListViewModel>));
            Locator.CurrentMutable.Register(() => new UpcomingMoviesCellView(), typeof(IViewFor<UpcomingMoviesCellViewModel>));
            Locator.CurrentMutable.Register(() => new MovieDetailView(), typeof(IViewFor<MovieDetailViewModel>));
            Locator.CurrentMutable.Register(() => new AboutView(), typeof(IViewFor<AboutViewModel>));

            Locator.CurrentMutable.Register(() => new Cache(), typeof(ICache));
            Locator.CurrentMutable.Register(() => new ApiService(), typeof(IApiService));
            Locator.CurrentMutable.Register(() => new MovieService(), typeof(IMovieService));

            Router
                .NavigateAndReset
                .Execute(new UpcomingMoviesListViewModel())
                .Subscribe();
        }

        /// <summary>
        /// Gets or sets the router which is used to navigate between views.
        /// </summary>
        public RoutingState Router { get; protected set; }

        /// <summary>
        /// Creates the first main page used within the application.
        /// </summary>
        /// <returns>The page generated.</returns>
        public static Page CreateMainPage()
        {
            // NB: This returns the opening page that the platform-specific
            // boilerplate code will look for. It will know to find us because
            // we've registered our AppBootstrappScreen.
            return new RoutedViewHost();
        }
    }
}
