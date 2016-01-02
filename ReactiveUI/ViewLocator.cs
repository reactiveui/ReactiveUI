using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using Splat;

namespace ReactiveUI
{
    public static class ViewLocator
    {
        public static IViewLocator Current {
            get {
                var ret = Locator.Current.GetService<IViewLocator>();
                if (ret == null) {
                    throw new Exception("Could not find a default ViewLocator. This should never happen, your dependency resolver is broken");
                }
                return ret;
            }
        }
    }

    class DefaultViewLocator : IViewLocator, IEnableLogger
    {
        public Func<string, string> ViewModelToViewFunc { get; set; }

        public DefaultViewLocator(Func<string, string> viewModelToViewFunc = null)
        {
            ViewModelToViewFunc = viewModelToViewFunc ?? 
                (vm => interfaceifyTypeName(vm.Replace("ViewModel", "View")));
        }

        /// <summary>
        /// Returns the View associated with a ViewModel, deriving the name of
        /// the Type via ViewModelToViewFunc, then discovering it via
        /// ServiceLocator.
        /// </summary>
        /// <param name="viewModel">The ViewModel for which to find the
        /// associated View.</param>
        /// <returns>The View for the ViewModel.</returns>
        public IViewFor ResolveView<T>(T viewModel, string contract = null)
            where T : class
        {
            // Given IFooBarViewModel (whose name we derive from T), we'll look 
            // for a few things:
            // * IFooBarView that implements IViewFor
            // * IViewFor<IFooBarViewModel>
            // * IViewFor<FooBarViewModel> (the original behavior in RxUI 3.1)

            // IFooBarView that implements IViewFor (or custom ViewModelToViewFunc)
            var typeToFind = ViewModelToViewFunc(viewModel.GetType().AssemblyQualifiedName);
                
            var ret = attemptToResolveView(Reflection.ReallyFindType(typeToFind, false), contract);
            if (ret != null) return ret;

            // IViewFor<FooBarViewModel> (the original behavior in RxUI 3.1)
            var viewType = typeof (IViewFor<>);
            return attemptToResolveView(viewType.MakeGenericType(viewModel.GetType()), contract);
        }

        IViewFor attemptToResolveView(Type type, string contract)
        {
            if (type == null) return null;

            var ret = default(IViewFor);

            try {
                ret = (IViewFor)Locator.Current.GetService(type, contract);
            } catch (Exception ex) {
                this.Log().ErrorException("Failed to instantiate view: " + type.FullName, ex);
                throw;
            }

            return ret;
        }

        static string interfaceifyTypeName(string typeName)
        {
            var idxComma = typeName.IndexOf( ',' );
            var idxPeriod = typeName.LastIndexOf( '.', idxComma - 1 );
            return typeName.Insert( idxPeriod+1, "I" );
        }
    }
}

