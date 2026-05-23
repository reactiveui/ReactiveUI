// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using ReactiveUI.Internal;
using Splat;

namespace ReactiveUI.Winforms;

/// <summary>
/// ActivationForViewFetcher is how ReactiveUI determine when a
/// View is activated or deactivated. This is usually only used when porting
/// ReactiveUI to a new UI framework.
/// </summary>
public class ActivationForViewFetcher : IActivationForViewFetcher, IEnableLogger
{
    /// <summary>Caches whether the control is being used at design time.</summary>
    private bool? _isDesignModeCache;

    /// <inheritdoc/>
    public int GetAffinityForView(Type view) =>
        typeof(Control).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? BindingAffinity.ExactType : 0;

    /// <inheritdoc/>
    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        switch (view)
        {
            // Startup: Control.HandleCreated > Control.BindingContextChanged > Form.Load > Control.VisibleChanged > Form.Activated > Form.Shown
            // Shutdown: Form.Closing > Form.FormClosing > Form.Closed > Form.FormClosed > Form.Deactivate
            // https://docs.microsoft.com/en-us/dotnet/framework/winforms/order-of-events-in-windows-forms
            case Control control when GetCachedIsDesignMode(control):
                break;
            case Control control:
                return GetActivationForControl(control);

            case null:
                {
                    this.Log().Warn(
                                    CultureInfo.InvariantCulture,
                                    "Expected a view of type System.Windows.Forms.Control it was null");
                    break;
                }

            default:
                {
                    // Show a friendly warning in the log that this view will never be activated
                    this.Log().Warn(
                        CultureInfo.InvariantCulture,
                        "Expected a view of type System.Windows.Forms.Control but it is {0}.\r\nYou need to implement your own IActivationForViewFetcher for {0}.",
                        view.GetType());
                    break;
                }
        }

        return EmptyObservable<bool>.Instance;
    }

    /// <summary>Builds the activation observable for a WinForms control, including form lifecycle when applicable.</summary>
    /// <param name="control">The control to observe.</param>
    /// <returns>An observable that signals when the control is activated and deactivated.</returns>
    private static MergedDistinctObservable<bool> GetActivationForControl(Control control)
    {
        var handleDestroyed = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(false);
            control.HandleDestroyed += Handler;
            return new ActionDisposable(() => control.HandleDestroyed -= Handler);
        });

        var handleCreated = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(true);
            control.HandleCreated += Handler;
            return new ActionDisposable(() => control.HandleCreated -= Handler);
        });

        var visibleChanged = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(control.Visible);
            control.VisibleChanged += Handler;
            return new ActionDisposable(() => control.VisibleChanged -= Handler);
        });

        if (control is not Form form)
        {
            // Replaces Merge(handleDestroyed, handleCreated, visibleChanged).DistinctUntilChanged().
            return new(handleDestroyed, handleCreated, visibleChanged);
        }

        var formClosed = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, FormClosedEventArgs e) => onNext(control.Visible);
            form.FormClosed += Handler;
            return new ActionDisposable(() => form.FormClosed -= Handler);
        });

        // Replaces Merge(...).Merge(formClosed).DistinctUntilChanged().
        return new(handleDestroyed, handleCreated, visibleChanged, formClosed);
    }

    /// <summary>Determines whether the control is currently running at design time.</summary>
    /// <param name="control">The control to inspect.</param>
    /// <returns>true if the control is in design mode; otherwise, false.</returns>
    private static bool GetIsDesignMode(Control control) =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime || control.GetIsAncestorSiteInDesignMode();

    /// <summary>Gets the cached design-mode state for the control, computing it on first access.</summary>
    /// <param name="control">The control to inspect.</param>
    /// <returns>true if the control is in design mode; otherwise, false.</returns>
    private bool GetCachedIsDesignMode(Control control)
    {
        _isDesignModeCache ??= GetIsDesignMode(control);

        return _isDesignModeCache.Value;
    }
}
