using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MobileSample_WinRT.ViewModels;
using ReactiveUI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MobileSample_WinRT.Views
{
    public sealed partial class TestPage1View : Page, IViewFor<TestPage1ViewModel>
    {
        public TestPage1View()
        {
            this.InitializeComponent();
            this.OneWayBind(ViewModel, x => x.RandomGuid, x => x.RandomGuid.Text);
        }

        public TestPage1ViewModel ViewModel {
            get { return (TestPage1ViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TestPage1ViewModel), typeof(TestPage1View), new PropertyMetadata(null));

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (TestPage1ViewModel)value; }
        }
    }
}
