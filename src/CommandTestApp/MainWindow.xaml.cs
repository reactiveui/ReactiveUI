// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ReactiveUI;

namespace CommandTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.NormalCommand, v => v.NormalBtn).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.NormalAsyncCommand, v => v.NormalAsyncBtn).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.TaskCommand, v => v.TaskBtn).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.TaskTokenCommand, v => v.TaskTokenBtn).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ObservableCommand, v => v.ObservableBtn).DisposeWith(d);
            });
        }
    }
}
