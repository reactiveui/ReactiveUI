// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using DynamicData;
using DynamicData.Binding;

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
        private readonly ObservableCollection<IRoutableViewModel> _navigationStack;

        [IgnoreDataMember]
        private IScheduler _scheduler;

        /// <summary>
        /// Initializes static members of the <see cref="RoutingState"/> class.
        /// </summary>
        static RoutingState()
        {
            RxApp.EnsureInitialized();
        }

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
            _navigationStack = new ObservableCollection<IRoutableViewModel>();
            _scheduler = scheduler;
            SetupRx();
        }

        /// <summary>
        /// Gets the current navigation stack, the last element in the
        /// collection being the currently visible ViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ObservableCollection<IRoutableViewModel> NavigationStack => _navigationStack;

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
        /// Gets or sets a observable which will navigate back to the previous element in the stack.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<Unit, Unit> NavigateBack { get; protected set; }

        /// <summary>
        /// Navigates to the a new element in the stack - the Execute parameter
        /// must be a ViewModel that implements IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> Navigate { get; protected set; }

        /// <summary>
        /// Navigates to a new element and resets the navigation stack (i.e. the
        /// new ViewModel will now be the only element in the stack) - the
        /// Execute parameter must be a ViewModel that implements
        /// IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateAndReset { get; protected set; }

        /// <summary>
        /// Gets the current view model which is to be shown for the Routing.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IRoutableViewModel> CurrentViewModel { get; protected set; }

        /// <summary>
        /// Gets a Observable which will trigger when the Navigation changes.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IChangeSet<IRoutableViewModel>> NavigationChanged { get; protected set; }

        [OnDeserialized]
        private void SetupRx(StreamingContext sc)
        {
            SetupRx();
        }

        private void SetupRx()
        {
            var navigateScheduler = _scheduler ?? RxApp.MainThreadScheduler;

            NavigationChanged = _navigationStack.ToObservableChangeSet();

            var countAsBehavior = Observable.Concat(
                                                    Observable.Defer(() => Observable.Return(NavigationStack.Count)),
                                                    NavigationChanged.CountChanged().Select(_ => NavigationStack.Count));
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
                if (vm == null)
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
