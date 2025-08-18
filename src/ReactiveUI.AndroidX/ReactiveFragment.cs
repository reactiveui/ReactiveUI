// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.AndroidX;

/// <summary>
/// This is a Fragment that is both an Activity and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("ReactiveFragment inherits from ReactiveObject which uses extension methods that require dynamic code generation")]
[RequiresUnreferencedCode("ReactiveFragment inherits from ReactiveObject which uses extension methods that may require unreferenced code")]
#endif
[ExcludeFromCodeCoverage]
public class ReactiveFragment : global::AndroidX.Fragment.App.Fragment, IReactiveNotifyPropertyChanged<ReactiveFragment>, IReactiveObject, IHandleObservableErrors
{
    private readonly Subject<Unit> _activated = new();
    private readonly Subject<Unit> _deactivated = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveFragment"/> class.
    /// </summary>
    protected ReactiveFragment()
    {
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

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

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveFragment>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveFragment>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <inheritdoc />
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

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
