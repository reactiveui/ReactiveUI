using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using Splat;

namespace MobileSample_Android
{
    [Activity (Label = "SecondaryActivity")]
    public class SecondaryView : ReactiveActivity<SecondaryViewModel>
    {
        readonly AutoSuspendHelper suspendHelper;

        public SecondaryView()
        {
        }
    }
    
    public class SecondaryViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment {
            get { return "Secondary!"; }
        }

        public IScreen HostScreen { get; protected set; }

        public SecondaryViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();
        }
    }
}

