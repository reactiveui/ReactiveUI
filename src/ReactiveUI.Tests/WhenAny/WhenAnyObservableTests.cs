// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests WhenAnyObservable.
    /// </summary>
    public class WhenAnyObservableTests
    {
        /// <summary>
        /// Tests that null observables do not cause exceptions.
        /// </summary>
        [Fact]
        public void NullObservablesDoNotCauseExceptions()
        {
            TestWhenAnyObsViewModel? fixture = new();
            fixture.Command1 = null;

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

            // these are the overloads of WhenAnyObservable that perform a CombineLatest
#pragma warning disable RCS1163 // Unused parameter.
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
#pragma warning restore RCS1163 // Unused parameter.
        }

        /// <summary>
        /// Performs a smoke test on combining WhenAnyObservable.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task WhenAnyObservableSmokeTestCombining()
        {
            TestWhenAnyObsViewModel? fixture = new();

            var list = new List<string?>();
            fixture.WhenAnyObservable(x => x.Command3, x => x.Command1, (s, i) => s + " : " + i).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
            Assert.Equal(0, list.Count);

            await fixture.Command1!.Execute(1);
            await fixture.Command3.Execute("foo");
            Assert.Equal(1, list.Count);

            await fixture.Command1.Execute(2);
            Assert.Equal(2, list.Count);

            await fixture.Command3.Execute("bar");
            Assert.Equal(3, list.Count);

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
        [Fact]
        public async Task WhenAnyObservableSmokeTestMerging()
        {
            TestWhenAnyObsViewModel fixture = new();

            var list = new List<int>();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command2).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
            Assert.Equal(0, list.Count);

            await fixture.Command1!.Execute(1);
            Assert.Equal(1, list.Count);

            await fixture.Command2.Execute(2);
            Assert.Equal(2, list.Count);

            await fixture.Command1.Execute(1);
            Assert.Equal(3, list.Count);

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
        [Fact]
        public void WhenAnyObservableWithNullObjectShouldUpdateWhenObjectIsntNullAnymore()
        {
            TestWhenAnyObsViewModel? fixture = new();
            fixture!.WhenAnyObservable(x => x.Changes)!.Bind(out var output).ObserveOn(ImmediateScheduler.Instance).Subscribe();
            Assert.Equal(0, output.Count);

            fixture.MyListOfInts = new ObservableCollectionExtended<int>();
            Assert.Equal(0, output.Count);

            fixture.MyListOfInts.Add(1);
            Assert.Equal(1, output.Count);

            fixture.MyListOfInts = null;
            Assert.Equal(1, output.Count);
        }
    }
}
