using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MobileSample_WinRT.Views
{
    public sealed partial class StringTileView : UserControl, IViewFor<StringTileViewModel>
    {
        public StringTileView()
        {
            this.InitializeComponent();

            this.WhenActivated(d => {
                d(this.OneWayBind(ViewModel, x => x.Model, x => x.Model.Text));
            });
        }

        public StringTileViewModel ViewModel {
            get { return (StringTileViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(StringTileViewModel), typeof(StringTileView), new PropertyMetadata(null));

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (StringTileViewModel)value; }
        }
    }
}