using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Windows.Input;
using Splat;

namespace ReactiveUI
{
    public interface IRoutingState
    {
        /// <summary>
        /// Represents the current navigation stack, the last element in the
        /// collection being the currently visible ViewModel.
        /// </summary>
        [IgnoreDataMember]
        ReactiveList<IRoutableViewModel> NavigationStack { get; }

        /// <summary>
        /// Navigates back to the previous element in the stack.
        /// </summary>
        [IgnoreDataMember]
        ReactiveCommand<Unit> NavigateBack { get; }

        /// <summary>
        /// Navigates to the a new element in the stack - the Execute parameter
        /// must be a ViewModel that implements IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        ReactiveCommand<object> Navigate { get; }

        /// <summary>
        /// Navigates to a new element and resets the navigation stack (i.e. the
        /// new ViewModel will now be the only element in the stack) - the
        /// Execute parameter must be a ViewModel that implements
        /// IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        ReactiveCommand<object> NavigateAndReset { get; }

        /// <summary>
        /// Gets the current view model.
        /// </summary>
        [IgnoreDataMember]
        IObservable<IRoutableViewModel> CurrentViewModel { get; }

        /// <summary>
        /// Gets the navigate back view model.
        /// </summary>
        [IgnoreDataMember]
        IObservable<IRoutableViewModel> NavigateBackViewModel { get; }

    }

    public interface IRoutingParams
    {
        bool NotInNavigationStack { get; set; }
        string Contract { get; set; }
    }

    public class RoutingParams : IRoutingParams
    {
        public bool NotInNavigationStack { get; set; }
        public string Contract { get; set; }
    }

    public interface IRoutableViewModelWithParams : IRoutableViewModel
    {
        IRoutingParams RoutingParams { get; }
        IRoutableViewModel RoutableViewModel { get; }
    }

    public class RoutableViewModelWithParams : IRoutableViewModelWithParams
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
        public IObservable<IObservedChange<object, object>> Changing { get; private set; }
        public IObservable<IObservedChange<object, object>> Changed { get; private set; }

        public IDisposable SuppressChangeNotifications()
        {
            throw new NotImplementedException();
        }

        public RoutableViewModelWithParams(IRoutableViewModel viewModel, IRoutingParams routingParams)
        {
            RoutableViewModel = viewModel;
            RoutingParams = routingParams;
        }

        public string UrlPathSegment { get { return RoutableViewModel.UrlPathSegment; } }
        public IScreen HostScreen { get { return RoutableViewModel.HostScreen; } }

