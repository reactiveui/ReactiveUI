// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using System.Collections.ObjectModel;
using Cinephile.Core.Models;
using DynamicData;
using DynamicData.PLinq;
using Cinephile.Core.Infrastructure;

namespace Cinephile.ViewModels
{
    public class UpcomingMoviesListViewModel : ViewModelBase
    {
        readonly ReadOnlyObservableCollection<UpcomingMoviesCellViewModel> _movies;
        public ReadOnlyObservableCollection<UpcomingMoviesCellViewModel> Movies => _movies;

        private UpcomingMoviesCellViewModel _selectedItem;
        public UpcomingMoviesCellViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { this.RaiseAndSetIfChanged(ref _selectedItem, value); }
        }

        string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }


        UpcomingMoviesCellViewModel _itemAppearing;
        public UpcomingMoviesCellViewModel ItemAppearing
        {
            get { return _itemAppearing; }
            set { this.RaiseAndSetIfChanged(ref _itemAppearing, value); }
        }

        public ReactiveCommand<int, Unit> LoadMovies 
        {
            get;
            set;
        }

        ObservableAsPropertyHelper<bool> _isRefreshing;
        public bool IsRefreshing => _isRefreshing.Value;

        private MovieService _movieService;

        public UpcomingMoviesListViewModel(IScheduler mainThreadScheduler = null, IScheduler taskPoolScheduler = null, IScreen hostScreen = null)
            : base("Upcoming Movies", mainThreadScheduler, taskPoolScheduler, hostScreen)
        {

            _movieService = new MovieService();

            LoadMovies = ReactiveCommand.CreateFromObservable<int, Unit>(count => _movieService.LoadUpcomingMovies(count));

            _movieService
                .UpcomingMovies
                .Connect()
                .SubscribeOn(_taskPoolScheduler)
                .ObserveOn(_taskPoolScheduler)
                .Transform(movie => new UpcomingMoviesCellViewModel(movie), (o, n) => o = new UpcomingMoviesCellViewModel(n))
                .DisposeMany()
                .ObserveOn(_mainThreadScheduler)
                .Bind(out _movies)
                .Subscribe();

            LoadMovies.Subscribe();

            this
                .WhenAnyValue(x => x.SelectedItem)
                .Where(x => x != null)
                .SelectMany(x => HostScreen
                .Router
                .Navigate
                .Execute(new MovieDetailViewModel(x.Movie)))
                .Subscribe();

            LoadMovies
                .ThrownExceptions
                .SelectMany(ex => ShowAlert.Handle(new AlertViewModel("Oops", ex.Message, "Ok")))
                .Subscribe();

            _isRefreshing =
                LoadMovies
                    .IsExecuting
                    .ToProperty(this, x => x.IsRefreshing, true);

            this
                .WhenAnyValue(x => x.ItemAppearing)
                .Select(item =>
                {
                    int offset = -1;

                    var itemIndex = Movies.IndexOf(item);
                    if (itemIndex == Movies.Count - 8)
                    {
                        offset = Movies.Count;
                    }

                    return offset;
                })
                .Where(index => index > 0)
                .InvokeCommand(LoadMovies);
        }
    }
}
