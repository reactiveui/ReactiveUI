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
            return
                (typeof(Page).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())) ||
                (typeof(Cell).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()))
                ? 10 : 0;
        }

        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            var ret = Observable.Never<bool>();

            var page = view as Page;

            if (page != null) {
                ret = Observable.Merge(
                    Observable.FromEventPattern<EventHandler, EventArgs>(x => page.Appearing += x, x => page.Appearing -= x).Select(_ => true),
                    Observable.FromEventPattern<EventHandler, EventArgs>(x => page.Disappearing += x, x => page.Disappearing -= x).Select(_ => false));
            } else {
                var cell = view as Cell;

                if (cell != null) {
                    ret = Observable
                        .Merge(
                            Observable.FromEventPattern<EventHandler, EventArgs>(x => cell.Appearing += x, x => cell.Appearing -= x).Select(_ => true),
                            Observable.FromEventPattern<EventHandler, EventArgs>(x => cell.Disappearing += x, x => cell.Disappearing -= x).Select(_ => false));
                }
            }

            return ret.DistinctUntilChanged();
        }
    }
}
