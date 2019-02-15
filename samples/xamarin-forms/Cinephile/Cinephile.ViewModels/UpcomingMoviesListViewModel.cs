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

namespace Cinephile.ViewModels
{
    public class UpcomingMoviesListViewModel : ViewModelBase
    {
        ReadOnlyObservableCollection<UpcomingMoviesCellViewModel> m_movies;
        public ReadOnlyObservableCollection<UpcomingMoviesCellViewModel> Movies => m_movies;

        UpcomingMoviesCellViewModel m_selectedItem;
        public UpcomingMoviesCellViewModel SelectedItem
        {
            get { return m_selectedItem; }
            set { this.RaiseAndSetIfChanged(ref m_selectedItem, value); }
        }

        string m_searchText;
        public string SearchText
        {
            get { return m_searchText; }
            set { this.RaiseAndSetIfChanged(ref m_searchText, value); }
        }


        UpcomingMoviesCellViewModel m_itemAppearing;
        public UpcomingMoviesCellViewModel ItemAppearing
        {
            get { return m_itemAppearing; }
            set { this.RaiseAndSetIfChanged(ref m_itemAppearing, value); }
        }

        public ReactiveCommand<int, Unit> LoadMovies
        {
            get;
        }

        ObservableAsPropertyHelper<bool> m_isRefreshing;
        public bool IsRefreshing => m_isRefreshing.Value;

        private MovieService movieService;
        IScheduler mainThreadScheduler;
        IScheduler taskPoolScheduler;

        public UpcomingMoviesListViewModel(IScheduler mainThreadScheduler = null, IScheduler taskPoolScheduler = null, IScreen hostScreen = null) : base(hostScreen)
        {
            this.mainThreadScheduler = mainThreadScheduler ?? RxApp.MainThreadScheduler;
            this.taskPoolScheduler = taskPoolScheduler ?? RxApp.TaskpoolScheduler;

            UrlPathSegment = "Upcoming Movies";

            movieService = new MovieService();

            LoadMovies = ReactiveCommand.Create<int, Unit>(offset =>
                movieService.LoadUpcomingMovies(offset),
                outputScheduler: this.mainThreadScheduler);

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                SelectedItem = null;

                var search = this
                    .WhenAnyValue(x => x.SearchText)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Throttle(TimeSpan.FromMilliseconds(300)) // Wait 100 ms after last keyboard press before searching
                    .StartWith(string.Empty)
                    .Select(SearchPredicate);

                movieService
                    .UpcomingMovies
                    .Connect()
                    .DisposeMany()
                    .Filter(search)
                    .Sort(SortExpressionComparer<Movie>.Ascending(i => i.ReleaseDate))
                    .Cast(movie => new UpcomingMoviesCellViewModel(movie))
                    .ObserveOn(this.mainThreadScheduler)
                    .Bind(out m_movies)
                    .Subscribe()
                    .DisposeWith(disposables);

                LoadMovies
                    .Subscribe()
                    .DisposeWith(disposables);

                this
                    .WhenAnyValue(x => x.SelectedItem)
                    .Where(x => x != null)
                    .Subscribe(x => LoadSelectedPage(x))
                    .DisposeWith(disposables);

                LoadMovies
                    .ThrownExceptions
                    .Subscribe((obj) =>
                    {
                        Debug.WriteLine(obj.Message);
                    });

                m_isRefreshing =
                    LoadMovies
                        .IsExecuting
                        .Select(x => x)
                        .Do(x => Debug.WriteLine($"Loading {x} == {DateTime.Now.ToString()}"))
                        .ToProperty(this, x => x.IsRefreshing, true)
                        .DisposeWith(disposables);

                WhenNeedToLoadMore()
                    .InvokeCommand(LoadMovies)
                    .DisposeWith(disposables);
            });
        }

        private IObservable<int> WhenNeedToLoadMore()
        {
            return this.WhenAnyValue(x => x.ItemAppearing)
                .Select(item =>
                {
                    if (item == null)
                        return -1; //causes initial load

                    return Movies.IndexOf(item);
                })
                .Where(index => index >= Movies.Count - 5);
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
