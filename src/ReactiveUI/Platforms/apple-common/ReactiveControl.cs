// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CoreGraphics;

using Foundation;

#if UIKIT
using UIKit;
#else
using AppKit;

using UIControl = AppKit.NSControl;
#endif

namespace ReactiveUI;

/// <summary>
/// This is a UIControl that is both and UIControl and has a ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
public class ReactiveControl : UIControl, IReactiveNotifyPropertyChanged<ReactiveControl>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
{
    private readonly Subject<Unit> _deactivated = new();
    private readonly Subject<Unit> _activated = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveControl"/> class.
    /// </summary>
    protected ReactiveControl()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveControl"/> class.
    /// </summary>
    /// <param name="c">The c.</param>
    protected ReactiveControl(NSCoder c)
        : base(c)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveControl"/> class.
    /// </summary>
    /// <param name="f">The f.</param>
    protected ReactiveControl(NSObjectFlag f)
        : base(f)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveControl"/> class.
    /// </summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveControl(CGRect frame)
        : base(frame)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveControl"/> class.
    /// </summary>
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
    /// <summary>
    /// Gets a observable when the control is activated.
    /// </summary>
    public new IObservable<Unit> Activated => _activated.AsObservable();
#else
    /// <summary>
    /// Gets a observable when the control is activated.
    /// </summary>
    public IObservable<Unit> Activated => _activated.AsObservable();
#endif

    /// <summary>
    /// Gets a observable that occurs when the control is deactivated.
    /// </summary>
    public IObservable<Unit> Deactivated => _deactivated.AsObservable();

#if UIKIT
    /// <inheritdoc/>
    public override void WillMoveToSuperview(UIView? newsuper)
#else
    /// <inheritdoc/>
    public override void ViewWillMoveToSuperview(NSView? newsuper)
#endif
    {
#if UIKIT
        base.WillMoveToSuperview(newsuper);
#else
        base.ViewWillMoveToSuperview(newsuper);
#endif
        (newsuper is not null ? _activated : _deactivated).OnNext(Unit.Default);
    }

    /// <inheritdoc/>
    void ICanForceManualActivation.Activate(bool activate) =>
        RxSchedulers.MainThreadScheduler.Schedule(() =>
            (activate ? _activated : _deactivated).OnNext(Unit.Default));

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

/// <summary>
/// This is a UIControl that is both and UIControl and has a ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
public abstract class ReactiveControl<TViewModel> : ReactiveControl, IViewFor<TViewModel>
    where TViewModel : class
{
    private TViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveControl{TViewModel}"/> class.
    /// </summary>
    protected ReactiveControl()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveControl{TViewModel}"/> class.
    /// </summary>
    /// <param name="c">The coder.</param>
    protected ReactiveControl(NSCoder c)
        : base(c)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveControl{TViewModel}"/> class.
    /// </summary>
    /// <param name="f">The object flag.</param>
    protected ReactiveControl(NSObjectFlag f)
        : base(f)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveControl{TViewModel}"/> class.
    /// </summary>
    /// <param name="handle">The pointer handle.</param>
    protected ReactiveControl(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveControl{TViewModel}"/> class.
    /// </summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveControl(CGRect frame)
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
