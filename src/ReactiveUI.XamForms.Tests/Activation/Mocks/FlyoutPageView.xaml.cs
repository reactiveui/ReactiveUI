// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReactiveUI.XamForms.Tests.Activation.Mocks
{
    /// <summary>
    /// Flyout Page View.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutPageView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlyoutPageView"/> class.
        /// </summary>
        public FlyoutPageView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                IsActiveCount++;
                d(Disposable.Create(() => IsActiveCount--));
            });

            FlyoutPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        /// <summary>
        /// Gets or sets the active count.
        /// </summary>
        public int IsActiveCount { get; set; }

        private void ListView_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as FlyoutPageViewFlyoutMenuItem;
            if (item is null)
            {
                return;
            }

            var page = (Page?)Activator.CreateInstance(item.TargetType);
            page!.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            FlyoutPage.ListView.SelectedItem = null;
        }
    }
}
