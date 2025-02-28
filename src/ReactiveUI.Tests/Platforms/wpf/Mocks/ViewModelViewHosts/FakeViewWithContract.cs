// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;

namespace ReactiveUI.Tests.Wpf;

public static class FakeViewWithContract
{
    internal const string ContractA = "ContractA";
    internal const string ContractB = "ContractB";

    public class MyViewModel : ReactiveObject
    {
    }

    /// <summary>
    /// Used as the default view with no contracted.
    /// </summary>
    public class View0 : UserControl, IViewFor<MyViewModel>
    {

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MyViewModel), typeof(View0), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ViewModel.
        /// </summary>
        public MyViewModel? ViewModel
        {
            get { return (MyViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (MyViewModel?)value; }
    }

    /// <summary>
    /// the view with ContractA.
    /// </summary>
    public class ViewA : UserControl, IViewFor<MyViewModel>
    {

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MyViewModel), typeof(ViewA), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ViewModel.
        /// </summary>
        public MyViewModel? ViewModel
        {
            get { return (MyViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (MyViewModel?)value; }
    }

    /// <summary>
    /// the view as ContractB.
    /// </summary>
    public class ViewB : UserControl, IViewFor<MyViewModel>
    {

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MyViewModel), typeof(ViewB), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ViewModel.
        /// </summary>
        public MyViewModel? ViewModel
        {
            get { return (MyViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (MyViewModel?)value; }
    }
}
