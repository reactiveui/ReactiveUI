// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;

using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Runtime;

namespace ReactiveUI;

/// <summary>
/// This is an Activity that is both an Activity and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("ReactivePreferenceActivity inherits from ReactiveObject which uses extension methods that require dynamic code generation")]
[RequiresUnreferencedCode("ReactivePreferenceActivity inherits from ReactiveObject which uses extension methods that may require unreferenced code")]
#endif
public class ReactivePreferenceActivity : PreferenceActivity, IReactiveObject, IReactiveNotifyPropertyChanged<ReactivePreferenceActivity>, IHandleObservableErrors
{
    private readonly Subject<Unit> _activated = new();
    private readonly Subject<Unit> _deactivated = new();
    private readonly Subject<(int requestCode, Result resultCode, Intent? intent)> _activityResult = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePreferenceActivity"/> class.
    /// </summary>
    protected ReactivePreferenceActivity()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePreferenceActivity"/> class.
    /// </summary>
    /// <param name="handle">The pointer.</param>
    /// <param name="ownership">The ownership.</param>
    [ObsoletedOSPlatform("android28.0")]
    protected ReactivePreferenceActivity(in IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceActivity>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceActivity>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>
    ///  Gets a signal when the activity is activated.
    /// </summary>
    /// <value>
    /// The deactivated.
    /// </value>
    public IObservable<Unit> Activated => _activated.AsObservable(); // TODO: Create Test

    /// <summary>
    ///  Gets a signal when the activity is deactivated.
    /// </summary>
    /// <value>
    /// The deactivated.
    /// </value>
    public IObservable<Unit> Deactivated => _deactivated.AsObservable(); // TODO: Create Test

    /// <summary>
    ///  Gets a signal with an activity result.
    /// </summary>
    /// <value>
    /// The deactivated.
    /// </value>
    public IObservable<(int requestCode, Result resultCode, Intent? intent)> ActivityResult => // TODO: Create Test
        _activityResult.AsObservable();

    /// <summary>
    /// When this method is called, an object will not fire change
    /// notifications (neither traditional nor Observable notifications)
    /// until the return value is disposed.
    /// </summary>
    /// <returns>An object that, when disposed, re-enables change
    /// notifications.</returns>
    public IDisposable SuppressChangeNotifications() => // TODO: Create Test
        IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <summary>
    /// Starts the activity for result asynchronously.
    /// </summary>
    /// <param name="intent">The intent.</param>
    /// <param name="requestCode">The request code.</param>
    /// <returns>A task with the result and intent.</returns>
    public Task<(Result resultCode, Intent? intent)> StartActivityForResultAsync(Intent? intent, int requestCode) // TODO: Create Test
    {
        // NB: It's important that we set up the subscription *before* we
        // call ActivityForResult
        var ret = ActivityResult
                  .Where(x => x.requestCode == requestCode)
                  .Select(x => (x.resultCode, x.intent))
                  .FirstAsync()
                  .ToTask();

        StartActivityForResult(intent, requestCode);
        return ret;
    }

    /// <summary>
    /// Starts the activity for result asynchronously.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="requestCode">The request code.</param>
    /// <returns>A task with the result and intent.</returns>
    public Task<(Result resultCode, Intent? intent)> StartActivityForResultAsync(Type type, int requestCode) // TODO: Create Test
    {
        // NB: It's important that we set up the subscription *before* we
        // call ActivityForResult
        var ret = ActivityResult
                  .Where(x => x.requestCode == requestCode)
                  .Select(x => (x.resultCode, x.intent))
                  .FirstAsync()
                  .ToTask();

        StartActivityForResult(type, requestCode);
        return ret;
    }

    /// <inheritdoc/>
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        _activityResult.OnNext((requestCode, resultCode, data));
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
}
