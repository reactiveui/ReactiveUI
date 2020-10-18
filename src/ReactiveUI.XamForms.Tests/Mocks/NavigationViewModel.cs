using System;
using System.Reactive;
using Splat;

namespace ReactiveUI.XamForms.Tests.Mocks
{
    public class NavigationViewModel : ReactiveObject, IScreen
    {
        public RoutingState Router { get; } = new RoutingState();

        public IObservable<IRoutableViewModel> Navigate(string name)
        {
            var viewModel = Locator.Current.GetService<IRoutableViewModel>(name);
            return Router.Navigate.Execute(viewModel);
        }

        public IObservable<IRoutableViewModel> NavigateToChild(string value)
        {
            var viewModel = new ChildViewModel(value);
            return Router.Navigate.Execute(viewModel);
        }

        public IObservable<IRoutableViewModel> NavigateAndResetToChild(string value)
        {
            var viewModel = new ChildViewModel(value);
            return Router.NavigateAndReset.Execute(viewModel);
        }

        public IObservable<Unit> NavigateBack()
        {
            return Router.NavigateBack.Execute();
        }
    }
}
