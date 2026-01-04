// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;
using DynamicData.Binding;

namespace ReactiveUI;

/// <summary>
/// RoutingState manages the ViewModel Stack and allows ViewModels to
/// navigate to other ViewModels.
/// </summary>
/// <remarks>
/// <para>
/// Use <see cref="RoutingState"/> from an <see cref="IScreen"/> implementation to coordinate navigation in
/// multi-page applications. The stack works in a last-in-first-out fashion, enabling forward navigation via
/// <see cref="Navigate"/> and back navigation via <see cref="NavigateBack"/>. Consumers can observe
/// <see cref="CurrentViewModel"/> or <see cref="NavigationChanged"/> to drive view presentation.
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
public class RoutingState : ReactiveObject
{
    [IgnoreDataMember]
    [JsonIgnore]
    private readonly IScheduler _scheduler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutingState"/> class.
    /// </summary>
    /// <param name="scheduler">A scheduler for where to send navigation changes to.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public RoutingState(IScheduler? scheduler = null)
    {
        _scheduler = scheduler ?? RxSchedulers.MainThreadScheduler;
        NavigationStack = [];
        SetupRx();
    }

    /// <summary>
    /// Gets or sets the current navigation stack, with the last element representing the active view model.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public ObservableCollection<IRoutableViewModel> NavigationStack { get; set; }

    /// <summary>
    /// Gets or sets a command which will navigate back to the previous element in the stack and emits the new current view model.
    /// The command can only execute when at least two view models exist in the stack.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ReactiveCommand<Unit, IRoutableViewModel> NavigateBack { get; protected set; }

    /// <summary>
    /// Gets or sets a command that adds a new element to the navigation stack. The command argument must implement <see cref="IRoutableViewModel"/>
    /// and the command emits the same instance once scheduling completes.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> Navigate { get; protected set; }

    /// <summary>
    /// Gets or sets a command that replaces the entire navigation stack with the supplied view model, effectively resetting navigation history.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateAndReset { get; protected set; }

    /// <summary>
    /// Gets or sets the observable that yields the currently active view model whenever the navigation stack changes.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IRoutableViewModel> CurrentViewModel { get; protected set; }

    /// <summary>
    /// Gets or sets an observable that signals detailed change sets for the navigation stack, enabling reactive views to animate push/pop operations.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IChangeSet<IRoutableViewModel>> NavigationChanged { get; protected set; }

    /// <summary>
    /// Sets up reactive commands and observables after deserialization.
    /// </summary>
    /// <param name="sc">The streaming context for deserialization.</param>
    [OnDeserialized]
    [RequiresUnreferencedCode("RoutingState uses ReactiveCommand which may require unreferenced code.")]
    [MemberNotNull(nameof(NavigationChanged), nameof(NavigateBack), nameof(Navigate), nameof(NavigateAndReset), nameof(CurrentViewModel))]
#if NET6_0_OR_GREATER
    private void SetupRx(in StreamingContext sc) => SetupRx();
#else
    private void SetupRx(StreamingContext sc) => SetupRx();
#endif

    [MemberNotNull(nameof(NavigationChanged), nameof(NavigateBack), nameof(Navigate), nameof(NavigateAndReset), nameof(CurrentViewModel))]
    private void SetupRx()
    {
        var navigateScheduler = _scheduler;
        NavigationChanged = NavigationStack.ToObservableChangeSet();

        var countAsBehavior = Observable.Defer(() => Observable.Return(NavigationStack.Count)).Concat(NavigationChanged.CountChanged().Select(_ => NavigationStack.Count));
        NavigateBack =
            ReactiveCommand.CreateFromObservable(
                            () =>
                            {
                                NavigationStack.RemoveAt(NavigationStack.Count - 1);
                                return Observable.Return(NavigationStack.Count > 0 ? NavigationStack[NavigationStack.Count - 1] : default!).ObserveOn(navigateScheduler);
                            },
                            countAsBehavior.Select(x => x > 1));

        Navigate = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(
         vm =>
         {
             if (vm is null)
             {
                 throw new Exception("Navigate must be called on an IRoutableViewModel");
             }

             NavigationStack.Add(vm);
             return Observable.Return(vm).ObserveOn(navigateScheduler);
         });

        NavigateAndReset = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(
         vm =>
         {
             NavigationStack.Clear();
             return Navigate.Execute(vm);
         });

        CurrentViewModel = NavigationChanged.Select(_ => NavigationStack.LastOrDefault()!);
    }
}
