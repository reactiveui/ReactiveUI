using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace ReactiveUI.Winforms
{
    [DefaultProperty("ViewModel")]
    public partial class RoutedViewHost : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        readonly CompositeDisposable disposables = new CompositeDisposable();

        IRoutingState _Router;
        Control defaultContent;
        IObservable<string> viewContractObservable;
        object viewModel;

        public RoutedViewHost()
        {
            this.InitializeComponent();

            this.disposables.Add(this.WhenAny(x => x.DefaultContent, x => x.Value).Subscribe(x => {
                if (x != null && this.Controls.Count == 0) {
                    this.Controls.Add(this.InitView(x));
                    this.components.Add(this.DefaultContent);
                }
            }));

            this.ViewContractObservable = Observable.Return(default(string));

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

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        [Category("ReactiveUI")]
        [Description("The default control when no viewmodel is specified")]
        public Control DefaultContent {
            get { return this.defaultContent; }
            set { this.RaiseAndSetIfChanged(ref this.defaultContent, value); }
        }

        [Category("ReactiveUI")]
        [Description("The router.")]
        public IRoutingState Router {
            get { return this._Router; }
            set { this.RaiseAndSetIfChanged(ref this._Router, value); }
        }

        [Browsable(false)]
        public IObservable<string> ViewContractObservable {
            get { return this.viewContractObservable; }
            set { this.RaiseAndSetIfChanged(ref this.viewContractObservable, value); }
        }

        [Browsable(false)]
        public IViewLocator ViewLocator { get; set; }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null)) {
                this.components.Dispose();
                this.disposables.Dispose();
            }

            base.Dispose(disposing);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null) {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        protected TRet RaiseAndSetIfChanged<TRet>(
            ref TRet backingField,
            TRet newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue)) {
                return newValue;
            }

            this.OnPropertyChanging(propertyName);
            backingField = newValue;
            this.OnPropertyChanged(propertyName);

            return newValue;
        }

        Control InitView(Control view)
        {
            view.Dock = DockStyle.Fill;
            return view;
        }
    }
}
