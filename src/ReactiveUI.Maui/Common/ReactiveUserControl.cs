// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if HAS_WINUI
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI
{
    /// <summary>
    /// A <see cref="UserControl"/> that is reactive.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is a <see cref="UserControl"/> that is also reactive. That is, it implements <see cref="IViewFor{TViewModel}"/>.
    /// You can extend this class to get an implementation of <see cref="IViewFor{TViewModel}"/> rather than writing one yourself.
    /// </para>
    /// <para>
    /// Note that the XAML for your control must specify the same base class, including the generic argument you provide for your view
    /// model. To do this, use the <c>TypeArguments</c> attribute as follows:
    /// <code>
    /// <![CDATA[
    /// <rxui:ReactiveUserControl
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
    /// </rxui:ReactiveUserControl>
    /// ]]>
    /// </code>
    /// </para>
    /// <para>
    /// Note that UWP and WinUI projects do not support the <c>TypeArguments</c> attribute. The XAML designer window in WPF projects also does not
    /// support generic types. To use <see cref="ReactiveUserControl{TViewModel}"/> in XAML documents you need to create a base class
    /// where you derive from <see cref="ReactiveUserControl{TViewModel}"/> with the type argument filled in.
    /// <code>
    /// <![CDATA[
    /// internal class YourViewBase : ReactiveUserControl<YourViewModel> { /* No code needed here */ }
    ///
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
    public
    class ReactiveUserControl<TViewModel> :
            UserControl, IViewFor<TViewModel>
            where TViewModel : class
    {
        /// <summary>
        /// The view model dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel",
                typeof(TViewModel),
                typeof(ReactiveUserControl<TViewModel>),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the binding root view model.
        /// </summary>
        public TViewModel? BindingRoot => ViewModel;

        /// <inheritdoc/>
        public TViewModel? ViewModel
        {
            get => (TViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel?)value;
        }
    }
}
#endif
