using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

#if WINRT
using Windows.ApplicationModel;
#endif

namespace ReactiveUI.Xaml
{
    /// <summary>
    /// Ignore me. This class is a secret handshake between RxUI and RxUI.Xaml
    /// in order to register certain classes on startup that would be difficult
    /// to register otherwise.
    /// </summary>
    public class ServiceLocationRegistration : IWantsToRegisterStuff
    {
        public void Register()
        {
#if !MONO
#if !WINRT
            RxApp.Register(typeof (DependencyObjectObservableForProperty), typeof (ICreatesObservableForProperty));
#endif

			RxApp.Register(typeof (XamlDefaultPropertyBinding), typeof (IDefaultPropertyBindingProvider));
            RxApp.Register(typeof (CommandBinderImplementation), typeof (ICommandBinderImplementation));
            RxApp.Register(typeof (CreatesCommandBindingViaCommandParameter), typeof(ICreatesCommandBinding));
            RxApp.Register(typeof (CreatesCommandBindingViaEvent), typeof(ICreatesCommandBinding));

            if (InDesignMode) {
                RxApp.Register(typeof(SampleDataProviderBinder), typeof(IPropertyBinderImplementation));
            }
#endif

#if WINRT
            RxApp.DeferredScheduler = System.Reactive.Concurrency.CoreDispatcherScheduler.Current;
#elif MONO
            // NB: Mono has like 37 UI Frameworks :)
#else
            if (!RxApp.InUnitTestRunner()) {
                RxApp.DeferredScheduler = System.Reactive.Concurrency.DispatcherScheduler.Current;
            }
#endif
        }

        static bool? inDesignMode;

        /// <summary>
        ///   Indicates whether or not the framework is in design-time mode.
        /// </summary>
        public static bool InDesignMode {
            get {
                if (inDesignMode == null) {
#if WINRT
                    inDesignMode = DesignMode.DesignModeEnabled;
#elif SILVERLIGHT
                    inDesignMode = DesignerProperties.IsInDesignTool;
#elif MONO
					return false;
#else
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    inDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

                    if (!inDesignMode.GetValueOrDefault(false) && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
                        inDesignMode = true;
#endif
                }

                return inDesignMode.GetValueOrDefault(false);
            }
        }
    }
}
