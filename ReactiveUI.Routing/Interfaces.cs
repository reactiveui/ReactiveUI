using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using ReactiveUI.Xaml;

namespace ReactiveUI.Routing
{
    public interface IRoutableViewModel : IReactiveNotifyPropertyChanged
    {
        string FriendlyUrlName { get; }
        IScreen HostScreen { get; }
    }

    public interface IViewForViewModel
    {
        object ViewModel { get; set; }
    }

    public interface IViewForViewModel<T> : IViewForViewModel
        where T : IReactiveNotifyPropertyChanged
    {
        T ViewModel { get; set; }
    }

    public interface IScreen
    {
        RoutingState Router;
    }

    public class ViewContractAttribute : Attribute
    {
        public string Contract { get; set; }
    }

    public static class ObservableUtils
    {
        public static IConnectableObservable<T> PermaRef<T>(this IConnectableObservable<T> This)
        {
            This.Connect();
            return This;
        }
    }
}
