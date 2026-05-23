// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;

namespace ReactiveUI.Tests.WhenAny;

/// <summary>
///     Tests for WhenAnyObservable functionality.
///     This test class is marked as NotInParallel because WhenAnyObservable relies on
///     the service locator (Locator.Current) to find ICreatesObservableForProperty implementations.
///     When tests run in parallel, they can interfere with each other's service locator state,
///     causing intermittent failures with "Could not find a ICreatesObservableForProperty" errors.
/// </summary>
[NotInParallel]
public class WhenAnyObservableTests
{
    private static readonly string[] ExpectedCombiningResults = ["foo : 1", "foo : 2", "bar : 2"];

    /// <summary>
    ///     Tests that null observables do not cause exceptions.
    /// </summary>
    [Test]
    public void NullObservablesDoNotCauseExceptions()
    {
        var fixture = new TestWhenAnyObsViewModel { Command1 = null };

        SubscribeToNullLowArityMergeOverloads(fixture);
        SubscribeToNullHighArityMergeOverloads(fixture);
        SubscribeToNullLowArityCombineLatestOverloads(fixture);
        SubscribeToNullHighArityCombineLatestOverloads(fixture);
    }

    /// <summary>
    ///     Performs a smoke test on combining WhenAnyObservable.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Test]
    public async Task WhenAnyObservableSmokeTestCombining()
    {
        const int SecondCommandValue = 2;
        const int CountAfterSecondEmission = 2;
        const int CountAfterThirdEmission = 3;
        var fixture = new TestWhenAnyObsViewModel();

        var list = new List<string?>();
        fixture.WhenAnyObservable(static x => x.Command3, static x => x.Command1, static (s, i) => s + " : " + i).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).IsEmpty();

        await fixture.Command1!.Execute(1);
        await fixture.Command3.Execute("foo");
        await Assert.That(list).Count().IsEqualTo(1);

        await fixture.Command1.Execute(SecondCommandValue);
        await Assert.That(list).Count().IsEqualTo(CountAfterSecondEmission);

        await fixture.Command3.Execute("bar");
        using (Assert.Multiple())
        {
            await Assert.That(list).Count().IsEqualTo(CountAfterThirdEmission);

            await Assert.That(
                    ExpectedCombiningResults.Zip(
                        list,
                        static (expected, actual) => new { expected, actual }).All(static x => x.expected == x.actual)).IsTrue();
        }
    }

    /// <summary>
    ///     Performs a smoke test testing WhenAnyObservable merging results.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Test]
    public async Task WhenAnyObservableSmokeTestMerging()
    {
        const int SecondCommandValue = 2;
        const int CountAfterSecondEmission = 2;
        const int CountAfterThirdEmission = 3;
        var fixture = new TestWhenAnyObsViewModel();

        var list = new List<int>();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command2).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).IsEmpty();

        await fixture.Command1!.Execute(1);
        await Assert.That(list).Count().IsEqualTo(1);

        await fixture.Command2.Execute(SecondCommandValue);
        await Assert.That(list).Count().IsEqualTo(CountAfterSecondEmission);

        await fixture.Command1.Execute(1);
        using (Assert.Multiple())
        {
            await Assert.That(list).Count().IsEqualTo(CountAfterThirdEmission);

            await Assert.That(
                    new[] { 1, SecondCommandValue, 1 }.Zip(
                        list,
                        static (expected, actual) => new { expected, actual }).All(static x => x.expected == x.actual)).IsTrue();
        }
    }

    /// <summary>
    ///     Tests WhenAnyObservable with null object should update when object isnt null anymore.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyObservableWithNullObjectShouldUpdateWhenObjectIsntNullAnymore()
    {
        var fixture = new TestWhenAnyObsViewModel();
        fixture.WhenAnyObservable(static x => x.Changes).Bind(out var output).ObserveOn(ImmediateScheduler.Instance).Subscribe();
        await Assert.That(output).IsEmpty();

        fixture.MyListOfInts = [];
        await Assert.That(output).IsEmpty();

        fixture.MyListOfInts.Add(1);
        await Assert.That(output).Count().IsEqualTo(1);

        fixture.MyListOfInts = null;
        await Assert.That(output).Count().IsEqualTo(1);
    }

    private static void SubscribeToNullLowArityMergeOverloads(TestWhenAnyObsViewModel fixture)
    {
        // these are the lower-arity overloads of WhenAnyObservable that perform a Merge
        fixture.WhenAnyObservable(static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
    }

    private static void SubscribeToNullHighArityMergeOverloads(TestWhenAnyObsViewModel fixture)
    {
        // these are the higher-arity overloads of WhenAnyObservable that perform a Merge
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
    }

    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Selector arity mirrors the variadic production WhenAny API.")]
    private static void SubscribeToNullLowArityCombineLatestOverloads(TestWhenAnyObsViewModel fixture)
    {
        // these are the lower-arity overloads of WhenAnyObservable that perform a CombineLatest
        fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static (_, _) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _, _) => Unit.Default).Subscribe();
    }

    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Selector arity mirrors the variadic production WhenAny API.")]
    private static void SubscribeToNullHighArityCombineLatestOverloads(TestWhenAnyObsViewModel fixture)
    {
        // these are the higher-arity overloads of WhenAnyObservable that perform a CombineLatest
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _, _, _) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _, _, _, _) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _, _, _, _, _) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _, _, _, _, _, _) => Unit.Default).Subscribe();
        fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _, _, _, _, _, _, _) => Unit.Default).Subscribe();
    }
}
