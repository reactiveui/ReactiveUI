// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Runtime.Versioning;
using Android.Runtime;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>This is a Fragment that is both an Activity and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
public class ReactiveFragment : Fragment, IReactiveNotifyPropertyChanged<ReactiveFragment>, IReactiveObject,
    IHandleObservableErrors
{
    /// <summary>The subject that signals when the fragment is activated.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject that signals when the fragment is deactivated.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveFragment"/> class.</summary>
    protected ReactiveFragment()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveFragment"/> class.</summary>
    /// <param name="handle">The handle.</param>
    /// <param name="ownership">The ownership.</param>
    [ObsoletedOSPlatform("android28.0")]
    protected ReactiveFragment(in IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveFragment>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveFragment>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>Gets the activated.</summary>
    /// <value>
    /// The activated.
    /// </value>
    public IObservable<RxVoid> Activated => _activated;

    /// <summary>Gets a signal when the fragment is deactivated.</summary>
    /// <value>
    /// The deactivated.
    /// </value>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <summary>
    /// When this method is called, an object will not fire change
    /// notifications (neither traditional nor Observable notifications)
    /// until the return value is disposed.
    /// </summary>
    /// <returns>An object that, when disposed, reenables change
    /// notifications.</returns>
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <inheritdoc/>
    [ObsoletedOSPlatform("android28.0")]
    public override void OnPause()
    {
        base.OnPause();
        _deactivated.OnNext(RxVoid.Default);
    }

    /// <inheritdoc/>
    [ObsoletedOSPlatform("android28.0")]
    public override void OnResume()
    {
        base.OnResume();
        _activated.OnNext(RxVoid.Default);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _activated?.Dispose();
            _deactivated?.Dispose();
        }

        base.Dispose(disposing);
    }
}
