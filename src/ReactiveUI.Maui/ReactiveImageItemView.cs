// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;

namespace ReactiveUI.Maui;

/// <summary>
/// A <see cref="ReactiveContentView{TViewModel}"/> that displays an image with text content similar to an ImageCell,
/// but designed for use with CollectionView and DataTemplates. This serves as a modern replacement
/// for ReactiveImageCell which relied on the deprecated ListView.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="ReactiveContentView{TViewModel}" />
#if NET6_0_OR_GREATER
[RequiresDynamicCode("ReactiveImageItemView uses methods that require dynamic code generation")]
[RequiresUnreferencedCode("ReactiveImageItemView uses methods that may require unreferenced code")]
#endif
public partial class ReactiveImageItemView<TViewModel> : ReactiveContentView<TViewModel>
    where TViewModel : class
{
    /// <summary>
    /// The image source bindable property.
    /// </summary>
    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(ReactiveImageItemView<TViewModel>),
        default(ImageSource));

    /// <summary>
    /// The text bindable property for the primary text.
    /// </summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ReactiveImageItemView<TViewModel>),
        default(string));

    /// <summary>
    /// The detail bindable property for the secondary text.
    /// </summary>
    public static readonly BindableProperty DetailProperty = BindableProperty.Create(
        nameof(Detail),
        typeof(string),
        typeof(ReactiveImageItemView<TViewModel>),
        default(string));

    /// <summary>
    /// The text color bindable property.
    /// </summary>
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(ReactiveImageItemView<TViewModel>),
        default(Color));

    /// <summary>
    /// The detail color bindable property.
    /// </summary>
    public static readonly BindableProperty DetailColorProperty = BindableProperty.Create(
        nameof(DetailColor),
        typeof(Color),
        typeof(ReactiveImageItemView<TViewModel>),
        default(Color));

    private readonly Image _image;
    private readonly Label _textLabel;
    private readonly Label _detailLabel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveImageItemView{TViewModel}"/> class.
    /// </summary>
    public ReactiveImageItemView()
    {
        _image = new Image
        {
            WidthRequest = 40,
            HeightRequest = 40,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            Aspect = Aspect.AspectFill
        };

        _textLabel = new Label
        {
            FontSize = 16,
            VerticalOptions = LayoutOptions.Center
        };

        _detailLabel = new Label
        {
            FontSize = 12,
            VerticalOptions = LayoutOptions.Center,
            Opacity = 0.7
        };

        var textStackLayout = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            Children = { _textLabel, _detailLabel }
        };

        var mainStackLayout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(16, 8),
            Spacing = 12,
            Children = { _image, textStackLayout }
        };

        Content = mainStackLayout;

        // Bind the control properties to the bindable properties
        _image.SetBinding(Image.SourceProperty, new Binding(nameof(ImageSource), source: this));
        _textLabel.SetBinding(Label.TextProperty, new Binding(nameof(Text), source: this));
        _textLabel.SetBinding(Label.TextColorProperty, new Binding(nameof(TextColor), source: this));
        _detailLabel.SetBinding(Label.TextProperty, new Binding(nameof(Detail), source: this));
        _detailLabel.SetBinding(Label.TextColorProperty, new Binding(nameof(DetailColor), source: this));
    }

    /// <summary>
    /// Gets or sets the image source to display.
    /// </summary>
    public ImageSource? ImageSource
    {
        get => (ImageSource?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
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