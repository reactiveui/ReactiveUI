using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace MasterDetail
{
    public class NumberStreamViewModel : ReactiveObject, IRoutableViewModel
    {
        public NumberStreamViewModel()
        {
            NumberStream = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Select(x => x.ToString());
        }

        public IObservable<string> NumberStream { get; }

        public string UrlPathSegment => "Number Stream Page";

        public IScreen HostScreen { get; }
    }
}
