// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        private object _resolvedSource;
        private SerialDisposable _watcher;

        public ObservableTriggerBehavior()
        {
            _watcher = new SerialDisposable();
            _watcher.Disposable = Disposable.Empty;
        }

        public ActionCollection Actions
        {
            get
            {
                var actionCollection = (ActionCollection)GetValue(ObservableTriggerBehavior.ActionsProperty);

                if (actionCollection == null)
                {
                    actionCollection = new ActionCollection();
                    SetValue(ObservableTriggerBehavior.ActionsProperty, actionCollection);
                }

                return actionCollection;
            }
        }

        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(ObservableTriggerBehavior), new PropertyMetadata(null));

        public object SourceObject
        {
            get { return GetValue(ObservableTriggerBehavior.SourceObjectProperty); }
            set { SetValue(ObservableTriggerBehavior.SourceObjectProperty, value); }
        }

        public static readonly DependencyProperty SourceObjectProperty =
            DependencyProperty.Register("SourceObject", typeof(object), typeof(ObservableTriggerBehavior), new PropertyMetadata(null, OnSourceObjectChanged));

        public bool AutoResubscribeOnError { get; set; }

        public IObservable<object> Observable
        {
            get { return (IObservable<object>)GetValue(ObservableProperty); }
            set { SetValue(ObservableProperty, value); }
        }

        public static readonly DependencyProperty ObservableProperty =
            DependencyProperty.Register("Observable", typeof(IObservable<object>), typeof(ObservableTriggerBehavior), new PropertyMetadata(null, OnObservableChanged));

        private static void OnSourceObjectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var observableTriggerBehavior = (ObservableTriggerBehavior)dependencyObject;

            observableTriggerBehavior.SetResolvedSource(observableTriggerBehavior.ComputeResolvedSource());
        }

        private void SetResolvedSource(object newSource)
        {
            if (AssociatedObject == null || _resolvedSource == newSource)
            {
                return;
            }

            _resolvedSource = newSource;
        }

        private object ComputeResolvedSource()
        {
            if (ReadLocalValue(ObservableTriggerBehavior.SourceObjectProperty) != DependencyProperty.UnsetValue)
            {
                return SourceObject;
            }

            return AssociatedObject;
        }

        private static void OnObservableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ObservableTriggerBehavior @this = (ObservableTriggerBehavior)sender;

            @this._watcher.Disposable = ((IObservable<object>)e.NewValue).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
                x => Interaction.ExecuteActions(@this._resolvedSource, @this.Actions, x),
                ex =>
                {
                    if (!@this.AutoResubscribeOnError)
                    {
                        return;
                    }

                    OnObservableChanged(@this, e);
                });
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            SetResolvedSource(ComputeResolvedSource());
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            SetResolvedSource(null);
            base.OnDetaching();

            _watcher.Dispose();
        }
    }
}
