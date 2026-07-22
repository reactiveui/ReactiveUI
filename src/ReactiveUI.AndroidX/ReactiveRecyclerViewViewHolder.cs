// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using ReactiveUI.Internal;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.AndroidX;
#else
namespace ReactiveUI.AndroidX;
#endif
/// <summary>A <see cref="RecyclerView.ViewHolder"/> implementation that binds to a reactive view model.</summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
[RequiresUnreferencedCode(
    "Android property discovery uses reflection over generated resource types that may be trimmed.")]
[RequiresDynamicCode("Android property discovery discovery uses reflection that may require dynamic code generation.")]
public class ReactiveRecyclerViewViewHolder<TViewModel> : RecyclerView.ViewHolder, ILayoutViewHost,
    IViewFor<TViewModel>, IReactiveNotifyPropertyChanged<ReactiveRecyclerViewViewHolder<TViewModel>>, IReactiveObject,
    ICanActivate
    where TViewModel : class, IReactiveObject
{
    /// <summary>The subject that signals when the view is activated.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The subject that signals when the view is deactivated.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Initializes a new instance of the <see cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> class.</summary>
    /// <param name="view">The view.</param>
    protected ReactiveRecyclerViewViewHolder(View view)
        : base(view)
    {
        SetupRxObj();
        ArgumentExceptionHelper.ThrowIfNull(view);

        view.ViewAttachedToWindow += OnViewAttachedToWindow;
        view.ViewDetachedFromWindow += OnViewDetachedFromWindow;

        Selected = new FromEventObservable<int>(onNext =>
        {
            EventHandler handler = (_, _) => onNext(AbsoluteAdapterPosition);
            view.Click += handler;
            return new ActionDisposable(() => view.Click -= handler);
        });

        LongClicked = new FromEventObservable<int>(onNext =>
        {
            EventHandler<View.LongClickEventArgs> handler = (_, _) => onNext(AbsoluteAdapterPosition);
            view.LongClick += handler;
            return new ActionDisposable(() => view.LongClick -= handler);
        });

        SelectedWithViewModel = new FromEventObservable<TViewModel?>(onNext =>
        {
            EventHandler handler = (_, _) => onNext(ViewModel);
            view.Click += handler;
            return new ActionDisposable(() => view.Click -= handler);
        });

        LongClickedWithViewModel = new FromEventObservable<TViewModel?>(onNext =>
        {
            EventHandler<View.LongClickEventArgs> handler = (_, _) => onNext(ViewModel);
            view.LongClick += handler;
            return new ActionDisposable(() => view.LongClick -= handler);
        });
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets an observable that signals that this ViewHolder has been selected.
    /// <para>
    /// The <see cref="int"/> is the position of this ViewHolder in the <see cref="RecyclerView"/>
    /// and corresponds to the <see cref="RecyclerView.ViewHolder.AbsoluteAdapterPosition"/> property.
    /// </para>
    /// </summary>
    public IObservable<int> Selected { get; }

    /// <summary>
    /// Gets an observable that signals that this ViewHolder has been selected.
    /// <para>The <see cref="IObservable{TViewModel}"/> is the ViewModel of this ViewHolder in the <see cref="RecyclerView"/>.</para>
    /// </summary>
    public IObservable<TViewModel?> SelectedWithViewModel { get; }

    /// <summary>
    /// Gets an observable that signals that this ViewHolder has been long-clicked.
    /// <para>
    /// The <see cref="int"/> is the position of this ViewHolder in the <see cref="RecyclerView"/>
    /// and corresponds to the <see cref="RecyclerView.ViewHolder.AbsoluteAdapterPosition"/> property.
    /// </para>
    /// </summary>
    public IObservable<int> LongClicked { get; }

    /// <summary>
    /// Gets an observable that signals that this ViewHolder has been long-clicked.
    /// <para>The <see cref="IObservable{TViewModel}"/> is the ViewModel of this ViewHolder in the <see cref="RecyclerView"/>.</para>
    /// </summary>
    public IObservable<TViewModel?> LongClickedWithViewModel { get; }

    /// <inheritdoc/>
    public IObservable<RxVoid> Activated => _activated;

    /// <inheritdoc/>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <summary>Gets the current view being shown.</summary>
    public View View => ItemView;

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets an observable which signals when exceptions are thrown.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }

    /// <inheritdoc/>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changing =>
        this.GetChangingObservable();

    /// <inheritdoc/>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changed =>
        this.GetChangedObservable();

    /// <summary>Gets or sets the lazily-computed set of public properties used by legacy reflection-based wiring.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    protected Lazy<PropertyInfo[]>? AllPublicProperties { get; set; }

    /// <inheritdoc/>
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <summary>Gets if change notifications via the INotifyPropertyChanged interface are being sent.</summary>
    /// <returns>A value indicating whether change notifications are enabled or not.</returns>
    public bool AreChangeNotificationsEnabled() => IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            View.ViewAttachedToWindow -= OnViewAttachedToWindow;
            View.ViewDetachedFromWindow -= OnViewDetachedFromWindow;

            _activated.Dispose();
            _deactivated.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>Sets up the reactive object after deserialization.</summary>
    /// <param name="sc">The streaming context.</param>
    [OnDeserialized]
    private void SetupRxObj(in StreamingContext sc) => SetupRxObj();

    /// <summary>Sets up the reactive object by initializing the public property cache.</summary>
    private void SetupRxObj() =>
        AllPublicProperties = new(() => [.. GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)]);

    /// <summary>Handles the view being attached to the window and signals activation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event arguments.</param>
    private void OnViewAttachedToWindow(object? sender, View.ViewAttachedToWindowEventArgs args) =>
        _activated.OnNext(RxVoid.Default);

    /// <summary>Handles the view being detached from the window and signals deactivation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event arguments.</param>
    private void OnViewDetachedFromWindow(object? sender, View.ViewDetachedFromWindowEventArgs args) =>
        _deactivated.OnNext(RxVoid.Default);
}
