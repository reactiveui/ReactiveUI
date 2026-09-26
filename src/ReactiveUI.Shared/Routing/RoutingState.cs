// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
/// <see cref="Navigate"/> and back navigation via <see cref="NavigateBack"/>.
/// </para>
/// <para>
/// Every navigation notification is a plain observable of values: <see cref="CurrentViewModel"/> reports the view
/// model on top of the stack, <see cref="NavigationStackChanged"/> reports the whole stack after each change, and
/// <see cref="CanNavigateBack"/> reports whether there is a view model to go back to.
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

    /// <summary>
    /// Gets an observable of the view model on top of the navigation stack. It emits the current view model when you
    /// subscribe, then again after every change to the stack. It emits <see langword="null"/> while the stack is empty.
    /// </summary>
    /// <example>
    /// <code language="csharp">
    /// <![CDATA[
    /// using var subscription = screen.Router.CurrentViewModel
    ///     .Subscribe(viewModel => Console.WriteLine(viewModel?.UrlPathSegment ?? "(nothing)"));
    /// ]]>
    /// </code>
    /// </example>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IRoutableViewModel?> CurrentViewModel { get; protected set; }

    /// <summary>
    /// Gets an observable of the navigation stack. After every change to <see cref="NavigationStack"/> it emits a
    /// read-only snapshot of the whole stack, oldest view model first and the current view model last. It emits
    /// nothing when you subscribe; read <see cref="NavigationStack"/> for the stack as it is now.
    /// </summary>
    /// <remarks>
    /// Each snapshot is a copy. Navigating again does not change a snapshot you already hold.
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// <![CDATA[
    /// using var subscription = screen.Router.NavigationStackChanged
    ///     .Subscribe(stack => Console.WriteLine(string.Join(" > ", stack.Select(page => page.UrlPathSegment))));
    /// ]]>
    /// </code>
    /// </example>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IReadOnlyList<IRoutableViewModel>> NavigationStackChanged { get; protected set; }

    /// <summary>
    /// Gets an observable that reports whether there is a view model to go back to, which is true while the stack
    /// holds at least two view models. It emits the current answer when you subscribe, then again each time the
    /// answer changes. <see cref="NavigateBack"/> uses it to decide when it can execute.
    /// </summary>
    /// <example>
    /// <code language="csharp">
    /// <![CDATA[
    /// using var subscription = screen.Router.CanNavigateBack
    ///     .Subscribe(canGoBack => backButton.IsEnabled = canGoBack);
    /// ]]>
    /// </code>
    /// </example>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<bool> CanNavigateBack { get; protected set; }

    /// <summary>Copies a navigation stack into a snapshot that later navigation cannot change.</summary>
    /// <param name="stack">The stack to copy.</param>
    /// <returns>The snapshot, oldest view model first.</returns>
    internal static IReadOnlyList<IRoutableViewModel> Snapshot(ObservableCollection<IRoutableViewModel> stack)
    {
        if (stack.Count == 0)
        {
            return [];
        }

        var snapshot = new IRoutableViewModel[stack.Count];
        stack.CopyTo(snapshot, 0);
        return snapshot;
    }

    /// <summary>Gets the view model on top of a stack.</summary>
    /// <param name="stack">The stack to read.</param>
    /// <returns>The last view model, or <see langword="null"/> when the stack is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IRoutableViewModel? Top(IReadOnlyList<IRoutableViewModel> stack) =>
        stack.Count > 0 ? stack[stack.Count - 1] : null;

    /// <summary>Sets up reactive commands and observables after deserialization.</summary>
    /// <param name="sc">The streaming context for deserialization.</param>
    [OnDeserialized]
    [RequiresUnreferencedCode("RoutingState uses ReactiveCommand which may require unreferenced code.")]
    [MemberNotNull(
        nameof(NavigationStackChanged),
        nameof(CurrentViewModel),
        nameof(CanNavigateBack),
        nameof(NavigateBack),
        nameof(Navigate),
        nameof(NavigateAndReset))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private void SetupRx(in StreamingContext sc) => SetupRx();
#else
    private void SetupRx(StreamingContext sc) => SetupRx();
#endif

    /// <summary>Initializes reactive commands and observables for the navigation stack.</summary>
    [MemberNotNull(
        nameof(NavigationStackChanged),
        nameof(CurrentViewModel),
        nameof(CanNavigateBack),
        nameof(NavigateBack),
        nameof(Navigate),
        nameof(NavigateAndReset))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetupRx()
    {
        var navigateScheduler = _scheduler;
        NavigationStackChanged = new NavigationStackChangedObservable(this);
        CurrentViewModel = new CurrentViewModelObservable(this);
        CanNavigateBack = new CanNavigateBackObservable(this);

        NavigateBack =
            ReactiveCommand.CreateFromObservable(
                () =>
                {
                    NavigationStack.RemoveAt(NavigationStack.Count - 1);
                    return new ScheduledValueObservable<IRoutableViewModel>(
                        NavigationStack.Count > 0 ? NavigationStack[^1] : null!,
                        navigateScheduler);
                },
                CanNavigateBack);

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
            return new ReturnSignal<T>(value, Sequencer.Immediate).Subscribe(new SchedulingObserver<T>(observer, scheduler));
        }
    }

    /// <summary>
    /// Emits a snapshot of the owner's navigation stack after each change to it. The stack is read when an observer
    /// subscribes, so a stack assigned by deserialization is the one observed.
    /// </summary>
    /// <param name="owner">The owning routing state.</param>
    private sealed class NavigationStackChangedObservable(RoutingState owner) : IObservable<IReadOnlyList<IRoutableViewModel>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IReadOnlyList<IRoutableViewModel>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Subscription(owner.NavigationStack, observer);
        }

        /// <summary>Hooks the stack's collection-changed event and detaches it on dispose.</summary>
        private sealed class Subscription : IDisposable
        {
            /// <summary>The observed navigation stack.</summary>
            private readonly ObservableCollection<IRoutableViewModel> _stack;

            /// <summary>The observer receiving snapshots.</summary>
            private readonly IObserver<IReadOnlyList<IRoutableViewModel>> _observer;

            /// <summary>Initializes a new instance of the <see cref="Subscription"/> class and hooks the event.</summary>
            /// <param name="stack">The navigation stack to observe.</param>
            /// <param name="observer">The observer receiving snapshots.</param>
            public Subscription(ObservableCollection<IRoutableViewModel> stack, IObserver<IReadOnlyList<IRoutableViewModel>> observer)
            {
                _stack = stack;
                _observer = observer;
                _stack.CollectionChanged += OnCollectionChanged;
            }

            /// <inheritdoc/>
            public void Dispose() => _stack.CollectionChanged -= OnCollectionChanged;

            /// <summary>Forwards a snapshot of the stack after it changes.</summary>
            /// <param name="sender">The stack that changed.</param>
            /// <param name="e">The collection-changed event arguments.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
                _observer.OnNext(Snapshot(_stack));
        }
    }

    /// <summary>Emits the top of the owner's stack on subscribe, then the top of each <see cref="NavigationStackChanged"/> snapshot.</summary>
    /// <param name="owner">The owning routing state.</param>
    private sealed class CurrentViewModelObservable(RoutingState owner) : IObservable<IRoutableViewModel?>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IRoutableViewModel?> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            observer.OnNext(Top(owner.NavigationStack));
            return owner.NavigationStackChanged.Subscribe(new Sink(observer));
        }

        /// <summary>Maps each snapshot to the view model on top of it.</summary>
        /// <param name="downstream">The observer receiving the current view model.</param>
        private sealed class Sink(IObserver<IRoutableViewModel?> downstream) : IObserver<IReadOnlyList<IRoutableViewModel>>
        {
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnNext(IReadOnlyList<IRoutableViewModel> value) => downstream.OnNext(Top(value));

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>
    /// Emits whether the owner's stack holds at least two view models when an observer subscribes, then again each
    /// time a snapshot from <see cref="NavigationStackChanged"/> changes the answer.
    /// </summary>
    /// <param name="owner">The owning routing state.</param>
    private sealed class CanNavigateBackObservable(RoutingState owner) : IObservable<bool>
    {
        /// <summary>The fewest view models the stack must hold for there to be one to go back to.</summary>
        private const int MinimumCountToGoBack = 2;

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var canGoBack = owner.NavigationStack.Count >= MinimumCountToGoBack;
            observer.OnNext(canGoBack);
            return owner.NavigationStackChanged.Subscribe(new Sink(observer, canGoBack));
        }

        /// <summary>Forwards the answer only when it differs from the last one delivered.</summary>
        /// <param name="downstream">The observer receiving the answer.</param>
        /// <param name="last">The answer delivered on subscribe.</param>
        private sealed class Sink(IObserver<bool> downstream, bool last) : IObserver<IReadOnlyList<IRoutableViewModel>>
        {
            /// <summary>The answer most recently delivered.</summary>
            private bool _last = last;

            /// <inheritdoc/>
            public void OnNext(IReadOnlyList<IRoutableViewModel> value)
            {
                var canGoBack = value.Count >= MinimumCountToGoBack;
                if (canGoBack == _last)
                {
                    return;
                }

                _last = canGoBack;
                downstream.OnNext(canGoBack);
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted() => downstream.OnCompleted();
        }
    }
}
