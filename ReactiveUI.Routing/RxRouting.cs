using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

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

        public static IViewForViewModel ResolveView<T>(T viewModel)
            where T : IReactiveNotifyPropertyChanged
        {
            string view = ViewModelToViewFunc(viewModel.GetType().AssemblyQualifiedName);
            var type = Type.GetType(view, true);
            var attrs = type.GetCustomAttributes(typeof (ViewContractAttribute), true);
            string key = null;

            if (attrs.Length > 0) {
                key = ((ViewContractAttribute) attrs[0]).Contract;
            }

            return (IViewForViewModel)ServiceLocator.Current.GetInstance(type, key);
        }
    }
}