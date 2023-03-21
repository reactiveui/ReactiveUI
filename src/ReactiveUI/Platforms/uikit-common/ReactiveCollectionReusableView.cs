// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ReactiveUI;

/// <summary>
/// This is a UICollectionReusableView that is both an UICollectionReusableView and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
[SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
public abstract class ReactiveCollectionReusableView : UICollectionReusableView, IReactiveNotifyPropertyChanged<ReactiveCollectionReusableView>, IHandleObservableErrors, IReactiveObject, ICanActivate
{
    private readonly Subject<Unit> _activated = new();
    private readonly Subject<Unit> _deactivated = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView"/> class.
    /// </summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveCollectionReusableView(CGRect frame)
        : base(frame)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView"/> class.
    /// </summary>
    protected ReactiveCollectionReusableView()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView"/> class.
    /// </summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveCollectionReusableView(IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView"/> class.
    /// </summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveCollectionReusableView(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView"/> class.
    /// </summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveCollectionReusableView(NSCoder coder)
        : base(coder)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionReusableView>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionReusableView>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <inheritdoc/>
    public IObservable<Unit> Activated => _activated.AsObservable();

    /// <inheritdoc/>
    public IObservable<Unit> Deactivated => _deactivated.AsObservable();

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
    {
        var handler = PropertyChanging;
        if (handler is not null)
        {
            handler(this, args);
        }
    }

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        var handler = PropertyChanged;
        if (handler is not null)
        {
            handler(this, args);
        }
    }

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
        _activated.OnNext(Unit.Default);
    }

    /// <inheritdoc/>
    public override void RemoveFromSuperview()
    {
        base.RemoveFromSuperview();
        _deactivated.OnNext(Unit.Default);
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

/// <summary>
/// This is a UICollectionReusableView that is both an UICollectionReusableView and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
[SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
public abstract class ReactiveCollectionReusableView<TViewModel> : ReactiveCollectionReusableView, IViewFor<TViewModel>
    where TViewModel : class
{
    private TViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView{TViewModel}"/> class.
    /// </summary>
    protected ReactiveCollectionReusableView()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView{TViewModel}"/> class.
    /// </summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveCollectionReusableView(IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView{TViewModel}"/> class.
    /// </summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveCollectionReusableView(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView{TViewModel}"/> class.
    /// </summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveCollectionReusableView(NSCoder coder)
        : base(coder)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView{TViewModel}"/> class.
    /// </summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveCollectionReusableView(CGRect frame)
        : base(frame)
    {
    }

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => _viewModel;
        set => this.RaiseAndSetIfChanged(ref _viewModel, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel)value!;
    }
}
