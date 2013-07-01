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
using System.Reactive.Linq;

namespace MobileSample_WinRT
{
    sealed partial class App : AutoSuspendApplication
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            ((ModernDependencyResolver)RxApp.DependencyResolver).Register(() => new AppBootstrapper(), typeof(IApplicationRootState));

            base.OnLaunched(args);
            var host = RxApp.DependencyResolver.GetService<ISuspensionHost>();
            host.SetupDefaultSuspendResume();
        }
    }

    public static class ButtonMixin
    {
        public static ButtonEvents Events(this Button This)
        {
            return new ButtonEvents(This);
        }
    }

    public class ButtonEvents
    {
        Button This;

        public ButtonEvents(Button This)
        {
            this.This = This;
        }

        public IObservable<RoutedEventArgs> Click {
            get { return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(x => This.Click += x, x => This.Click -= x).Select(x => x.EventArgs); }
        }
    }
}
