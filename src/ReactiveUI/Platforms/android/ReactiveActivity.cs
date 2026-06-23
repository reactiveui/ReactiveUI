// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Android.Content;
using Android.Runtime;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>This is an Activity that is both an Activity and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
public class ReactiveActivity : Activity, IReactiveObject, IReactiveNotifyPropertyChanged<ReactiveActivity>,
    IHandleObservableErrors
{
    /// <summary>The subject that signals when the activity is activated.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject that signals when the activity is deactivated.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>The subject that signals activity results.</summary>
    private readonly Signal<(int requestCode, Result resultCode, Intent? intent)> _activityResult = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveActivity"/> class.</summary>
    protected ReactiveActivity()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveActivity"/> class.</summary>
    /// <param name="handle">The handle.</param>
    /// <param name="ownership">The ownership.</param>
    protected ReactiveActivity(in IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveActivity>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveActivity>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>Gets a signal when the activity is activated.</summary>
    public IObservable<RxVoid> Activated => _activated;

    /// <summary>Gets a signal when the activity is deactivated.</summary>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <summary>Gets the activity result.</summary>
    /// <value>
    /// The activity result.
    /// </value>
    public IObservable<(int requestCode, Result resultCode, Intent? intent)> ActivityResult => _activityResult;

    /// <summary>
    /// When this method is called, an object will not fire change
    /// notifications (neither traditional nor Observable notifications)
    /// until the return value is disposed.
    /// </summary>
    /// <returns>An object that, when disposed, reenables change
    /// notifications.</returns>
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <summary>Starts the activity for result asynchronously.</summary>
    /// <param name="intent">The intent.</param>
    /// <param name="requestCode">The request code.</param>
    /// <returns>A task with the result and the intent.</returns>
    public Task<(Result resultCode, Intent? intent)>
        StartActivityForResultAsync(Intent? intent, int requestCode)
    {
        var ret = ActivityResultAwaiter.Await(ActivityResult, requestCode);

        StartActivityForResult(intent, requestCode);
        return ret;
    }

    /// <summary>Starts the activity for result asynchronously.</summary>
    /// <param name="type">The type.</param>
    /// <param name="requestCode">The request code.</param>
    /// <returns>A task with the result and intent.</returns>
    public Task<(Result resultCode, Intent? intent)>
        StartActivityForResultAsync(Type type, int requestCode)
    {
        var ret = ActivityResultAwaiter.Await(ActivityResult, requestCode);

        StartActivityForResult(type, requestCode);
        return ret;
    }

    /// <inheritdoc/>
    protected override void OnPause()
    {
        base.OnPause();
        _deactivated.OnNext(RxVoid.Default);
    }

    /// <inheritdoc/>
    protected override void OnResume()
    {
        base.OnResume();
        _activated.OnNext(RxVoid.Default);
    }

    /// <inheritdoc/>
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        _activityResult.OnNext((requestCode, resultCode, data));
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _activated?.Dispose();
            _deactivated?.Dispose();
            _activityResult?.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Completes a task with the first activity result matching a request code, then unsubscribes — replacing
    /// <c>ActivityResult.Where(matching).Select(...).FirstAsync().ToTask()</c>.
    /// </summary>
    private sealed class ActivityResultAwaiter
        : IObserver<(int requestCode, Result resultCode, Intent? intent)>, IDisposable
    {
        /// <summary>The request code this awaiter is waiting for.</summary>
        private readonly int _requestCode;

        /// <summary>Completes when the first matching activity result arrives.</summary>
        private readonly TaskCompletionSource<(Result resultCode, Intent? intent)> _completion = new();

        /// <summary>The subscription to the activity-result stream.</summary>
        private readonly OnceDisposable _subscription = new();

        /// <summary>Set to 1 once the task has been settled, so only the first matching result wins.</summary>
        private int _settled;

        /// <summary>Initializes a new instance of the <see cref="ActivityResultAwaiter"/> class.</summary>
        /// <param name="requestCode">The request code to wait for.</param>
        private ActivityResultAwaiter(int requestCode) => _requestCode = requestCode;

        /// <summary>Subscribes to <paramref name="source"/> and returns a task that completes on the first matching result.</summary>
        /// <param name="source">The activity-result stream.</param>
        /// <param name="requestCode">The request code to wait for.</param>
        /// <returns>A task carrying the matching result and intent.</returns>
        public static Task<(Result resultCode, Intent? intent)> Await(
            IObservable<(int requestCode, Result resultCode, Intent? intent)> source,
            int requestCode)
        {
            var awaiter = new ActivityResultAwaiter(requestCode);
            awaiter._subscription.Disposable = source.Subscribe(awaiter);
            return awaiter._completion.Task;
        }

        /// <inheritdoc/>
        public void OnNext((int requestCode, Result resultCode, Intent? intent) value)
        {
            if (value.requestCode != _requestCode || Interlocked.Exchange(ref _settled, 1) != 0)
            {
                return;
            }

            _ = _completion.TrySetResult((value.resultCode, value.intent));
            Dispose();
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            if (Interlocked.Exchange(ref _settled, 1) != 0)
            {
                return;
            }

            _ = _completion.TrySetException(error);
            Dispose();
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
            if (Interlocked.Exchange(ref _settled, 1) != 0)
            {
                return;
            }

            _ = _completion.TrySetCanceled();
            Dispose();
        }

        /// <inheritdoc/>
        public void Dispose() => _subscription.Dispose();
    }
}
