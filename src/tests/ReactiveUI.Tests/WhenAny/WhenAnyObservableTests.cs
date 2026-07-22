// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
    /// <summary>The expected sequence of combined results produced by the WhenAnyObservable combining tests.</summary>
    private static readonly string[] ExpectedCombiningResults = ["foo : 1", "foo : 2", "bar : 2"];

    /// <summary>Tests that null observables do not cause exceptions.</summary>
    [Test]
    public void NullObservablesDoNotCauseExceptions()
    {
        var fixture = new TestWhenAnyObsViewModel { Command1 = null };

        SubscribeToNullLowArityMergeOverloads(fixture);
        SubscribeToNullHighArityMergeOverloads(fixture);
        SubscribeToNullLowArityCombineLatestOverloads(fixture);
        SubscribeToNullHighArityCombineLatestOverloads(fixture);
    }

    /// <summary>Performs a smoke test on combining WhenAnyObservable.</summary>
    /// <returns>A task to monitor the progress.</returns>
    [Test]
    public async Task WhenAnyObservableSmokeTestCombining()
    {
        const int SecondCommandValue = 2;
        const int CountAfterSecondEmission = 2;
        const int CountAfterThirdEmission = 3;
        var fixture = new TestWhenAnyObsViewModel();

        var list = new List<string?>();
        _ = fixture.WhenAnyObservable(static x => x.Command3, static x => x.Command1, static (s, i) => $"{s} : {i}").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
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
                        static (expected, actual) => (expected, actual)).All(static x => x.expected == x.actual)).IsTrue();
        }
    }

    /// <summary>Performs a smoke test testing WhenAnyObservable merging results.</summary>
    /// <returns>A task to monitor the progress.</returns>
    [Test]
    public async Task WhenAnyObservableSmokeTestMerging()
    {
        const int SecondCommandValue = 2;
        const int CountAfterSecondEmission = 2;
        const int CountAfterThirdEmission = 3;
        var fixture = new TestWhenAnyObsViewModel();

        var list = new List<int>();
        _ = fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command2).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
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
                        static (expected, actual) => (expected, actual)).All(static x => x.expected == x.actual)).IsTrue();
        }
    }

    /// <summary>Subscribes to the lower-arity merge overloads of WhenAnyObservable against a null observable.</summary>
    /// <param name="fixture">The view model whose observable properties are null.</param>
    private static void SubscribeToNullLowArityMergeOverloads(TestWhenAnyObsViewModel fixture)
    {
        // these are the lower-arity overloads of WhenAnyObservable that perform a Merge
        _ = fixture.WhenAnyObservable(static x => x.Command1).Subscribe();
        _ = fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1).Subscribe();
        _ = fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static x => x.Command1).Subscribe();
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
    }

    /// <summary>Subscribes to the higher-arity merge overloads of WhenAnyObservable against a null observable.</summary>
    /// <param name="fixture">The view model whose observable properties are null.</param>
    private static void SubscribeToNullHighArityMergeOverloads(TestWhenAnyObsViewModel fixture)
    {
        SubscribeToNullMergeOverloadsArity7Through9(fixture);
        SubscribeToNullMergeOverloadsArity10Through12(fixture);
    }

    /// <summary>Subscribes to the arity 7-9 merge overloads of WhenAnyObservable against a null observable.</summary>
    /// <param name="fixture">The view model whose observable properties are null.</param>
    private static void SubscribeToNullMergeOverloadsArity7Through9(TestWhenAnyObsViewModel fixture)
    {
        // these are the higher-arity overloads of WhenAnyObservable that perform a Merge
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1).Subscribe();
        _ = fixture.WhenAnyObservable(
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

    /// <summary>Subscribes to the arity 10-12 merge overloads of WhenAnyObservable against a null observable.</summary>
    /// <param name="fixture">The view model whose observable properties are null.</param>
    private static void SubscribeToNullMergeOverloadsArity10Through12(TestWhenAnyObsViewModel fixture)
    {
        _ = fixture.WhenAnyObservable(
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
        _ = fixture.WhenAnyObservable(
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
        _ = fixture.WhenAnyObservable(
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

    /// <summary>Subscribes to the lower-arity combine-latest overloads of WhenAnyObservable against a null observable.</summary>
    /// <param name="fixture">The view model whose observable properties are null.</param>
    private static void SubscribeToNullLowArityCombineLatestOverloads(TestWhenAnyObsViewModel fixture)
    {
        // these are the lower-arity overloads of WhenAnyObservable that perform a CombineLatest
        _ = fixture.WhenAnyObservable(static x => x.Command1, static x => x.Command1, static (_, _) => RxVoid.Default).Subscribe();
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _) => RxVoid.Default).Subscribe();
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _) => RxVoid.Default).Subscribe();
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _) => RxVoid.Default).Subscribe();
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _) => RxVoid.Default).Subscribe();
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _, _) => RxVoid.Default).Subscribe();
    }

    /// <summary>Subscribes to the higher-arity combine-latest overloads of WhenAnyObservable against a null observable.</summary>
    /// <param name="fixture">The view model whose observable properties are null.</param>
    private static void SubscribeToNullHighArityCombineLatestOverloads(TestWhenAnyObsViewModel fixture)
    {
        SubscribeToNullCombineLatestOverloadsArity8Through10(fixture);
        SubscribeToNullCombineLatestOverloadsArity11Through12(fixture);
    }

    /// <summary>Subscribes to the arity 8-10 combine-latest overloads of WhenAnyObservable against a null observable.</summary>
    /// <param name="fixture">The view model whose observable properties are null.</param>
    private static void SubscribeToNullCombineLatestOverloadsArity8Through10(TestWhenAnyObsViewModel fixture)
    {
        // these are the higher-arity overloads of WhenAnyObservable that perform a CombineLatest
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _, _, _) => RxVoid.Default).Subscribe();
        _ = fixture.WhenAnyObservable(
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static x => x.Command1,
            static (_, _, _, _, _, _, _, _, _) => RxVoid.Default).Subscribe();
        _ = fixture.WhenAnyObservable(
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
            static (_, _, _, _, _, _, _, _, _, _) => RxVoid.Default).Subscribe();
    }

    /// <summary>Subscribes to the arity 11-12 combine-latest overloads of WhenAnyObservable against a null observable.</summary>
    /// <param name="fixture">The view model whose observable properties are null.</param>
    private static void SubscribeToNullCombineLatestOverloadsArity11Through12(TestWhenAnyObsViewModel fixture)
    {
        _ = fixture.WhenAnyObservable(
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
            static (_, _, _, _, _, _, _, _, _, _, _) => RxVoid.Default).Subscribe();
        _ = fixture.WhenAnyObservable(
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
            static (_, _, _, _, _, _, _, _, _, _, _, _) => RxVoid.Default).Subscribe();
    }
}
