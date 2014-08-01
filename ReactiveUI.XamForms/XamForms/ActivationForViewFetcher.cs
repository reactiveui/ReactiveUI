using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    public class ActivationForViewFetcher : IActivationForViewFetcher
    {
        public int GetAffinityForView(Type view)
        {
            return (typeof(Page).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())) ? 10 : 0;
        }

        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            var fe = view as Page;

            var ret = Observable.Merge(
                Observable.FromEventPattern<EventHandler, EventArgs>(x => fe.Appearing += x, x => fe.Appearing -= x).Select(_ => true),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => fe.Disappearing += x, x => fe.Disappearing -= x).Select(_ => false));

            return ret.DistinctUntilChanged();
        }
    }
}
