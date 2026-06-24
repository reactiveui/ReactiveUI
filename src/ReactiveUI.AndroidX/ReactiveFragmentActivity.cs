// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Android.Content;
using AndroidX.Fragment.App;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.AndroidX;
#else
namespace ReactiveUI.AndroidX;
#endif
/// <summary>This is an Activity that is both an Activity and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
public class ReactiveFragmentActivity : FragmentActivity, IReactiveObject,
    IReactiveNotifyPropertyChanged<ReactiveFragmentActivity>, IHandleObservableErrors
{
    /// <summary>The subject that signals when the activity is activated.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject that signals when the activity is deactivated.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>The subject that signals activity results.</summary>
    private readonly Signal<(int requestCode, Result result, Intent? intent)> _activityResult = new();

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

    /// <summary>Gets a signal when the activity fragment is activated.</summary>
    public IObservable<RxVoid> Activated => _activated;

    /// <summary>Gets a signal when the activity fragment is deactivated.</summary>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <summary>Gets the activity result.</summary>
    public IObservable<(int requestCode, Result result, Intent? intent)> ActivityResult =>
        _activityResult;

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <inheritdoc />
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <summary>Starts the activity for result asynchronously.</summary>
    /// <param name="intent">The intent.</param>
    /// <param name="requestCode">The request code.</param>
    /// <returns>A task with the result and intent.</returns>
    public Task<(Result result, Intent? intent)> StartActivityForResultAsync(Intent intent, int requestCode)
    {
        // NB: It's important that we set up the subscription *before* we
        // call ActivityForResult
        var ret = ActivityResultAwaiter.Await(ActivityResult, requestCode);

        StartActivityForResult(intent, requestCode);
        return ret;
    }

    /// <summary>Starts the activity for result asynchronously.</summary>
    /// <param name="type">The type.</param>
    /// <param name="requestCode">The request code.</param>
    /// <returns>A task with the result and intent.</returns>
    public Task<(Result result, Intent? intent)> StartActivityForResultAsync(Type type, int requestCode)
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
            _activated.Dispose();
            _deactivated.Dispose();
            _activityResult.Dispose();
        }

        base.Dispose(disposing);
    }
}
