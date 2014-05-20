using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using ReactiveUI;

namespace ReactiveUI.Winforms
{
    [DefaultProperty("ViewModel")]
    public partial class ViewModelViewHost : UserControl, IReactiveObject
    {
        readonly CompositeDisposable disposables = new CompositeDisposable();

        Control currentView;
        Control defaultContent;
        IObservable<string> viewContractObservable;
        object viewModel;

        object content;

        bool cacheViews = true;

        public ViewModelViewHost()
        {
            this.InitializeComponent();


            var viewChanges =
              this.WhenAnyValue(x => x.Content)
                  .Select(x => x as Control)
                  .Where(x => x != null)
                  .Subscribe(x =>
                  {
                      //change the view in the ui
                      this.SuspendLayout();

                      //clear out existing visible control view
                      foreach (Control c in this.Controls)
                      {
                          c.Dispose();
                          this.Controls.Remove(c);
                      }

                      x.Dock = DockStyle.Fill;
                      this.Controls.Add(x);
                      this.ResumeLayout();
                  });

            this.disposables.Add(viewChanges);

            this.disposables.Add(this.WhenAny(x => x.DefaultContent, x => x.Value).Subscribe(x => {
                if (x != null && this.currentView == null) {
                    this.Content = DefaultContent;
                }
            }));

            this.ViewContractObservable = Observable.Return(default(string));

            var vmAndContract =
                this.WhenAny(x => x.ViewModel, x => x.Value)
                    .CombineLatest(this.WhenAnyObservable(x => x.ViewContractObservable),
                        (vm, contract) => new { ViewModel = vm, Contract = contract });



            this.disposables.Add(vmAndContract.Subscribe(x => {
                //clear all hosted controls (view or default content)
                if (this.ViewModel == null) {
                    if (this.DefaultContent != null) {
                        this.Content = DefaultContent;
                    }
                    return;
                }

                if (CacheViews) {
                    //when caching views, check the current viewmodel and type
                    var c = content as IViewFor;
                    if (c != null && c.ViewModel != null
                        && c.ViewModel.GetType() == x.ViewModel.GetType()) {
                            c.ViewModel = x.ViewModel;
                        return;
                    }
                }

                IViewLocator viewLocator = this.ViewLocator ?? ReactiveUI.ViewLocator.Current;
                IViewFor view = viewLocator.ResolveView(x.ViewModel, x.Contract);
                this.Content = view;
                view.ViewModel = x.ViewModel;

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

        public Control CurrentView
        {
            get { return this.content as Control; }
        }

        [Category("ReactiveUI")]
        [Description("The default control when no viewmodel is specified")]
        public Control DefaultContent
        {
            get { return this.defaultContent; }
            set { this.RaiseAndSetIfChanged(ref this.defaultContent, value); }
        }

        [Browsable(false)]
        public IObservable<string> ViewContractObservable
        {
            get { return this.viewContractObservable; }
            set { this.RaiseAndSetIfChanged(ref this.viewContractObservable, value); }
        }

        [Browsable(false)]
        public IViewLocator ViewLocator { get; set; }

        [Category("ReactiveUI")]
        [Description("The viewmodel to host.")]
        [Bindable(true)]
        public object ViewModel
        {
            get { return this.viewModel; }
            set { this.RaiseAndSetIfChanged(ref this.viewModel, value); }
        }

        [Category("ReactiveUI")]
        [Description("The Current View")]
        [Bindable(true)]
        public object Content
        {
            get { return this.content; }
            protected set { this.RaiseAndSetIfChanged(ref this.content, value); }
        }

        [Category("ReactiveUI")]
        [Description("Cache Views")]
        [Bindable(true)]
        [DefaultValue(true)]
        public bool CacheViews
        {
            get { return this.cacheViews; }
            set { this.RaiseAndSetIfChanged(ref this.cacheViews, value); }
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
                this.disposables.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
