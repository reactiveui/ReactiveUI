// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;

#if UIKIT
using NSViewController = UIKit.UIViewController;
#else
using AppKit;
#endif

namespace ReactiveUI;

/// <summary>
/// This is a View that is both a NSViewController and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
public class ReactiveViewController : NSViewController, IReactiveNotifyPropertyChanged<ReactiveViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
{
    private readonly Subject<Unit> _activated = new();
    private readonly Subject<Unit> _deactivated = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewController"/> class.
    /// </summary>
    protected ReactiveViewController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewController"/> class.
    /// </summary>
    /// <param name="c">The coder.</param>
    protected ReactiveViewController(NSCoder c)
        : base(c)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewController"/> class.
    /// </summary>
    /// <param name="f">The object flag.</param>
    protected ReactiveViewController(NSObjectFlag f)
        : base(f)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewController"/> class.
    /// </summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveViewController(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewController"/> class.
    /// </summary>
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
    public IObservable<Unit> Activated => _activated.AsObservable();

    /// <inheritdoc/>
    public IObservable<Unit> Deactivated => _deactivated.AsObservable();

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
        _activated.OnNext(Unit.Default);
        this.ActivateSubviews(true);
    }

    /// <inheritdoc/>
    public override void ViewDidDisappear(bool animated)
    {
        base.ViewDidDisappear(animated);
        _deactivated.OnNext(Unit.Default);
        this.ActivateSubviews(false);
    }
#else
    /// <inheritdoc/>
    public override void ViewWillAppear()
    {
        base.ViewWillAppear();
        _activated.OnNext(Unit.Default);
        this.ActivateSubviews(true);
    }

    /// <inheritdoc/>
    public override void ViewDidDisappear()
    {
        base.ViewDidDisappear();
        _deactivated.OnNext(Unit.Default);
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

/// <summary>
/// This is a View that is both a NSViewController and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
#if NET6_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
public abstract class ReactiveViewController<TViewModel> : ReactiveViewController, IViewFor<TViewModel>
    where TViewModel : class
{
    private TViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewController{TViewModel}"/> class.
    /// </summary>
    protected ReactiveViewController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewController{TViewModel}"/> class.
    /// </summary>
    /// <param name="c">The coder.</param>
    protected ReactiveViewController(NSCoder c)
        : base(c)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewController{TViewModel}"/> class.
    /// </summary>
    /// <param name="f">The object flag.</param>
    protected ReactiveViewController(NSObjectFlag f)
        : base(f)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewController{TViewModel}"/> class.
    /// </summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveViewController(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewController{TViewModel}"/> class.
    /// </summary>
    /// <param name="nibNameOrNull">The name.</param>
    /// <param name="nibBundleOrNull">The bundle.</param>
    protected ReactiveViewController(string nibNameOrNull, NSBundle nibBundleOrNull)
        : base(nibNameOrNull, nibBundleOrNull)
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
