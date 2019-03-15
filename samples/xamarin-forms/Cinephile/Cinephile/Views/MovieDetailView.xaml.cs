// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using Cinephile.ViewModels;
using ReactiveUI;

namespace Cinephile.Views
{
    /// <summary>
    /// A page which contains details about a movie.
    /// </summary>
    public partial class MovieDetailView : ContentPageBase<MovieDetailViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieDetailView"/> class.
        /// </summary>
        public MovieDetailView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, x => x.PosterBig, x => x.Poster.Source, x => x).DisposeWith(disposables);
                this.OneWayBind(ViewModel, x => x.PosterSmall, x => x.Poster.LoadingPlaceholder, x => x).DisposeWith(disposables);
                this.OneWayBind(ViewModel, x => x.Genres, x => x.Genres.Text, x => x).DisposeWith(disposables);
                this.OneWayBind(ViewModel, x => x.ReleaseDate, x => x.ReleaseDate.Text, x => x).DisposeWith(disposables);
                this.OneWayBind(ViewModel, x => x.Overview, x => x.Overview.Text, x => x).DisposeWith(disposables);
            });
        }
    }
}
