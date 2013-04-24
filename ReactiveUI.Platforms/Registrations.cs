﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using ReactiveUI.Routing;
using System.Reactive.Concurrency;

#if COCOA
using MonoTouch.UIKit;
using ReactiveUI.Cocoa;
#endif

#if UIKIT
using NSApplication = MonoTouch.UIKit.UIApplication;
#endif

namespace ReactiveUI.Xaml
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
            registerFunction(() => new KVOObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new CocoaDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
            registerFunction(() => new TargetActionCommandBinder(), typeof(ICreatesCommandBinding));

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
