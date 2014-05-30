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

namespace ReactiveUI.QuickUI
{
    public class ActivationForViewFetcher : IActivationForViewFetcher
    {
        public int GetAffinityForView(Type view)
        {
            return (typeof(Page).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())) ? 10 : 0;
        }

        public Tuple<IObservable<Unit>, IObservable<Unit>> GetActivationForView(IActivatable view)
        {
            var fe = view as Page;

            return Tuple.Create(
                Observable.FromEventPattern<EventHandler, EventArgs>(x => fe.Appearing += x, x => fe.Disappearing -= x).Select(_ => Unit.Default),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => fe.Appearing += x, x => fe.Disappearing -= x).Select(_ => Unit.Default));
        }
    }
}
