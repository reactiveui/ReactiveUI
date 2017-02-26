using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.Serialization;

namespace ReactiveUI
{
    /// <summary>
    /// Routing State Mixins
    /// </summary>
    public static class RoutingStateMixins
    {
        /// <summary>
        /// Locate the first ViewModel in the stack that matches a certain Type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This">The this.</param>
        /// <returns>The matching ViewModel or null if none exists.</returns>
        public static T FindViewModelInStack<T>(this RoutingState This)
            where T : IRoutableViewModel
        {
            return This.NavigationStack.Reverse().OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the currently visible ViewModel
        /// </summary>
        /// <param name="This">The this.</param>
        /// <returns></returns>
        public static IRoutableViewModel GetCurrentViewModel(this RoutingState This)
        {
            return This.NavigationStack.LastOrDefault();
        }
    }

    /// <summary>
    /// RoutingState manages the ViewModel Stack and allows ViewModels to navigate to other ViewModels.
    /// </summary>
    [DataContract]
    public class RoutingState : ReactiveObject
    {
        [DataMember] private ReactiveList<IRoutableViewModel> _NavigationStack;

        [IgnoreDataMember]
        private IScheduler scheduler;

        static RoutingState()
        {
            RxApp.EnsureInitialized();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingState"/> class.
        /// </summary>
        public RoutingState()
        {
            this._NavigationStack = new ReactiveList<IRoutableViewModel>();
            setupRx();
        }

        /// <summary>
        /// Gets or sets the current view model.
        /// </summary>
        /// <value>The current view model.</value>
        [IgnoreDataMember]
        public IObservable<IRoutableViewModel> CurrentViewModel { get; protected set; }

        /// <summary>
        /// Navigates to the a new element in the stack - the Execute parameter must be a ViewModel
        /// that implements IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> Navigate { get; protected set; }

        /// <summary>
        /// Navigates to a new element and resets the navigation stack (i.e. the new ViewModel will
        /// now be the only element in the stack) - the Execute parameter must be a ViewModel that
        /// implements IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateAndReset { get; protected set; }

        /// <summary>
        /// Navigates back to the previous element in the stack.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<Unit, Unit> NavigateBack { get; protected set; }

        /// <summary>
        /// Represents the current navigation stack, the last element in the collection being the
        /// currently visible ViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveList<IRoutableViewModel> NavigationStack
        {
            get { return this._NavigationStack; }
            protected set { this._NavigationStack = value; }
        }

        /// <summary>
        /// The scheduler used for commands. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </summary>
        [IgnoreDataMember]
        public IScheduler Scheduler
        {
            get
            {
                return this.scheduler;
            }

            set
            {
                if (this.scheduler != value) {
                    this.scheduler = value;
                    this.setupRx();
                }
            }
        }

        [OnDeserialized]
        private void setupRx(StreamingContext sc)
        { setupRx(); }

        private void setupRx()
        {
            var scheduler = this.scheduler ?? RxApp.MainThreadScheduler;

            var countAsBehavior = Observable.Concat(
                Observable.Defer(() => Observable.Return(this._NavigationStack.Count)),
                this.NavigationStack.CountChanged);

            this.NavigateBack =
                ReactiveCommand.CreateFromObservable(() => {
                    this.NavigationStack.RemoveAt(this.NavigationStack.Count - 1);
                    return Observables.Unit;
                },
                countAsBehavior.Select(x => x > 1),
                scheduler);

            this.Navigate = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(x => {
                var vm = x as IRoutableViewModel;
                if (vm == null) {
                    throw new Exception("Navigate must be called on an IRoutableViewModel");
                }

                this.NavigationStack.Add(vm);
                return Observable.Return(x);
            },
            outputScheduler: scheduler);

            this.NavigateAndReset = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(x => {
                this.NavigationStack.Clear();
                return this.Navigate.Execute(x);
            },
            outputScheduler: scheduler);

            this.CurrentViewModel = Observable.Concat(
                Observable.Defer(() => Observable.Return(this.NavigationStack.LastOrDefault())),
                this.NavigationStack.Changed.Select(_ => this.NavigationStack.LastOrDefault()));
        }
    }
}