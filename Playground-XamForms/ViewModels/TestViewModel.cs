using System;
using ReactiveUI;
using System.Runtime.Serialization;
using Splat;

namespace PlaygroundXamForms
{
    [DataContract]
    public class TestViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment {
            get { return "Akavache Test"; }
        }

        public IScreen HostScreen { get; protected set; }

        string _TheGuid;
        [DataMember] public string TheGuid {
            get { return _TheGuid; }
            set { this.RaiseAndSetIfChanged(ref _TheGuid, value); }
        }

        public TestViewModel(IScreen hostScreen = null)
        {
            TheGuid = Guid.NewGuid().ToString();
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();
        }
    }
}