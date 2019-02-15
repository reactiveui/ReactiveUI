// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using System.Collections.ObjectModel;
using Cinephile.Core.Models;
using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;

namespace Cinephile.ViewModels
{
    public class UpcomingMoviesListViewModel : ViewModelBase
    {
        ReadOnlyObservableCollection<UpcomingMoviesCellViewModel> _movies;
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
        }

        ObservableAsPropertyHelper<bool> _isRefreshing;
        public bool IsRefreshing => _isRefreshing.Value;

        private MovieService _movieService;
        private readonly IScheduler _mainThreadScheduler;
        private readonly IScheduler _taskPoolScheduler;

        public UpcomingMoviesListViewModel(IScheduler mainThreadScheduler = null, IScheduler taskPoolScheduler = null, IScreen hostScreen = null) : base(hostScreen)
        {
            _mainThreadScheduler = mainThreadScheduler ?? RxApp.MainThreadScheduler;
            _taskPoolScheduler = taskPoolScheduler ?? RxApp.TaskpoolScheduler;

            UrlPathSegment = "Upcoming Movies";

            _movieService = new MovieService();

            LoadMovies = ReactiveCommand.Create<int, Unit>(_movieService.LoadUpcomingMovies,
                outputScheduler: _mainThreadScheduler);

            var search = this
                .WhenAnyValue(x => x.SearchText)
                .Where(x => !string.IsNullOrEmpty(x))
                .Throttle(TimeSpan.FromMilliseconds(300)) // Wait 100 ms after last keyboard press before searching
                .StartWith(string.Empty)
                .Select(SearchPredicate);

            _movieService
                .UpcomingMovies
                .Connect()
                .DisposeMany()
                .Filter(search)
                .Sort(SortExpressionComparer<Movie>.Ascending(i => i.ReleaseDate))
                .Transform(movie => new UpcomingMoviesCellViewModel(movie))
                .SubscribeOn(_taskPoolScheduler)
                .ObserveOn(_mainThreadScheduler)
                .Bind(out _movies)
                .Subscribe();

            SelectedItem = null;

            LoadMovies
                .Subscribe();

            this
                .WhenAnyValue(x => x.SelectedItem)
                .Where(x => x != null)
                .Subscribe(LoadSelectedPage);

            LoadMovies
                .ThrownExceptions
                .Subscribe((obj) =>
                {
                    Debug.WriteLine(obj.Message);
                });

            _isRefreshing =
                LoadMovies
                    .IsExecuting
                    .Select(x => x)
                    .Do(x => Debug.WriteLine($"Loading {x} == {DateTime.Now.ToString()}"))
                    .ToProperty(this, x => x.IsRefreshing, true);

            WhenNeedToLoadMore()
                .InvokeCommand(LoadMovies);
        }

        private IObservable<int> WhenNeedToLoadMore()
        {
            return this
                .WhenAnyValue(x => x.ItemAppearing)
                .Select(item =>
                {
                    int offset = -1;
                    var itemIndex = Movies.IndexOf(item);
                    var loadedItemsCount = Movies.Count();

                    if (loadedItemsCount % MovieService.PageSize == 0 && itemIndex == loadedItemsCount - 8)
                    {
                        offset = loadedItemsCount;
                    }

                    return offset;
                })
                .Where(index => index > 0);
        }
        private Func<Movie, bool> SearchPredicate(string searchText)
        {
            if (searchText == null || searchText.Length < 3)
                return s => true;

            return s => string.IsNullOrEmpty(s.Title) ||
                s.Title.ToLowerInvariant().Contains(searchText.ToLowerInvariant());
        }

        void LoadSelectedPage(UpcomingMoviesCellViewModel viewModel)
        {
            HostScreen
                .Router
                .Navigate
                .Execute(new MovieDetailViewModel(viewModel.Movie))
                .Subscribe();
        }
    }
}
