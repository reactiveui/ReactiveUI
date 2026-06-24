// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Android.Content;
using Android.Runtime;
using AndroidX.AppCompat.App;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.AndroidX;
#else
namespace ReactiveUI.AndroidX;
#endif
/// <summary>This is an Activity that is both an Activity and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
public class ReactiveAppCompatActivity : AppCompatActivity, IReactiveObject,
    IReactiveNotifyPropertyChanged<ReactiveAppCompatActivity>, IHandleObservableErrors
{
    /// <summary>The subject that signals when the activity is activated.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject that signals when the activity is deactivated.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>The subject that signals activity results.</summary>
    private readonly Signal<(int requestCode, Result result, Intent? intent)> _activityResult = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveAppCompatActivity" /> class.</summary>
    protected ReactiveAppCompatActivity()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveAppCompatActivity" /> class.</summary>
    /// <param name="handle">The handle.</param>
    /// <param name="ownership">The ownership.</param>
    protected ReactiveAppCompatActivity(in IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveAppCompatActivity>> Changing =>
        this.GetChangingObservable();

    /// <inheritdoc/>
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveAppCompatActivity>> Changed =>
        this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>Gets a signal when activated.</summary>
    /// <value>
    /// The activated.
    /// </value>
    public IObservable<RxVoid> Activated => _activated;

    /// <summary>Gets a signal when deactivated.</summary>
    /// <value>
    /// The deactivated.
    /// </value>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <summary>Gets the activity result.</summary>
    /// <value>
    /// The activity result.
    /// </value>
    public IObservable<(int requestCode, Result result, Intent? intent)> ActivityResult =>
        _activityResult;

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <inheritdoc/>
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
