﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Splat;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// This content control will automatically load the View associated with
    /// the ViewModel property and display it. This control is very useful
    /// inside a DataTemplate to display the View associated with a ViewModel.
    /// </summary>
    public class ViewModelViewHost : TransitioningContentControl, IViewFor, IEnableLogger, IDisposable
    {
        /// <summary>
        /// The default content dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

        /// <summary>
        /// The view model dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null, SomethingChanged));

        /// <summary>
        /// The view contract observable dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewContractObservableProperty =
            DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string>), typeof(ViewModelViewHost), new PropertyMetadata(Observable<string>.Default, SomethingChanged));

        private readonly Subject<Unit> _updateViewModel = new Subject<Unit>();
        private string _viewContract;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelViewHost"/> class.
        /// </summary>
        public ViewModelViewHost()
        {
#if NETFX_CORE
            DefaultStyleKey = typeof(ViewModelViewHost);
#endif

            if (ModeDetector.InUnitTestRunner())
            {
                ViewContractObservable = Observable<string>.Never;

                // NB: InUnitTestRunner also returns true in Design Mode
                return;
            }

            var platform = Locator.Current.GetService<IPlatformOperations>();
            Func<string> platformGetter = () => default(string);

            if (platform == null)
            {
                // NB: This used to be an error but WPF design mode can't read
                // good or do other stuff good.
                this.Log().Error("Couldn't find an IPlatformOperations implementation. Please make sure you have installed the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation/nuget-packages for guidance.");
            }
            else
            {
                platformGetter = () => platform.GetOrientation();
            }

            ViewContractObservable = Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>(x => SizeChanged += x, x => SizeChanged -= x)
                .Select(_ => platformGetter())
                .StartWith(platformGetter())
                .DistinctUntilChanged();

            var contractChanged = _updateViewModel.Select(_ => ViewContractObservable).Switch();
            var viewModelChanged = _updateViewModel.Select(_ => ViewModel);

            var vmAndContract = contractChanged.CombineLatest(viewModelChanged, (contract, vm) => new { ViewModel = vm, Contract = contract });

            vmAndContract.Subscribe(x => ResolveViewForViewModel(x.ViewModel, x.Contract));
            contractChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => _viewContract = x);
        }

        /// <summary>
        /// Gets or sets the view contract observable.
        /// </summary>
        public IObservable<string> ViewContractObservable
        {
            get => (IObservable<string>)GetValue(ViewContractObservableProperty);
            set => SetValue(ViewContractObservableProperty, value);
        }

        /// <summary>
        /// Gets or sets the content displayed by default when no content is set.
        /// </summary>
        public object DefaultContent
        {
            get => GetValue(DefaultContentProperty);
            set => SetValue(DefaultContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the ViewModel to display.
        /// </summary>
        public object ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Gets or sets the view contract.
        /// </summary>
        public string ViewContract
        {
            get => _viewContract;
            set => ViewContractObservable = Observable.Return(value);
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        public IViewLocator ViewLocator { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources inside the class.
        /// </summary>
        /// <param name="isDisposing">If we are disposing managed resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                _updateViewModel?.Dispose();
            }

            _isDisposed = true;
        }

        private static void SomethingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((ViewModelViewHost)dependencyObject)._updateViewModel.OnNext(Unit.Default);
        }

        private void ResolveViewForViewModel(object viewModel, string contract)
        {
            if (viewModel == null)
            {
                Content = DefaultContent;
                return;
            }

            var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
            var viewInstance = viewLocator.ResolveView(viewModel, contract) ?? viewLocator.ResolveView(viewModel, null);

            if (viewInstance == null)
            {
                Content = DefaultContent;
                this.Log().Warn($"The {nameof(ViewModelViewHost)} could not find a valid view for the view model of type {viewModel.GetType()} and value {viewModel}.");
                return;
            }

            viewInstance.ViewModel = viewModel;

            Content = viewInstance;
        }
    }
}
