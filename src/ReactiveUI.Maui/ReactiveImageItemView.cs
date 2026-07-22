// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif

/// <summary>A <see cref="ReactiveContentView{TViewModel}"/> that displays an image with text content for use with CollectionView and DataTemplates.</summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="ReactiveContentView{TViewModel}" />
public class ReactiveImageItemView<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes
        .PublicParameterlessConstructor)]
TViewModel> : ReactiveContentView<TViewModel>
    where TViewModel : class
{
    /// <summary>The image source bindable property.</summary>
    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(ReactiveImageItemView<TViewModel>),
        propertyChanged: OnImageSourceChanged);

    /// <summary>The text bindable property for the primary text.</summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ReactiveImageItemView<TViewModel>),
        propertyChanged: OnTextChanged);

    /// <summary>The detail bindable property for the secondary text.</summary>
    public static readonly BindableProperty DetailProperty = BindableProperty.Create(
        nameof(Detail),
        typeof(string),
        typeof(ReactiveImageItemView<TViewModel>),
        propertyChanged: OnDetailChanged);

    /// <summary>The text color bindable property.</summary>
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(ReactiveImageItemView<TViewModel>),
        propertyChanged: OnTextColorChanged);

    /// <summary>The detail color bindable property.</summary>
    public static readonly BindableProperty DetailColorProperty = BindableProperty.Create(
        nameof(DetailColor),
        typeof(Color),
        typeof(ReactiveImageItemView<TViewModel>),
        propertyChanged: OnDetailColorChanged);

    /// <summary>The width and height of the image, in device-independent units.</summary>
    private const double ImageSize = 40;

    /// <summary>The font size of the primary text label.</summary>
    private const double PrimaryFontSize = 16;

    /// <summary>The font size of the detail text label.</summary>
    private const double DetailFontSize = 12;

    /// <summary>The opacity applied to the detail text label.</summary>
    private const double DetailOpacity = 0.7;

    /// <summary>The padding around the content layout.</summary>
    private const double ContentPadding = 16;

    /// <summary>The spacing between the image and the text layout.</summary>
    private const double ContentSpacing = 12;

    /// <summary>The image element that displays the bound image source.</summary>
    private readonly Image _image;

    /// <summary>The label that displays the primary text.</summary>
    private readonly Label _textLabel;

    /// <summary>The label that displays the detail text.</summary>
    private readonly Label _detailLabel;

    /// <summary>Initializes a new instance of the <see cref="ReactiveImageItemView{TViewModel}"/> class.</summary>
    public ReactiveImageItemView()
    {
        _image = new()
        {
            WidthRequest = ImageSize,
            HeightRequest = ImageSize,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start
        };

        _textLabel = new()
        {
            FontSize = PrimaryFontSize,
            VerticalOptions = LayoutOptions.Center
        };

        _detailLabel = new()
        {
            FontSize = DetailFontSize,
            VerticalOptions = LayoutOptions.Center,
            Opacity = DetailOpacity
        };

        _image.Source = ImageSource;
        _textLabel.Text = Text;
        _detailLabel.Text = Detail;

        var textStackLayout = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Fill,
            Children = { _textLabel, _detailLabel }
        };

        Content = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            VerticalOptions = LayoutOptions.Center,
            Padding = ContentPadding,
            Spacing = ContentSpacing,
            Children = { _image, textStackLayout }
        };
    }

    /// <summary>Gets or sets the image source to display.</summary>
    public ImageSource? ImageSource
    {
        get => (ImageSource?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
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

    /// <summary>Handles changes to the <see cref="ImageSource"/> property by updating the image control.</summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="_">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnImageSourceChanged(BindableObject bindable, object? _, object? newValue)
    {
        if (bindable is not ReactiveImageItemView<TViewModel> view)
        {
            return;
        }

        view._image.Source = (ImageSource?)newValue;
    }

    /// <summary>Handles changes to the <see cref="Text"/> property by updating the primary text label.</summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="_">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnTextChanged(BindableObject bindable, object? _, object? newValue)
    {
        if (bindable is not ReactiveImageItemView<TViewModel> view)
        {
            return;
        }

        view._textLabel.Text = (string?)newValue;
    }

    /// <summary>Handles changes to the <see cref="TextColor"/> property by updating the primary text label color.</summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="_">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnTextColorChanged(BindableObject bindable, object? _, object? newValue)
    {
        if (bindable is not ReactiveImageItemView<TViewModel> view)
        {
            return;
        }

        view._textLabel.TextColor = (Color?)newValue;
    }

    /// <summary>Handles changes to the <see cref="Detail"/> property by updating the detail text label.</summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="_">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnDetailChanged(BindableObject bindable, object? _, object? newValue)
    {
        if (bindable is not ReactiveImageItemView<TViewModel> view)
        {
            return;
        }

        view._detailLabel.Text = (string?)newValue;
    }

    /// <summary>Handles changes to the <see cref="DetailColor"/> property by updating the detail text label color.</summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="_">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnDetailColorChanged(BindableObject bindable, object? _, object? newValue)
    {
        if (bindable is not ReactiveImageItemView<TViewModel> view)
        {
            return;
        }

        view._detailLabel.TextColor = (Color?)newValue;
    }
}
