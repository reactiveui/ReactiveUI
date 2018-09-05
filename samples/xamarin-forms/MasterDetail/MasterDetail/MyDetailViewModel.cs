using ReactiveUI;

namespace MasterDetail
{
    public class MyDetailViewModel : ReactiveObject
    {
        private MyModel _model;

        public MyModel Model
        {
            get => _model;
            set => this.RaiseAndSetIfChanged(ref _model, value);
        }

        public string Title => Model.Title;
    }
}
