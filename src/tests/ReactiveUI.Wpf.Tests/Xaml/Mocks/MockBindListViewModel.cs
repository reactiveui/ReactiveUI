// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Splat;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>A mock list view model used by binding tests.</summary>
public sealed class MockBindListViewModel : ReactiveObject
{
    /// <summary>Initializes static members of the <see cref="MockBindListViewModel"/> class.</summary>
    static MockBindListViewModel()
    {
        AppLocator.CurrentMutable.Register(static () => new MockBindListView(), typeof(IViewFor<MockBindListViewModel>));
    }

    /// <summary>Initializes a new instance of the <see cref="MockBindListViewModel"/> class.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S3366:\"this\" should not be exposed from constructors",
        Justification = "OAPH/WhenAny initialization requires 'this'; single-threaded test fixture.")]
    public MockBindListViewModel()
    {
        ListItems = new ReadOnlyObservableCollection<MockBindListItemViewModel>(ActiveListItem);

        // ActiveItem tracks the last element; re-raise it whenever the source list changes.
        ActiveListItem.CollectionChanged += (_, _) => this.RaisePropertyChanged(nameof(ActiveItem));

        SelectItem = ReactiveCommand.Create(
            (MockBindListItemViewModel item) =>
            {
                var index = ActiveListItem.IndexOf(item);
                for (var i = ActiveListItem.Count - 1; i > index; i--)
                {
                    ActiveListItem.RemoveAt(i);
                }
            });
    }

    /// <summary>Gets the item that is currently loaded in the list; add or remove elements to modify the list.</summary>
    public ObservableCollection<MockBindListItemViewModel> ActiveListItem { get; } = [];

    /// <summary>Gets the deepest item of the currect list. (Last element of ActiveListItem).</summary>
    public MockBindListItemViewModel? ActiveItem => ActiveListItem.Count > 0 ? ActiveListItem[^1] : null;

    /// <summary>
    /// Gets the items to be represented by the selected item which is passed as a parameter.
    /// Only this item and its ancestors are kept, the rest of the items are removed.
    /// </summary>
    public ReactiveCommand<MockBindListItemViewModel, RxVoid> SelectItem { get; }

    /// <summary>Gets the list items.</summary>
    public ReadOnlyObservableCollection<MockBindListItemViewModel> ListItems { get; }
}
