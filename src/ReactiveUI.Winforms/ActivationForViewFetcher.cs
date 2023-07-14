// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace ReactiveUI.Winforms;

/// <summary>
/// ActivationForViewFetcher is how ReactiveUI determine when a
/// View is activated or deactivated. This is usually only used when porting
/// ReactiveUI to a new UI framework.
/// </summary>
public class ActivationForViewFetcher : IActivationForViewFetcher, IEnableLogger
{
    private bool? _isDesignModeCache;

    /// <inheritdoc/>
    public int GetAffinityForView(Type view) => typeof(Control).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? 10 : 0;

    /// <inheritdoc/>
    public IObservable<bool> GetActivationForView(IActivatableView view)
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

            var handleDestroyed = Observable.FromEvent<EventHandler, bool>(
                                                                           eventHandler => (_, _) => eventHandler(false),
                                                                           h => control.HandleDestroyed += h,
                                                                           h => control.HandleDestroyed -= h);

            var handleCreated = Observable.FromEvent<EventHandler, bool>(
                                                                         eventHandler => (_, _) => eventHandler(true),
                                                                         h => control.HandleCreated += h,
                                                                         h => control.HandleCreated -= h);

            var visibleChanged = Observable.FromEvent<EventHandler, bool>(
                                                                          eventHandler => (_, _) => eventHandler(control.Visible),
                                                                          h => control.VisibleChanged += h,
                                                                          h => control.VisibleChanged -= h);

            var controlActivation = Observable.Merge(handleDestroyed, handleCreated, visibleChanged)
                                              .DistinctUntilChanged();

            if (view is Form form)
            {
                var formClosed = Observable.FromEvent<FormClosedEventHandler, bool>(
                 eventHandler =>
                 {
                     void Handler(object? sender, FormClosedEventArgs e) => eventHandler(control.Visible);
                     return Handler;
                 },
                 h => form.FormClosed += h,
                 h => form.FormClosed -= h);
                controlActivation = controlActivation.Merge(formClosed)
                                                     .DistinctUntilChanged();
            }

            return controlActivation;
        }

        if (view is null)
        {
            this.Log().Warn(
                            CultureInfo.InvariantCulture,
                            "Expected a view of type System.Windows.Forms.Control it was null");
        }
        else
        {
            // Show a friendly warning in the log that this view will never be activated
            this.Log().Warn(
                            CultureInfo.InvariantCulture,
                            "Expected a view of type System.Windows.Forms.Control but it is {0}.\r\nYou need to implement your own IActivationForViewFetcher for {0}.",
                            view.GetType());
        }

        return Observable<bool>.Empty;
    }

    private static bool GetIsDesignMode(Control control)
    {
        var isDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime || control.Site?.DesignMode == true || control.Parent?.Site?.DesignMode == true;

        return isDesignMode;
    }

    private bool GetCachedIsDesignMode(Control control)
    {
        _isDesignModeCache ??= GetIsDesignMode(control);

        return _isDesignModeCache.Value;
    }
}