using System;
using System.Reflection;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.VisualTree;

namespace ReactiveUI.Avalonia
{
    public class ActivationForViewFetcher : IActivationForViewFetcher
    {
        public int GetAffinityForView(Type view)
        {
            return typeof(IVisual).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? 10 : 0;
        }

        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            var visual = view as IVisual;
            if (visual == null) return Observable.Return(false);
            
            var viewLoaded = Observable
                .FromEventPattern<VisualTreeAttachmentEventArgs>(
                    x => visual.AttachedToVisualTree += x,
                    x => visual.DetachedFromVisualTree -= x)
                .Select(args => true);

            var viewUnloaded = Observable
                .FromEventPattern<VisualTreeAttachmentEventArgs>(
                    x => visual.DetachedFromVisualTree += x,
                    x => visual.DetachedFromVisualTree -= x)
                .Select(args => false);

            return viewLoaded
                .Merge(viewUnloaded)
                .DistinctUntilChanged();
        }
    }
}