using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI.Xaml;

namespace ReactiveUI.Routing
{
    /// <summary>
    /// This content control will automatically load the View associated with
    /// the ViewModel property and display it. This control is very useful
    /// inside a DataTemplate to display the View associated with a ViewModel.
    /// </summary>
    public class ViewModelViewHost : ContentControl
    {
        /// <summary>
        /// The ViewModel to display
        /// </summary>
        public IReactiveNotifyPropertyChanged ViewModel {
            get { return (IReactiveNotifyPropertyChanged)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty = 
            DependencyProperty.Register("ViewModel", typeof(IReactiveNotifyPropertyChanged), typeof(ViewModelViewHost), new PropertyMetadata(null));

        /// <summary>
        /// If no ViewModel is displayed, this content (i.e. a control) will be displayed.
        /// </summary>
        public object DefaultContent {
            get { return (object)GetValue(DefaultContentProperty); }
            set { SetValue(DefaultContentProperty, value); }
        }
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register("DefaultContent", typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

        public ViewModelViewHost()
        {
            var latestViewModel = Observable.CombineLatest(
                this.ObservableFromDP(x => x.ViewModel).Select(x => x.Value).StartWith((IReactiveNotifyPropertyChanged)null),
                this.ObservableFromDP(x => x.DataContext).Select(x => x.Value).OfType<IReactiveNotifyPropertyChanged>().StartWith((IReactiveNotifyPropertyChanged)null),
                (vm, dc) => vm ?? dc);

            latestViewModel.Subscribe(vm => {
                if (vm == null) {
                    Content = DefaultContent;
                    return;
                }

                var view = RxRouting.ResolveView(vm);
                view.ViewModel = vm;
                Content = view;
            });
        }
    }
}