// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests WhenAnyObservable.
/// </summary>
[TestFixture]
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
        fixture.WhenAnyObservable(x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();

        // these are the overloads of WhenAnyObservable that perform a CombineLatest
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, (zero, one) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, (zero, one, two) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, (zero, one, two, three) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, (zero, one, two, three, four) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, (zero, one, two, three, four, five) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, (zero, one, two, three, four, five, six) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, (zero, one, two, three, four, five, six, seven) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, (zero, one, two, three, four, five, six, seven, eight) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, (zero, one, two, three, four, five, six, seven, eight, nine) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, (zero, one, two, three, four, five, six, seven, eight, nine, ten) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, (zero, one, two, three, four, five, six, seven, eight, nine, ten, eleven) => Unit.Default).Subscribe();
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
        fixture.WhenAnyObservable(x => x.Command3, x => x.Command1, (s, i) => s + " : " + i).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        Assert.That(list.Count, Is.Zero);

        await fixture.Command1!.Execute(1);
        await fixture.Command3.Execute("foo");
        Assert.That(list, Has.Count.EqualTo(1));

        await fixture.Command1.Execute(2);
        Assert.That(list, Has.Count.EqualTo(2));

        await fixture.Command3.Execute("bar");
        Assert.That(list, Has.Count.EqualTo(3));

        Assert.True(
                    new[] { "foo : 1", "foo : 2", "bar : 2", }.Zip(
                                                                   list,
                                                                   (expected, actual) => new
                                                                   {
                                                                       expected,
                                                                       actual
                                                                   }).All(x => x.expected == x.actual));
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
        fixture.WhenAnyObservable(x => x.Command1, x => x.Command2).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        Assert.That(list.Count, Is.Zero);

        await fixture.Command1!.Execute(1);
        Assert.That(list, Has.Count.EqualTo(1));

        await fixture.Command2.Execute(2);
        Assert.That(list, Has.Count.EqualTo(2));

        await fixture.Command1.Execute(1);
        Assert.That(list, Has.Count.EqualTo(3));

        Assert.True(
                    new[] { 1, 2, 1, }.Zip(
                                           list,
                                           (expected, actual) => new
                                           {
                                               expected,
                                               actual
                                           }).All(x => x.expected == x.actual));
    }

    /// <summary>
    /// Tests WhenAnyObservable with null object should update when object isnt null anymore.
    /// </summary>
    [Test]
    public void WhenAnyObservableWithNullObjectShouldUpdateWhenObjectIsntNullAnymore()
    {
        var fixture = new TestWhenAnyObsViewModel();
        fixture!.WhenAnyObservable(x => x.Changes)!.Bind(out var output).ObserveOn(ImmediateScheduler.Instance).Subscribe();
        Assert.That(output.Count, Is.Zero);

        fixture.MyListOfInts = [];
        Assert.That(output.Count, Is.Zero);

        fixture.MyListOfInts.Add(1);
        Assert.That(output, Has.Count.EqualTo(1));

        fixture.MyListOfInts = null;
        Assert.That(output, Has.Count.EqualTo(1));
    }
}
