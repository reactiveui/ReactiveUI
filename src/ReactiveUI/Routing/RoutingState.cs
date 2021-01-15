// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using DynamicData;
using DynamicData.Binding;
#pragma warning disable 8618

namespace ReactiveUI
{
    /// <summary>
    /// RoutingState manages the ViewModel Stack and allows ViewModels to
    /// navigate to other ViewModels.
    /// </summary>
    [DataContract]
    public class RoutingState : ReactiveObject
    {
        [DataMember]
#if WINUI3UWP
        private readonly DynamicData.Binding.WinUI3UWP.ObservableCollection<IRoutableViewModel> _navigationStack;
#else
        private readonly ObservableCollection<IRoutableViewModel> _navigationStack;
#endif

        [IgnoreDataMember]
        private IScheduler _scheduler;

        /// <summary>
        /// Initializes static members of the <see cref="RoutingState"/> class.
        /// </summary>R
        static RoutingState() => RxApp.EnsureInitialized();

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingState"/> class.
        /// </summary>
        public RoutingState()
            : this(RxApp.MainThreadScheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingState"/> class.
        /// </summary>
        /// <param name="scheduler">A scheduler for where to send navigation changes to.</param>
        public RoutingState(IScheduler scheduler)
        {
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
#if WINUI3UWP
            _navigationStack = new DynamicData.Binding.WinUI3UWP.ObservableCollection<IRoutableViewModel>();
#else
            _navigationStack = new ObservableCollection<IRoutableViewModel>();
#endif
            SetupRx();
        }

        /// <summary>
        /// Gets the current navigation stack, the last element in the
        /// collection being the currently visible ViewModel.
        /// </summary>
        [IgnoreDataMember]
#if WINUI3UWP
        public DynamicData.Binding.WinUI3UWP.ObservableCollection<IRoutableViewModel> NavigationStack => _navigationStack;
#else
        public ObservableCollection<IRoutableViewModel> NavigationStack => _navigationStack;
#endif

        /// <summary>
        /// Gets or sets the scheduler used for commands. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </summary>
        [IgnoreDataMember]
        public IScheduler Scheduler
        {
            get => _scheduler;
            set
            {
                if (_scheduler != value)
                {
                    _scheduler = value;
                    SetupRx();
                }
            }
        }

        /// <summary>
        /// Gets or sets a command which will navigate back to the previous element in the stack.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<Unit, Unit> NavigateBack { get; protected set; }

        /// <summary>
        /// Gets or sets a command that navigates to the a new element in the stack - the Execute parameter
        /// must be a ViewModel that implements IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> Navigate { get; protected set; }

        /// <summary>
        /// Gets or sets a command that navigates to a new element and resets the navigation stack (i.e. the
        /// new ViewModel will now be the only element in the stack) - the
        /// Execute parameter must be a ViewModel that implements
        /// IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateAndReset { get; protected set; }

        /// <summary>
        /// Gets or sets the current view model which is to be shown for the Routing.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IRoutableViewModel?> CurrentViewModel { get; protected set; }

        /// <summary>
        /// Gets or sets an observable which will signal when the Navigation changes.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IChangeSet<IRoutableViewModel>> NavigationChanged { get; protected set; }

        [OnDeserialized]
        private void SetupRx(StreamingContext sc) => SetupRx();

        private void SetupRx()
        {
            var navigateScheduler = _scheduler;

            NavigationChanged = _navigationStack.ToObservableChangeSet();

            var countAsBehavior = Observable.Defer(() => Observable.Return(NavigationStack.Count)).Concat(NavigationChanged.CountChanged().Select(_ => NavigationStack.Count));
            NavigateBack =
                ReactiveCommand.CreateFromObservable(
                    () =>
                    {
                        _navigationStack.RemoveAt(NavigationStack.Count - 1);
                        return Observables.Unit;
                    },
                    countAsBehavior.Select(x => x > 1),
                    navigateScheduler);

            Navigate = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(
                vm =>
                {
                    if (vm is null)
                    {
                        throw new Exception("Navigate must be called on an IRoutableViewModel");
                    }

                    _navigationStack.Add(vm);
                    return Observable.Return(vm);
                },
                outputScheduler: navigateScheduler);

            NavigateAndReset = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(
                vm =>
                {
                    _navigationStack.Clear();
                    return Navigate.Execute(vm);
                },
                outputScheduler: navigateScheduler);

            CurrentViewModel = Observable.Defer(() => Observable.Return(NavigationStack.LastOrDefault())).Concat(NavigationChanged.Select(_ => NavigationStack.LastOrDefault()));
        }
    }
}