        public IRoutingParams RoutingParams { get; private set; }
        public IRoutableViewModel RoutableViewModel { get; private set; }
    }

    /// <summary>
    /// RoutingState manages the ViewModel Stack and allows ViewModels to
    /// navigate to other ViewModels.
    /// </summary>
    [DataContract]
    public class RoutingState : ReactiveObject, IRoutingState
    {
        [DataMember]
        ReactiveList<IRoutableViewModel> _NavigationStack;

        /// <summary>
        /// Represents the current navigation stack, the last element in the
        /// collection being the currently visible ViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveList<IRoutableViewModel> NavigationStack
        {
            get { return _NavigationStack; }
            protected set { _NavigationStack = value; }
        }

        /// <summary>
        /// Navigates back to the previous element in the stack.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<Unit> NavigateBack { get; protected set; }

        /// <summary>
        /// Navigates to the a new element in the stack - the Execute parameter
        /// must be a ViewModel that implements IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<object> Navigate { get; protected set; }

        /// <summary>
        /// Navigates to a new element and resets the navigation stack (i.e. the
        /// new ViewModel will now be the only element in the stack) - the
        /// Execute parameter must be a ViewModel that implements
        /// IRoutableViewModel.
        /// </summary>
        [IgnoreDataMember]
        public ReactiveCommand<object> NavigateAndReset { get; protected set; }

        [IgnoreDataMember]
        public IObservable<IRoutableViewModel> CurrentViewModel { get; protected set; }

        [IgnoreDataMember]
        public IObservable<IRoutableViewModel> NavigateBackViewModel { get; protected set; }

        public RoutingState()
        {
            _NavigationStack = new ReactiveList<IRoutableViewModel>();

            setupRx();
        }

        public virtual void ExcecuteNavigateWithParams(Tuple<IRoutableViewModel, IRoutingParams> routingParams)
        {
            Navigate.Execute(new RoutableViewModelWithParams(routingParams.Item1, routingParams.Item2));
        }

        public virtual void ExcecuteNavigateWithParams(IRoutableViewModel viewModel, IRoutingParams routingParams)
        {
            Navigate.Execute(new RoutableViewModelWithParams(viewModel, routingParams));
        }

        public virtual void ProccessNavigate(IRoutableViewModel viewModel = null, IRoutableViewModelWithParams viewModelWithParams = null)
        {
            if (viewModel == null && viewModelWithParams == null)
            {
                throw new Exception("ProccessNavigate must be with either an IRoutableViewModel or IRoutableViewModelWithParams");
            }
            if (viewModel != null)
            {
                NavigationStack.Add(viewModel);
            }
            else
            {
                //NEIN!!! (ViewModel in Stack und Views kommen via IViewLocator!!) => ViewInitializeParams (object) => ruft Methode auf View auf und die Viewobjekte liegen nicht als Objekt in Stack (Memory!!)
                // => NotInNavigationStack (bool)
                // => TransitionType (Enum)

                if (viewModelWithParams.RoutingParams != null && !viewModelWithParams.RoutingParams.NotInNavigationStack)
                {
                    NavigationStack.Add(viewModelWithParams);
                }
            }
        }

        [OnDeserialized]
        void setupRx(StreamingContext sc) { setupRx(); }

        void setupRx()
        {
            NavigateBackViewModel = new Subject<IRoutableViewModel>();

            NavigateBack = ReactiveCommand.Create(
                NavigationStack.CountChanged.StartWith(_NavigationStack.Count).Select(x => x > 1),
                _ => Observable.Return(Unit.Default));

            NavigateBack.Subscribe(_ =>
            {
                if (NavigationStack.Count > 1)
                {
                    NavigationStack.RemoveAt(NavigationStack.Count - 1);
                    ((ISubject<IRoutableViewModel>)NavigateBackViewModel).OnNext(NavigationStack.LastOrDefault());
                }
            });


            Navigate = new ReactiveCommand<object>(Observable.Return(true), x => Observable.Return(x));
            Navigate.Subscribe(x =>
            {
                var viewModelWithParams = x as IRoutableViewModelWithParams;
                if (viewModelWithParams != null)
                {
                    ProccessNavigate(null, viewModelWithParams);
                }
                else
                {
                    var vm = x as IRoutableViewModel;
                    if (vm != null)
                    {
                        ProccessNavigate(vm);
                    }
                }
            });

            NavigateAndReset = new ReactiveCommand<object>(Observable.Return(true), x => Observable.Return(x));
            NavigateAndReset.Subscribe(x =>
            {
                NavigationStack.Clear();
                Navigate.ExecuteAsync(x);
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

        /// <summary>
        /// Creates a ReactiveCommand which will, when invoked, navigate to the 
        /// type specified by the type parameter via looking it up in the
        /// Dependency Resolver.
        /// </summary>
        public static IReactiveCommand NavigateCommandFor<T>(this RoutingState This)
            where T : IRoutableViewModel
        {
            var ret = new ReactiveCommand<object>(This.Navigate.CanExecuteObservable, x => Observable.Return(x));
            ret.Select(_ => (IRoutableViewModel)Locator.Current.GetService<T>()).InvokeCommand(This.Navigate);

            return ret;
        }

        public static Tuple<T, IRoutingParams> AsRoutableViewModel<T>(this object This)
            where T : class
        {

            if (This != null){
                if (This.GetType() == typeof(Tuple<T, IRoutingParams>)) return This as Tuple<T, IRoutingParams>;
                var vmNparams = This as IRoutableViewModelWithParams;
                if (vmNparams != null){
                    return new Tuple<T, IRoutingParams>(vmNparams.RoutableViewModel as T, vmNparams.RoutingParams);
                }
                else
                {
                    var vm = This as IRoutableViewModel;
                    if (vm != null){
                        return new Tuple<T, IRoutingParams>(vm as T, null);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Navigates the specified router.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="routingParams">The routing parameters.</param>
        public static void Navigate(this IRoutingState router, IRoutableViewModel viewModel, IRoutingParams routingParams)
        {
            if (router != null){
                router.Navigate.Execute(new RoutableViewModelWithParams(viewModel, routingParams));
            }
        }

        /// <summary>
        /// Navigates the specified router.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="notInNavigationStack">if set to <c>true</c> [not in navigation stack].</param>
        public static void Navigate(this IRoutingState router, IRoutableViewModel viewModel, bool notInNavigationStack)
        {
            router.Navigate(viewModel,
                new RoutingParams
                {
                    NotInNavigationStack = notInNavigationStack
                });
        }


    }

}
