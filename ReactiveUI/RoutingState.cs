using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using Splat;

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

        public RoutingState()
        {
            _NavigationStack = new ReactiveList<IRoutableViewModel>();
            setupRx();
        }

        [OnDeserialized]
        void setupRx(StreamingContext sc) { setupRx();  }

        void setupRx()
        {
            var countAsBehavior = Observable.Concat(
                Observable.Defer(() => Observable.Return(_NavigationStack.Count)),
                NavigationStack.CountChanged);

            NavigateBack = 
                ReactiveCommand.Create(() => {
                    NavigationStack.RemoveAt(NavigationStack.Count - 1);
                    return Observable.Return(Unit.Default);
                },
                countAsBehavior.Select(x => x > 1));

            Navigate = ReactiveCommand.Create<IRoutableViewModel, IRoutableViewModel>(x => {
                var vm = x as IRoutableViewModel;
                if (vm == null) {
                    throw new Exception("Navigate must be called on an IRoutableViewModel");
                }

                NavigationStack.Add(vm);
                return Observable.Return(x);
            });

            NavigateAndReset = ReactiveCommand.Create<IRoutableViewModel, IRoutableViewModel>(x => {
                NavigationStack.Clear();
                return Navigate.ExecuteAsync(x);
            });
            
            CurrentViewModel = Observable.Concat(
                Observable.Defer(() => Observable.Return(NavigationStack.LastOrDefault())),
                NavigationStack.Changed.Select(_ => NavigationStack.LastOrDefault()));
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

        /////// <summary>
        /////// Creates a ReactiveCommand which will, when invoked, navigate to the 
        /////// type specified by the type parameter via looking it up in the
        /////// Dependency Resolver.
        /////// </summary>
        ////public static NewReactiveCommand<object, IRoutableViewModel> NavigateCommandFor<T>(this RoutingState This)
        ////    where T : IRoutableViewModel, new()
        ////{
        ////    var ret = new ReactiveCommand<object>(This.Navigate.CanExecute, x => Observable.Return(x));
        ////    ret.Select(_ => (IRoutableViewModel)Locator.Current.GetService<T>() ?? new T()).InvokeCommand(This.Navigate);
                
        ////    return ret;
        ////}
    }
}
