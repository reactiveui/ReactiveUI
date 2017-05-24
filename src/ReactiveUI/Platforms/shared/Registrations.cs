using System;
using System.Reactive.Concurrency;

namespace ReactiveUI
{
    /// <summary>
    /// Ignore me. This class is a secret handshake between RxUI and RxUI.Xaml
    /// in order to register certain classes on startup that would be difficult
    /// to register otherwise.
    /// </summary>
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));

#if !NETFX_CORE && !WP8 && !WP81
            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
#endif

#if !MONO
            registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            registerFunction(() => new DependencyObjectObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new BooleanToVisibilityTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
#endif

#if ANDROID
            registerFunction(() => new AndroidObservableForWidgets(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new AndroidCommandBinders(), typeof(ICreatesCommandBinding));
#endif

#if UIKIT
            registerFunction(() => new UIKitObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new UIKitCommandBinders(), typeof(ICreatesCommandBinding));
            registerFunction(() => new DateTimeNSDateConverter(), typeof(IBindingTypeConverter));
#endif

#if COCOA
            registerFunction(() => new KVOObservableForProperty(), typeof(ICreatesObservableForProperty));
#endif

#if COCOA && !UIKIT
            registerFunction(() => new TargetActionCommandBinder(), typeof(ICreatesCommandBinding));
#endif

            RxApp.TaskpoolScheduler = System.Reactive.Concurrency.TaskPoolScheduler.Default;

#if COCOA
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new NSRunloopScheduler());
#endif

#if !MONO && !NETFX_CORE
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => DispatcherScheduler.Current);
#endif

#if NETFX_CORE
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => CoreDispatcherScheduler.Current);
#endif

#if ANDROID
            RxApp.MainThreadScheduler = HandlerScheduler.MainThreadScheduler;
#endif

#if WP8
            registerFunction(() => new PhoneServiceStateDriver(), typeof (ISuspensionDriver));
#elif NETFX_CORE
            registerFunction(() => new WinRTAppDataDriver(), typeof(ISuspensionDriver));
#elif UIKIT
            registerFunction(() => new AppSupportJsonSuspensionDriver(), typeof(ISuspensionDriver));
#elif ANDROID
            registerFunction(() => new BundleSuspensionDriver(), typeof(ISuspensionDriver));
#endif
        }
    }
}
