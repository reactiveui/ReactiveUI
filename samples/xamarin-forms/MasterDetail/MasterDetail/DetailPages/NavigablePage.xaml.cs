using System;
using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms.Xaml;

namespace MasterDetail
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NavigablePage : ReactiveContentPage<NavigableViewModel>
    {
        public NavigablePage()
        {
			InitializeComponent();

            this.WhenActivated(
                disposables =>
                {
                    this
                        .BindCommand(ViewModel, vm => vm.NavigateToDummyPage, v => v.NavigateButton)
                        .DisposeWith(disposables);
                });
        }
    }
}
