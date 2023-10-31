// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Runtime.Serialization;

using DynamicData;
using DynamicData.Binding;
#pragma warning disable 8618

namespace ReactiveUI;

/// <summary>
/// RoutingState manages the ViewModel Stack and allows ViewModels to
/// navigate to other ViewModels.
/// </summary>
[DataContract]
public class RoutingState : ReactiveObject
{
    [IgnoreDataMember]
    private readonly IScheduler _scheduler;

    /// <summary>
    /// Initializes static members of the <see cref="RoutingState"/> class.
    /// </summary>R
    static RoutingState() => RxApp.EnsureInitialized();

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutingState"/> class.
    /// </summary>
    /// <param name="scheduler">A scheduler for where to send navigation changes to.</param>
    public RoutingState(IScheduler? scheduler = null)
    {
        _scheduler = scheduler ?? RxApp.MainThreadScheduler;
        NavigationStack = new();
        SetupRx();
    }

    /// <summary>
    /// Gets the current navigation stack, the last element in the
    /// collection being the currently visible ViewModel.
    /// </summary>
    [DataMember]
    public ObservableCollection<IRoutableViewModel> NavigationStack { get; }

    /// <summary>
    /// Gets or sets a command which will navigate back to the previous element in the stack.
    /// </summary>
    [IgnoreDataMember]
    public ReactiveCommand<Unit, IRoutableViewModel> NavigateBack { get; protected set; }

    /// <summary>
    /// Gets or sets a command that navigates to the a new element in the stack - the Execute parameter
    /// must be a ViewModel that implements IRoutableViewModel.
    /// </summary>
    [IgnoreDataMember]
    public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> Navigate { get; protected set; }

    /// <summary>
    /// Gets or sets a command that navigates to a new element and resets the navigation stack (i.e. the
    /// new ViewModel will now be the only element in the stack) - the
    /// Execute parameter must be a ViewModel that implements
    /// IRoutableViewModel.
    /// </summary>
    [IgnoreDataMember]
    public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateAndReset { get; protected set; }

    /// <summary>
    /// Gets or sets the current view model which is to be shown for the Routing.
    /// </summary>
    [IgnoreDataMember]
    public IObservable<IRoutableViewModel> CurrentViewModel { get; protected set; }

    /// <summary>
    /// Gets or sets an observable which will signal when the Navigation changes.
    /// </summary>
    [IgnoreDataMember]
    public IObservable<IChangeSet<IRoutableViewModel>> NavigationChanged { get; protected set; } // TODO: Create Test

    [OnDeserialized]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RCS1231 // Make parameter ref read-only.
    private void SetupRx(StreamingContext sc) => SetupRx();
#pragma warning restore RCS1231 // Make parameter ref read-only.
#pragma warning restore RCS1163 // Unused parameter.

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

        CurrentViewModel = Observable.Defer(() => Observable.Return(NavigationStack.LastOrDefault()!)).Concat(NavigationChanged.Select(_ => NavigationStack.LastOrDefault()!));
    }
}
