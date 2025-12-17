// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides a non-generic abstraction over views so infrastructure can interact with <see cref="IViewFor{T}"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// Most application code implements <see cref="IViewFor{T}"/> instead of this interface directly. The non-generic
/// type exists so routing and binding helpers can store heterogeneous view references at runtime while still exposing
/// the <see cref="ViewModel"/> property.
/// </para>
/// </remarks>
public interface IViewFor : IActivatableView
{
    /// <summary>
    /// Gets or sets the view model associated with the view.
    /// </summary>
    object? ViewModel { get; set; }
}

#pragma warning disable SA1402 // File may only contain a single type
/// <summary>
/// Implement this interface on views to participate in ReactiveUI routing, activation, and binding.
/// </summary>
/// <typeparam name="T">The type of view model presented by the view.</typeparam>
/// <remarks>
/// <para>
/// Views typically expose <see cref="ViewModel"/> as a bindable property (dependency property in XAML, BindableProperty
/// in .NET MAUI, etc.). Implementations should also handle activation by calling <c>WhenActivated</c> inside the view to
/// manage subscriptions.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// public partial class LoginView : ReactiveUserControl<LoginViewModel>
/// {
///     public LoginView()
///     {
///         InitializeComponent();
///         this.WhenActivated(disposables =>
///             this.Bind(ViewModel, vm => vm.UserName, v => v.UserNameTextBox.Text)
///                 .DisposeWith(disposables));
///     }
/// }
/// ]]>
/// </code>
/// </example>
public interface IViewFor<T> : IViewFor
#pragma warning restore SA1402 // File may only contain a single type
    where T : class
{
    /// <summary>
    /// Gets or sets the strongly typed view model. Override this property to integrate with the platform's binding system.
    /// </summary>
    new T? ViewModel { get; set; }
}
