using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

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
            RxApp.Register(typeof (DependencyObjectObservableForProperty), typeof (ICreatesObservableForProperty));
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
