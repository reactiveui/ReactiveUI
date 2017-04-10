using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
        object resolvedSource;
        SerialDisposable watcher;

        public ObservableTriggerBehavior()
        {
            watcher = new SerialDisposable();
            watcher.Disposable = Disposable.Empty;
        }
                
        public ActionCollection Actions
        {
            get {
                var actionCollection = (ActionCollection) this.GetValue(ObservableTriggerBehavior.ActionsProperty);

                if (actionCollection == null) {
                    actionCollection = new ActionCollection();
                    this.SetValue(ObservableTriggerBehavior.ActionsProperty, actionCollection);
                }

                return actionCollection;
            }
        }

        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(ObservableTriggerBehavior), new PropertyMetadata(null));

        public object SourceObject {
            get { return this.GetValue(ObservableTriggerBehavior.SourceObjectProperty); }
            set { this.SetValue(ObservableTriggerBehavior.SourceObjectProperty, value); }
        }
        public static readonly DependencyProperty SourceObjectProperty =
            DependencyProperty.Register("SourceObject", typeof(object), typeof(ObservableTriggerBehavior), new PropertyMetadata(null, OnSourceObjectChanged));

        public bool AutoResubscribeOnError { get; set; }

        public IObservable<object> Observable {
            get { return (IObservable<object>)GetValue(ObservableProperty); }
            set { SetValue(ObservableProperty, value); }
        }
        public static readonly DependencyProperty ObservableProperty =
            DependencyProperty.Register("Observable", typeof(IObservable<object>), typeof(ObservableTriggerBehavior), new PropertyMetadata(null, onObservableChanged));

        static void OnSourceObjectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var observableTriggerBehavior = (ObservableTriggerBehavior)dependencyObject;

            observableTriggerBehavior.setResolvedSource(observableTriggerBehavior.computeResolvedSource());
        }

        void setResolvedSource(object newSource)
        {
            if (this.AssociatedObject == null || this.resolvedSource == newSource) {
                return;
            }

            this.resolvedSource = newSource;
        }

        object computeResolvedSource()
        {
            if (this.ReadLocalValue(ObservableTriggerBehavior.SourceObjectProperty) != DependencyProperty.UnsetValue) {
                return this.SourceObject;
            } else {
                return this.AssociatedObject;
            }
        }

        static void onObservableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ObservableTriggerBehavior This = (ObservableTriggerBehavior)sender;

            This.watcher.Disposable = ((IObservable<object>)e.NewValue).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
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
            this.setResolvedSource(this.computeResolvedSource());
        }

        protected override void OnDetaching()
        {
            this.setResolvedSource(null);
            base.OnDetaching();

            watcher.Dispose();
        }
    }
}
