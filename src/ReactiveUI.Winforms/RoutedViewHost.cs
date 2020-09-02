﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace ReactiveUI.Winforms
{
    /// <summary>
    /// A control host which will handling routing between different ViewModels and Views.
    /// </summary>
    [DefaultProperty("ViewModel")]
    public partial class RoutedControlHost : UserControl, IReactiveObject
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private RoutingState? _router;
        private Control? _defaultContent;
        private IObservable<string>? _viewContractObservable;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutedControlHost"/> class.
        /// </summary>
        public RoutedControlHost()
        {
            InitializeComponent();

            _disposables.Add(this.WhenAny(x => x.DefaultContent, x => x.Value).Subscribe(x =>
            {
                if (x != null && Controls.Count == 0)
                {
                    Controls.Add(InitView(x));
                    components.Add(DefaultContent);
                }
            }));

            ViewContractObservable = Observable<string>.Default;

            var vmAndContract =
                this.WhenAnyObservable(x => x.Router!.CurrentViewModel!)
                    .CombineLatest(
                        this.WhenAnyObservable(x => x.ViewContractObservable!),
                        (vm, contract) => new { ViewModel = vm, Contract = contract });

            Control viewLastAdded = null!;
            _disposables.Add(vmAndContract.Subscribe(
                x =>
            {
                // clear all hosted controls (view or default content)
                SuspendLayout();
                Controls.Clear();

                if (viewLastAdded != null)
                {
                    viewLastAdded.Dispose();
                }

                if (x.ViewModel == null)
                {
                    if (DefaultContent != null)
                    {
                        InitView(DefaultContent);
                        Controls.Add(DefaultContent);
                    }

                    ResumeLayout();
                    return;
                }

                IViewLocator viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                IViewFor? view = viewLocator.ResolveView(x.ViewModel, x.Contract);
                if (view != null)
                {
                    view.ViewModel = x.ViewModel;

                    viewLastAdded = InitView((Control)view);
                }

                Controls.Add(viewLastAdded);
                ResumeLayout();
            }, RxApp.DefaultExceptionHandler!.OnNext));
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the default content.
        /// </summary>
        /// <value>
        /// The default content.
        /// </value>
        [Category("ReactiveUI")]
        [Description("The default control when no viewmodel is specified")]
        public Control? DefaultContent
        {
            get => _defaultContent;
            set => this.RaiseAndSetIfChanged(ref _defaultContent, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="RoutingState"/> of the view model stack.
        /// </summary>
        [Category("ReactiveUI")]
        [Description("The router.")]
        public RoutingState? Router
        {
            get => _router;
            set => this.RaiseAndSetIfChanged(ref _router, value);
        }

        /// <summary>
        /// Gets or sets the view contract observable.
        /// </summary>
        [Browsable(false)]
        public IObservable<string>? ViewContractObservable
        {
            get => _viewContractObservable;
            set => this.RaiseAndSetIfChanged(ref _viewContractObservable, value);
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        [Browsable(false)]
        public IViewLocator? ViewLocator { get; set; }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChanging?.Invoke(this, args);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
                _disposables.Dispose();
            }

            base.Dispose(disposing);
        }

        private static Control InitView(Control view)
        {
            view.Dock = DockStyle.Fill;
            return view;
        }
    }
}
