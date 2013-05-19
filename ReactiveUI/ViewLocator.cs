using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;

namespace ReactiveUI
{
    public interface IViewLocator : IEnableLogger
    {
        IViewFor ResolveView<T>(T viewModel) where T : class;
    }

    public static class ViewLocator
    {
        public static IViewLocator Current {
            get { return RxApp.DependencyResolver.GetService<IViewLocator>() ?? new DefaultViewLocator(); }
        }
    }

    public class DefaultViewLocator : IViewLocator
    {
        public static Func<string, string> ViewModelToViewFunc { get; set; }

        public DefaultViewLocator()
        {
            ViewModelToViewFunc = (vm) => interfaceifyTypeName(vm.Replace("ViewModel", "View"));
        }

        /// <summary>
        /// Returns the View associated with a ViewModel, deriving the name of
        /// the Type via ViewModelToViewFunc, then discovering it via
        /// ServiceLocator.
        /// </summary>
        /// <param name="viewModel">The ViewModel for which to find the
        /// associated View.</param>
        /// <returns>The View for the ViewModel.</returns>
        public IViewFor ResolveView<T>(T viewModel)
            where T : class
        {
            // Given IFooBarViewModel (whose name we derive from T), we'll look 
            // for a few things:
            // * IFooBarView that implements IViewFor
            // * IViewFor<IFooBarViewModel>
            // * IViewFor<FooBarViewModel> (the original behavior in RxUI 3.1)

            var attrs = viewModel.GetType().GetCustomAttributes(typeof (ViewContractAttribute), true);
            string key = null;

            if (attrs.Any()) {
                key = ((ViewContractAttribute) attrs.First()).Contract;
            }

            // IFooBarView that implements IViewFor (or custom ViewModelToViewFunc)
            var typeToFind = ViewModelToViewFunc(viewModel.GetType().AssemblyQualifiedName);
            try {
                var type = Reflection.ReallyFindType(typeToFind, false);

                if (type != null) {
                    var ret = RxApp.DependencyResolver.GetService(type, key) as IViewFor;
                    if (ret != null) return ret;
                }
            } catch (Exception ex) {
                LogHost.Default.DebugException("Couldn't instantiate " + typeToFind, ex);
            }

            var viewType = typeof (IViewFor<>);

            // IViewFor<FooBarViewModel> (the original behavior in RxUI 3.1)
            return (IViewFor) RxApp.DependencyResolver.GetService(viewType.MakeGenericType(viewModel.GetType()), key);
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

