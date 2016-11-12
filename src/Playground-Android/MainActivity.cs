using System;
using System.Reactive.Linq;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ReactiveUI;
using Splat;

namespace MobileSample_Android
{
    [Activity (Label = "AndroidPlayground", MainLauncher = true)]
    public class MainView : ReactiveActivity<MainViewModel> 
    {
        int count = 1;
        public TextView SavedGuid { get; set; }

        public MainView()
        {
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
        
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
                        
            this.WireUpControls();

            RxApp.SuspensionHost.ObserveAppState<AppBootstrapper>()
                .Select(x => x.SavedGuid)
                .Do(x => {
                    Console.WriteLine(x);
                })
                .BindTo(this, x => x.SavedGuid.Text);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Console.WriteLine("DEADED");
        }
    }

    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
        }
    }
}
