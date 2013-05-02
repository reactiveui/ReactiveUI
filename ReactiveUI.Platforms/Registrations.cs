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
using MonoTouch.UIKit;
using ReactiveUI.Cocoa;
#endif

#if UIKIT
using NSApplication = MonoTouch.UIKit.UIApplication;
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
        public void Register(IMutableDependencyResolver resolver)
        {
#if !WINRT && !WP8
            resolver.Register<IBindingTypeConverter>(() => new ComponentModelTypeConverter());
#endif

#if !MONO
            resolver.Register<ICreatesObservableForProperty>(() => new DependencyObjectObservableForProperty());
            resolver.Register<IDefaultPropertyBindingProvider>(() => new XamlDefaultPropertyBinding());
            resolver.Register<ICreatesCommandBinding>(() => new CreatesCommandBindingViaCommandParameter());
            resolver.Register<ICreatesCommandBinding>(() => new CreatesCommandBindingViaEvent());
            resolver.Register<IBindingTypeConverter>(() => new BooleanToVisibilityTypeConverter());
            resolver.Register<IPropertyBindingHook>(() => new AutoDataTemplateBindingHook());

#endif

#if COCOA
            resolver.Register<ICreatesObservableForProperty>(() => new KVOObservableForProperty());
            resolver.Register<IDefaultPropertyBindingProvider>(() => new CocoaDefaultPropertyBinding());
            resolver.Register<ICreatesCommandBinding>(() => new TargetActionCommandBinder());

            RxApp.DeferredScheduler = new WaitForDispatcherScheduler(() => new NSRunloopScheduler(NSApplication.SharedApplication));
#endif

#if !MONO && !WINRT
            RxApp.DeferredScheduler = new WaitForDispatcherScheduler(() => DispatcherScheduler.Current);
#endif

#if WINRT
            RxApp.DeferredScheduler = new WaitForDispatcherScheduler(() => CoreDispatcherScheduler.Current);
#endif

            RxApp.InUnitTestRunnerOverride = RealUnitTestDetector.InUnitTestRunner();
        }
    }
}
