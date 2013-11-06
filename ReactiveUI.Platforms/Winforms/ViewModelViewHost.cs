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
    public partial class ViewModelViewHost : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        readonly CompositeDisposable disposables = new CompositeDisposable();

        Control currentView;
        Control defaultContent;
        IObservable<string> viewContractObservable;
        object viewModel;

        public ViewModelViewHost()
        {
            this.InitializeComponent();

            this.disposables.Add(this.WhenAny(x => x.DefaultContent, x => x.Value).Subscribe(x => {
                if (x != null && this.currentView == null) {
                    this.Controls.Clear();
                    this.Controls.Add(this.InitView(x));
                    this.components.Add(this.DefaultContent);
                }
            }));

            this.ViewContractObservable = Observable.Return(default(string));

            var vmAndContract =
                this.WhenAny(x => x.ViewModel, x => x.Value)
                    .CombineLatest(this.WhenAnyObservable(x => x.ViewContractObservable),
                        (vm, contract) => new { ViewModel = vm, Contract = contract });

            this.disposables.Add(vmAndContract.Subscribe(x => {
                //clear all hosted controls (view or default content)
                this.Controls.Clear();

                if (this.currentView != null) {
                    this.currentView.Dispose();
                }

                if (this.ViewModel == null) {
                    if (this.DefaultContent != null) {
                        this.InitView(this.DefaultContent);
                        this.Controls.Add(this.DefaultContent);
                    }
                    return;
                }

                IViewLocator viewLocator = this.ViewLocator ?? ReactiveUI.ViewLocator.Current;
                IViewFor view = viewLocator.ResolveView(x.ViewModel, x.Contract);
                view.ViewModel = x.ViewModel;

                this.CurrentView = this.InitView((Control)view);
                this.Controls.Add(this.CurrentView);
            }, RxApp.DefaultExceptionHandler.OnNext));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        public Control CurrentView {
            get { return this.currentView; }
            private set { this.RaiseAndSetIfChanged(ref this.currentView, value); }
        }

        [Category("ReactiveUI")]
        [Description("The default control when no viewmodel is specified")]
        public Control DefaultContent {
            get { return this.defaultContent; }
            set { this.RaiseAndSetIfChanged(ref this.defaultContent, value); }
        }

        [Browsable(false)]
        public IObservable<string> ViewContractObservable {
            get { return this.viewContractObservable; }
            set { this.RaiseAndSetIfChanged(ref this.viewContractObservable, value); }
        }

        [Browsable(false)]
        public IViewLocator ViewLocator { get; set; }

        [Category("ReactiveUI")]
        [Description("The viewmodel to host.")]
        [Bindable(true)]
        public object ViewModel {
            get { return this.viewModel; }
            set { this.RaiseAndSetIfChanged(ref this.viewModel, value); }
        }

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
