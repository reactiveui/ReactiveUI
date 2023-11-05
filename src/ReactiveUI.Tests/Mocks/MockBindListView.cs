// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// MockBindListView.
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    public class MockBindListView : UserControl, IViewFor<MockBindListViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(MockBindListViewModel), typeof(MockBindListView), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="MockBindListView"/> class.
        /// </summary>
        public MockBindListView()
        {
            ItemList = new();

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(@"
            <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                          xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <StackPanel Orientation=""Horizontal"">
                    <TextBlock
                        VerticalAlignment=""Stretch""
                        Text=""{Binding Name}""
                        TextAlignment=""Center"" />
                </StackPanel>
            </DataTemplate> "));
            ItemList.ItemTemplate = (DataTemplate)XamlReader.Load(ms);
            var ms1 = new MemoryStream(Encoding.UTF8.GetBytes(@"
            <ItemsPanelTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <StackPanel Orientation=""Horizontal"" />
             </ItemsPanelTemplate> "));
            ItemList.ItemsPanel = (ItemsPanelTemplate)XamlReader.Load(ms1);

            ViewModel = new();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.ListItems, v => v.ItemList.ItemsSource).DisposeWith(d);
                this.WhenAnyValue(v => v.ItemList.SelectedItem)
                    .Where(i => i is not null)
                    .Cast<MockBindListItemViewModel>()
                    .Do(_ => ItemList.UnselectAll())
                    .InvokeCommand(this, v => v!.ViewModel!.SelectItem).DisposeWith(d);
            });
        }

        /// <summary>
        /// Gets or sets the ViewModel corresponding to this specific View. This should be
        /// a DependencyProperty if you're using XAML.
        /// </summary>
        public MockBindListViewModel? ViewModel
        {
            get => (MockBindListViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public ListView ItemList { get; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MockBindListViewModel?)value;
        }
    }
}
