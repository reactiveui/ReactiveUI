using System;
using ReactiveUI;
using Splat;
using Xamarin.Forms;
using ReactiveUI.XamForms;

namespace PlaygroundXamForms
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        // The Router holds the ViewModels for the back stack. Because it's
        // in this object, it will be serialized automatically.
        public RoutingState Router { get; protected set; }

        public AppBootstrapper()
        {
            Router = new RoutingState();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));

            Locator.CurrentMutable.Register(() => new MainPage(), typeof(IViewFor<MainPageViewModel>));
            Locator.CurrentMutable.Register(() => new DemoListViewView(), typeof(IViewFor<DemoListViewViewModel>));
            Locator.CurrentMutable.Register(() => new ListViewItemView(), typeof(IViewFor<DogsItemViewModel>));

            Router.Navigate.Execute(new MainPageViewModel());
       
        }

        public Page CreateMainPage()
        {
            return new RoutedViewHost();
        }
    }
}