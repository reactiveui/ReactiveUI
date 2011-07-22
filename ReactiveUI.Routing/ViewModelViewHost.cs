using System;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI.Xaml;

namespace ReactiveUI.Routing
{
    public class ViewModelViewHost : ContentControl
    {
        public IReactiveNotifyPropertyChanged ViewModel {
            get { return (IReactiveNotifyPropertyChanged)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty = 
            DependencyProperty.Register("ViewModel", typeof(IReactiveNotifyPropertyChanged), typeof(ViewModelViewHost), new PropertyMetadata(null));

        public object DefaultContent {
            get { return (object)GetValue(DefaultContentProperty); }
            set { SetValue(DefaultContentProperty, value); }
        }
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register("DefaultContent", typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

        public ViewModelViewHost()
        {
            this.ObservableFromDP(x => x.ViewModel)
                .Subscribe(vm => {
                    if (vm.Value == null) {
                        Content = DefaultContent;
                        return;
                    }

                    var view = RxRouting.ResolveView(vm.Value);
                    view.ViewModel = vm.Value;
                    Content = view;
                });
        }
    }
}