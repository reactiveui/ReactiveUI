// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Cinephile.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Cinephile.Views
{
    public partial class UpcomingMoviesCellView : ReactiveViewCell<UpcomingMoviesCellViewModel>
    {
        protected readonly CompositeDisposable SubscriptionDisposables = new CompositeDisposable();

        public UpcomingMoviesCellView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, x => x.Title, x => x.Title.Text).DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, x => x.PosterPath, x => x.Poster.Source, x => x).DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, x => x.Genres, x => x.Genres.Text, x => x).DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, x => x.ReleaseDate, x => x.ReleaseDate.Text, x => x).DisposeWith(SubscriptionDisposables);
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            SubscriptionDisposables.Clear();
        }
    }
}
