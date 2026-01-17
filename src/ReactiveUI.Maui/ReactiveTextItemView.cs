// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
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
public partial class ReactiveTextItemView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TViewModel> : ReactiveContentView<TViewModel>
    where TViewModel : class
{
    /// <summary>
    /// The text bindable property for the primary text.
    /// </summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ReactiveTextItemView<TViewModel>),
        default(string));

    /// <summary>
    /// The detail bindable property for the secondary text.
    /// </summary>
    public static readonly BindableProperty DetailProperty = BindableProperty.Create(
        nameof(Detail),
        typeof(string),
        typeof(ReactiveTextItemView<TViewModel>),
        default(string));

    /// <summary>
    /// The text color bindable property.
    /// </summary>
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(ReactiveTextItemView<TViewModel>),
        default(Color));

    /// <summary>
    /// The detail color bindable property.
    /// </summary>
    public static readonly BindableProperty DetailColorProperty = BindableProperty.Create(
        nameof(DetailColor),
        typeof(Color),
        typeof(ReactiveTextItemView<TViewModel>),
        default(Color));

    private readonly CompositeDisposable _propertyBindings = [];
    private readonly Label _textLabel;
    private readonly Label _detailLabel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTextItemView{TViewModel}"/> class.
    /// </summary>
    public ReactiveTextItemView()
    {
        _textLabel = new Label
        {
            FontSize = 16,
            VerticalOptions = LayoutOptions.Center,
            Text = Text // Set initial value
        };

        _detailLabel = new Label
        {
            FontSize = 12,
            VerticalOptions = LayoutOptions.Center,
            Opacity = 0.7,
            Text = Detail // Set initial value
        };

        var stackLayout = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            VerticalOptions = LayoutOptions.Center,
            Padding = 16,
            Children = { _textLabel, _detailLabel }
        };

        Content = stackLayout;

        // Use expression-based property observation instead of string-based bindings (AOT-safe)
        MauiReactiveHelpers.CreatePropertyValueObservable(this, nameof(Text), () => Text)
            .Subscribe(value => _textLabel.Text = value)
            .DisposeWith(_propertyBindings);

        MauiReactiveHelpers.CreatePropertyValueObservable(this, nameof(TextColor), () => TextColor)
            .Subscribe(value => _textLabel.TextColor = value)
            .DisposeWith(_propertyBindings);

        MauiReactiveHelpers.CreatePropertyValueObservable(this, nameof(Detail), () => Detail)
            .Subscribe(value => _detailLabel.Text = value)
            .DisposeWith(_propertyBindings);

        MauiReactiveHelpers.CreatePropertyValueObservable(this, nameof(DetailColor), () => DetailColor)
            .Subscribe(value => _detailLabel.TextColor = value)
            .DisposeWith(_propertyBindings);
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
}
