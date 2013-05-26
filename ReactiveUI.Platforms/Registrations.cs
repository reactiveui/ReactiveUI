using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using ReactiveUI;
using System.Reactive.Concurrency;

#if COCOA
using ReactiveUI.Cocoa;
#endif

#if UIKIT
using MonoTouch.UIKit;
using NSApplication = MonoTouch.UIKit.UIApplication;
#elif COCOA && !UIKIT
using MonoMac.AppKit;
#endif

#if ANDROID
namespace ReactiveUI.Android
#elif COCOA
namespace ReactiveUI.Cocoa
#else
namespace ReactiveUI.Xaml
#endif
{
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

#if !WINRT && !WP8
            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
#endif

#if !MONO
            registerFunction(() => new DependencyObjectObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new XamlDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
            registerFunction(() => new CreatesCommandBindingViaCommandParameter(), typeof(ICreatesCommandBinding));
            registerFunction(() => new CreatesCommandBindingViaEvent(), typeof(ICreatesCommandBinding));
            registerFunction(() => new BooleanToVisibilityTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
#endif

#if COCOA
            registerFunction(() => new UIKitObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new KVOObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new CocoaDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
            registerFunction(() => new TargetActionCommandBinder(), typeof(ICreatesCommandBinding));
#endif

            RxApp.InUnitTestRunnerOverride = PlatformUnitTestDetector.InUnitTestRunner();
            if (RxApp.InUnitTestRunner()) {
                return;
            }

#if COCOA
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new NSRunloopScheduler(NSApplication.SharedApplication));
#endif

#if !MONO && !WINRT
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => DispatcherScheduler.Current);
#endif

#if WINRT
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => CoreDispatcherScheduler.Current);
#endif
        }
    }
}
