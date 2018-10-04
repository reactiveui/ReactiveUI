// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Forms;
using Splat;

namespace ReactiveUI.Winforms
{
    /// <summary>
    /// ActiveationForViewFetcher is how ReactiveUI determine when a
    /// View is activated or deactivated. This is usually only used when porting
    /// ReactiveUI to a new UI framework.
    /// </summary>
    public class ActivationForViewFetcher : IActivationForViewFetcher, IEnableLogger
    {
        private bool? _isDesignModeCache;

        /// <inheritdoc/>
        public int GetAffinityForView(Type view)
        {
            return typeof(Control).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? 10 : 0;
        }

        /// <inheritdoc/>
        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            // Startup: Control.HandleCreated > Control.BindingContextChanged > Form.Load > Control.VisibleChanged > Form.Activated > Form.Shown
            // Shutdown: Form.Closing > Form.FormClosing > Form.Closed > Form.FormClosed > Form.Deactivate
            // https://docs.microsoft.com/en-us/dotnet/framework/winforms/order-of-events-in-windows-forms
            if (view is Control control)
            {
                if (GetCachedIsDesignMode(control))
                {
                    return Observable<bool>.Empty;
                }

                var handleDestroyed = Observable.FromEventPattern(control, "HandleDestroyed").Select(_ => false);
                var handleCreated = Observable.FromEventPattern(control, "HandleCreated").Select(_ => true);
                var visibleChanged = Observable.FromEventPattern(control, "VisibleChanged").Select(_ => control.Visible);

                var controlActivation = Observable.Merge(handleDestroyed, handleCreated, visibleChanged)
                    .DistinctUntilChanged();

                if (view is Form form)
                {
                    var formClosed = Observable.FromEventPattern(form, "FormClosed").Select(_ => false);
                    controlActivation = controlActivation.Merge(formClosed)
                        .DistinctUntilChanged();
                }

                return controlActivation;
            }

            // Show a friendly warning in the log that this view will never be activated
            this.Log().Warn("Expected a view of type System.Windows.Forms.Control but it is {0}.\r\nYou need to implement your own IActivationForViewFetcher for {0}.", view.GetType());

            return Observable<bool>.Empty;
        }

        private bool GetCachedIsDesignMode(Control control)
        {
            if (_isDesignModeCache == null)
            {
                _isDesignModeCache = GetIsDesignMode(control);
            }

            return _isDesignModeCache.Value;
        }

        private bool GetIsDesignMode(Control control)
        {
            var isDesignMode = false;

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                isDesignMode = true;
            }

            if (control.Site?.DesignMode == true)
            {
                isDesignMode = true;
            }

            if (control.Parent?.Site?.DesignMode == true)
            {
                isDesignMode = true;
            }

            return isDesignMode;
        }
    }
}
