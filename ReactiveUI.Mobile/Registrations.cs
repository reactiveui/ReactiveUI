using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Mobile
{
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
#if WP8
            registerFunction(() => new WP8SuspensionHost(), typeof (ISuspensionHost));
            registerFunction(() => new PhoneServiceStateDriver(), typeof (ISuspensionDriver));
#elif WINRT
            registerFunction(() => new WinRTSuspensionHost(), typeof(ISuspensionHost));
            registerFunction(() => new WinRTAppDataDriver(), typeof(ISuspensionDriver));
#elif UIKIT
            registerFunction(() => new CocoaSuspensionHost(), typeof(ISuspensionHost));
            registerFunction(() => new AppSupportJsonSuspensionDriver(), typeof(ISuspensionDriver));
#elif ANDROID
            registerFunction(() => new AndroidSuspensionHost(), typeof(ISuspensionHost));
            registerFunction(() => new BundleSuspensionDriver(), typeof(ISuspensionDriver));
#endif
        }
    }
}
