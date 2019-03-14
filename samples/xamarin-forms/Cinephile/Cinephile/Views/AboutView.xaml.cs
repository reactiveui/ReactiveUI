using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows.Input;
using Cinephile.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace Cinephile.Views
{
    public partial class AboutView : ContentPageBase<AboutViewModel>
    {
        public AboutView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, x => x.ShowIconCredits, x => x.OpenBrowser.Command).DisposeWith(disposables);
            });
        }

        void OpenBrowserWithUrl(string url)
        {
            Device.OpenUri(new Uri(url));
        }

    }
}
