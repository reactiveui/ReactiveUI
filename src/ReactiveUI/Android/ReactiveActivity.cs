using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;

namespace ReactiveUI
{
    /// <summary>
    /// This is an Activity that is both an Activity and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged)
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveUI.ReactiveActivity"/>
    /// <seealso cref="ReactiveUI.IViewFor{TViewModel}"/>
    /// <seealso cref="ReactiveUI.ICanActivate"/>
    public class ReactiveActivity<TViewModel> : ReactiveActivity, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        private TViewModel _ViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveActivity{TViewModel}"/> class.
        /// </summary>
        protected ReactiveActivity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveActivity{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="ownership">The ownership.</param>
        protected ReactiveActivity(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        /// <summary>
        /// The ViewModel corresponding to this specific View. This should be a DependencyProperty if
        /// you're using XAML.
        /// </summary>
        public TViewModel ViewModel
        {
            get { return this._ViewModel; }
            set { this.RaiseAndSetIfChanged(ref this._ViewModel, value); }
        }

        object IViewFor.ViewModel
        {
            get { return this._ViewModel; }
            set { this._ViewModel = (TViewModel)value; }
        }
    }

    /// <summary>
    /// This is an Activity that is both an Activity and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged)
    /// </summary>
    public class ReactiveActivity : Activity, IReactiveObject, IReactiveNotifyPropertyChanged<ReactiveActivity>, IHandleObservableErrors
    {
        private readonly Subject<Unit> activated = new Subject<Unit>();

        private readonly Subject<Tuple<int, Result, Intent>> activityResult = new Subject<Tuple<int, Result, Intent>>();

        private readonly Subject<Unit> deactivated = new Subject<Unit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveActivity"/> class.
        /// </summary>
        protected ReactiveActivity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveActivity"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="ownership">The ownership.</param>
        protected ReactiveActivity(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        /// <remarks>To be added.</remarks>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { PropertyChangedEventManager.AddHandler(this, value); }
            remove { PropertyChangedEventManager.RemoveHandler(this, value); }
        }

        /// <summary>
        /// Occurs when [property changing].
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        /// <summary>
        /// Returns when activated.
        /// </summary>
        /// <value>The activated.</value>
        public IObservable<Unit> Activated { get { return this.activated.AsObservable(); } }

        /// <summary>
        /// Gets the activity result.
        /// </summary>
        /// <value>The activity result.</value>
        public IObservable<Tuple<int, Result, Intent>> ActivityResult
        {
            get { return this.activityResult.AsObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveActivity>> Changed
        {
            get { return this.getChangedObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveActivity>> Changing
        {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Returns when deactivated.
        /// </summary>
        /// <value>The deactivated.</value>
        public IObservable<Unit> Deactivated { get { return this.deactivated.AsObservable(); } }

        /// <summary>
        /// Fires whenever an exception would normally terminate ReactiveUI internal state.
        /// </summary>
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// Starts the activity for result asynchronous.
        /// </summary>
        /// <param name="intent">The intent.</param>
        /// <param name="requestCode">The request code.</param>
        /// <returns></returns>
        public Task<Tuple<Result, Intent>> StartActivityForResultAsync(Intent intent, int requestCode)
        {
            // NB: It's important that we set up the subscription *before* we call ActivityForResult
            var ret = this.ActivityResult
                .Where(x => x.Item1 == requestCode)
                .Select(x => Tuple.Create(x.Item2, x.Item3))
                .FirstAsync()
                .ToTask();

            StartActivityForResult(intent, requestCode);
            return ret;
        }

        /// <summary>
        /// Starts the activity for result asynchronous.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="requestCode">The request code.</param>
        /// <returns></returns>
        public Task<Tuple<Result, Intent>> StartActivityForResultAsync(Type type, int requestCode)
        {
            // NB: It's important that we set up the subscription *before* we call ActivityForResult
            var ret = this.ActivityResult
                .Where(x => x.Item1 == requestCode)
                .Select(x => Tuple.Create(x.Item2, x.Item3))
                .FirstAsync()
                .ToTask();

            StartActivityForResult(type, requestCode);
            return ret;
        }

        /// <summary>
        /// When this method is called, an object will not fire change notifications (neither
        /// traditional nor Observable notifications) until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change notifications.</returns>
        public IDisposable SuppressChangeNotifications()
        {
            return this.suppressChangeNotifications();
        }

        /// <summary>
        /// Called when an activity you launched exits, giving you the requestCode you started it
        /// with, the resultCode it returned, and any additional data from it.
        /// </summary>
        /// <param name="requestCode">
        /// The integer request code originally supplied to startActivityForResult(), allowing you to
        /// identify who this result came from.
        /// </param>
        /// <param name="resultCode">
        /// The integer result code returned by the child activity through its setResult().
        /// </param>
        /// <param name="data">
        /// An Intent, which can return result data to the caller (various data can be attached to
        /// Intent "extras").
        /// </param>
        /// <remarks>
        /// <para tool="javadoc-to-mdoc">
        /// Called when an activity you launched exits, giving you the requestCode you started it
        /// with, the resultCode it returned, and any additional data from it. The <format
        /// type="text/html"><var>resultCode</var></format> will be <c><see
        /// cref="F:Android.App.Result.Canceled"/></c> if the activity explicitly returned that,
        /// didn't return any result, or crashed during its operation.
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// You will receive this call immediately before onResume() when your activity is re-starting.
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// <format type="text/html"><a
        /// href="http://developer.android.com/reference/android/app/Activity.html#onActivityResult(int,
        /// int, android.content.Intent)" target="_blank">[Android Documentation]</a></format>
        /// </para>
        /// </remarks>
        /// <since version="Added in API level 1"/>
        /// <altmember cref="M:Android.App.Activity.StartActivityForResult(Android.Content.Intent, System.Int32)"/>
        /// <altmember cref="M:Android.App.Activity.CreatePendingResult(System.Int32, Android.Content.Intent, Android.Content.Intent)"/>
        /// <altmember cref="M:Android.App.Activity.SetResult(Android.App.Result)"/>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.activityResult.OnNext(Tuple.Create(requestCode, resultCode, data));
        }

        /// <summary>
        /// Called as part of the activity lifecycle when an activity is going into the background,
        /// but has not (yet) been killed.
        /// </summary>
        /// <remarks>
        /// <para tool="javadoc-to-mdoc">
        /// Called as part of the activity lifecycle when an activity is going into the background,
        /// but has not (yet) been killed. The counterpart to <c><see cref="M:Android.App.Activity.OnResume"/></c>.
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// When activity B is launched in front of activity A, this callback will be invoked on A. B
        /// will not be created until A's <c><see cref="M:Android.App.Activity.OnPause"/></c>
        /// returns, so be sure to not do anything lengthy here.
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// This callback is mostly used for saving any persistent state the activity is editing, to
        /// present a "edit in place" model to the user and making sure nothing is lost if there are
        /// not enough resources to start the new activity without first killing this one. This is
        /// also a good place to do things like stop animations and other things that consume a
        /// noticeable amount of CPU in order to make the switch to the next activity as fast as
        /// possible, or to close resources that are exclusive access such as the camera.
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// In situations where the system needs more memory it may kill paused processes to reclaim
        /// resources. Because of this, you should be sure that all of your state is saved by the
        /// time you return from this function. In general <c><see
        /// cref="M:Android.App.Activity.OnSaveInstanceState(Android.OS.Bundle)"/></c> is used to
        /// save per-instance state in the activity and this method is used to store global
        /// persistent data (in content providers, files, etc.)
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// After receiving this call you will usually receive a following call to <c><see
        /// cref="M:Android.App.Activity.OnStop"/></c> (after the next activity has been resumed and
        /// displayed), however in some cases there will be a direct call back to <c><see
        /// cref="M:Android.App.Activity.OnResume"/></c> without going through the stopped state.
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// <i>Derived classes must call through to the super class's implementation of this method.
        /// If they do not, an exception will be thrown.</i>
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// <format type="text/html"><a
        /// href="http://developer.android.com/reference/android/app/Activity.html#onPause()"
        /// target="_blank">[Android Documentation]</a></format>
        /// </para>
        /// </remarks>
        /// <since version="Added in API level 1"/>
        /// <altmember cref="M:Android.App.Activity.OnResume"/>
        /// <altmember cref="M:Android.App.Activity.OnSaveInstanceState(Android.OS.Bundle)"/>
        /// <altmember cref="M:Android.App.Activity.OnStop"/>
        protected override void OnPause()
        {
            base.OnPause();
            this.deactivated.OnNext(Unit.Default);
        }

        /// <summary>
        /// Called after <c><see
        /// cref="M:Android.App.Activity.OnRestoreInstanceState(Android.OS.Bundle)"/></c>, <c><see
        /// cref="M:Android.App.Activity.OnRestart"/></c>, or <c><see
        /// cref="M:Android.App.Activity.OnPause"/></c>, for your activity to start interacting with
        /// the user.
        /// </summary>
        /// <remarks>
        /// <para tool="javadoc-to-mdoc">
        /// Called after <c><see
        /// cref="M:Android.App.Activity.OnRestoreInstanceState(Android.OS.Bundle)"/></c>, <c><see
        /// cref="M:Android.App.Activity.OnRestart"/></c>, or <c><see
        /// cref="M:Android.App.Activity.OnPause"/></c>, for your activity to start interacting with
        /// the user. This is a good place to begin animations, open exclusive-access devices (such
        /// as the camera), etc.
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// Keep in mind that onResume is not the best indicator that your activity is visible to the
        /// user; a system window such as the keyguard may be in front. Use <c><see
        /// cref="M:Android.App.Activity.OnWindowFocusChanged(System.Boolean)"/></c> to know for
        /// certain that your activity is visible to the user (for example, to resume a game).
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// <i>Derived classes must call through to the super class's implementation of this method.
        /// If they do not, an exception will be thrown.</i>
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// <format type="text/html"><a
        /// href="http://developer.android.com/reference/android/app/Activity.html#onResume()"
        /// target="_blank">[Android Documentation]</a></format>
        /// </para>
        /// </remarks>
        /// <since version="Added in API level 1"/>
        /// <altmember cref="M:Android.App.Activity.OnRestoreInstanceState(Android.OS.Bundle)"/>
        /// <altmember cref="M:Android.App.Activity.OnRestart"/>
        /// <altmember cref="M:Android.App.Activity.OnPostResume"/>
        /// <altmember cref="M:Android.App.Activity.OnPause"/>
        protected override void OnResume()
        {
            base.OnResume();
            this.activated.OnNext(Unit.Default);
        }
    }
}