// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows.Controls;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// A fixed reminder banner with nothing to bind. It uses the parameterless <c>WhenActivated</c> overload purely to
/// trigger <see cref="OfficeHoursBannerViewModel"/>'s activation lifecycle, which logs when the banner comes on
/// screen, without the empty <c>WhenActivated(_ =&gt; { })</c> boilerplate a view with real bindings would use.
/// </summary>
[DebuggerDisplay("OfficeHoursBannerView")]
public sealed class OfficeHoursBannerView : ReactiveUserControl<OfficeHoursBannerViewModel>
{
    /// <summary>The label the banner shows.</summary>
    private readonly TextBlock _text = new() { Text = "Office hours: 9am-4pm, Monday to Friday." };

    /// <summary>Initializes a new instance of the <see cref="OfficeHoursBannerView"/> class.</summary>
    public OfficeHoursBannerView()
    {
        Content = _text;
        ViewModel = new OfficeHoursBannerViewModel();
        ViewModel.Activator.Activated.Subscribe(static _ => Console.WriteLine("Office hours banner activated."));

        _ = this.WhenActivated();
    }
}
