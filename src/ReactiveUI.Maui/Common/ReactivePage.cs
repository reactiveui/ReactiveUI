// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
#if WINUI_TARGET
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// Alias Page: the Maui-windows TFM also imports Microsoft.Maui.Controls implicitly, so a bare Page would be
// ambiguous between Microsoft.UI.Xaml.Controls.Page and Microsoft.Maui.Controls.Page.
using Page = Microsoft.UI.Xaml.Controls.Page;
#else
using Microsoft.Maui.Controls;
#endif

#if IS_MAUI
#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif
#else
#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
#endif

/// <summary>A <see cref="Page"/> that is reactive.</summary>
/// <remarks>
/// <para>
/// This class is a <see cref="Page"/> that is also reactive. That is, it implements <see cref="IViewFor{TViewModel}"/>.
/// You can extend this class to get an implementation of <see cref="IViewFor{TViewModel}"/> rather than writing one yourself.
/// </para>
/// <para>
/// Note that the XAML for your control must specify the same base class, including the generic argument you provide for your view
/// model. To do this, use the <c>TypeArguments</c> attribute as follows:
/// <code>
/// <![CDATA[
/// <rxui:ReactivePage
///         x:Class="Foo.Bar.Views.YourView"
///         x:TypeArguments="vms:YourViewModel"
///         xmlns:rxui="http://reactiveui.net"
///         xmlns:vms="clr-namespace:Foo.Bar.ViewModels"
///         xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
///         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
///         xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
///         xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
///         mc:Ignorable="d">
///     <!-- view XAML here -->
/// </rxui:ReactivePage>
/// ]]>
/// </code>
/// </para>
/// <para>
/// Note that UWP and WinUI projects do not support the <c>TypeArguments</c> attribute. The XAML designer window in WPF projects also does not
/// support generic types. To use <see cref="ReactivePage{TViewModel}"/> in XAML documents you need to create a base class
/// where you derive from <see cref="ReactivePage{TViewModel}"/> with the type argument filled in.
/// <code>
/// <![CDATA[
/// internal class YourViewBase : ReactivePage<YourViewModel> { /* No code needed here */ }
/// public partial class YourView : YourViewBase
/// {
///     /* Your code */
/// }
/// ]]>
/// </code>
/// Then you can use this base class as root in your XAML document.
/// <code>
/// <![CDATA[
/// <views:YourViewBase
///         x:Class="Foo.Bar.Views.YourView"
///         xmlns:rxui="http://reactiveui.net"
///         xmlns:vms="clr-namespace:Foo.Bar.ViewModels"
///         xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
///         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
///         xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
///         xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
///         mc:Ignorable="d">
///     <!-- view XAML here -->
/// </views:YourViewBase>
/// ]]>
/// </code>
/// </para>
/// </remarks>
/// <typeparam name="TViewModel">
/// The type of the view model backing the view.
/// </typeparam>
#if WINUI_TARGET
public partial class ReactivePage<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TViewModel> :
    Page, IViewFor<TViewModel>
    where TViewModel : class
#else
public class ReactivePage<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TViewModel> :
    Page, IViewFor<TViewModel>
    where TViewModel : class
#endif
{
    /// <summary>The shared view model property for this closed generic page type.</summary>
#if WINUI_TARGET
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ReactivePage<TViewModel>),
            new(null));
#else
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
        nameof(ViewModel),
        typeof(TViewModel),
        typeof(ReactivePage<TViewModel>),
        propertyChanged: OnViewModelChanged);
#endif

    /// <summary>Gets the binding root view model.</summary>
    public TViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => (TViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }

#if !WINUI_TARGET
    /// <inheritdoc/>
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        ViewModel = BindingContext as TViewModel;
    }

    /// <summary>Updates the binding context when the view model changes.</summary>
    /// <param name="bindableObject">The bindable object whose property changed.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnViewModelChanged(BindableObject bindableObject, object? oldValue, object? newValue) =>
        bindableObject.BindingContext = newValue;
#endif
}
