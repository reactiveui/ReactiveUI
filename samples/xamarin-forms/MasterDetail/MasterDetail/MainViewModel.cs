using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace MasterDetail
{
    public class MainViewModel : ReactiveObject, IScreen
    {
        public MainViewModel()
        {
            Router = new RoutingState();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));

            MenuItems = GetMenuItems();

            NavigateToMenuItem = ReactiveCommand.CreateFromObservable<IRoutableViewModel, Unit>(
                routableVm => Router.NavigateAndReset.Execute(routableVm).Select(_ => Unit.Default));

            this.WhenAnyValue(x => x.Selected)
                .Where(x => x != null)
                .StartWith(MenuItems.First())
                .Select(x => Locator.Current.GetService<IRoutableViewModel>(x.TargetType.FullName))
                .InvokeCommand(NavigateToMenuItem);
        }

        private MasterCellViewModel _selected;
        public MasterCellViewModel Selected
        {
            get => _selected;
            set => this.RaiseAndSetIfChanged(ref _selected, value);
        }

        public ReactiveCommand<IRoutableViewModel, Unit> NavigateToMenuItem { get; }

        public IEnumerable<MasterCellViewModel> MenuItems { get; }

        public RoutingState Router { get; }

        private IEnumerable<MasterCellViewModel> GetMenuItems()
        {
            return new[]
            {
                new MasterCellViewModel { Title = "Navigable Page", IconSource = "contacts.png", TargetType = typeof(NavigableViewModel) },
                new MasterCellViewModel { Title = "Number Stream Page", IconSource = "reminders.png", TargetType = typeof(NumberStreamViewModel) },
                new MasterCellViewModel { Title = "Letter Stream Page", IconSource = "todo.png", TargetType = typeof(LetterStreamViewModel) },
            };
        }
    }
}
