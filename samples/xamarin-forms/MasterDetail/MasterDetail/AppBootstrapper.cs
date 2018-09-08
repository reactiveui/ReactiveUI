using System;
using ReactiveUI;
using Splat;

namespace MasterDetail
{
    public class AppBootstrapper
    {
        public AppBootstrapper()
        {
            RegisterViews();
            RegisterViewModels();
        }

        private void RegisterViews()
        {
            Locator.CurrentMutable.Register(() => new DummyPage(), typeof(IViewFor<DummyViewModel>));
            Locator.CurrentMutable.Register(() => new MasterCell(), typeof(IViewFor<MasterCellViewModel>));

            // Detail pages
            Locator.CurrentMutable.Register(() => new NavigablePage(), typeof(IViewFor<NavigableViewModel>));
            Locator.CurrentMutable.Register(() => new NumberStreamPage(), typeof(IViewFor<NumberStreamViewModel>));
            Locator.CurrentMutable.Register(() => new LetterStreamPage(), typeof(IViewFor<LetterStreamViewModel>));
        }

        public MainViewModel CreateMainViewModel()
        {
            // In a typical routing example the IScreen implementation would be this bootstrapper class.
            // However, a MasterDetailPage is designed to at the root. So, we assign the master-detail
            // view model to play the part of IScreen, instead.
            var viewModel = new MainViewModel();

            return viewModel;
        }

        private void RegisterViewModels()
        {
            // Here, we use contracts to distinguish which routable view model we want to instantiate.
            // This helps us avoid a manual cast to IRoutableViewModel when calling Router.Navigate.Execute(...)
            Locator.CurrentMutable.Register(() => new NavigableViewModel(), typeof(IRoutableViewModel), typeof(NavigableViewModel).FullName);
            Locator.CurrentMutable.Register(() => new NumberStreamViewModel(), typeof(IRoutableViewModel), typeof(NumberStreamViewModel).FullName);
            Locator.CurrentMutable.Register(() => new LetterStreamViewModel(), typeof(IRoutableViewModel), typeof(LetterStreamViewModel).FullName);
        }
    }
}
