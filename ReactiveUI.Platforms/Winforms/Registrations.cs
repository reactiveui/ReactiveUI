using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using ReactiveUI;
using System.Reactive.Concurrency;


namespace ReactiveUI.Winforms
{
    using System.Windows.Forms;

    /// <summary>
    /// Ignore me. This class is a secret handshake between RxUI and RxUI.Xaml
    /// in order to register certain classes on startup that would be difficult
    /// to register otherwise.
    /// </summary>
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));

            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
          
            registerFunction(() => new WinformsDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
            registerFunction(() => new CreatesWinformsCommandBinding(), typeof(ICreatesCommandBinding));
         

            RxApp.InUnitTestRunnerOverride = PlatformUnitTestDetector.InUnitTestRunner();
            if (RxApp.InUnitTestRunner()) {
                return;
            }

            WindowsFormsSynchronizationContext.AutoInstall = true;
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new SynchronizationContextScheduler(new WindowsFormsSynchronizationContext()));

        }
    }
}
