// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.AndroidX;
#else
namespace ReactiveUI.AndroidX;
#endif
/// <summary>This is a Fragment that is both an Activity and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
public class ReactiveDialogFragment : global::AndroidX.Fragment.App.DialogFragment,
    IReactiveNotifyPropertyChanged<ReactiveDialogFragment>, IReactiveObject, IHandleObservableErrors
{
    /// <summary>The subject that signals when the fragment is activated.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject that signals when the fragment is deactivated.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveDialogFragment"/> class.</summary>
    protected ReactiveDialogFragment()
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>Gets a observable that signals when the fragment is activated.</summary>
    public IObservable<RxVoid> Activated => _activated;

    /// <summary>Gets a observable that signals when the fragment is deactivated.</summary>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveDialogFragment>> Changing =>
        this.GetChangingObservable();

    /// <inheritdoc/>
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveDialogFragment>> Changed =>
        this.GetChangedObservable();

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <inheritdoc />
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <inheritdoc/>
    public override void OnPause()
    {
        base.OnPause();
        _deactivated.OnNext(RxVoid.Default);
    }

    /// <inheritdoc/>
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
            _activated.Dispose();
            _deactivated.Dispose();
        }

        base.Dispose(disposing);
    }
}
