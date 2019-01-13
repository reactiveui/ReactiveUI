﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    /// <summary>
    /// This is an <see cref="CarouselPage"/> that is also an <see cref="IViewFor{T}"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="Xamarin.Forms.CarouselPage" />
    /// <seealso cref="ReactiveUI.IViewFor{TViewModel}" />
    public class ReactiveCarouselPage<TViewModel> : CarouselPage, IViewFor<TViewModel>
        where TViewModel : class
    {
        /// <summary>
        /// The view model bindable property.
        /// </summary>
        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ReactiveCarouselPage<TViewModel>),
            default(TViewModel),
            BindingMode.OneWay,
            propertyChanged: OnViewModelChanged);

        /// <summary>
        /// Gets or sets the ViewModel to display.
        /// </summary>
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
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            ViewModel = BindingContext as TViewModel;
        }

        private static void OnViewModelChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            bindableObject.BindingContext = newValue;
        }
    }
}
