using System;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Mobile;
using Splat;

namespace MobileSample_Android
{
    public class App
    {
        static App _Current;
        public static App Current {
            get { return (_Current = _Current ?? new App()); }
        }

        protected App()
        {
            // TODO: Fix Me
            //RxApp.ConfigureServiceLocator(
            //    (t, s) => locator.GetAllServices(t, s).FirstOrDefault(),
            //    (t, s) => locator.GetAllServices(t, s).ToArray(),
            //    (c, t, s) => locator.Register(() => Activator.CreateInstance(c), t, s));

            Locator.CurrentMutable.Register(() => typeof(MainView), typeof(IViewFor<MainViewModel>));
            Locator.CurrentMutable.Register(() => typeof(SecondaryView), typeof(IViewFor<SecondaryViewModel>));

            RxApp.SuspensionHost.CreateNewAppState = () => new AppBootstrapper();

            // TODO: Fix Me
            //RxApp.Register(typeof(AppBootstrapper), typeof(IApplicationRootState));
        }
    }
}

