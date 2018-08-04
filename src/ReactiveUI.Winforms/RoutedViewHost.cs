﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace ReactiveUI.Winforms
{
    [DefaultProperty("ViewModel")]
    public partial class RoutedControlHost : UserControl, IReactiveObject
    {
        readonly CompositeDisposable disposables = new CompositeDisposable();

        RoutingState _Router;
        Control defaultContent;
        IObservable<string> viewContractObservable;

        public RoutedControlHost()
        {
            InitializeComponent();

            this.disposables.Add(this.WhenAny(x => x.DefaultContent, x => x.Value).Subscribe(x => {
                if (x != null && this.Controls.Count == 0) {
                    this.Controls.Add(this.InitView(x));
                    this.components.Add(this.DefaultContent);
                }
            }));

            this.ViewContractObservable = Observable<string>.Default;

            var vmAndContract =
                this.WhenAnyObservable(x => x.Router.CurrentViewModel)
                    .CombineLatest(this.WhenAnyObservable(x => x.ViewContractObservable),
                        (vm, contract) => new { ViewModel = vm, Contract = contract });

            Control viewLastAdded = null;
            this.disposables.Add(vmAndContract.Subscribe(x => {
                // clear all hosted controls (view or default content)
                this.Controls.Clear();

                if (viewLastAdded != null) {
                    viewLastAdded.Dispose();
                }

                if (x.ViewModel == null) {
                    if (this.DefaultContent != null) {
                        this.InitView(this.DefaultContent);
                        this.Controls.Add(this.DefaultContent);
                    }
                    return;
                }

                IViewLocator viewLocator = this.ViewLocator ?? ReactiveUI.ViewLocator.Current;
                IViewFor view = viewLocator.ResolveView(x.ViewModel, x.Contract);
                view.ViewModel = x.ViewModel;

                viewLastAdded = this.InitView((Control)view);
                this.Controls.Add(viewLastAdded);
            }, RxApp.DefaultExceptionHandler.OnNext));
        }

        public event PropertyChangingEventHandler PropertyChanging
        {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { PropertyChangedEventManager.AddHandler(this, value); }
            remove { PropertyChangedEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        [Category("ReactiveUI")]
        [Description("The default control when no viewmodel is specified")]
        public Control DefaultContent
        {
            get { return this.defaultContent; }
            set { this.RaiseAndSetIfChanged(ref this.defaultContent, value); }
        }

        [Category("ReactiveUI")]
        [Description("The router.")]
        public RoutingState Router
        {
            get { return this._Router; }
            set { this.RaiseAndSetIfChanged(ref this._Router, value); }
        }

        [Browsable(false)]
        public IObservable<string> ViewContractObservable
        {
            get { return this.viewContractObservable; }
            set { this.RaiseAndSetIfChanged(ref this.viewContractObservable, value); }
        }

        [Browsable(false)]
        public IViewLocator ViewLocator { get; set; }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
                this.disposables.Dispose();
            }
            base.Dispose(disposing);
        }

        Control InitView(Control view)
        {
            view.Dock = DockStyle.Fill;
            return view;
        }
    }
}
