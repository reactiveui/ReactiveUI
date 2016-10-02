using System;
using ReactiveUI;
using Splat;

namespace ReactiveUI
{
    public static class ViewLocator
    {
        public static IViewLocator Current
        {
            get
            {
                var ret = Locator.Current.GetService<IViewLocator>();
                if (ret == null) {
                    throw new Exception("Could not find a default ViewLocator. This should never happen, your dependency resolver is broken");
                }
                return ret;
            }
        }
    }

    internal class DefaultViewLocator : IViewLocator, IEnableLogger
    {
        public DefaultViewLocator(Func<string, string> viewModelToViewFunc = null)
        {
            ViewModelToViewFunc = viewModelToViewFunc ??
                (vm => interfaceifyTypeName(vm.Replace("ViewModel", "View")));
        }

        public Func<string, string> ViewModelToViewFunc { get; set; }

        /// <summary>
        /// Returns the View associated with a ViewModel, deriving the name of the Type via
        /// ViewModelToViewFunc, then discovering it via ServiceLocator.
        /// </summary>
        /// <param name="viewModel">The ViewModel for which to find the associated View.</param>
        /// <returns>The View for the ViewModel.</returns>
        public IViewFor ResolveView<T>(T viewModel, string contract = null)
            where T : class
        {
            // Given IFooBarViewModel (whose name we derive from T), we'll look for a few things:
            // * IFooBarView that implements IViewFor
            // * IViewFor<IFooBarViewModel>
            // * IViewFor<FooBarViewModel> (the original behavior in RxUI 3.1)

            // IFooBarView that implements IViewFor (or custom ViewModelToViewFunc)
            var typeToFind = ViewModelToViewFunc(viewModel.GetType().AssemblyQualifiedName);

            var ret = attemptToResolveView(Reflection.ReallyFindType(typeToFind, false), contract);
            if (ret != null) return ret;

            // IViewFor<FooBarViewModel> (the original behavior in RxUI 3.1)
            var viewType = typeof(IViewFor<>);
            return attemptToResolveView(viewType.MakeGenericType(viewModel.GetType()), contract);
        }

        private static string interfaceifyTypeName(string typeName)
        {
            var idxComma = typeName.IndexOf(',');
            var idxPeriod = typeName.LastIndexOf('.', idxComma - 1);
            return typeName.Insert(idxPeriod + 1, "I");
        }

        private static string interfaceifyTypeName(Type type)
        {
            var typeName = type.AssemblyQualifiedName;
            var idxComma = typeName.IndexOf(',');
            int idxPeriod;
            if (idxComma >= 1) {
                idxPeriod = typeName.LastIndexOf('.', idxComma - 1);
            } else {
                idxPeriod = typeName.LastIndexOf('.');
            }

            return typeName.Insert(idxPeriod + 1, "I");
        }

        private IViewFor attemptToResolveView(Type type, string contract)
        {
            if (type == null) return null;

            object ret;

            try {
                ret = Locator.Current.GetService(type, contract) as IViewFor;
                if (ret == null) {

                    // Try to get the Type of the Interface For the IViewFor Class
                    var it = Type.GetType(interfaceifyTypeName(type));
                    var interfaceType = typeof(IViewFor<>).MakeGenericType(new Type[] { it });
                    ret = Locator.Current.GetService(interfaceType, contract) as IViewFor;
                    if (ret == null) {
                        ret = default(IViewFor);
                    }
                }
                return (IViewFor)ret;
            } catch (Exception ex) {
                this.Log().ErrorException("Failed to instantiate view: " + type.FullName, ex);
                throw;
            }
        }
    }
}