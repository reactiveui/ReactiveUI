using System;
using ReactiveUI;
using Splat;
using Xamarin.Forms;
using ReactiveUI.XamForms;

namespace PlaygroundXamForms
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public RoutingState Router { get; protected set; }

        public AppBootstrapper()
        {
            Router = new RoutingState();

            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
            Locator.CurrentMutable.Register(() => new TestView(), typeof(IViewFor<TestViewModel>));
            Locator.CurrentMutable.Register(() => new DifferentView(), typeof(IViewFor<DifferentViewModel>));

            Router.Navigate.Execute(new TestViewModel(this));
           // Router.NavigationStack.Add(new TestViewModel(this));
        }

        public Page CreateMainView()
        {
            return new RoutedViewHost();
        }
    }
}