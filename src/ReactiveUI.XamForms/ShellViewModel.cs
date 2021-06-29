// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    /// <summary>
    /// ShellViewModel.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ShellContent" />
    public class ShellViewModel<TViewModel> : ShellContent, IActivatableView
        where TViewModel : class, new()
    {
        /// <summary>
        /// The contract property.
        /// </summary>
        public static readonly BindableProperty ContractProperty = BindableProperty.Create(
            nameof(Contract),
            typeof(string),
            typeof(ShellViewModel<TViewModel>),
            null);

        /// <summary>
        /// The view model property.
        /// </summary>
        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ShellViewModel<TViewModel>),
            default(TViewModel),
            BindingMode.Default,
            propertyChanged: ViewModelChanged);

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel{TViewModel}"/> class.
        /// </summary>
        public ShellViewModel()
        {
            ViewLocator = Locator.Current.GetService<IViewLocator>();
            ViewModel = new TViewModel();
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel
        {
            get => (TViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Gets or sets the contract.
        /// </summary>
        /// <value>
        /// The contract.
        /// </value>
        public string Contract
        {
            get => (string)GetValue(ContractProperty);
            set => SetValue(ContractProperty, value);
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        /// <value>
        /// The view locator.
        /// </value>
        public IViewLocator? ViewLocator { get; set; }

        private static void ViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShellViewModel<TViewModel> svm)
            {
                var view = svm.ViewLocator?.ResolveView(newValue as TViewModel, svm.Contract);
                svm.ContentTemplate = new DataTemplate(() => view);
            }
        }
    }
}
