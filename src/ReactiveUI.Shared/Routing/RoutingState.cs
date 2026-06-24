// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>RoutingState manages the ViewModel Stack and allows ViewModels to navigate to other ViewModels.</summary>
/// <remarks>
/// <para>
/// Use <see cref="RoutingState"/> from an <see cref="IScreen"/> implementation to coordinate navigation in
/// multi-page applications. The stack works in a last-in-first-out fashion, enabling forward navigation via
/// <see cref="Navigate"/> and back navigation via <see cref="NavigateBack"/>. Consumers can observe
/// <see cref="CurrentViewModel"/> or <see cref="NavigationChanges"/> to drive view presentation.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// public class ShellViewModel : ReactiveObject, IScreen
/// {
///     public RoutingState Router { get; } = new();
///
///     public ShellViewModel()
///     {
///         Router.Navigate.Execute(new HomeViewModel(this)).Subscribe();
///     }
///
///     public void ShowDetails() =>
///         Router.Navigate.Execute(new DetailsViewModel(this)).Subscribe();
///
///     public void GoBack() =>
///         Router.NavigateBack.Execute(Unit.Default).Subscribe();
/// }
///
/// public partial class ShellView : ReactiveUserControl<ShellViewModel>
/// {
///     public ShellView()
///     {
///         this.WhenActivated(disposables =>
///             ViewModel!.Router.CurrentViewModel
///                 .Subscribe(viewModel => contentHost.NavigateTo(viewModel))
///                 .DisposeWith(disposables));
///     }
/// }
/// ]]>
/// </code>
/// </example>
[DataContract]
[System.Diagnostics.DebuggerDisplay("NavigationStack Count = {NavigationStack.Count}")]
public class RoutingState : ReactiveObject
{
    /// <summary>The scheduler used to deliver navigation change notifications.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private readonly ISequencer _scheduler;

    /// <summary>Initializes a new instance of the <see cref="RoutingState"/> class using the default main thread scheduler.</summary>
#pragma warning disable CS8618
    public RoutingState()
        : this(null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="RoutingState"/> class.</summary>
    /// <param name="scheduler">A scheduler for where to send navigation changes to.</param>
    public RoutingState(ISequencer? scheduler)
    {
        _scheduler = scheduler ?? RxSchedulers.MainThreadScheduler;
        NavigationStack = [];
        SetupRx();
    }

    /// <summary>Gets or sets the current navigation stack, with the last element representing the active view model.</summary>
    [DataMember]
    [JsonRequired]
    public ObservableCollection<IRoutableViewModel> NavigationStack { get; set; }

    /// <summary>
    /// Gets a command which will navigate back to the previous element in the stack and emits the new current view model.
    /// The command can only execute when at least two view models exist in the stack.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ReactiveCommand<RxVoid, IRoutableViewModel> NavigateBack { get; protected set; }

    /// <summary>
    /// Gets a command that adds a new element to the navigation stack. The command argument must implement <see cref="IRoutableViewModel"/>
    /// and the command emits the same instance once scheduling completes.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> Navigate { get; protected set; }

    /// <summary>
    /// Gets a command that replaces the entire navigation stack with the supplied view model, effectively resetting navigation history.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateAndReset { get; protected set; }

    /// <summary>Gets the observable that yields the currently active view model whenever the navigation stack changes.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IRoutableViewModel> CurrentViewModel { get; protected set; }

    /// <summary>
    /// Gets an observable that signals detailed change sets for the navigation stack, enabling reactive views to
    /// animate push/pop operations.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IReactiveChangeSet<IRoutableViewModel>> NavigationChanges { get; protected set; }

    /// <summary>Sets up reactive commands and observables after deserialization.</summary>
    /// <param name="sc">The streaming context for deserialization.</param>
    [OnDeserialized]
    [RequiresUnreferencedCode("RoutingState uses ReactiveCommand which may require unreferenced code.")]
    [MemberNotNull(
        nameof(NavigationChanges),
        nameof(NavigateBack),
        nameof(Navigate),
        nameof(NavigateAndReset),
        nameof(CurrentViewModel))]
#if NET6_0_OR_GREATER
    private void SetupRx(in StreamingContext sc) => SetupRx();
#else
    private void SetupRx(StreamingContext sc) => SetupRx();
#endif

    /// <summary>Initializes reactive commands and observables for the navigation stack.</summary>
    [MemberNotNull(
        nameof(NavigationChanges),
        nameof(NavigateBack),
        nameof(Navigate),
        nameof(NavigateAndReset),
        nameof(CurrentViewModel))]
    private void SetupRx()
    {
        var navigateScheduler = _scheduler;
        NavigationChanges = NavigationStack.ToReactiveChangeSet();

        var countAsBehavior = new NavigationCountObservable(this);
        NavigateBack =
            ReactiveCommand.CreateFromObservable(
                () =>
                {
                    NavigationStack.RemoveAt(NavigationStack.Count - 1);
                    return new ScheduledValueObservable<IRoutableViewModel>(
                        NavigationStack.Count > 0 ? NavigationStack[^1] : null!,
                        navigateScheduler);
                },
                new MapSignal<int, bool>(countAsBehavior, static x => x > 1));

        Navigate = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(vm =>
        {
            if (vm is null)
            {
                throw new InvalidOperationException("Navigate must be called on an IRoutableViewModel");
            }

            NavigationStack.Add(vm);
            return new ScheduledValueObservable<IRoutableViewModel>(vm, navigateScheduler);
        });

        NavigateAndReset = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(vm =>
        {
            NavigationStack.Clear();
            return Navigate.Execute(vm);
        });

        CurrentViewModel = new MapSignal<IReactiveChangeSet<IRoutableViewModel>, IRoutableViewModel>(
            NavigationChanges,
            _ => NavigationStack.Count > 0 ? NavigationStack[NavigationStack.Count - 1] : null!);
    }

    /// <summary>Emits a single value delivered on a scheduler. Replaces <c>Observable.Return(value).ObserveOn(scheduler)</c>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="value">The value to emit.</param>
    /// <param name="scheduler">The scheduler the value is delivered on.</param>
    private sealed class ScheduledValueObservable<T>(T value, ISequencer scheduler) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new SingleValueObservable<T>(value).Subscribe(new SchedulingObserver<T>(observer, scheduler));
        }
    }

    /// <summary>
    /// Emits the navigation stack's current count on subscription and the new count after each navigation change.
    /// Replaces the prior <c>Observable.Defer(...).Concat(...CountChanged()...)</c> behavior.
    /// </summary>
    /// <param name="owner">The owning routing state.</param>
    private sealed class NavigationCountObservable(RoutingState owner) : IObservable<int>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<int> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            observer.OnNext(owner.NavigationStack.Count);
            return owner.NavigationChanges.WhenCountChanged()
                .Subscribe(new DelegateObserver<IReactiveChangeSet<IRoutableViewModel>>(_ => observer.OnNext(owner.NavigationStack.Count)));
        }
    }
}
