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
    public class WhenAnyObservableTests
    {
        [Fact]
        public void NullObservablesDoNotCauseExceptions()
        {
            var fixture = new TestWhenAnyObsViewModel();
            fixture.Command1 = null;

            // these are the overloads of WhenAnyObservable that perform a Merge
#pragma warning disable CS8603 // Possible null reference return.
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
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public async Task WhenAnyObservableSmokeTestCombining()
        {
            var fixture = new TestWhenAnyObsViewModel();

            var list = new List<string>();
#pragma warning disable CS8603 // Possible null reference return.
            fixture.WhenAnyObservable(x => x.Command3, x => x.Command1, (s, i) => s + " : " + i).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
#pragma warning restore CS8603 // Possible null reference return.

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

        [Fact]
        public async Task WhenAnyObservableSmokeTestMerging()
        {
            var fixture = new TestWhenAnyObsViewModel();

            var list = new List<int>();
#pragma warning disable CS8603 // Possible null reference return.
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command2).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
#pragma warning restore CS8603 // Possible null reference return.

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

        [Fact]
        public void WhenAnyObservableWithNullObjectShouldUpdateWhenObjectIsntNullAnymore()
        {
            var fixture = new TestWhenAnyObsViewModel();
#pragma warning disable CS8603 // Possible null reference return.
            fixture.WhenAnyObservable(x => x.Changes).Bind(out var output).ObserveOn(ImmediateScheduler.Instance).Subscribe();
#pragma warning restore CS8603  // Possible null reference return.

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
