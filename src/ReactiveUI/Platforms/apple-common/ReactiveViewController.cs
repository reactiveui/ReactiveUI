// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Foundation;

#if UIKIT
using NSViewController = UIKit.UIViewController;
#else
using AppKit;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>This is a View that is both a NSViewController and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
[DebuggerDisplay("{Activated}, {Deactivated}")]
public class ReactiveViewController : NSViewController, IReactiveNotifyPropertyChanged<ReactiveViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
{
    /// <summary>The subject used to signal view activation.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject used to signal view deactivation.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveViewController"/> class.</summary>
    protected ReactiveViewController()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveViewController"/> class.</summary>
    /// <param name="c">The coder.</param>
    protected ReactiveViewController(NSCoder c)
        : base(c)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveViewController"/> class.</summary>
    /// <param name="f">The object flag.</param>
    protected ReactiveViewController(NSObjectFlag f)
        : base(f)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveViewController"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveViewController(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveViewController"/> class.</summary>
    /// <param name="nibNameOrNull">The name.</param>
    /// <param name="nibBundleOrNull">The bundle.</param>
    protected ReactiveViewController(string nibNameOrNull, NSBundle nibBundleOrNull)
        : base(nibNameOrNull, nibBundleOrNull)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewController>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewController>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <inheritdoc/>
    public IObservable<RxVoid> Activated => _activated;

    /// <inheritdoc/>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <summary>
    /// When this method is called, an object will not fire change
    /// notifications (neither traditional nor Observable notifications)
    /// until the return value is disposed.
    /// </summary>
    /// <returns>An object that, when disposed, reenables change
    /// notifications.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

#if UIKIT
    /// <inheritdoc/>
    public override void ViewWillAppear(bool animated)
    {
        base.ViewWillAppear(animated);
        this.RaiseActivation(_activated, _deactivated, true);
    }

    /// <inheritdoc/>
    public override void ViewDidDisappear(bool animated)
    {
        base.ViewDidDisappear(animated);
        this.RaiseActivation(_activated, _deactivated, false);
    }
#else
    /// <inheritdoc/>
    public override void ViewWillAppear()
    {
        base.ViewWillAppear();
        this.RaiseActivation(_activated, _deactivated, true);
    }

    /// <inheritdoc/>
    public override void ViewDidDisappear()
    {
        base.ViewDidDisappear();
        this.RaiseActivation(_activated, _deactivated, false);
    }
#endif

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        ActivationSignals.DisposeWhen(disposing, _activated, _deactivated);
        base.Dispose(disposing);
    }
}
