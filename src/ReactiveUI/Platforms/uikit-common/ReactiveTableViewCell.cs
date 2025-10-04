// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CoreGraphics;

using Foundation;

using UIKit;

namespace ReactiveUI;

/// <summary>
/// This is a UITableViewCell that is both an UITableViewCell and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
[SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
#if NET6_0_OR_GREATER
[RequiresDynamicCode("ReactiveTableViewCell inherits from ReactiveObject which uses extension methods that require dynamic code generation")]
[RequiresUnreferencedCode("ReactiveTableViewCell inherits from ReactiveObject which uses extension methods that may require unreferenced code")]
#endif
public abstract class ReactiveTableViewCell : UITableViewCell, IReactiveNotifyPropertyChanged<ReactiveTableViewCell>, IHandleObservableErrors, IReactiveObject, ICanActivate
{
    private readonly Subject<Unit> _activated = new();
    private readonly Subject<Unit> _deactivated = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
    /// </summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveTableViewCell(CGRect frame)
        : base(frame)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
    /// </summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveTableViewCell(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
    /// </summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveTableViewCell(NSCoder coder)
        : base(NSObjectFlag.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
    /// </summary>
    protected ReactiveTableViewCell()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
    /// </summary>
    /// <param name="style">The ui table view cell style.</param>
    /// <param name="reuseIdentifier">The reuse identifier.</param>
    protected ReactiveTableViewCell(UITableViewCellStyle style, string reuseIdentifier)
        : base(style, reuseIdentifier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
    /// </summary>
    /// <param name="style">The ui table view cell style.</param>
    /// <param name="reuseIdentifier">The reuse identifier.</param>
    protected ReactiveTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier)
        : base(style, reuseIdentifier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
    /// </summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveTableViewCell(in IntPtr handle)
        : base(handle)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewCell>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewCell>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <inheritdoc/>
    public IObservable<Unit> Activated => _activated.AsObservable();

    /// <inheritdoc/>
    public IObservable<Unit> Deactivated => _deactivated.AsObservable();

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
        (newsuper is not null ? _activated : _deactivated).OnNext(Unit.Default);
    }

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

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
/// This is a UITableViewCell that is both an UITableViewCell and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
[SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
#if NET6_0_OR_GREATER
[RequiresDynamicCode("ReactiveTableViewCell<TViewModel> inherits from ReactiveObject which uses extension methods that require dynamic code generation")]
[RequiresUnreferencedCode("ReactiveTableViewCell<TViewModel> inherits from ReactiveObject which uses extension methods that may require unreferenced code")]
#endif
public abstract class ReactiveTableViewCell<TViewModel> : ReactiveTableViewCell, IViewFor<TViewModel>
    where TViewModel : class
{
    private TViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
    /// </summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveTableViewCell(CGRect frame)
        : base(frame)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
    /// </summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveTableViewCell(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
    /// </summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveTableViewCell(NSCoder coder)
        : base(NSObjectFlag.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
    /// </summary>
    protected ReactiveTableViewCell()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
    /// </summary>
    /// <param name="style">The ui table view cell style.</param>
    /// <param name="reuseIdentifier">The reuse identifier.</param>
    protected ReactiveTableViewCell(UITableViewCellStyle style, string reuseIdentifier)
        : base(style, reuseIdentifier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
    /// </summary>
    /// <param name="style">The ui table view cell style.</param>
    /// <param name="reuseIdentifier">The reuse identifier.</param>
    protected ReactiveTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier)
        : base(style, reuseIdentifier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
    /// </summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveTableViewCell(in IntPtr handle)
        : base(handle)
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
