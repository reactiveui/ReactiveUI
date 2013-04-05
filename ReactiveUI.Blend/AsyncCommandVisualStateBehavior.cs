using System;
using System.Collections.Generic;
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
using System.Windows.Interactivity;
using System.Linq;

namespace ReactiveUI.Blend
{
#if SILVERLIGHT
    public class AsyncCommandVisualStateBehavior : Behavior<Control>
#else
    public class AsyncCommandVisualStateBehavior : Behavior<FrameworkElement>
#endif
    {
        public IReactiveAsyncCommand AsyncCommand {
            get { return (IReactiveAsyncCommand)GetValue(AsyncCommandProperty); }
            set { SetValue(AsyncCommandProperty, value); }
        }
        public static readonly DependencyProperty AsyncCommandProperty =
            DependencyProperty.Register("AsyncCommand", typeof(IReactiveAsyncCommand), typeof(AsyncCommandVisualStateBehavior), new PropertyMetadata(onAsyncCommandChanged));

#if SILVERLIGHT
        public Control TargetObject {
            get { return (Control)GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register("TargetObject", typeof(Control), typeof(AsyncCommandVisualStateBehavior), new PropertyMetadata(null));
#else
        public FrameworkElement TargetObject {
            get { return (FrameworkElement)GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register("TargetObject", typeof(FrameworkElement), typeof(AsyncCommandVisualStateBehavior), new PropertyMetadata(null));
#endif

        public string AllItemsCompletedState {get; set;}
        public string OneItemCompletedState {get; set;}
        public string ItemStartedState {get; set;}
        public string ErrorState { get; set; }

        public bool AutoResubscribeOnError { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            gotoState(AllItemsCompletedState, false);
        }

        protected override void OnDetaching()
        {
            if (watcher != null) {
                watcher.Dispose();
                watcher = null;
            }
            base.OnDetaching();
        }

        IDisposable watcher;
        protected static void onAsyncCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AsyncCommandVisualStateBehavior This = (AsyncCommandVisualStateBehavior)sender;
            IReactiveAsyncCommand cmd = e.NewValue as IReactiveAsyncCommand;

            if (This.watcher != null) {
                This.watcher.Dispose();
                This.watcher = null;
            }

            This.watcher = cmd.ItemsInflight.Zip(cmd.ItemsInflight.Skip(1), (now, prev) => new { Now = now, Delta = now - prev })
                .Subscribe(x => {
                    var state = (x.Delta > 0 ? This.ItemStartedState : This.OneItemCompletedState);
                    state = (x.Now == 0 && x.Delta < 0 ? This.AllItemsCompletedState : state);
                    This.gotoState(state);
                }, ex => {
                    This.gotoState(This.ErrorState);
                    if (!This.AutoResubscribeOnError)
                        return;
                    onAsyncCommandChanged(This, e);
                });
        }

        void gotoState(string state, bool animate = true)
        {
            if (String.IsNullOrEmpty(state))
                return;
            VisualStateManager.GoToState(TargetObject ?? this.AssociatedObject, state, animate);
        }
    }

}
