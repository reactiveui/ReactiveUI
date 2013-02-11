using MobileSample_WinRT.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MobileSample_WinRT.ViewModels;
using ReactiveUI;
using ReactiveUI.Mobile;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MobileSample_WinRT
{
    sealed partial class App : AutoSuspendApplication
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            RxApp.Register(typeof(AppBootstrapper), typeof(IApplicationRootState));

            var host = RxApp.GetService<ISuspensionHost>();
            host.SetupDefaultSuspendResume();
        }
    }
}
