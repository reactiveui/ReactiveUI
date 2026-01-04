// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

using Android.Content;
using Android.Views;

using static ReactiveUI.ControlFetcherMixin;

namespace ReactiveUI;

/// <summary>
/// A class that implements the Android ViewHolder pattern with a ViewModel.
/// Use it along with GetViewHost.
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
/// <remarks>
/// <para>
/// Trimming/AOT: Prefer constructors that do not enable legacy auto-wireup. These paths avoid reflection and do not
/// allocate property metadata.
/// </para>
/// <para>
/// Compatibility: A legacy constructor is provided that enables reflection-based wiring and initializes
/// <see cref="allPublicProperties"/> for older infrastructure.
/// </para>
/// </remarks>
public abstract class ReactiveViewHost<TViewModel> :
    LayoutViewHost,
    IViewFor<TViewModel>,
    IReactiveNotifyPropertyChanged<ReactiveViewHost<TViewModel>>,
    IReactiveObject
    where TViewModel : class, IReactiveObject
{
    /// <summary>
    /// All public properties.
    /// </summary>
    /// <remarks>
    /// This field is used by legacy reflection-based wiring. It is not initialized by default in AOT-safe construction
    /// paths to avoid reflection and allocations. If a derived type requires this, use the legacy constructor.
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401: Field should be private", Justification = "Legacy reasons")]
    [SuppressMessage("Design", "CA1051: Do not declare visible instance fields", Justification = "Legacy reasons")]
    [IgnoreDataMember]
    [JsonIgnore]
    protected Lazy<PropertyInfo[]>? allPublicProperties;

    /// <summary>
    /// Backing field for <see cref="ViewModel"/>.
    /// </summary>
    private TViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewHost{TViewModel}"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor performs no inflation or wiring and is AOT-safe.
    /// Derived types may assign <see cref="LayoutViewHost.View"/> manually.
    /// </remarks>
    protected ReactiveViewHost()
    {
        SetupRxObjAot();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewHost{TViewModel}"/> class by inflating a layout resource.
    /// </summary>
    /// <param name="ctx">The Android context.</param>
    /// <param name="layoutId">The layout resource identifier.</param>
    /// <param name="parent">The parent view group.</param>
    /// <param name="attachToRoot">Whether to attach the inflated view to the parent.</param>
    /// <remarks>
    /// This constructor is fully AOT- and trimming-safe and performs no reflection-based auto-wireup.
    /// </remarks>
    protected ReactiveViewHost(Context ctx, int layoutId, ViewGroup parent, bool attachToRoot = false)
        : base(ctx, layoutId, parent, attachToRoot)
    {
        SetupRxObjAot();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewHost{TViewModel}"/> class by inflating a layout resource
    /// and invoking an explicit, AOT-safe binder callback.
    /// </summary>
    /// <param name="ctx">The Android context.</param>
    /// <param name="layoutId">The layout resource identifier.</param>
    /// <param name="parent">The parent view group.</param>
    /// <param name="attachToRoot">Whether to attach the inflated view to the parent.</param>
    /// <param name="bind">
    /// A callback responsible for explicitly wiring child views to the host.
    /// </param>
    /// <remarks>
    /// This constructor is fully AOT-safe and avoids reflection entirely.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="bind"/> is <see langword="null"/>.</exception>
    protected ReactiveViewHost(Context ctx, int layoutId, ViewGroup parent, bool attachToRoot, Action<ReactiveViewHost<TViewModel>, View> bind)
        : base(
            ctx,
            layoutId,
            parent,
            attachToRoot,
            (host, view) =>
            {
                // The base constructor guarantees 'host' is the derived instance.
                bind((ReactiveViewHost<TViewModel>)host, view);
            })
    {
        ArgumentNullException.ThrowIfNull(bind);
        SetupRxObjAot();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveViewHost{TViewModel}"/> class by inflating a layout resource
    /// and optionally performing reflection-based auto-wireup.
    /// </summary>
    /// <param name="ctx">The Android context.</param>
    /// <param name="layoutId">The layout resource identifier.</param>
    /// <param name="parent">The parent view group.</param>
    /// <param name="attachToRoot">Whether to attach the inflated view to the parent.</param>
    /// <param name="performAutoWireup">
    /// If <see langword="true"/>, performs automatic wiring using reflection.
    /// </param>
    /// <param name="resolveStrategy">
    /// The member resolution strategy used during auto-wireup.
    /// </param>
    /// <remarks>
    /// This constructor exists for backward compatibility and is not trimming/AOT safe when
    /// <paramref name="performAutoWireup"/> is <see langword="true"/>.
    /// </remarks>
    [RequiresUnreferencedCode("Legacy auto-wireup uses reflection and member discovery.")]
    [RequiresDynamicCode("Legacy auto-wireup relies on runtime type inspection.")]
    protected ReactiveViewHost(
        Context ctx,
        int layoutId,
        ViewGroup parent,
        bool attachToRoot,
        bool performAutoWireup,
        ResolveStrategy resolveStrategy)
        : base(ctx, layoutId, parent, attachToRoot, performAutoWireup, resolveStrategy)
    {
        SetupRxObjLegacyReflection();
    }

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
        set => ViewModel = (TViewModel?)value;
    }

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewHost<TViewModel>>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewHost<TViewModel>>> Changed => this.GetChangedObservable();

    /// <summary>
    /// Gets an observable of exceptions thrown during reactive operations on this instance.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>
    /// When this method is called, an object will not fire change notifications (neither traditional nor observable)
    /// until the return value is disposed.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> that re-enables change notifications when disposed.</returns>
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <summary>
    /// Gets a value indicating whether change notifications are enabled.
    /// </summary>
    /// <returns><see langword="true"/> if change notifications are enabled; otherwise, <see langword="false"/>.</returns>
    public bool AreChangeNotificationsEnabled() => IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <summary>
    /// Reinitializes reactive infrastructure after deserialization.
    /// </summary>
    /// <param name="sc">The streaming context.</param>
    [OnDeserialized]
    private void SetupRxObj(in StreamingContext sc) => SetupRxObjAot();

    /// <summary>
    /// Initializes the instance for AOT-safe operation.
    /// </summary>
    /// <remarks>
    /// This method intentionally does not touch <see cref="allPublicProperties"/> to avoid reflection and allocations.
    /// </remarks>
    private void SetupRxObjAot()
    {
        // No reflection-based property caching in AOT-safe paths.
        allPublicProperties = null;
    }

    /// <summary>
    /// Initializes legacy reflection metadata used by older auto-wireup infrastructure.
    /// </summary>
    /// <remarks>
    /// This allocates reflection metadata and is not trimming/AOT safe.
    /// </remarks>
    [RequiresUnreferencedCode("This method uses reflection to enumerate public instance properties.")]
    [RequiresDynamicCode("This method uses reflection to enumerate public instance properties.")]
    private void SetupRxObjLegacyReflection()
    {
        allPublicProperties = new Lazy<PropertyInfo[]>(
            () => GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance));
    }
}
