// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

using Android.Content;
using Android.Views;

namespace ReactiveUI;

/// <summary>
/// A class that implements the Android ViewHolder pattern with a
/// ViewModel. Use it along with GetViewHost.
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public abstract class ReactiveViewHost<TViewModel> : LayoutViewHost, IViewFor<TViewModel>, IReactiveNotifyPropertyChanged<ReactiveViewHost<TViewModel>>, IReactiveObject
    where TViewModel : class, IReactiveObject
{
    /// <summary>
    /// All public properties.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401: Field should be private", Justification = "Legacy reasons")]
    [SuppressMessage("Design", "CA1051: Do not declare visible instance fields", Justification = "Legacy reasons")]
    [IgnoreDataMember]
    protected Lazy<PropertyInfo[]>? allPublicProperties;

    private TViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewHost{TViewModel}"/> class.
    /// </summary>
    /// <param name="ctx">The CTX.</param>
    /// <param name="layoutId">The layout identifier.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="attachToRoot">if set to <c>true</c> [attach to root].</param>
    /// <param name="performAutoWireup">if set to <c>true</c> [perform automatic wireup].</param>
    protected ReactiveViewHost(Context ctx, int layoutId, ViewGroup parent, bool attachToRoot = false, bool performAutoWireup = true)
        : base(ctx, layoutId, parent, attachToRoot, performAutoWireup) =>
        SetupRxObj();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewHost{TViewModel}"/> class.
    /// </summary>
    protected ReactiveViewHost() => SetupRxObj();

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

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
        set => _viewModel = (TViewModel?)value!;
    }

    /// <inheritdoc />
    [IgnoreDataMember]
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewHost<TViewModel>>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    [IgnoreDataMember]
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewHost<TViewModel>>> Changed => this.GetChangedObservable();

    /// <summary>
    /// Gets the thrown exceptions.
    /// </summary>
    [IgnoreDataMember]
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>
    /// When this method is called, an object will not fire change
    /// notifications (neither traditional nor Observable notifications)
    /// until the return value is disposed.
    /// </summary>
    /// <returns>An object that, when disposed, reenables change
    /// notifications.</returns>
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this); // TODO: Create Test

    /// <summary>
    /// Gets a value indicating if change notifications are enabled.
    /// </summary>
    /// <returns>A value indicating if change notifications are on or off.</returns>
    public bool AreChangeNotificationsEnabled() => IReactiveObjectExtensions.AreChangeNotificationsEnabled(this); // TODO: Create Test

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    [OnDeserialized]
    private void SetupRxObj(StreamingContext sc) => SetupRxObj();

    private void SetupRxObj() =>
        allPublicProperties = new Lazy<PropertyInfo[]>(() =>
                                                           GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray());
}
