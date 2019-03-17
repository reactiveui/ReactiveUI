// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using Cinephile.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace Cinephile.Views
{
    /// <summary>
    /// A cell which contains details about upcoming movies.
    /// </summary>
    [SuppressMessage("Design", "CA1001: Type owns disposable field(s) but is not disposable", Justification = "Done by the UI events.")]
    public partial class UpcomingMoviesCellView : ReactiveViewCell<UpcomingMoviesCellViewModel>
    {
        private readonly CompositeDisposable _subscriptionDisposables = new CompositeDisposable();

        /// <summary>
        /// Initializes a new instance of the <see cref="UpcomingMoviesCellView"/> class.
        /// </summary>
        public UpcomingMoviesCellView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, x => x.Title, x => x.Title.Text).DisposeWith(_subscriptionDisposables);
                this.OneWayBind(ViewModel, x => x.Genres, x => x.Genres.Text, x => x).DisposeWith(_subscriptionDisposables);
                this.OneWayBind(ViewModel, x => x.ReleaseDate, x => x.ReleaseDate.Text, x => x).DisposeWith(_subscriptionDisposables);
            });
        }

        /// <inheritdoc/>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _subscriptionDisposables.Clear();
        }

        /// <inheritdoc/>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Poster.Source = null;

            if (!(BindingContext is UpcomingMoviesCellViewModel item))
            {
                return;
            }

            Poster.Source = item.PosterPath;
        }
    }
}
