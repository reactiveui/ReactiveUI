// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;

namespace ReactiveUI.Tests.WhenAny;

/// <summary>
///     A view model used to exercise the WhenAnyObservable tests.
/// </summary>
public class TestWhenAnyObsViewModel : ReactiveObject
{
    private IObservable<IChangeSet<int>>? _changes;

    private ObservableCollectionExtended<int>? _myListOfInts;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TestWhenAnyObsViewModel" /> class.
    /// </summary>
    public TestWhenAnyObsViewModel()
    {
        Command1 = ReactiveCommand.CreateFromObservable<int, int>(
            Observable.Return,
            outputScheduler: ImmediateScheduler.Instance);
        Command2 = ReactiveCommand.CreateFromObservable<int, int>(
            Observable.Return,
            outputScheduler: ImmediateScheduler.Instance);
        Command3 = ReactiveCommand.CreateFromObservable<string, string>(
            Observable.Return,
            outputScheduler: ImmediateScheduler.Instance);
    }

    /// <summary>
    ///     Gets or sets the change set produced from <see cref="MyListOfInts" />.
    /// </summary>
    public IObservable<IChangeSet<int>>? Changes
    {
        get => _changes;
        set => this.RaiseAndSetIfChanged(ref _changes, value);
    }

    /// <summary>
    ///     Gets or sets the first command.
    /// </summary>
    public ReactiveCommand<int, int>? Command1 { get; set; }

    /// <summary>
    ///     Gets or sets the second command.
    /// </summary>
    public ReactiveCommand<int, int> Command2 { get; set; }

    /// <summary>
    ///     Gets or sets the third command.
    /// </summary>
    public ReactiveCommand<string, string> Command3 { get; set; }

    /// <summary>
    ///     Gets or sets the list of integers observed by the tests.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4004:Collection properties should be readonly",
        Justification = "Setter is required; tests assign this collection.")]
    public ObservableCollectionExtended<int>? MyListOfInts
    {
        get => _myListOfInts;
        set
        {
            this.RaiseAndSetIfChanged(ref _myListOfInts, value);
            Changes = MyListOfInts?.ToObservableChangeSet();
        }
    }
}
