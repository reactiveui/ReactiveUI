// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using CoreGraphics;
using Foundation;

#if UIKIT
using UIKit;
#else
using AppKit;

using UIControl = AppKit.NSControl;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// This is a UIControl that is both and UIControl and has a ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
public class ReactiveControl : UIControl, IReactiveNotifyPropertyChanged<ReactiveControl>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
{
    /// <summary>The subject that emits when the control is deactivated (removed from its superview).</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>The subject that emits when the control is activated (added to a superview).</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveControl"/> class.</summary>
    protected ReactiveControl()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveControl"/> class.</summary>
    /// <param name="c">The c.</param>
    protected ReactiveControl(NSCoder c)
        : base(c)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveControl"/> class.</summary>
    /// <param name="f">The f.</param>
    protected ReactiveControl(NSObjectFlag f)
        : base(f)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveControl"/> class.</summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveControl(CGRect frame)
        : base(frame)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveControl"/> class.</summary>
    /// <param name="handle">The handle.</param>
    protected ReactiveControl(in IntPtr handle)
        : base(handle)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveControl>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveControl>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

#if MAC
    /// <summary>Gets a observable when the control is activated.</summary>
    public new IObservable<RxVoid> Activated => _activated;
#else
    /// <summary>Gets a observable when the control is activated.</summary>
    public IObservable<RxVoid> Activated => _activated;
#endif

    /// <summary>Gets a observable that occurs when the control is deactivated.</summary>
    public IObservable<RxVoid> Deactivated => _deactivated;

#if UIKIT
    /// <inheritdoc/>
    public override void WillMoveToSuperview(UIView? newsuper)
#else
    /// <inheritdoc/>
    public override void ViewWillMoveToSuperview(NSView? newSuperview)
#endif
    {
#if UIKIT
        base.WillMoveToSuperview(newsuper);
        var superview = newsuper;
#else
        base.ViewWillMoveToSuperview(newSuperview);
        var superview = newSuperview;
#endif
        (superview is not null ? _activated : _deactivated).OnNext(RxVoid.Default);
    }

    /// <inheritdoc/>
    void ICanForceManualActivation.Activate(bool isActivating) =>
        RxSchedulers.MainThreadScheduler.Schedule(() =>
            (isActivating ? _activated : _deactivated).OnNext(RxVoid.Default));

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
