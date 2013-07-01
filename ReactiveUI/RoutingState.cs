using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace ReactiveUI
{
    /// <summary>
    /// RoutingState manages the ViewModel Stack and allows ViewModels to
    /// navigate to other ViewModels.
    /// </summary>
    [DataContract]
    public class RoutingState : ReactiveObject, IRoutingState
    {
        [DataMember] ReactiveList<IRoutableViewModel> _NavigationStack;

        /// <summary>
        /// Represents the current navigation stack, the last element in the
        /// collection being the currently visible ViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveList<IRoutableViewModel> NavigationStack {
            get { return _NavigationStack; }
            protected set { _NavigationStack = value; }
        }

        /// <summary>
        /// Navigates back to the previous element in the stack.
        /// </summary>
        [IgnoreDataMember]
        public IReactiveCommand NavigateBack { get; protected set; }

        /// <summary>
        /// Navigates to the a new element in the stack - the Execute parameter
        /// must be a ViewModel that implements IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        public INavigateCommand Navigate { get; protected set; }

        /// <summary>
        /// Navigates to a new element and resets the navigation stack (i.e. the
        /// new ViewModel will now be the only element in the stack) - the
        /// Execute parameter must be a ViewModel that implements
        /// IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        public INavigateCommand NavigateAndReset { get; protected set; }

        [IgnoreDataMember]
        public IObservable<IRoutableViewModel> CurrentViewModel { get; protected set; }

        public RoutingState()
        {
            _NavigationStack = new ReactiveList<IRoutableViewModel>();
            setupRx();
        }

        [OnDeserialized]
        void setupRx(StreamingContext sc) { setupRx();  }

        void setupRx()
        {
            NavigateBack = new ReactiveCommand(
                NavigationStack.CountChanged.StartWith(_NavigationStack.Count).Select(x => x > 1));
            NavigateBack.Subscribe(_ =>
                NavigationStack.RemoveAt(NavigationStack.Count - 1));

            Navigate = new NavigationReactiveCommand();
            Navigate.Subscribe(x => {
                var vm = x as IRoutableViewModel;
                if (vm == null) {
                    throw new Exception("Navigate must be called on an IRoutableViewModel");
                }

                NavigationStack.Add(vm);
            });

            NavigateAndReset = new NavigationReactiveCommand();
            NavigateAndReset.Subscribe(x => {
                NavigationStack.Clear();
                Navigate.Execute(x);
            });

            CurrentViewModel = Observable.Concat(
                Observable.Defer(() => Observable.Return(NavigationStack.LastOrDefault())),
                NavigationStack.Changed.Select(_ => NavigationStack.LastOrDefault()));
        }
    }

    class NavigationReactiveCommand : ReactiveCommand, INavigateCommand { }

    public static class RoutingStateMixins
    {
        /// <summary>
        /// Locate the first ViewModel in the stack that matches a certain Type.
        /// </summary>
        /// <returns>The matching ViewModel or null if none exists.</returns>
        public static T FindViewModelInStack<T>(this IRoutingState This)
            where T : IRoutableViewModel
        {
            return This.NavigationStack.Reverse().OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the currently visible ViewModel
        /// </summary>
        public static IRoutableViewModel GetCurrentViewModel(this IRoutingState This)
        {
            return This.NavigationStack.LastOrDefault();
        }

        /// <summary>
        /// Creates a ReactiveCommand which will, when invoked, navigate to the 
        /// type specified by the type parameter via looking it up in the
        /// Dependency Resolver.
        /// </summary>
        public static IReactiveCommand NavigateCommandFor<T>(this IRoutingState This)
            where T : IRoutableViewModel
        {
            var ret = new ReactiveCommand(This.Navigate.CanExecuteObservable);
                ret.Select(_ => (IRoutableViewModel)RxApp.DependencyResolver.GetService<T>()).InvokeCommand(This.Navigate);
                return ret;
        }
    }
}
