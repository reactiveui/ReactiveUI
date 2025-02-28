// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.App;
using Android.Content;
using Android.Runtime;

namespace ReactiveUI;

/// <summary>
/// This is an Activity that is both an Activity and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
public class ReactiveActivity : Activity, IReactiveObject, IReactiveNotifyPropertyChanged<ReactiveActivity>, IHandleObservableErrors
{
    private readonly Subject<Unit> _activated = new();
    private readonly Subject<Unit> _deactivated = new();
    private readonly Subject<(int requestCode, Result resultCode, Intent? intent)> _activityResult = new();

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

    /// <summary>
    /// Gets a signal when the activity is activated.
    /// </summary>
    public IObservable<Unit> Activated => _activated.AsObservable(); // TODO: Create Test

    /// <summary>
    ///  Gets a signal when the activity is deactivated.
    /// </summary>
    public IObservable<Unit> Deactivated => _deactivated.AsObservable(); // TODO: Create Test

    /// <summary>
    /// Gets the activity result.
    /// </summary>
    /// <value>
    /// The activity result.
    /// </value>
    public IObservable<(int requestCode, Result resultCode, Intent? intent)> ActivityResult => // TODO: Create Test
        _activityResult.AsObservable();

    /// <summary>
    /// When this method is called, an object will not fire change
    /// notifications (neither traditional nor Observable notifications)
    /// until the return value is disposed.
    /// </summary>
    /// <returns>An object that, when disposed, reenables change
    /// notifications.</returns>
    public IDisposable SuppressChangeNotifications() => // TODO: Create Test
        IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => // TODO: Create Test
        PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => // TODO: Create Test
        PropertyChanged?.Invoke(this, args);

    /// <summary>
    /// Starts the activity for result asynchronously.
    /// </summary>
    /// <param name="intent">The intent.</param>
    /// <param name="requestCode">The request code.</param>
    /// <returns>A task with the result and the intent.</returns>
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
}
