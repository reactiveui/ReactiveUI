using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Mobile
{
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(IMutableDependencyResolver resolver)
        {
#if WP8
            resolver.Register<ISuspensionHost>(() => new WP8SuspensionHost());
            resolver.Register<ISuspensionDriver>(() => new PhoneServiceStateDriver());
#elif WINRT
            resolver.Register<ISuspensionHost>(() => new WinRTSuspensionHost());
            resolver.Register<ISuspensionDriver>(() => new WinRTAppDataDriver());
#elif UIKIT
            resolver.Register<ISuspensionHost>(() => new CocoaSuspensionHost());
            resolver.Register<ISuspensionDriver>(() => new AppSupportJsonSuspensionDriver());
#elif ANDROID
            resolver.Register<ISuspensionHost>(() => new AndroidSuspensionHost());
            resolver.Register<ISuspensionDriver>(() => new BundleSuspensionDriver());
#endif
        }
    }
}
