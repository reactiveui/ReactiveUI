﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using Splat;

#if WINUI3UWP
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#elif NETFX_CORE || HAS_UNO
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

#if HAS_UNO
namespace ReactiveUI.Uno
#else
namespace ReactiveUI
#endif
{
    /// <summary>
    /// This control hosts the View associated with a Router, and will display
    /// the View and wire up the ViewModel whenever a new ViewModel is
    /// navigated to. Put this control as the only control in your Window.
    /// </summary>
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "Deliberate usage")]
    public
#if HAS_UNO
        partial
#endif
        class RoutedViewHost : TransitioningContentControl, IActivatableView, IEnableLogger
    {
        /// <summary>
        /// The router dependency property.
        /// </summary>
        public static readonly DependencyProperty RouterProperty =
            DependencyProperty.Register("Router", typeof(RoutingState), typeof(RoutedViewHost), new PropertyMetadata(null));

        /// <summary>
        /// The default content property.
        /// </summary>
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register("DefaultContent", typeof(object), typeof(RoutedViewHost), new PropertyMetadata(null));

        /// <summary>
        /// The view contract observable property.
        /// </summary>
        public static readonly DependencyProperty ViewContractObservableProperty =
            DependencyProperty.Register("ViewContractObservable", typeof(IObservable<string>), typeof(RoutedViewHost), new PropertyMetadata(Observable<string>.Default));

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutedViewHost"/> class.
        /// </summary>
        public RoutedViewHost()
        {
#if NETFX_CORE
            DefaultStyleKey = typeof(RoutedViewHost);
#endif
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            if (ModeDetector.InUnitTestRunner())
            {
                ViewContractObservable = Observable<string>.Never;
                return;
            }

            var platform = Locator.Current.GetService<IPlatformOperations>();
            Func<string?> platformGetter = () => default!;

            if (platform == null)
            {
                // NB: This used to be an error but WPF design mode can't read
                // good or do other stuff good.
                this.Log().Error("Couldn't find an IPlatformOperations implementation. Please make sure you have installed the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance.");
            }
            else
            {
                platformGetter = () => platform.GetOrientation();
            }

            ViewContractObservable = Observable.FromEvent<SizeChangedEventHandler, string?>(
                    eventHandler =>
                    {
                        void Handler(object sender, SizeChangedEventArgs e) => eventHandler(platformGetter());
                        return Handler;
                    },
                    x => SizeChanged += x,
                    x => SizeChanged -= x)
                .DistinctUntilChanged()
                .StartWith(platformGetter())
                .Select(x => x);

            var vmAndContract = this.WhenAnyObservable(x => x.Router.CurrentViewModel!).CombineLatest(
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (viewModel, contract) => (viewModel, contract));

            this.WhenActivated(d =>
            {
                // NB: The DistinctUntilChanged is useful because most views in
                // WinRT will end up getting here twice - once for configuring
                // the RoutedViewHost's ViewModel, and once on load via SizeChanged
                d(vmAndContract.DistinctUntilChanged().Subscribe(
                    x =>
                    {
                        if (x.viewModel == null)
                        {
                            Content = DefaultContent;
                            return;
                        }

                        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                        var view = viewLocator.ResolveView(x.viewModel, x.contract) ?? viewLocator.ResolveView(x.viewModel);

                        if (view == null)
                        {
                            throw new Exception($"Couldn't find view for '{x.viewModel}'.");
                        }

                        view.ViewModel = x.viewModel;
                        Content = view;
                    },
                    ex => RxApp.DefaultExceptionHandler.OnNext(ex)));
            });
        }

        /// <summary>
        /// Gets or sets the <see cref="RoutingState"/> of the view model stack.
        /// </summary>
        public RoutingState Router
        {
            get => (RoutingState)GetValue(RouterProperty);
            set => SetValue(RouterProperty, value);
        }

        /// <summary>
        /// Gets or sets the content displayed whenever there is no page currently
        /// routed.
        /// </summary>
        public object DefaultContent
        {
            get => GetValue(DefaultContentProperty);
            set => SetValue(DefaultContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the view contract observable.
        /// </summary>
        /// <value>
        /// The view contract observable.
        /// </value>
        public IObservable<string?> ViewContractObservable
        {
            get => (IObservable<string?>)GetValue(ViewContractObservableProperty);
            set => SetValue(ViewContractObservableProperty, value);
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        /// <value>
        /// The view locator.
        /// </value>
        public IViewLocator? ViewLocator { get; set; }
    }
}
