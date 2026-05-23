// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace ReactiveUI.Maui;

/// <summary>
/// A <see cref="ReactiveContentView{TViewModel}"/> that displays text content similar to a TextCell,
/// but designed for use with CollectionView and DataTemplates. This serves as a modern replacement
/// for ReactiveTextCell which relied on the deprecated ListView.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="ReactiveContentView{TViewModel}" />
public class ReactiveTextItemView<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes
        .PublicParameterlessConstructor)]
    TViewModel> : ReactiveContentView<TViewModel>
    where TViewModel : class
{
    /// <summary>
    /// The text bindable property for the primary text.
    /// </summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ReactiveTextItemView<TViewModel>),
        propertyChanged: OnTextChanged);

    /// <summary>
    /// The detail bindable property for the secondary text.
    /// </summary>
    public static readonly BindableProperty DetailProperty = BindableProperty.Create(
        nameof(Detail),
        typeof(string),
        typeof(ReactiveTextItemView<TViewModel>),
        propertyChanged: OnDetailChanged);

    /// <summary>
    /// The text color bindable property.
    /// </summary>
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(ReactiveTextItemView<TViewModel>),
        propertyChanged: OnTextColorChanged);

    /// <summary>
    /// The detail color bindable property.
    /// </summary>
    public static readonly BindableProperty DetailColorProperty = BindableProperty.Create(
        nameof(DetailColor),
        typeof(Color),
        typeof(ReactiveTextItemView<TViewModel>),
        propertyChanged: OnDetailColorChanged);

    /// <summary>
    /// The font size of the primary text label.
    /// </summary>
    private const double PrimaryFontSize = 16;

    /// <summary>
    /// The font size of the detail text label.
    /// </summary>
    private const double DetailFontSize = 12;

    /// <summary>
    /// The opacity applied to the detail text label.
    /// </summary>
    private const double DetailOpacity = 0.7;

    /// <summary>
    /// The padding around the content layout.
    /// </summary>
    private const double ContentPadding = 16;

    /// <summary>
    /// The label that displays the primary text.
    /// </summary>
    private readonly Label _textLabel;

    /// <summary>
    /// The label that displays the detail text.
    /// </summary>
    private readonly Label _detailLabel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTextItemView{TViewModel}"/> class.
    /// </summary>
    public ReactiveTextItemView()
    {
        _textLabel = new()
        {
            FontSize = PrimaryFontSize, VerticalOptions = LayoutOptions.Center, Text = Text // Set initial value
        };

        _detailLabel = new()
        {
            FontSize = DetailFontSize, VerticalOptions = LayoutOptions.Center, Opacity = DetailOpacity, Text = Detail // Set initial value
        };

        Content = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            VerticalOptions = LayoutOptions.Center,
            Padding = ContentPadding,
            Children = { _textLabel, _detailLabel }
        };
    }

    /// <summary>
    /// Gets or sets the primary text to display.
    /// </summary>
    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the detail text to display.
    /// </summary>
    public string? Detail
    {
        get => (string?)GetValue(DetailProperty);
        set => SetValue(DetailProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the primary text.
    /// </summary>
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the detail text.
    /// </summary>
    public Color DetailColor
    {
        get => (Color)GetValue(DetailColorProperty);
        set => SetValue(DetailColorProperty, value);
    }

    /// <summary>
    /// Handles changes to the <see cref="Text"/> property by updating the primary text label.
    /// </summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnTextChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not ReactiveTextItemView<TViewModel> view)
        {
            return;
        }

        view._textLabel.Text = (string?)newValue;
    }

    /// <summary>
    /// Handles changes to the <see cref="Detail"/> property by updating the detail text label.
    /// </summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnDetailChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not ReactiveTextItemView<TViewModel> view)
        {
            return;
        }

        view._detailLabel.Text = (string?)newValue;
    }

    /// <summary>
    /// Handles changes to the <see cref="TextColor"/> property by updating the primary text label color.
    /// </summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnTextColorChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not ReactiveTextItemView<TViewModel> view)
        {
            return;
        }

        view._textLabel.TextColor = (Color?)newValue;
    }

    /// <summary>
    /// Handles changes to the <see cref="DetailColor"/> property by updating the detail text label color.
    /// </summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnDetailColorChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not ReactiveTextItemView<TViewModel> view)
        {
            return;
        }

        view._detailLabel.TextColor = (Color?)newValue;
    }
}
