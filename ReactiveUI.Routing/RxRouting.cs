using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;

namespace ReactiveUI.Routing
{
    public static class RxRouting
    {
        public static Func<string, string> ViewModelToViewFunc { get; set; }

        static RxRouting()
        {
            ViewModelToViewFunc = (vm) => 
                vm.Replace("ViewModel", "View");
        }

        /// <summary>
        /// Returns the View associated with a ViewModel, deriving the name of
        /// the Type via ViewModelToViewFunc, then discovering it via
        /// ServiceLocator.
        /// </summary>
        /// <param name="viewModel">The ViewModel for which to find the
        /// associated View.</param>
        /// <returns>The View for the ViewModel.</returns>
        public static IViewForViewModel ResolveView<T>(T viewModel)
            where T : IReactiveNotifyPropertyChanged
        {
            var attrs = viewModel.GetType().GetCustomAttributes(typeof (ViewContractAttribute), true);
            string key = null;

            if (attrs.Count() > 0) {
                key = ((ViewContractAttribute) attrs.First()).Contract;
            }

            var viewType = typeof (IViewForViewModel<>);
            return (IViewForViewModel) RxApp.GetService(viewType.MakeGenericType(viewModel.GetType()), key);
        }
    }

    public static class RoutableViewModelMixin
    {
        /// <summary>
        /// This Observable fires whenever the current ViewModel is navigated to.
        /// Note that this method is difficult to use directly without leaking
        /// memory, you most likely want to use WhenNavigatedTo.
        /// </summary>
        public static IObservable<Unit> NavigatedToMe(this IRoutableViewModel This)
        {
            return This.HostScreen.Router.ViewModelObservable()
                .Where(x => x == This)
                .Select(_ => Unit.Default);
        }

        /// <summary>
        /// This Observable fires whenever the current ViewModel is navigated
        /// away from.  Note that this method is difficult to use directly
        /// without leaking memory, you most likely want to use WhenNavigatedTo.
        /// </summary>
        public static IObservable<Unit> NavigatedFromMe(this IRoutableViewModel This)
        {
            return This.HostScreen.Router.ViewModelObservable()
                .Where(x => x != This)
                .Select(_ => Unit.Default);
        }

        /// <summary>
        /// This method allows you to set up connections that only operate
        /// while the ViewModel has focus, and cleans up when the ViewModel
        /// loses focus.
        /// </summary>
        /// <param name="onNavigatedTo">Called when the ViewModel is navigated
        /// to - return an IDisposable that cleans up all of the things that are
        /// configured in the method.</param>
        /// <returns>An IDisposable that lets you disconnect the entire process
        /// earlier than normal.</returns>
        public static IDisposable WhenNavigatedTo(this IRoutableViewModel This, Func<IDisposable> onNavigatedTo)
        {
            IDisposable inner = null;

            var router = This.HostScreen.Router;
            return router.NavigationStack.CollectionCountChanged.Subscribe(_ => {
                if (router.GetCurrentViewModel() == This) {
                    if (inner != null)  inner.Dispose();
                    inner = onNavigatedTo();
                } else {
                    if (inner != null) {
                        inner.Dispose();
                        inner = null;
                    }
                }
            });
        }
    }
}
