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
    public sealed partial class TestPage2View : Page, IViewFor<TestPage2ViewModel>
    {
        public TestPage2View()
        {
            this.InitializeComponent();
            this.OneWayBind(ViewModel, x => x.RandomGuid, x => x.RandomGuid.Text);
        }

        public TestPage2ViewModel ViewModel {
            get { return (TestPage2ViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TestPage2ViewModel), typeof(TestPage2View), new PropertyMetadata(null));

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (TestPage2ViewModel)value; }
        }
    }
}
