// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// This is a UICollectionViewCell that is both an UICollectionViewCell and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
public class ReactiveCollectionViewCell : UICollectionViewCell, IReactiveNotifyPropertyChanged<ReactiveCollectionViewCell>, IHandleObservableErrors, IReactiveObject, ICanActivate
{
    /// <summary>The subject used to signal view activation.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject used to signal view deactivation.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewCell"/> class.</summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveCollectionViewCell(CGRect frame)
        : base(frame)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewCell"/> class.</summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveCollectionViewCell(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewCell"/> class.</summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveCollectionViewCell(NSCoder coder)
        : base(NSObjectFlag.Empty)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewCell"/> class.</summary>
    protected ReactiveCollectionViewCell()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewCell"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveCollectionViewCell(in IntPtr handle)
        : base(handle)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewCell>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewCell>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <inheritdoc/>
    public IObservable<RxVoid> Activated => _activated;

    /// <inheritdoc/>
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
    public override void WillMoveToSuperview(UIView? newsuper)
    {
        base.WillMoveToSuperview(newsuper);
        (newsuper is not null ? _activated : _deactivated).OnNext(RxVoid.Default);
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
