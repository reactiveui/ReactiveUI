using System;
using System.Reactive.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ReactiveUI;
using System.ComponentModel;
using ReactiveUI.Android;
using ReactiveUI.Mobile;
using Splat;

namespace MobileSample_Android
{
    [Activity (Label = "AndroidPlayground", MainLauncher = true)]
    public class MainView : ReactiveActivity<MainViewModel> 
    {
        int count = 1;
        readonly AutoSuspendActivityHelper suspendHelper;

        public TextView SavedGuid { get; set; }

        public MainView()
        {
            suspendHelper = new AutoSuspendActivityHelper(this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            suspendHelper.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
                        
            this.WireUpControls();

            RxApp.SuspensionHost.ObserveAppState<AppBootstrapper>()
                .StartWith(RxApp.SuspensionHost.GetAppState<AppBootstrapper>())
                .Select(x => x.SavedGuid)
                .Do(x => {
                    Console.WriteLine(x);
                })
                .BindTo(this, x => x.SavedGuid.Text);
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

    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
        }
    }
}