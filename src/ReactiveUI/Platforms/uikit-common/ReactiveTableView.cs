// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CoreGraphics;
using Foundation;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>This is a TableView that is both an TableView and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
[SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
public abstract class ReactiveTableView : UITableView, IReactiveNotifyPropertyChanged<ReactiveTableView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
{
    /// <summary>The subject used to signal view activation.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject used to signal view deactivation.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView"/> class.</summary>
    protected ReactiveTableView()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView"/> class.</summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveTableView(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView"/> class.</summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveTableView(NSCoder coder)
        : base(coder)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView"/> class.</summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveTableView(CGRect frame)
        : base(frame)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView"/> class.</summary>
    /// <param name="frame">The frame.</param>
    /// <param name="style">The table view style.</param>
    protected ReactiveTableView(CGRect frame, UITableViewStyle style)
        : base(frame, style)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveTableView(in IntPtr handle)
        : base(handle)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableView>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableView>> Changed => this.GetChangedObservable();

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

    /// <inheritdoc/>
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <inheritdoc/>
    public override void WillMoveToSuperview(UIView? newsuper)
    {
        base.WillMoveToSuperview(newsuper);
        (newsuper is not null ? _activated : _deactivated).OnNext(RxVoid.Default);
    }

    /// <inheritdoc/>
    void ICanForceManualActivation.Activate(bool isActivating) =>
        RxSchedulers.MainThreadScheduler.Schedule(() =>
                                               (isActivating ? _activated : _deactivated).OnNext(RxVoid.Default));

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
