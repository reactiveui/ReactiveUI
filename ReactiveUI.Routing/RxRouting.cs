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
            // Given IFooBarViewModel (whose name we derive from T), we'll look 
            // for a few things:
            // * IFooBarView that implements IViewForViewModel
            // * IViewForViewModel<IFooBarViewModel>
            // * IViewForViewModel<FooBarViewModel> (the original behavior in RxUI 3.1)

            var attrs = viewModel.GetType().GetCustomAttributes(typeof (ViewContractAttribute), true);
            string key = null;

            if (attrs.Any()) {
                key = ((ViewContractAttribute) attrs.First()).Contract;
            }

            // IFooBarView that implements IViewForViewModel
            var typeToFind = interfaceifyTypeName(ViewModelToViewFunc(typeof(T).FullName));
            try {
                var type = RxApp.reallyFindType(typeToFind, true);

                var ret = RxApp.GetService(type, key) as IViewForViewModel;
                if (ret != null) return ret;
            } catch (TypeLoadException ex) {
                LogHost.Default.DebugException("Couldn't instantiate " + typeToFind, ex);
            }

            var viewType = typeof (IViewForViewModel<>);

            // IViewForViewModel<IFooBarViewModel>
            try {
                var type = RxApp.reallyFindType(interfaceifyTypeName(typeof(T).FullName), true);
                var ret =  RxApp.GetService(viewType.MakeGenericType(type), key) as IViewForViewModel;
                if (ret != null) return ret;
            } catch (TypeLoadException ex) {
                LogHost.Default.DebugException("Couldn't instantiate View via pure interface type", ex);
            }

            // IViewForViewModel<FooBarViewModel> (the original behavior in RxUI 3.1)
            return (IViewForViewModel) RxApp.GetService(viewType.MakeGenericType(viewModel.GetType()), key);
        }

        static string interfaceifyTypeName(string typeName)
        {
            var parts = typeName.Split('.');
            parts[parts.Length - 1] = "I" + parts[parts.Length - 1];
            return String.Join(".", parts, 0, parts.Length);
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
