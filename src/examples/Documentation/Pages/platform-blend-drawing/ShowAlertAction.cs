// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace ReactiveUI.Documentation.PlatformBlendDrawing;

/// <summary>
/// A Blend action that writes whatever <see cref="Blend.ObservableTrigger"/> hands it to a label. Every value the
/// trigger's observable emits reaches <see cref="Invoke"/> as <c>parameter</c>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("ShowAlertAction")]
public sealed class ShowAlertAction : TriggerAction<FrameworkElement>
{
    /// <summary>The label the alert text is written to.</summary>
    public static readonly DependencyProperty TargetTextProperty =
        DependencyProperty.Register(nameof(TargetText), typeof(TextBlock), typeof(ShowAlertAction), new PropertyMetadata(null));

    /// <summary>Gets or sets the label the alert text is written to.</summary>
    public TextBlock? TargetText
    {
        get => (TextBlock?)GetValue(TargetTextProperty);
        set => SetValue(TargetTextProperty, value);
    }

    /// <inheritdoc/>
    protected override void Invoke(object parameter)
    {
        if (TargetText is null)
        {
            return;
        }

        TargetText.Text = parameter?.ToString() ?? string.Empty;
    }
}
