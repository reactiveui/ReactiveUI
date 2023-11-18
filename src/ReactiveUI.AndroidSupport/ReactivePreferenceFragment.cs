// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Support.V7.Preferences;

namespace ReactiveUI.AndroidSupport;

/// <summary>
/// This is a PreferenceFragment that is both an Activity and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
public abstract class ReactivePreferenceFragment<TViewModel> : ReactivePreferenceFragment, IViewFor<TViewModel>, ICanActivate
    where TViewModel : class
{
    private TViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePreferenceFragment{TViewModel}"/> class.
    /// </summary>
    protected ReactivePreferenceFragment()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePreferenceFragment{TViewModel}"/> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="ownership">The ownership.</param>
    protected ReactivePreferenceFragment(in IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
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
        get => _viewModel;
        set => _viewModel = (TViewModel?)value;
    }
}

/// <summary>
/// This is a PreferenceFragment that is both an Activity and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
public abstract class ReactivePreferenceFragment : PreferenceFragmentCompat, IReactiveNotifyPropertyChanged<ReactivePreferenceFragment>, IReactiveObject, IHandleObservableErrors
{
    private readonly Subject<Unit> _activated = new();
    private readonly Subject<Unit> _deactivated = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePreferenceFragment"/> class.
    /// </summary>
    protected ReactivePreferenceFragment()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePreferenceFragment"/> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="ownership">The ownership.</param>
    protected ReactivePreferenceFragment(in IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceFragment>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceFragment>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>
    /// Gets a signal when the fragment is activated.
    /// </summary>
    public IObservable<Unit> Activated => _activated.AsObservable();

    /// <summary>
    /// Gets a signal when the fragment is deactivated.
    /// </summary>
    public IObservable<Unit> Deactivated => _deactivated.AsObservable();

    /// <inheritdoc/>
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    public override void OnPause()
    {
        base.OnPause();
        _deactivated.OnNext(Unit.Default);
    }

    /// <inheritdoc/>
    public override void OnResume()
    {
        base.OnResume();
        _activated.OnNext(Unit.Default);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _activated.Dispose();
            _deactivated.Dispose();
        }

        base.Dispose(disposing);
    }
}
