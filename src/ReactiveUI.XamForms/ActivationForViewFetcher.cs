using System;
using System.ComponentModel;
using System.Reactive.Linq;
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
                (typeof(View).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())) ||
                (typeof(Cell).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()))
                ? 10 : 0;
        }

        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            var activation =
                GetActivationFor(view as ICanActivate) ??
                GetActivationFor(view as Page) ??
                GetActivationFor(view as View) ??
                GetActivationFor(view as Cell) ??
                Observable<bool>.Never;

            return activation.DistinctUntilChanged();
        }

        private static IObservable<bool> GetActivationFor(ICanActivate canActivate)
        {
            if (canActivate == null) {
                return null;
            }

            return Observable.Merge(
                canActivate.Activated.Select(_ => true),
                canActivate.Deactivated.Select(_ => false));
        }

        private static IObservable<bool> GetActivationFor(Page page)
        {
            if (page == null) {
                return null;
            }

            return Observable.Merge(
                Observable.FromEventPattern<EventHandler, EventArgs>(x => page.Appearing += x, x => page.Appearing -= x).Select(_ => true),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => page.Disappearing += x, x => page.Disappearing -= x).Select(_ => false));
        }

        private static IObservable<bool> GetActivationFor(View view)
        {
            if (view == null) {
                return null;
            }
            
            var propertyChanged = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                x => view.PropertyChanged += x,
                x => view.PropertyChanged -= x);
            return propertyChanged
                .Where(x => x.EventArgs.PropertyName == "IsVisible")
                .Select(_ => view.IsVisible)
                .StartWith(view.IsVisible);
        }

        private static IObservable<bool> GetActivationFor(Cell cell)
        {
            if (cell == null) {
                return null;
            }

            return Observable.Merge(
                Observable.FromEventPattern<EventHandler, EventArgs>(x => cell.Appearing += x, x => cell.Appearing -= x).Select(_ => true),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => cell.Disappearing += x, x => cell.Disappearing -= x).Select(_ => false));
        }

        private static Page GetPageFor(Element element)
        {
            Page page = null;

            while (element != null)
            {
                page = element as Page;

                if (page != null)
                {
                    return page;
                }

                element = element.ParentView;
            }

            return null;
        }
    }
}