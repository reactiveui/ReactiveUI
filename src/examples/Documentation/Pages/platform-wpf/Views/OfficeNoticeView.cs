// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows.Controls;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// The school office's view for <see cref="OfficeNoticeViewModel"/>. It stands in for a view from a library built
/// without the source generator: <see cref="ExcludeFromViewRegistrationAttribute"/> keeps it out of the generated view
/// lookup, and the app registers it with the service locator only.
/// </summary>
[ExcludeFromViewRegistration]
[DebuggerDisplay("OfficeNoticeView")]
public sealed class OfficeNoticeView : ReactiveUserControl<OfficeNoticeViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="OfficeNoticeView"/> class.</summary>
    public OfficeNoticeView()
    {
        Content = MessageText;
        _ = this.WhenActivated(d => this.OneWayBind(ViewModel, vm => vm.Message, v => v.MessageText.Text).DisposeWith(d));
    }

    /// <summary>Gets the text block that shows the notice.</summary>
    public TextBlock MessageText { get; } = new();
}
