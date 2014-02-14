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
    public sealed partial class TestPage3View : Page, IViewFor<TestPage3ViewModel>
    {
        public TestPage3View()
        {
            this.InitializeComponent();
            this.OneWayBind(ViewModel, x => x.RandomGuid, x => x.RandomGuid.Text);
        }

        public TestPage3ViewModel ViewModel {
            get { return (TestPage3ViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TestPage3ViewModel), typeof(TestPage3View), new PropertyMetadata(null));

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (TestPage3ViewModel)value; }
        }
    }
}
