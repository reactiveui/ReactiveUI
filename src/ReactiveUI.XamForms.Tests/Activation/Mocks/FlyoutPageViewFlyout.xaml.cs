// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReactiveUI.XamForms.Tests.Activation.Mocks
{
    /// <summary>
    /// Flyout Page View Flyout.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutPageViewFlyout : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlyoutPageViewFlyout"/> class.
        /// </summary>
        public FlyoutPageViewFlyout()
        {
            InitializeComponent();

            BindingContext = new FlyoutPageView1FlyoutViewModel();
            ListView = MenuItemsListView;
        }

        /// <summary>
        /// Gets the ListView.
        /// </summary>
        /// <value>
        /// The ListView.
        /// </value>
        public ListView ListView { get; }

        private class FlyoutPageView1FlyoutViewModel : INotifyPropertyChanged
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FlyoutPageView1FlyoutViewModel"/> class.
            /// </summary>
            public FlyoutPageView1FlyoutViewModel()
            {
                MenuItems = new ObservableCollection<FlyoutPageViewFlyoutMenuItem>(new[]
                {
                    new FlyoutPageViewFlyoutMenuItem { Id = 0, Title = "Page 1" },
                    new FlyoutPageViewFlyoutMenuItem { Id = 1, Title = "Page 2" },
                    new FlyoutPageViewFlyoutMenuItem { Id = 2, Title = "Page 3" },
                    new FlyoutPageViewFlyoutMenuItem { Id = 3, Title = "Page 4" },
                    new FlyoutPageViewFlyoutMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler? PropertyChanged;

            /// <summary>
            /// Gets or sets the menu items.
            /// </summary>
            /// <value>
            /// The menu items.
            /// </value>
            public ObservableCollection<FlyoutPageViewFlyoutMenuItem> MenuItems { get; set; }

            private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged is null)
                {
                    return;
                }

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
