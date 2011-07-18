using System;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI.Xaml;

namespace ReactiveUI.Routing
{
    public class ViewModelViewHost : ContentControl
    {
        IDisposable _inner = null;
        
        public IReactiveNotifyPropertyChanged ViewModel {
            get { return (IReactiveNotifyPropertyChanged)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty = 
            DependencyProperty.Register("ViewModel", typeof(IReactiveNotifyPropertyChanged), typeof(ViewModelViewHost), new PropertyMetadata(null));

        public ViewModelViewHost()
        {
            this.ObservableFromDP(x => x.ViewModel)
                .Subscribe(vm => {
                    if (vm.Value == null) {
                        // XXX: Replace with default view
                        Content = null;
                        return;
                    }

                    var view = RxRouting.ResolveView(vm.Value);
                    view.ViewModel = vm.Value;
                    Content = view;
                });
        }
    }
}