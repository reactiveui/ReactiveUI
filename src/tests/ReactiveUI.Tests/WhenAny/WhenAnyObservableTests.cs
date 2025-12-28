// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI.Tests;

public class WhenAnyObservableTests
{
    /// <summary>
    /// Tests that null observables do not cause exceptions.
    /// </summary>
    [Test]
    public void NullObservablesDoNotCauseExceptions()
    {
        var fixture = new TestWhenAnyObsViewModel
        {
            Command1 = null
        };

        // these are the overloads of WhenAnyObservable that perform a Merge
        fixture.WhenAnyObservable(static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();

        // these are the overloads of WhenAnyObservable that perform a CombineLatest
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static (zero, one) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static (zero, one, two) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static (zero, one, two, three) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static (zero, one, two, three, four) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static (zero, one, two, three, four, five) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static (zero, one, two, three, four, five, six) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static (zero, one, two, three, four, five, six, seven) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static (zero, one, two, three, four, five, six, seven, eight) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static (zero, one, two, three, four, five, six, seven, eight, nine) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static (zero, one, two, three, four, five, six, seven, eight, nine, ten) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static x => x.Command1, static (zero, one, two, three, four, five, six, seven, eight, nine, ten, eleven) => Unit.Default).Subscribe();
    }

    /// <summary>
    /// Performs a smoke test on combining WhenAnyObservable.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Test]
    public async Task WhenAnyObservableSmokeTestCombining()
    {
        var fixture = new TestWhenAnyObsViewModel();

        var list = new List<string?>();
        fixture.WhenAnyObservable(static x => x.Command3, static x => x.Command1, static (s, i) => s + " : " + i).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).IsEmpty();

        await fixture.Command1!.Execute(1);
        await fixture.Command3.Execute("foo");
        await Assert.That(list).Count().IsEqualTo(1);

        await fixture.Command1.Execute(2);
        await Assert.That(list).Count().IsEqualTo(2);

        await fixture.Command3.Execute("bar");
        using (Assert.Multiple())
        {
            await Assert.That(list).Count().IsEqualTo(3);

            await Assert.That(new[] { "foo : 1", "foo : 2", "bar : 2", }.Zip(
                                                                       list,
                                                                       static (expected, actual) => new
                                                                       {
                                                                           expected,
                                                                           actual
                                                                       }).All(static x => x.expected == x.actual)).IsTrue();
        }
    }

    /// <summary>
    /// Performs a smoke test testing WhenAnyObservable merging results.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Test]
    public async Task WhenAnyObservableSmokeTestMerging()
    {
        var fixture = new TestWhenAnyObsViewModel();

        var list = new List<int>();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command2).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).IsEmpty();

        await fixture.Command1!.Execute(1);
        await Assert.That(list).Count().IsEqualTo(1);

        await fixture.Command2.Execute(2);
        await Assert.That(list).Count().IsEqualTo(2);

        await fixture.Command1.Execute(1);
        using (Assert.Multiple())
        {
            await Assert.That(list).Count().IsEqualTo(3);

            await Assert.That(new[] { 1, 2, 1, }.Zip(
                                               list,
                                               static (expected, actual) => new
                                               {
                                                   expected,
                                                   actual
                                               }).All(static x => x.expected == x.actual)).IsTrue();
        }
    }

    /// <summary>
    /// Tests WhenAnyObservable with null object should update when object isnt null anymore.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyObservableWithNullObjectShouldUpdateWhenObjectIsntNullAnymore()
    {
        var fixture = new TestWhenAnyObsViewModel();
        fixture!.WhenAnyObservable(static x => x.Changes)!.Bind(out var output).ObserveOn(ImmediateScheduler.Instance).Subscribe();
        await Assert.That(output).IsEmpty();

        fixture.MyListOfInts = [];
        await Assert.That(output).IsEmpty();

        fixture.MyListOfInts.Add(1);
        await Assert.That(output).Count().IsEqualTo(1);

        fixture.MyListOfInts = null;
        await Assert.That(output).Count().IsEqualTo(1);
    }
}
