using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Mobile
{
    public class ServiceLocationRegistration : IWantsToRegisterStuff
    {
        public void Register()
        {
#if WP8
            RxApp.Register(typeof(WP8SuspensionHost), typeof (ISuspensionHost));
            RxApp.Register(typeof(PhoneServiceStateDriver), typeof (ISuspensionDriver));
#elif WINRT
            RxApp.Register(typeof(WinRTSuspensionHost), typeof (ISuspensionHost));
            RxApp.Register(typeof(WinRTAppDataDriver), typeof (ISuspensionDriver));
#elif UIKIT
            RxApp.Register(typeof(CocoaSuspensionHost), typeof(ISuspensionHost));
            RxApp.Register(typeof(AppSupportJsonSuspensionDriver), typeof(ISuspensionDriver));
#elif ANDROID
            RxApp.Register(typeof(AndroidSuspensionHost), typeof(ISuspensionHost));
            RxApp.Register(typeof(BundleSuspensionDriver), typeof(ISuspensionDriver));
#endif
        }
    }
}
