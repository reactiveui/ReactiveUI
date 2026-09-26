// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>The view for <see cref="AlertViewModel"/>. The view locator finds it because it implements <see cref="IViewFor{T}"/>.</summary>
[System.Diagnostics.DebuggerDisplay("AlertView")]
public sealed class AlertView : ReactiveUserControl<AlertViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="AlertView"/> class.</summary>
    public AlertView()
    {
        Content = Label;

        this.WhenActivated(disposables =>
        {
            IDisposable subscription = this.OneWayBind(ViewModel, viewModel => viewModel.Message, view => view.Label.Text);
            disposables(subscription);
        });
    }

    /// <summary>Gets the label showing the current alert.</summary>
    internal TextBlock Label { get; } = new();
}
