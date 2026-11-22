// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
#if REACTIVE_SHIM
using ReactiveUI.Reactive.Maui.Internal;
#else
using ReactiveUI.Maui.Internal;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif

/// <summary>A <see cref="ReactiveContentView{TViewModel}"/> that displays text content for use with CollectionView and DataTemplates.</summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="ReactiveContentView{TViewModel}" />
[DebuggerDisplay("{Text}, {Detail}")]
public class ReactiveTextItemView<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes
        .PublicParameterlessConstructor)]
TViewModel> : ReactiveContentView<TViewModel>
    where TViewModel : class
{
    /// <summary>The text bindable property for the primary text.</summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ReactiveTextItemView<TViewModel>),
        propertyChanged: static (bindable, _, newValue) =>
        {
            if (bindable is ReactiveTextItemView<TViewModel> view)
            {
                view._textLabel.Text = (string?)newValue;
            }
        });

    /// <summary>The detail bindable property for the secondary text.</summary>
    public static readonly BindableProperty DetailProperty = BindableProperty.Create(
        nameof(Detail),
        typeof(string),
        typeof(ReactiveTextItemView<TViewModel>),
        propertyChanged: static (bindable, _, newValue) =>
        {
            if (bindable is ReactiveTextItemView<TViewModel> view)
            {
                view._detailLabel.Text = (string?)newValue;
            }
        });

    /// <summary>The text color bindable property.</summary>
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(ReactiveTextItemView<TViewModel>),
        propertyChanged: static (bindable, _, newValue) =>
        {
            if (bindable is ReactiveTextItemView<TViewModel> view)
            {
                view._textLabel.TextColor = (Color?)newValue;
            }
        });

    /// <summary>The detail color bindable property.</summary>
    public static readonly BindableProperty DetailColorProperty = BindableProperty.Create(
        nameof(DetailColor),
        typeof(Color),
        typeof(ReactiveTextItemView<TViewModel>),
        propertyChanged: static (bindable, _, newValue) =>
        {
            if (bindable is ReactiveTextItemView<TViewModel> view)
            {
                view._detailLabel.TextColor = (Color?)newValue;
            }
        });

    /// <summary>The label that displays the primary text.</summary>
    private readonly Label _textLabel;

    /// <summary>The label that displays the detail text.</summary>
    private readonly Label _detailLabel;

    /// <summary>Initializes a new instance of the <see cref="ReactiveTextItemView{TViewModel}"/> class.</summary>
    public ReactiveTextItemView()
    {
        ItemViewLayoutHelper.CreateTextLabels(out _textLabel, out _detailLabel, Text, Detail);
        Content = ItemViewLayoutHelper.CreateTextStack(_textLabel, _detailLabel, ItemViewLayoutHelper.ContentPadding, fillHorizontal: false);
    }

    /// <summary>Gets or sets the primary text to display.</summary>
    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Gets or sets the detail text to display.</summary>
    public string? Detail
    {
        get => (string?)GetValue(DetailProperty);
        set => SetValue(DetailProperty, value);
    }

    /// <summary>Gets or sets the color of the primary text.</summary>
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>Gets or sets the color of the detail text.</summary>
    public Color DetailColor
    {
        get => (Color)GetValue(DetailColorProperty);
        set => SetValue(DetailColorProperty, value);
    }
}
