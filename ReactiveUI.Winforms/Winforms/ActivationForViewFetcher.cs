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

        public Tuple<IObservable<Unit>, IObservable<Unit>> GetActivationForView(IActivatable view)
        {
            var control = view as Control;
            if (control == null) {
                //show a friendly warning in the log that this view will never be activated
                this.Log().Warn("Expected a view of type System.Windows.Forms.Control but it is {0}.\r\nYou need to implement your own IActivationForViewFetcher for {0}.", view.GetType());
                return Tuple.Create(Observable.Empty<Unit>(), Observable.Empty<Unit>());
            }

            // create an observable stream of booleans
            // true representing the control is active, false representing the control is not active
            // by using DistinctUntilChanged, a control can either be active or not active
            // this should also fix #610 for winforms

            var controlVisible = Observable.FromEventPattern(control, "VisibleChanged").Select(_ => control.Visible);
            var handleDestroyed = Observable.FromEventPattern(control, "HandleDestroyed").Select(_ => false);
            var handleCreated = Observable.FromEventPattern(control, "HandleCreated").Select(_ => true);

            var controlActive = Observable.Merge(controlVisible, handleDestroyed, handleCreated)
                .DistinctUntilChanged();


            var controlActivated = controlActive.Where(x => x).Select(_ => Unit.Default);
            var controlDeactivated = controlActive.Where(x => !x).Select(_ => Unit.Default);

            var form = view as Form;
            if (form != null) {
                var formActive = Observable.FromEventPattern(form, "Closed").Select(_ => false);
                var formDeactivated = controlActive.Merge(formActive).DistinctUntilChanged().Where(x => !x).Select(_ => Unit.Default);
                return Tuple.Create(controlActivated, formDeactivated);
            }



            return Tuple.Create(controlActivated, controlDeactivated);
        }
    }
}
