using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace ReactiveUI.Blend
{
    [ContentProperty(Name = "Actions")]
    public sealed class ObservableTriggerBehavior : Behavior<DependencyObject>
    {
        private object resolvedSource;
                
        public ActionCollection Actions
        {
            get {
                ActionCollection actionCollection = (ActionCollection) this.GetValue(ObservableTriggerBehavior.ActionsProperty);
                if (actionCollection == null) {
                    actionCollection = new ActionCollection();
                    this.SetValue(ObservableTriggerBehavior.ActionsProperty, actionCollection);
                }
                return actionCollection;
            }
        }
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(ObservableTriggerBehavior), new PropertyMetadata(null));

        public object SourceObject
        {
            get { return this.GetValue(ObservableTriggerBehavior.SourceObjectProperty); }
            set { this.SetValue(ObservableTriggerBehavior.SourceObjectProperty, value); }
        }
        public static readonly DependencyProperty SourceObjectProperty =
            DependencyProperty.Register("SourceObject", typeof(object), typeof(ObservableTriggerBehavior), new PropertyMetadata(null, OnSourceObjectChanged));

        private static void OnSourceObjectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            ObservableTriggerBehavior observableTriggerBehavior = (ObservableTriggerBehavior)dependencyObject;
            observableTriggerBehavior.SetResolvedSource(observableTriggerBehavior.ComputeResolvedSource());
        }

        public IObservable<object> Observable
        {
            get { return (IObservable<object>)GetValue(ObservableProperty); }
            set { SetValue(ObservableProperty, value); }
        }
        public static readonly DependencyProperty ObservableProperty =
            DependencyProperty.Register("Observable", typeof(IObservable<object>), typeof(ObservableTriggerBehavior), new PropertyMetadata(null, onObservableChanged));

        private void SetResolvedSource(object newSource)
        {
            if (this.AssociatedObject == null || this.resolvedSource == newSource)
            {
                return;
            }
            this.resolvedSource = newSource;
        }

        private object ComputeResolvedSource()
        {
            if (this.ReadLocalValue(ObservableTriggerBehavior.SourceObjectProperty) != DependencyProperty.UnsetValue) {
                return this.SourceObject;
            } else {
                return this.AssociatedObject;
            }
        }

        public bool AutoResubscribeOnError { get; set; }

        IDisposable watcher;
        protected static void onObservableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ObservableTriggerBehavior This = (ObservableTriggerBehavior)sender;
            if (This.watcher != null)
            {
                This.watcher.Dispose();
                This.watcher = null;
            }

            This.watcher = ((IObservable<object>)e.NewValue).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
                x => Interaction.ExecuteActions(This.resolvedSource, This.Actions, x),
                ex =>
                {
                    if (!This.AutoResubscribeOnError)
                        return;
                    onObservableChanged(This, e);
                });
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.SetResolvedSource(this.ComputeResolvedSource());
        }

        protected override void OnDetaching()
        {
            this.SetResolvedSource(null);
            base.OnDetaching();
        }
    }
}
