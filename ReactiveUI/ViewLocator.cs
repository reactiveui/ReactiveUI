using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;

namespace ReactiveUI
{
    public static class ViewLocator
    {
        public static IViewLocator Current {
            get {
                var ret = RxApp.DependencyResolver.GetService<IViewLocator>();
                if (ret == null) {
                    throw new Exception("Could not find a default ViewLocator. This should never happen, your dependency resolver is broken");
                }
                return ret;
            }
        }
    }

    class DefaultViewLocator : IViewLocator
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

            var attrs = viewModel.GetType().GetCustomAttributes(typeof (ViewContractAttribute), true);

            if (attrs.Any()) {
                contract = contract ?? ((ViewContractAttribute) attrs.First()).Contract;
            }

            // IFooBarView that implements IViewFor (or custom ViewModelToViewFunc)
            var typeToFind = ViewModelToViewFunc(viewModel.GetType().AssemblyQualifiedName);
            try {
                var type = Reflection.ReallyFindType(typeToFind, false);

                if (type != null) {
                    var ret = RxApp.DependencyResolver.GetService(type, contract) as IViewFor;
                    if (ret != null) return ret;
                }
            } catch (Exception ex) {
                LogHost.Default.DebugException("Couldn't instantiate " + typeToFind, ex);
            }

            var viewType = typeof (IViewFor<>);

            // IViewFor<FooBarViewModel> (the original behavior in RxUI 3.1)
            return (IViewFor) RxApp.DependencyResolver.GetService(viewType.MakeGenericType(viewModel.GetType()), contract);
        }

        static string interfaceifyTypeName(string typeName)
        {
            var typeVsAssembly = typeName.Split(',');
            var parts = typeVsAssembly[0].Split('.');
            parts[parts.Length - 1] = "I" + parts[parts.Length - 1];

            var newType = String.Join(".", parts, 0, parts.Length);
            return newType + "," + String.Join(",", typeVsAssembly.Skip(1));
        }
    }
}

