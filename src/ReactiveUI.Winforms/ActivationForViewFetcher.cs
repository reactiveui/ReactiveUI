using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Forms;

using Splat;

namespace ReactiveUI.Winforms
{
    public class ActivationForViewFetcher : IActivationForViewFetcher, IEnableLogger
    {
        public int GetAffinityForView(Type view)
        {
            return (typeof(Control).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())) ? 10 : 0;
        }

        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            var control = view as Control;
            if (control == null) {
                // Show a friendly warning in the log that this view will never be activated
                this.Log().Warn("Expected a view of type System.Windows.Forms.Control but it is {0}.\r\nYou need to implement your own IActivationForViewFetcher for {0}.", view.GetType());
                return Observable<bool>.Empty;
            }

            // Create an observable stream of booleans
            // true representing the control is active, false representing the control is not active
            // by using DistinctUntilChanged, a control can either be active or not active
            // This should also fix #610 for winforms

            var controlVisible = Observable.FromEventPattern(control, "VisibleChanged").Select(_ => control.Visible);
            var handleDestroyed = Observable.FromEventPattern(control, "HandleDestroyed").Select(_ => false);
            var handleCreated = Observable.FromEventPattern(control, "HandleCreated").Select(_ => true);

            var controlActive = Observable.Merge(controlVisible, handleDestroyed, handleCreated)
                .DistinctUntilChanged();

            

            var form = view as Form;
            if (form != null) {
                var formActive = Observable.FromEventPattern(form, "Closed").Select(_ => false);
                return controlActive.Merge(formActive).DistinctUntilChanged();
            }

            return controlActive;
        }
    }
}
