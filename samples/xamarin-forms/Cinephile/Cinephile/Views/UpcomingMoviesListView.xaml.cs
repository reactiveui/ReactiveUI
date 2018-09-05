// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Cinephile.Core.Model; 
using Cinephile.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace Cinephile.Views
{
    public partial class UpcomingMoviesListView : ContentPageBase<UpcomingMoviesListViewModel>
    {
        public UpcomingMoviesListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                ViewModel.SelectedItem = null;

                this.OneWayBind(ViewModel, x => x.Movies, x => x.UpcomingMoviesList.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, x => x.SelectedItem, x => x.UpcomingMoviesList.SelectedItem)
                    .DisposeWith(disposables);

                UpcomingMoviesList
                    .Events()
                    .ItemAppearing
                    .Select((e) => e.Item as UpcomingMoviesCellViewModel)
                    .BindTo(this, x => x.ViewModel.ItemAppearing)
                    .DisposeWith(disposables);

            });
        }
    }
}
