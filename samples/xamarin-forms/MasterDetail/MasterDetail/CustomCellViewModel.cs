using ReactiveUI;

namespace MasterDetail
{
    public class CustomCellViewModel : ReactiveObject
    {
        public CustomCellViewModel(MyModel model)
        {
            Model = model;
        }

        public MyModel Model { get; }

        public string Title => Model.Title;

        public string IconSource => Model.IconSource;
    }
}
