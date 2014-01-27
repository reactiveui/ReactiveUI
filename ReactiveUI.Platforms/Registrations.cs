using System;
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
using ReactiveUI.Android.Android;

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
            registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            registerFunction(() => new DependencyObjectObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new XamlDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
            registerFunction(() => new CreatesCommandBindingViaCommandParameter(), typeof(ICreatesCommandBinding));
            registerFunction(() => new CreatesCommandBindingViaEvent(), typeof(ICreatesCommandBinding));
            registerFunction(() => new BooleanToVisibilityTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
#endif

#if ANDROID
            registerFunction(() => new AndroidDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
            registerFunction(() => new AndroidObservableForWidgets(), typeof(ICreatesObservableForProperty));
            registerFunction(() => AndroidCommandBinders.Instance.Value, typeof(ICreatesCommandBinding));
#endif

#if UIKIT
            registerFunction(() => UIKitObservableForProperty.Instance.Value, typeof(ICreatesObservableForProperty));
            registerFunction(() => UIKitCommandBinders.Instance.Value, typeof(ICreatesCommandBinding));
            registerFunction(() => DateTimeNSDateConverter.Instance.Value, typeof(IBindingTypeConverter));
#endif

#if COCOA
            registerFunction(() => new KVOObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new CocoaDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
#endif

#if COCOA && !UIKIT
            registerFunction(() => new TargetActionCommandBinder(), typeof(ICreatesCommandBinding));
#endif

            RxApp.InUnitTestRunnerOverride = PlatformUnitTestDetector.InUnitTestRunner();
            if (RxApp.InUnitTestRunner()) {
                return;
            }

            RxApp.TaskpoolScheduler = System.Reactive.Concurrency.TaskPoolScheduler.Default;

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
