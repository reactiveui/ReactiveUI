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
        static RoutingState()
        {
            RxApp.EnsureInitialized();
        }

        [DataMember]
        private readonly ObservableCollection<IRoutableViewModel> _navigationStack;

        /// <summary>
        /// Represents the current navigation stack, the last element in the
        /// collection being the currently visible ViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ObservableCollection<IRoutableViewModel> NavigationStack => _navigationStack;

        [IgnoreDataMember]
        private IScheduler scheduler;

        /// <summary>
        /// The scheduler used for commands. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </summary>
        [IgnoreDataMember]
        public IScheduler Scheduler {
            get => scheduler;
            set {
                if (scheduler != value) {
                    scheduler = value;
                    setupRx();
                }
            }
        }

        /// <summary>
        /// Navigates back to the previous element in the stack.
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

        [IgnoreDataMember]
        public IObservable<IRoutableViewModel> CurrentViewModel { get; protected set; }

        [IgnoreDataMember]
        public IObservable<IChangeSet<IRoutableViewModel>> NavigationChanged { get; protected set; }

        public RoutingState()
        {
            _navigationStack = new ObservableCollection<IRoutableViewModel>();
            setupRx();
        }

        [OnDeserialized]
        void setupRx(StreamingContext sc) { setupRx();  }

        void setupRx()
        {
            var navigateScheduler = this.scheduler ?? RxApp.MainThreadScheduler;

            NavigationChanged = _navigationStack.ToObservableChangeSet();

            var countAsBehavior = Observable.Concat(
                                                    Observable.Defer(() => Observable.Return(NavigationStack.Count)),
                                                    NavigationChanged.CountChanged().Select(_ => NavigationStack.Count));
            NavigateBack = 
                ReactiveCommand.CreateFromObservable(() => {
                    _navigationStack.RemoveAt(NavigationStack.Count - 1);
                    return Observables.Unit;
                },
                countAsBehavior.Select(x => x > 1),
                navigateScheduler);

            Navigate = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(x => {
                var vm = x;
                if (vm == null) {
                    throw new Exception("Navigate must be called on an IRoutableViewModel");
                }

                _navigationStack.Add(vm);
                return Observable.Return(x);
            },
            outputScheduler: navigateScheduler);

            NavigateAndReset = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(x => {
                _navigationStack.Clear();
                return Navigate.Execute(x);
            },
            outputScheduler: navigateScheduler);
            
            CurrentViewModel = Observable.Defer(() => Observable.Return(NavigationStack.LastOrDefault())).Concat(NavigationChanged.Select(_ => NavigationStack.LastOrDefault()));
        }
    }

    public static class RoutingStateMixins
    {
        /// <summary>
        /// Locate the first ViewModel in the stack that matches a certain Type.
        /// </summary>
        /// <returns>The matching ViewModel or null if none exists.</returns>
        public static T FindViewModelInStack<T>(this RoutingState This)
            where T : IRoutableViewModel
        {
            return This.NavigationStack.Reverse().OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the currently visible ViewModel
        /// </summary>
        public static IRoutableViewModel GetCurrentViewModel(this RoutingState This)
        {
            return This.NavigationStack.LastOrDefault();
        }
    }
}
