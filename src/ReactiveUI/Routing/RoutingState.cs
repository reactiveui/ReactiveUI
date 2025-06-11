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
[DataContract]
#if NET6_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
public class RoutingState : ReactiveObject
{
    [IgnoreDataMember]
    [JsonIgnore]
    private readonly IScheduler _scheduler;

    /// <summary>
    /// Initializes static members of the <see cref="RoutingState"/> class.
    /// </summary>R
    static RoutingState() => RxApp.EnsureInitialized();

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutingState"/> class.
    /// </summary>
    /// <param name="scheduler">A scheduler for where to send navigation changes to.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public RoutingState(IScheduler? scheduler = null)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _scheduler = scheduler ?? RxApp.MainThreadScheduler;
        NavigationStack = [];
        SetupRx();
    }

    /// <summary>
    /// Gets or sets the current navigation stack, the last element in the
    /// collection being the currently visible ViewModel.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public ObservableCollection<IRoutableViewModel> NavigationStack { get; set; }

    /// <summary>
    /// Gets or sets a command which will navigate back to the previous element in the stack.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ReactiveCommand<Unit, IRoutableViewModel> NavigateBack { get; protected set; }

    /// <summary>
    /// Gets or sets a command that navigates to the a new element in the stack - the Execute parameter
    /// must be a ViewModel that implements IRoutableViewModel.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> Navigate { get; protected set; }

    /// <summary>
    /// Gets or sets a command that navigates to a new element and resets the navigation stack (i.e. the
    /// new ViewModel will now be the only element in the stack) - the
    /// Execute parameter must be a ViewModel that implements
    /// IRoutableViewModel.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateAndReset { get; protected set; }

    /// <summary>
    /// Gets or sets the current view model which is to be shown for the Routing.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IRoutableViewModel> CurrentViewModel { get; protected set; }

    /// <summary>
    /// Gets or sets an observable which will signal when the Navigation changes.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public IObservable<IChangeSet<IRoutableViewModel>> NavigationChanged { get; protected set; } // TODO: Create Test

    [OnDeserialized]
#if NET6_0_OR_GREATER
    private void SetupRx(in StreamingContext sc) => SetupRx();
#else
    private void SetupRx(StreamingContext sc) => SetupRx();
#endif

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
