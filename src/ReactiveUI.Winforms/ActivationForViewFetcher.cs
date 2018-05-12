// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Forms;
using Splat;

namespace ReactiveUI.Winforms
{
    public class ActivationForViewFetcher : IActivationForViewFetcher, IEnableLogger
    {
        public int GetAffinityForView(Type view)
        {
            return (typeof(Control).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())) ? 10 : 0;
        }

        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            // Check for design time
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) {
                return Observable<bool>.Default;
            }

            // Startup: Control.HandleCreated > Control.BindingContextChanged > Form.Load > Control.VisibleChanged > Form.Activated > Form.Shown
            // Shutdown: Form.Closing > Form.FormClosing > Form.Closed > Form.FormClosed > Form.Deactivate
            // https://docs.microsoft.com/en-us/dotnet/framework/winforms/order-of-events-in-windows-forms

            if (view is Form form) {
                var formActivated = Observable.FromEventPattern(form, "Activated").Select(_ => true);
                var formDeactivate = Observable.FromEventPattern(form, "Deactivate").Select(_ => false);
                return Observable.Merge(formDeactivate, formActivated)
                    .DistinctUntilChanged();
            }
            else if (view is Control control) {
                var controlVisible = Observable.FromEventPattern(control, "VisibleChanged").Select(_ => control.Visible);
                var handleDestroyed = Observable.FromEventPattern(control, "HandleDestroyed").Select(_ => false);
                var handleCreated = Observable.FromEventPattern(control, "HandleCreated").Select(_ => true);
                return Observable.Merge(controlVisible, handleDestroyed, handleCreated)
                    .DistinctUntilChanged();
            }
            else {
                // Show a friendly warning in the log that this view will never be activated
                this.Log().Warn("Expected a view of type System.Windows.Forms.Control but it is {0}.\r\nYou need to implement your own IActivationForViewFetcher for {0}.", view.GetType());
                return Observable<bool>.Empty;
            }
        }
    }
}
