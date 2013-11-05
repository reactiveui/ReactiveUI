using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReactiveUI.Winforms
{
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;

    [DefaultPropertyAttribute("ViewModel")]
    public partial class RoutedViewHost : UserControl, INotifyPropertyChanged, System.ComponentModel.INotifyPropertyChanging
    {
        object viewModel;

        Control defaultContent;
     

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public RoutedViewHost()
        {
            InitializeComponent();
            
            disposables.Add(
                this.WhenAny(x => x.DefaultContent, x => x.Value).Subscribe(x =>
            {
                if (x != null && this.Controls.Count==0) {
                    this.Controls.Add(InitView(x));
                    components.Add(DefaultContent);
                }
            }));
             

            ViewContractObservable = Observable.Return(default(string));

            var vmAndContract = Observable.CombineLatest(
                this.WhenAnyObservable(x => x.Router.CurrentViewModel),
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (vm, contract) => new { ViewModel = vm, Contract = contract });

            Control viewLastAdded = null;
            disposables.Add(
                vmAndContract.Subscribe(x =>
            {
                //clear all hosted controls (view or default content)
                this.Controls.Clear();

                if (viewLastAdded != null)
                {
                    viewLastAdded.Dispose();
                }

                if (x.ViewModel == null)
                {
                    if (DefaultContent != null)
                    {
                        InitView(DefaultContent);
                        this.Controls.Add(DefaultContent);
                    }
                    return;
                }

                var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                var view = viewLocator.ResolveView(x.ViewModel, x.Contract);
                view.ViewModel = x.ViewModel;

                viewLastAdded = InitView((Control)view);
                this.Controls.Add(viewLastAdded);
            },RxApp.DefaultExceptionHandler.OnNext));
        }

        private Control InitView(Control view)
        {
            view.Dock = DockStyle.Fill;
            return view;
        }

        IRoutingState _Router;
        [Category("ReactiveUI"), Description("The router.")]
        public IRoutingState Router
        {
            get { return _Router; }
            set { this.RaiseAndSetIfChanged(ref _Router, value); }
        }
       


        [Category("ReactiveUI"), Description("The default control when no viewmodel is specified")]
        public Control DefaultContent
        {
            get
            {
                return this.defaultContent;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref defaultContent, value);
            }
        }

        [Browsable(false)]
        public IViewLocator ViewLocator { get; set; }

        IObservable<string> viewContractObservable;
        [Browsable(false)]
        public IObservable<string> ViewContractObservable
        {
            get { return viewContractObservable; }
            set { this.RaiseAndSetIfChanged(ref viewContractObservable, value); }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                disposables.Dispose();
            }
            base.Dispose(disposing);
        }



        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;
        public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;



        protected TRet RaiseAndSetIfChanged<TRet>(
            ref TRet backingField,
            TRet newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return newValue;
            }

            OnPropertyChanging(propertyName);
            backingField = newValue;
            OnPropertyChanged(propertyName);
            return newValue;
        }

        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
