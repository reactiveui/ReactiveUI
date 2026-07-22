// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests WhenAnyObservable over a DynamicData change-set property, which is why these live in the DynamicData test leaf.</summary>
public class WhenAnyObservableChangeSetTests
{
    /// <summary>Tests WhenAnyObservable with null object should update when object isnt null anymore.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyObservableWithNullObjectShouldUpdateWhenObjectIsntNullAnymore()
    {
        var fixture = new ChangeSetWhenAnyViewModel();
        _ = fixture.WhenAnyObservable(static x => x.Changes).Bind(out var output).ObserveOn(ImmediateScheduler.Instance).Subscribe();
        await Assert.That(output).IsEmpty();

        fixture.MyListOfInts = [];
        await Assert.That(output).IsEmpty();

        fixture.MyListOfInts!.Add(1);
        await Assert.That(output).Count().IsEqualTo(1);

        fixture.MyListOfInts = null;
        await Assert.That(output).Count().IsEqualTo(1);
    }

    /// <summary>A view model whose change-set property is rebuilt whenever its backing list is reassigned.</summary>
    private sealed class ChangeSetWhenAnyViewModel : ReactiveObject
    {
        /// <summary>Gets the change set produced from <see cref="MyListOfInts" />.</summary>
        public IObservable<IChangeSet<int>>? Changes
        {
            get;
            private set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets the list of integers observed by the tests.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "SST2305:Collection properties should not be settable",
            Justification = "The test reassigns this collection (null, empty, null) to drive the observed change-set through null/non-null transitions; the setter is the mechanism under test.")]
        public ObservableCollection<int>? MyListOfInts
        {
            get;
            internal set
            {
                _ = this.RaiseAndSetIfChanged(ref field, value);
                Changes = MyListOfInts?.ToObservableChangeSet();
            }
        }
    }
}
