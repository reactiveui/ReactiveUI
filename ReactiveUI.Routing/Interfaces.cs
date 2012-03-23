using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using ReactiveUI.Xaml;

namespace ReactiveUI.Routing
{
    /// <summary>
    /// Implement this interface for ViewModels that can be navigated to.
    /// </summary>
    public interface IRoutableViewModel : IReactiveNotifyPropertyChanged
    {
        /// <summary>
        /// A string token representing the current ViewModel, such as 'login' or 'user'
        /// </summary>
        string UrlPathSegment { get; }

        /// <summary>
        /// The IScreen that this ViewModel is currently being shown in. This
        /// is usually passed into the ViewModel in the Constructor and saved
        /// as a ReadOnly Property.
        /// </summary>
        IScreen HostScreen { get; }
    }

    public interface IViewForViewModel
    {
        object ViewModel { get; set; }
    }

    /// <summary>
    /// Implement this interface on your Views.
    /// </summary>
    public interface IViewForViewModel<T> : IViewForViewModel
        where T : IReactiveNotifyPropertyChanged
    {
        /// <summary>
        /// The ViewModel corresponding to this specific View.
        /// </summary>
        T ViewModel { get; set; }
    }

    /// <summary>
    /// IScreen represents any object that is hosting its own routing -
    /// usually this object is your AppViewModel or MainWindow object.
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// The Router associated with this Screen.
        /// </summary>
        RoutingState Router { get; }
    }

    /// <summary>
    /// Allows an additional string to make view resolution more specific than just a type.
    /// </summary>
    public class ViewContractAttribute : Attribute
    {
        /// <summary>
        /// A unique string that will be used along with the type to resolve a View
        /// </summary>
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
