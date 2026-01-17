// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

using Android.Views;

namespace ReactiveUI;

/// <summary>
/// Android implementation that provides binding to an ICommand in the ViewModel to a control in the View.
/// </summary>
[Preserve(AllMembers = true)]
public sealed class AndroidCommandBinders : FlexibleCommandBinder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidCommandBinders"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the <c>Enabled</c> property cannot be found on <see cref="View"/>, which is required for binding.
    /// </exception>
    public AndroidCommandBinders()
    {
        var viewType = typeof(View);

        // Cache reflection metadata once at registration time.
        var enabledProperty =
            viewType.GetRuntimeProperty("Enabled")
            ?? throw new InvalidOperationException(
                "Could not find property 'Enabled' on type View, which is needed for binding");

        // Precompute the setter once; ForEvent will no-op enabled sync if null (but for View it should exist).
        var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);

        Register(viewType, 9, (cmd, target, commandParameter) =>
        {
            // Keep existing behavior: ForEvent throws if cmd is null.
            // Also keep the "null commandParameter means use target" idiom (handled inside ForEvent overload).
            var view = (View)target!;

            return ForEvent(
                cmd,
                view,
                commandParameter,
                addHandler: h => view.Click += h,
                removeHandler: h => view.Click -= h,
                enabledSetter: enabledSetter);
        });
    }
}
