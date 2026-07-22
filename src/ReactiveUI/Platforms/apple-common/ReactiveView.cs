// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using CoreGraphics;
using Foundation;

#if UIKIT
using NSView = UIKit.UIView;
#else
using AppKit;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>This is a View that is both a NSView and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
public class ReactiveView : NSView, IReactiveNotifyPropertyChanged<ReactiveView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
{
    /// <summary>The subject that signals when the view is activated (moved to a superview).</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject that signals when the view is deactivated (removed from a superview).</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveView"/> class.</summary>
    protected ReactiveView()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveView"/> class.</summary>
    /// <param name="c">The coder.</param>
    protected ReactiveView(NSCoder c)
        : base(c)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveView"/> class.</summary>
    /// <param name="f">The object flag.</param>
    protected ReactiveView(NSObjectFlag f)
        : base(f)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveView"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveView(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveView"/> class.</summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveView(CGRect frame)
        : base(frame)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <inheritdoc/>
    public IObservable<RxVoid> Activated => _activated;

    /// <inheritdoc/>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveView>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveView>> Changed => this.GetChangedObservable();

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
    public override void WillMoveToSuperview(NSView? newsuper)
#else
    /// <inheritdoc/>
    public override void ViewWillMoveToSuperview(NSView? newSuperview)
#endif
    {
#if UIKIT
        base.WillMoveToSuperview(newsuper);
        var superview = newsuper;
#else
        // Xamarin throws ArgumentNullException if newsuper is null
        if (newSuperview is not null)
        {
            base.ViewWillMoveToSuperview(newSuperview);
        }

        var superview = newSuperview;
#endif
        (superview is not null ? _activated : _deactivated).OnNext(RxVoid.Default);
    }

    /// <inheritdoc/>
    void ICanForceManualActivation.Activate(bool isActivating) =>
        RxSchedulers.MainThreadScheduler.Schedule(
            (Owner: this, IsActivating: isActivating),
            static (_, state) =>
            {
                (state.IsActivating ? state.Owner._activated : state.Owner._deactivated).OnNext(RxVoid.Default);
                return EmptyDisposable.Instance;
            });

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
