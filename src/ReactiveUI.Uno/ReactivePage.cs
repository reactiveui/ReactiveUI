// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ReactiveUI.Uno
{
    /// <summary>
    /// This is a <see cref="Page"/> that is also an <see cref="IViewFor{T}"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public class ReactivePage<TViewModel> : Page, IViewFor<TViewModel>
        where TViewModel : class
    {
        /// <summary>
        /// The view model bindable property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ReactivePage<TViewModel>),
            new PropertyMetadata(default, OnViewModelChanged));

        /// <inheritdoc/>
        public TViewModel ViewModel
        {
            get => (TViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <inheritdoc/>
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        /// <inheritdoc/>
        protected override void OnDataContextChanged()
        {
            base.OnDataContextChanged();
            ViewModel = DataContext as TViewModel;
        }

        private static void OnViewModelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
        }
    }
}
