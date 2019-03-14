using System;
using System.Collections.Generic;
using System.Windows.Input;
using Cinephile.ViewModels;
using Xamarin.Forms;

namespace Cinephile.Views
{
    public partial class AboutView : ContentPageBase<AboutViewModel>
    {
        public AboutView()
        {
            InitializeComponent();
        }

        void OpenBrowserWithUrl(string url)
        {
            Device.OpenUri(new Uri(url));
        }

    }
}
