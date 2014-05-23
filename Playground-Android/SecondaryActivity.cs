using System;
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
using ReactiveUI.Android;
using ReactiveUI.Mobile;
using System.ComponentModel;
using Splat;

namespace MobileSample_Android
{
    [Activity (Label = "SecondaryActivity")]
    public class SecondaryView : ReactiveActivity<SecondaryViewModel>
    {
        readonly AutoSuspendActivityHelper suspendHelper;

        public SecondaryView()
        {
            // NB: Super Dumb
            Console.WriteLine(App.Current);
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new AndroidUIScheduler(this));

            suspendHelper = new AutoSuspendActivityHelper(this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            suspendHelper.OnCreate(bundle);

            SetContentView(Resource.Layout.Secondary);
        }
                
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            suspendHelper.OnSaveInstanceState(outState);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            suspendHelper.OnRestart();
        }

        protected override void OnStart()
        {
            base.OnStart();
            suspendHelper.OnStart();
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

