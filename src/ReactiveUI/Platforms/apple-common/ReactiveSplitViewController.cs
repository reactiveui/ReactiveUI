// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Foundation;

#if UIKIT
using NSSplitViewController = UIKit.UISplitViewController;
#else
using AppKit;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// This is a View that is both a NSSplitViewController and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
public class ReactiveSplitViewController : NSSplitViewController, IReactiveNotifyPropertyChanged<ReactiveSplitViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
{
    /// <summary>The subject used to signal view activation.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject used to signal view deactivation.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

#if UIKIT
    /// <summary>Initializes a new instance of the <see cref="ReactiveSplitViewController"/> class.</summary>
    /// <param name="nibName">The name.</param>
    /// <param name="bundle">The bundle.</param>
    protected ReactiveSplitViewController(string nibName, NSBundle bundle)
        : base(nibName, bundle)
    {
    }
#endif

    /// <summary>Initializes a new instance of the <see cref="ReactiveSplitViewController"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveSplitViewController(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveSplitViewController"/> class.</summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveSplitViewController(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveSplitViewController"/> class.</summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveSplitViewController(NSCoder coder)
        : base(coder)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveSplitViewController"/> class.</summary>
    protected ReactiveSplitViewController()
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveSplitViewController>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveSplitViewController>> Changed => this.GetChangedObservable();

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

#if UIKIT
    /// <inheritdoc/>
    public override void ViewWillAppear(bool animated)
    {
        base.ViewWillAppear(animated);
        _activated.OnNext(RxVoid.Default);
        this.ActivateSubviews(true);
    }

    /// <inheritdoc/>
    public override void ViewDidDisappear(bool animated)
    {
        base.ViewDidDisappear(animated);
        _deactivated.OnNext(RxVoid.Default);
        this.ActivateSubviews(false);
    }

#else
    /// <inheritdoc/>
    public override void ViewWillAppear()
    {
        base.ViewWillAppear();
        _activated.OnNext(RxVoid.Default);
        this.ActivateSubviews(true);
    }

    /// <inheritdoc/>
    public override void ViewDidDisappear()
    {
        base.ViewDidDisappear();
        _deactivated.OnNext(RxVoid.Default);
        this.ActivateSubviews(false);
    }
#endif

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
