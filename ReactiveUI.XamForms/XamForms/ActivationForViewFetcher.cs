using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
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
                GetActivationFor(view as Page) ??
                GetActivationFor(view as View) ??
                GetActivationFor(view as Cell) ??
                Observable.Never<bool>();

            return activation.DistinctUntilChanged();
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

            //return GetAnyParentChangedFor(view)
            //    .StartWith(Unit.Default)
            //    .Select(_ => GetPageFor(view))
            //    .Select(x =>
            //        x == null ?
            //        Observable.Return(false) :
            //        Observable
            //        .Merge(
            //            Observable
            //                .FromEventPattern<EventHandler, EventArgs>(y => x.Appearing += y, y => x.Appearing -= y)
            //                .Do(_ => System.Diagnostics.Debug.WriteLine("{0} APPEARING", view))
            //                .Select(_ => true),
            //            Observable
            //                .FromEventPattern<EventHandler, EventArgs>(y => x.Disappearing += y, y => x.Disappearing -= y)
            //                .Do(_ => System.Diagnostics.Debug.WriteLine("{0} DISAPPEARING", view))
            //                .Select(_ => false))
            //        .Do(_ => System.Diagnostics.Debug.WriteLine("PAGE FOUND FOR {0}, IS VISIBLE: {1}", view, x.IsVisible))
            //        .StartWith(x.IsVisible))
            //    .Switch()
            //    .Do(x => System.Diagnostics.Debug.WriteLine("{0} ACTIVATED: {1}", view, x));
        }

        private static IObservable<Element> GetParentChangedFor(Element element)
        {
            var propertyChanged = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                x => element.PropertyChanged += x,
                x => element.PropertyChanged -= x);
            return propertyChanged
                .Where(x => x.EventArgs.PropertyName == "Parent")
                .Select(_ => element.Parent);
        }

        private static IObservable<Unit> GetParentChangedForRecursive(Element element)
        {
            var observables = new List<IObservable<Unit>>();

            while (element != null)
            {
                observables.Add(GetParentChangedFor(element).Select(_ => Unit.Default));
                element = element.Parent;
            }

            return Observable.Merge(observables);
        }

        private static IObservable<Unit> GetAnyParentChangedFor(Element element)
        {
            return GetParentChangedForRecursive(element)
                .Select(_ => GetParentChangedForRecursive(element).StartWith(Unit.Default))
                .Switch();
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