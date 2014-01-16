using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

#if !WINRT
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace ReactiveUI.Blend
{
#if SILVERLIGHT || WINRT
    public class FollowObservableStateBehavior : Behavior<Control>
#else
    public class FollowObservableStateBehavior : Behavior<FrameworkElement>
#endif
    {
        public IObservable<string> StateObservable {
            get { return (IObservable<string>)GetValue(StateObservableProperty); }
            set { SetValue(StateObservableProperty, value); }
        }
        public static readonly DependencyProperty StateObservableProperty =
            DependencyProperty.Register("StateObservable", typeof(IObservable<string>), typeof(FollowObservableStateBehavior), new PropertyMetadata(null, onStateObservableChanged));

#if SILVERLIGHT || WINRT
        public Control TargetObject {
            get { return (Control)GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register("TargetObject", typeof(Control), typeof(FollowObservableStateBehavior), new PropertyMetadata(null));
#else
        public FrameworkElement TargetObject {
            get { return (FrameworkElement)GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register("TargetObject", typeof(FrameworkElement), typeof(FollowObservableStateBehavior), new PropertyMetadata(null));
#endif

        public bool AutoResubscribeOnError { get; set; }

        IDisposable watcher;

        protected override void OnDetaching()
        {
            if (watcher != null) {
                watcher.Dispose();
                watcher = null;
            }
            base.OnDetaching();
        }

        protected static void onStateObservableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var This = (FollowObservableStateBehavior)sender;
            if (This.watcher != null) {
                This.watcher.Dispose();
                This.watcher = null;
            }

            This.watcher = ((IObservable<string>)e.NewValue).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
                x => {
                    var target = This.TargetObject ?? This.AssociatedObject;
#if SILVERLIGHT || WINRT
                    VisualStateManager.GoToState(target, x, true);
#else
                    if (target is Control) {
                        VisualStateManager.GoToState(target, x, true);
                    } else {
                        VisualStateManager.GoToElementState(target, x, true);
                    }
#endif
                },
                ex => {
                    if (!This.AutoResubscribeOnError)
                        return;
                    onStateObservableChanged(This, e);
                });
        }
    }
}