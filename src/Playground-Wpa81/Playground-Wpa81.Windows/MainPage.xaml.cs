using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ReactiveUI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Playground_Wpa81
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IViewFor<MainPageViewModel>
    {
        public MainPage()
        {
            this.InitializeComponent();
            RxApp.SuspensionHost.ObserveAppState<MainPageViewModel>()
                .BindTo(this, x => x.ViewModel);

            this.BindCommand(ViewModel, x => x.DoIt, x => x.doIt);

            int count = 0;
            this.WhenAnyObservable(x => x.ViewModel.DoIt).Subscribe(_ => {
                count++;
                result.Text = String.Format("You clicked {0} times!", count);
            });

            this.OneWayBind(ViewModel, x => x.SavedGuid, x => x.SavedGuid.Text);

            this.BindCommand(ViewModel, x => x.ThreadedDoIt, x => x.threadedDoIt);
            this.OneWayBind(ViewModel, x => x.ThreadedResult, x => x.threadedResult.Text);
        }

        public MainPageViewModel ViewModel {
            get { return (MainPageViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainPageViewModel), typeof(MainPage), new PropertyMetadata(null));

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (MainPageViewModel)ViewModel; }
        }
    }
}
