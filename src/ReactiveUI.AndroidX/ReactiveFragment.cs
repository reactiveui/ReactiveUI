// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.AndroidX;
#else
namespace ReactiveUI.AndroidX;
#endif
/// <summary>This is a Fragment that is both an Activity and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
[ExcludeFromCodeCoverage]
[System.Diagnostics.DebuggerDisplay("{Activated}, {Deactivated}")]
public class ReactiveFragment : global::AndroidX.Fragment.App.Fragment,
    IReactiveNotifyPropertyChanged<ReactiveFragment>, IReactiveObject, IHandleObservableErrors
{
    /// <summary>The subject that signals when the fragment is activated.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject that signals when the fragment is deactivated.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveFragment"/> class.</summary>
    protected ReactiveFragment()
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>Gets a signal when the fragment is activated.</summary>
    public IObservable<RxVoid> Activated => _activated;

    /// <summary>Gets a signal when the fragment is deactivated.</summary>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveFragment>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveFragment>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <inheritdoc/>
    public override void OnPause()
    {
        base.OnPause();
        ActivationSignals.Raise(_deactivated);
    }

    /// <inheritdoc/>
    public override void OnResume()
    {
        base.OnResume();
        ActivationSignals.Raise(_activated);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        ActivationSignals.DisposeWhen(disposing, _activated, _deactivated);
        base.Dispose(disposing);
    }
}
