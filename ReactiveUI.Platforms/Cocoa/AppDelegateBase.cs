using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Splat;

namespace ReactiveUI.Cocoa
{
    public abstract class AppDelegateBase : UIApplicationDelegate, IEnableLogger, IPlatformOperations
    {


        public override bool WillFinishLaunching(UIApplication application, NSDictionary launchOptions)
        {
            RegisterServices();
            RegisterViewTypes();

            return base.WillFinishLaunching(application, launchOptions);
        }

        /// <summary>
        /// Registers the services.
        /// </summary>
        public virtual void RegisterServices()
        {
            var resolver = Locator.CurrentMutable;

            // Register ReactiveUI
            this.Log().Debug("AppDelegateBase.InitializeReactiveUI()");

            // Register ReactiveUI stuff
            (new ReactiveUI.Registrations()).Register((f, t) => resolver.Register(f, t));

            // Register Platform stuff 
            (new ReactiveUI.Cocoa.Registrations()).Register((f, t) => resolver.Register(f, t));
        }


        /// <summary>
        /// Registers the view types. 
        /// </summary>
        public abstract void RegisterViewTypes();


        public string GetOrientation()
        {
            return PlatformOperations.GetOrientation();
        }

        public DeviceOrientation GetOrientationEnum()
        {
            return PlatformOperations.GetOrientationEnum();
        }
    }
}