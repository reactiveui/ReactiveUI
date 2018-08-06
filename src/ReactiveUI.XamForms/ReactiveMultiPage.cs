// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using ReactiveUI;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    public abstract class ReactiveMultiPage<TPage, TViewModel> : MultiPage<TPage>, IViewFor<TViewModel>
        where TPage : Page
        where TViewModel : class
    {
        /// <summary>
        /// The ViewModel to display
        /// </summary>
        public TViewModel ViewModel {
            get { return (TViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ReactiveMultiPage<TPage, TViewModel>),
            default(TViewModel),
            BindingMode.OneWay,
            propertyChanged: OnViewModelChanged);

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            this.ViewModel = this.BindingContext as TViewModel;
        }

        private static void OnViewModelChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            bindableObject.BindingContext = newValue;
        }
    }
}