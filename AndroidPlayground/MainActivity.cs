using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ActionbarSherlock.App;
using ReactiveUI;
using ReactiveUI.Routing;
using System.ComponentModel;
using ReactiveUI.Android;
using ReactiveUI.Mobile;

namespace AndroidPlayground
{
    [Activity (Label = "AndroidPlayground", MainLauncher = true)]
    public class MainView : SherlockActivity, IViewFor<MainViewModel>, INotifyPropertyChanged
    {
        int count = 1;
        readonly ActivityRoutedViewHost routeHelper;
        readonly AutoSuspendActivityHelper suspendHelper;

        #region Boring copy-paste code I want to die
        MainViewModel _ViewModel;
        public MainViewModel ViewModel {
            get { return _ViewModel; }
            set {
                if (_ViewModel == value) return;
                _ViewModel = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ViewModel"));
            }
        }
        
        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel)value; }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public MainView()
        {
            // NB: This is dumb.
            Console.WriteLine(App.Current);
            RxApp.DeferredScheduler = new AndroidUIScheduler(this);

            suspendHelper = new AutoSuspendActivityHelper(this);
            suspendHelper.SuspensionHost.SetupDefaultSuspendResume();

            routeHelper = new ActivityRoutedViewHost(this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            suspendHelper.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);
            
            button.Click += (o,e) => {
                ViewModel.HostScreen.Router.Navigate.Execute(new SecondaryViewModel(ViewModel.HostScreen));
            };

            this.OneWayBind(ViewModel, x => x.UrlPathSegment, x => x.ActionBar.Title);
        }

        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            return routeHelper.OnKeyUp(keyCode, e);
        }
        
        protected override void OnResume()
        {
            base.OnResume();
            suspendHelper.OnResume();
        }
        
        protected override void OnPause()
        {
            base.OnPause();
            suspendHelper.OnPause();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            suspendHelper.OnSaveInstanceState(outState);
        }
    }

    public class MainViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment {
            get { return "Main!"; }
        }

        public IScreen HostScreen { get; protected set; }

        public MainViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? RxApp.GetService<IScreen>();
        }
    }
}