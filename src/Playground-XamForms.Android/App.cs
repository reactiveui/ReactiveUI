using System;
using Android.App;
using Android.Runtime;
using ReactiveUI;
using PlaygroundXamForms;

namespace MobileSample_Android
{
    [Application(Label = "AndroidPlayground")]
    public class App : Application
    {
        AutoSuspendHelper suspendHelper;

        App(IntPtr handle, JniHandleOwnership owner) : base(handle, owner) { }

        public override void OnCreate()
        {
            base.OnCreate();

            suspendHelper = new AutoSuspendHelper(this);

            RxApp.SuspensionHost.CreateNewAppState = () => {
                Console.WriteLine("Creating app state");
                return new AppBootstrapper();
            };

            RxApp.SuspensionHost.SetupDefaultSuspendResume();
        }
    }
}
