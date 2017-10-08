// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Cinephile.Core.Model;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace Cinephile.ViewModels
{
    public class UpcomingMoviesListViewModel : ViewModelBase
    {
        public ObservableCollection<UpcomingMoviesCellViewModel> Movies { get; } = 
            new ObservableCollection<UpcomingMoviesCellViewModel>();

        UpcomingMoviesCellViewModel m_selectedItem;
        public UpcomingMoviesCellViewModel SelectedItem
        {
            get { return m_selectedItem; }
            set { this.RaiseAndSetIfChanged(ref m_selectedItem, value); }
        }

        UpcomingMoviesCellViewModel m_itemAppearing;
        public UpcomingMoviesCellViewModel ItemAppearing
        {
            get { return m_itemAppearing; }
            set { this.RaiseAndSetIfChanged(ref m_itemAppearing, value); }
        }

        public ReactiveCommand<int, IEnumerable<Movie>> LoadMovies
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

            LoadMovies = ReactiveCommand
                .CreateFromObservable((int index) => 
                    movieService.GetUpcomingMovies(index),
                    outputScheduler: this.mainThreadScheduler);

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                SelectedItem = null;

                LoadMovies
                    .Where(movies => movies != null)
                    .Select(movies => movies.Select(movie => new UpcomingMoviesCellViewModel(movie)))
                    .SelectMany(movieCell => movieCell)
                    .Where(movieCell => !Movies.Select(m => m.Title).Contains(movieCell.Title))
                    .Do(_ => Debug.WriteLine($"Adding Movie Items"))
                    .Subscribe(movieViewModel => Movies.Add(movieViewModel))
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
                        .ToProperty(this, x => x.IsRefreshing, true)
                        .DisposeWith(disposables);


                this.WhenAnyValue(x=> x.ItemAppearing)
                    .Select(item =>
                    {
                        if (item == null)
                            return -1; //causes initial load

                        return Movies.IndexOf(item);
                    })
                    .Do(index => Debug.WriteLine($"==> index {index} >= {Movies.Count - 5} = {index >= Movies.Count - 5}"))
                    .Where(index => index >= Movies.Count - 5)
                    .InvokeCommand(LoadMovies)
                    .DisposeWith(disposables);

            });
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
