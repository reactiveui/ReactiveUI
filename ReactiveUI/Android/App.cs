using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI.Android;
using Splat;

namespace ReactiveUI.Android
{
    public abstract class App : Application, IEnableLogger
    {

        private static App _current;

        // ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="transfer">The transfer.</param>
        public App(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            // => OnCreate
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            // => OnCreate
        }


        // Properties

        /// <summary>
        /// Gets the current application instance.
        /// </summary>
        public static App Instance
        {
            get
            {
                return _current;
            }
        }

        // Event-Handler

        /// <summary>
        /// Called when [create].
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();
            AndroidEnvironment.UnhandledExceptionRaiser += OnAppUnhandledException;

            _current = this;

            RegisterServices();
            RegisterViewTypes();
        }

        /// <summary>
        /// Registers the services.
        /// </summary>
        public virtual void RegisterServices()
        {
            var resolver = Locator.CurrentMutable;

            resolver.RegisterConstant(new AndroidLogger(), typeof(ILogger));

            // Register ReactiveUI
            this.Log().Debug("App.InitializeReactiveUI()");

            // Register ReactiveUI stuff
            (new ReactiveUI.Registrations()).Register((f, t) => resolver.Register(f, t));
            //(new Mobile.Registrations()).Register((f, t) => resolver.Register(f, t));

            // Register Android stuff 
            (new ReactiveUI.PlatformRegistrations()).Register((f, t) => resolver.Register(f, t));
        }


        /// <summary>
        /// Registers the view types. 
        /// </summary>
        public abstract void RegisterViewTypes();

        /// <summary>
        /// Called when [application unhandled exception].
        /// When the unhandled exception handler has fired, the application has already crashed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RaiseThrowableEventArgs"/> instance containing the event data.</param>
        public virtual void OnAppUnhandledException(object sender, RaiseThrowableEventArgs e)
        {
            // When the unhandled exception handler has fired, the application has already crashed!!!
            //this.LogException(e.Exception, 0, 0);
            this.Log().ErrorException("AppUnhandledException: : " + e.Exception.GetType().FullName, e.Exception);
        } 

    }
}