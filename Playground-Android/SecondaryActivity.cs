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
        readonly ActivityRoutedViewHost routeHelper;
        readonly AutoSuspendActivityHelper suspendHelper;

        public SecondaryView()
        {
            // NB: Super Dumb
            Console.WriteLine(App.Current);
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new AndroidUIScheduler(this));

            routeHelper = new ActivityRoutedViewHost(this);
            suspendHelper = new AutoSuspendActivityHelper(this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            suspendHelper.OnCreate(bundle);

            SetContentView(Resource.Layout.Secondary);
        }

        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            return routeHelper.OnKeyUp(keyCode, e);
        }

        protected override void OnResume()
        {
            base.OnResume();
            suspendHelper.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            suspendHelper.OnPause();
        }
        
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            suspendHelper.OnSaveInstanceState(outState);
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

