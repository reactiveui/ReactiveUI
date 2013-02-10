using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MobileSample_WP8.ViewModels;
using ReactiveUI;

namespace MobileSample_WP8.Views
{
    public partial class TestPage1View : UserControl, IViewFor<TestPage1ViewModel>
    {
        public TestPage1View()
        {
            InitializeComponent();
        }

        public TestPage1ViewModel ViewModel {
            get { return (TestPage1ViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TestPage1ViewModel), typeof(TestPage1View), new PropertyMetadata(null));

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (TestPage1ViewModel) value; }
        }
    }
}
