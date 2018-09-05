using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace MasterDetail
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            var cellVms = GetData().Select(model => new CustomCellViewModel(model));
            MyList = new ObservableCollection<CustomCellViewModel>(cellVms);

            Detail = new MyDetailViewModel();
            Detail.Model = cellVms.First().Model;

            this.WhenAnyValue(x => x.Selected)
                .Where(x => x != null)
                .Subscribe(cellVm => Detail.Model = cellVm.Model);
        }

        private CustomCellViewModel _selected;
        public CustomCellViewModel Selected
        {
            get => _selected;
            set => this.RaiseAndSetIfChanged(ref _selected, value);
        }

        public MyDetailViewModel Detail { get; }

        public ObservableCollection<CustomCellViewModel> MyList { get; }

        private IEnumerable<MyModel> GetData()
        {
            return new[]
            {
                new MyModel { Title = "Contacts", IconSource = "contacts.png" },
                new MyModel { Title = "Reminders", IconSource = "reminders.png" },
                new MyModel { Title = "TodoList", IconSource = "todo.png" },
            };
        }
    }
}
