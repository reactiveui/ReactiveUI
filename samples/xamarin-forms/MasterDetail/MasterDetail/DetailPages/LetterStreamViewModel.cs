using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace MasterDetail
{
    public class LetterStreamViewModel : ReactiveObject, IRoutableViewModel
    {
        private ObservableAsPropertyHelper<char> _currentLetter;

        public LetterStreamViewModel()
        {
            _currentLetter = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Scan(64, (acc, current) => acc + 1)
                .Select(x => (char)x)
                .Take(26)
                .ToProperty(this, x => x.CurrentLetter, scheduler: RxApp.MainThreadScheduler);
        }

        public char CurrentLetter => _currentLetter.Value;

        public string UrlPathSegment => "Letter Stream Page";

        public IScreen HostScreen { get; }
    }
}
