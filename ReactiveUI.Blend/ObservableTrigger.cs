using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows;

namespace ReactiveUI.Blend
{
    public class ObservableTrigger : TriggerBase<FrameworkElement>
    {
        public IObservable<object> Observable {
            get { return (IObservable<object>)GetValue(ObservableProperty); }
            set { SetValue(ObservableProperty, value); }
        }
        public static readonly DependencyProperty ObservableProperty =
            DependencyProperty.Register("Observable", typeof(IObservable<object>), typeof(ObservableTrigger), new PropertyMetadata(onObservableChanged));

        public bool AutoResubscribeOnError { get; set; }

        IDisposable watcher;
        protected static void onObservableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ObservableTrigger This = (ObservableTrigger)sender;
            if (This.watcher != null) {
                This.watcher.Dispose();
                This.watcher = null;
            }

            This.watcher = ((IObservable<object>)e.NewValue).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
                x => This.InvokeActions(x), 
                ex => {
                    if (!This.AutoResubscribeOnError)
                        return;
                    onObservableChanged(This, e);
                });
        }
    }
}
