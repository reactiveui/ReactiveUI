// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Reactive;
using Android.Content;
using AndroidX.Fragment.App;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI.AndroidX;

/// <summary>
/// This is an Activity that is both an Activity and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
public class ReactiveFragmentActivity : FragmentActivity, IReactiveObject,
    IReactiveNotifyPropertyChanged<ReactiveFragmentActivity>, IHandleObservableErrors
{
    /// <summary>
    /// The subject that signals when the activity is activated.
    /// </summary>
    private readonly BroadcastSubject<Unit> _activated = new();

    /// <summary>
    /// The subject that signals when the activity is deactivated.
    /// </summary>
    private readonly BroadcastSubject<Unit> _deactivated = new();

    /// <summary>
    /// The subject that signals activity results.
    /// </summary>
    private readonly BroadcastSubject<(int requestCode, Result result, Intent intent)> _activityResult = new();

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveFragmentActivity>> Changing =>
        this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveFragmentActivity>> Changed =>
        this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>
    /// Gets a signal when the activity fragment is activated.
    /// </summary>
    public IObservable<Unit> Activated => _activated;

    /// <summary>
    /// Gets a signal when the activity fragment is deactivated.
    /// </summary>
    public IObservable<Unit> Deactivated => _deactivated;

    /// <summary>
    /// Gets the activity result.
    /// </summary>
    public IObservable<(int requestCode, Result result, Intent intent)> ActivityResult =>
        _activityResult;

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <inheritdoc />
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <summary>
    /// Starts the activity for result asynchronously.
    /// </summary>
    /// <param name="intent">The intent.</param>
    /// <param name="requestCode">The request code.</param>
    /// <returns>A task with the result and intent.</returns>
    public Task<(Result result, Intent intent)> StartActivityForResultAsync(Intent intent, int requestCode)
    {
        // NB: It's important that we set up the subscription *before* we
        // call ActivityForResult
        var ret = ActivityResultAwaiter.Await(ActivityResult, requestCode);

        StartActivityForResult(intent, requestCode);
        return ret;
    }

    /// <summary>
    /// Starts the activity for result asynchronously.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="requestCode">The request code.</param>
    /// <returns>A task with the result and intent.</returns>
    public Task<(Result result, Intent intent)> StartActivityForResultAsync(Type type, int requestCode)
    {
        // NB: It's important that we set up the subscription *before* we
        // call ActivityForResult
        var ret = ActivityResultAwaiter.Await(ActivityResult, requestCode);

        StartActivityForResult(type, requestCode);
        return ret;
    }

    /// <inheritdoc/>
    protected override void OnPause()
    {
        base.OnPause();
        _deactivated.OnNext(Unit.Default);
    }

    /// <inheritdoc/>
    protected override void OnResume()
    {
        base.OnResume();
        _activated.OnNext(Unit.Default);
    }

    /// <inheritdoc/>
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        ArgumentExceptionHelper.ThrowIfNull(data);

        base.OnActivityResult(requestCode, resultCode, data);
        _activityResult.OnNext((requestCode, resultCode, data));
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _activated.Dispose();
            _deactivated.Dispose();
            _activityResult.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Awaits the first <see cref="ActivityResult"/> that matches a request code, then completes the task and
    /// detaches. A fused, allocation-light replacement for <c>Where(...).Select(...).FirstAsync().ToTask()</c>:
    /// it is its own observer, settles exactly once, and unsubscribes on completion.
    /// </summary>
    private sealed class ActivityResultAwaiter
        : IObserver<(int requestCode, Result result, Intent intent)>, IDisposable
    {
        /// <summary>The request code this awaiter is waiting for.</summary>
        private readonly int _requestCode;

        /// <summary>Completion source backing the returned task.</summary>
        private readonly TaskCompletionSource<(Result result, Intent intent)> _completion = new();

        /// <summary>Holds the source subscription so it can be torn down once settled.</summary>
        private readonly OnceDisposable _subscription = new();

        /// <summary>Guards against settling more than once.</summary>
        private int _settled;

        /// <summary>Initializes a new instance of the <see cref="ActivityResultAwaiter"/> class.</summary>
        /// <param name="requestCode">The request code to await.</param>
        private ActivityResultAwaiter(int requestCode) => _requestCode = requestCode;

        /// <summary>Subscribes to the activity-result stream and returns a task for the first matching result.</summary>
        /// <param name="source">The activity-result stream.</param>
        /// <param name="requestCode">The request code to await.</param>
        /// <returns>A task that completes with the result and intent of the first matching activity result.</returns>
        public static Task<(Result result, Intent intent)> Await(
            IObservable<(int requestCode, Result result, Intent intent)> source,
            int requestCode)
        {
            var awaiter = new ActivityResultAwaiter(requestCode);
            awaiter._subscription.Disposable = source.Subscribe(awaiter);
            return awaiter._completion.Task;
        }

        /// <inheritdoc/>
        public void OnNext((int requestCode, Result result, Intent intent) value)
        {
            if (value.requestCode != _requestCode || Interlocked.Exchange(ref _settled, 1) != 0)
            {
                return;
            }

            _completion.TrySetResult((value.result, value.intent));
            Dispose();
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            if (Interlocked.Exchange(ref _settled, 1) != 0)
            {
                return;
            }

            _completion.TrySetException(error);
            Dispose();
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
            if (Interlocked.Exchange(ref _settled, 1) != 0)
            {
                return;
            }

            _completion.TrySetCanceled();
            Dispose();
        }

        /// <inheritdoc/>
        public void Dispose() => _subscription.Dispose();
    }
}
