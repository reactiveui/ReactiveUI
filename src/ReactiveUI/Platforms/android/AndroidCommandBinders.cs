// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Android.Views;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Android implementation that provides binding to an ICommand in the ViewModel to a control in the View.</summary>
[Preserve(AllMembers = true)]
public sealed class AndroidCommandBinders : FlexibleCommandBinder
{
    /// <summary>The binding affinity used when registering the <see cref="View.Click"/> command binding.</summary>
    private const int ViewClickBindingAffinity = 9;

    /// <summary>Initializes a new instance of the <see cref="AndroidCommandBinders"/> class.</summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the <c>Enabled</c> property cannot be found on <see cref="View"/>, which is required for binding.
    /// </exception>
    public AndroidCommandBinders()
    {
        var viewType = typeof(View);

        var enabledProperty =
            viewType.GetRuntimeProperty("Enabled")
            ?? throw new InvalidOperationException(
                "Could not find property 'Enabled' on type View, which is needed for binding");

        var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);

        Register(viewType, ViewClickBindingAffinity, (cmd, target, commandParameter) =>
        {
            var view = (View)target!;

            return ForEvent(
                cmd,
                view,
                commandParameter,
                h => view.Click += h,
                h => view.Click -= h,
                enabledSetter);
        });
    }
}
