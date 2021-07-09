// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;

namespace ReactiveUI.Tests
{
    public class MockBindListViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<MockBindListItemViewModel?> _activeItem;

        static MockBindListViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new MockBindListView(), typeof(IViewFor<MockBindListViewModel>));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockBindListViewModel"/> class.
        /// </summary>
        public MockBindListViewModel()
        {
            SelectItem = ReactiveCommand.Create((MockBindListItemViewModel item) =>
            {
                ActiveListItem.Edit(l =>
                {
                    var index = l.IndexOf(item);
                    for (var i = l.Count - 1; i > index; i--)
                    {
                        l.RemoveAt(i);
                    }
                });
            });

            ActiveListItem.Connect().Select(_ => ActiveListItem.Count > 0 ? ActiveListItem.Items.ElementAt(ActiveListItem.Count - 1) : null)
                .ToProperty(this, vm => vm.ActiveItem, out _activeItem);
        }

        /// <summary>
        /// Gets the item that is currently loaded in the list.
        /// Add or remove elements to modify the list.
        /// </summary>
        public ISourceList<MockBindListItemViewModel> ActiveListItem { get; } = new SourceList<MockBindListItemViewModel>();

        /// <summary>
        /// Gets the deepest item of the currect list. (Last element of ActiveListItem).
        /// </summary>
        public MockBindListItemViewModel? ActiveItem => _activeItem.Value;

        /// <summary>
        /// Gets the items to be represented by the selected item which is passed as a parameter.
        /// Only this item and its ancestors are kept, the rest of the items are removed.
        /// </summary>
        public ReactiveCommand<MockBindListItemViewModel, Unit> SelectItem { get; }
    }
}
