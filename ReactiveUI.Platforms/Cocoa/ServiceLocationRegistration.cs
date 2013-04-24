using System;
using ReactiveUI;
using ReactiveUI.Xaml;

#if UIKIT
using MonoTouch.UIKit;
using NSApplication = MonoTouch.UIKit.UIApplication;
#else
using MonoMac.AppKit;
#endif

namespace ReactiveUI.Cocoa
{
    public class ServiceLocationRegistration : IWantsToRegisterStuff
    {
        public void Register ()
        {
            RxApp.Register(typeof(KVOObservableForProperty), typeof(ICreatesObservableForProperty));
            RxApp.Register(typeof(CocoaDefaultPropertyBinding), typeof(IDefaultPropertyBindingProvider));
            RxApp.Register(typeof(TargetActionCommandBinder), typeof(ICreatesCommandBinding));

            RxApp.DeferredScheduler = new WaitForDispatcherScheduler(() => new NSRunloopScheduler(NSApplication.SharedApplication));
        }
    }
}