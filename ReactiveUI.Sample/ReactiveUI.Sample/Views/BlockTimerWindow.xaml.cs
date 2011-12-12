using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ReactiveUI.Sample.ViewModels;
using ReactiveUI.Sample.Models;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace ReactiveUI.Sample.Views
{
	/// <summary>
	/// Interaction logic for BlockTimerWindow.xaml
	/// </summary>
	public partial class BlockTimerWindow : Window
	{
		public BlockTimerWindow(BlockItem Model)
		{
			this.InitializeComponent();

            ViewModel = new BlockTimerViewModel(Model);

		    ViewModel.Cancel.Subscribe(_ => this.Close());

            // N.B: This is a bit of a hack - the completion of this observable
            // happens regardless of the value; I tried to use OnError for this
            // instead but it actually ended up throwing the exception. 
            ViewModel.TimerState
                .Where(x => x == BlockTimerViewState.ShouldCancel)
                .Subscribe(
                    _ => Dispatcher.BeginInvoke(new Action(() => Close())), 
                    () => Dispatcher.BeginInvoke(new Action(() => Close())));

		    Observable.Merge(
                    Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>(x => SizeChanged += x, x => SizeChanged -= x).Select(_ => new Unit()),
    		        ViewModel.WhenAny(x => x.ProgressPercentage, _ => new Unit()))
		        .Select(_ => progressParentBorder.ActualWidth * ViewModel.ProgressPercentage)
		        .Subscribe(x => progressBorder.Width = x);

		}

        public BlockTimerWindow() : this(new BlockItem() { Description = "Test Item" })
        {

        }

	    public BlockTimerViewModel ViewModel {
            get { return (BlockTimerViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(BlockTimerViewModel), typeof(BlockTimerWindow));
	}
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :