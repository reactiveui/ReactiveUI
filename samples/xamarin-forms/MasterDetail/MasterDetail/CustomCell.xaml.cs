using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace MasterDetail
{
    public partial class CustomCell : ReactiveViewCell<CustomCellViewModel>
    {
        public CustomCell()
        {
            InitializeComponent();

            this.WhenActivated(
                disposables =>
                {
                    this
                        .OneWayBind(ViewModel, vm => vm.Title, v => v.TitleLabel.Text)
                        .DisposeWith(disposables);
                    this
                        .OneWayBind(ViewModel, vm => vm.IconSource, v => v.IconImage.Source, x => ImageSource.FromFile(x))
                        .DisposeWith(disposables);
                });
        }
    }
}
