// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using PropertyMetadata = System.Windows.PropertyMetadata;

namespace ReactiveUI.Tests.Wpf.Mocks.ViewModelViewHosts;

/// <summary>
/// Container for mock views and a view model used by view-host contract resolution tests.
/// </summary>
public static class FakeViewWithContract
{
    /// <summary>
    /// The contract name for <see cref="ViewA"/>.
    /// </summary>
    internal const string ContractA = "ContractA";

    /// <summary>
    /// The contract name for <see cref="ViewB"/>.
    /// </summary>
    internal const string ContractB = "ContractB";

    /// <summary>
    /// A mock view model shared by the contract views.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2094:Classes should not be empty", Justification = "Marker type for tests.")]
    public sealed class MyViewModel : ReactiveUI.ReactiveObject;

    /// <summary>
    /// Used as the default view with no contracted.
    /// </summary>
    public sealed class View0 : UserControl, IViewFor<MyViewModel>
    {
        /// <summary>
        /// Identifies the <see cref="ViewModel"/> dependency property.
        /// </summary>
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

        /// <inheritdoc/>
        object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (MyViewModel?)value; }
    }

    /// <summary>
    /// the view with ContractA.
    /// </summary>
    public sealed class ViewA : UserControl, IViewFor<MyViewModel>
    {
        /// <summary>
        /// Identifies the <see cref="ViewModel"/> dependency property.
        /// </summary>
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

        /// <inheritdoc/>
        object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (MyViewModel?)value; }
    }

    /// <summary>
    /// the view as ContractB.
    /// </summary>
    public sealed class ViewB : UserControl, IViewFor<MyViewModel>
    {
        /// <summary>
        /// Identifies the <see cref="ViewModel"/> dependency property.
        /// </summary>
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

        /// <inheritdoc/>
        object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (MyViewModel?)value; }
    }
}
