using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms.Xaml;

namespace MasterDetail
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyDetailPage : ReactiveContentPage<MyDetailViewModel>
    {
        public MyDetailPage(MyDetailViewModel viewModel)
        {
			InitializeComponent();

            ViewModel = viewModel;

            this.WhenActivated(
                disposables =>
                {
                    this
                        .WhenAnyValue(x => x.ViewModel.Model)
                        .Where(x => x != null)
                        .Subscribe(model => PopulateFromModel(model))
                        .DisposeWith(disposables);
                });
        }

        private void PopulateFromModel(MyModel model)
        {
            Title = model.Title;
            TitleLabel.Text = model.Title;
        }
    }
}
