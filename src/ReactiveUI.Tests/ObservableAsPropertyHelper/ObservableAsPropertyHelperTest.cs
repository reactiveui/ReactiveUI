﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ObservableAsPropertyHelperTest
    {
        [Fact]
        public void OAPHShouldFireChangeNotifications()
        {
            var input = new[] { 1, 2, 3, 3, 4 }.ToObservable();
            var output = new List<int>();

            new TestScheduler().With(sched =>
            {
                var fixture = new ObservableAsPropertyHelper<int>(
                    input,
                    x => output.Add(x),
                    -5);

                sched.Start();

                Assert.Equal(input.LastAsync().Wait(), fixture.Value);

                // Note: Why doesn't the list match the above one? We're supposed
                // to suppress duplicate notifications, of course :)
                new[] { -5, 1, 2, 3, 4 }.AssertAreEqual(output);
            });
        }

        [Fact]
        public void OAPHShouldSkipFirstValueIfItMatchesTheInitialValue()
        {
            var input = new[] { 1, 2, 3 }.ToObservable();
            var output = new List<int>();

            new TestScheduler().With(sched =>
            {
                var fixture = new ObservableAsPropertyHelper<int>(
                    input,
                    x => output.Add(x),
                    1);

                sched.Start();

                Assert.Equal(input.LastAsync().Wait(), fixture.Value);

                new[] { 1, 2, 3 }.AssertAreEqual(output);
            });
        }

        [Fact]
        public void OAPHShouldProvideInitialValueImmediatelyRegardlessOfScheduler()
        {
            var output = new List<int>();

            new TestScheduler().With(sched =>
            {
                var fixture = new ObservableAsPropertyHelper<int>(
                    Observable<int>.Never,
                    x => output.Add(x),
                    32);

                Assert.Equal(32, fixture.Value);
            });
        }

        [Fact]
        public void OAPHShouldProvideLatestValue()
        {
            var sched = new TestScheduler();
            var input = new Subject<int>();

            var fixture = new ObservableAsPropertyHelper<int>(
                input,
                _ => { },
                -5,
                scheduler: sched);

            Assert.Equal(-5, fixture.Value);
            new[] { 1, 2, 3, 4 }.Run(x => input.OnNext(x));

            sched.Start();
            Assert.Equal(4, fixture.Value);

            input.OnCompleted();
            sched.Start();
            Assert.Equal(4, fixture.Value);
        }

        [Fact]
        public void OAPHShouldSubscribeImmediatelyToSource()
        {
            var isSubscribed = false;

            var observable = Observable.Create<int>(o =>
            {
                isSubscribed = true;
                o.OnNext(42);
                o.OnCompleted();

                return Disposable.Empty;
            });

            var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, 0);

            Assert.True(isSubscribed);
            Assert.Equal(42, fixture.Value);
        }

        [Fact]
        public void OAPHDeferSubscriptionParameterDefersSubscriptionToSource()
        {
            var isSubscribed = false;

            var observable = Observable.Create<int>(o =>
            {
                isSubscribed = true;
                o.OnNext(42);
                o.OnCompleted();

                return Disposable.Empty;
            });

            var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, 0, true);

            Assert.False(isSubscribed);
            Assert.Equal(42, fixture.Value);
            Assert.True(isSubscribed);
        }

        [Fact]
        public void OAPHDeferSubscriptionParameterIsSubscribedIsNotTrueInitially()
        {
            var observable = Observable.Create<int>(o =>
            {
                o.OnNext(42);
                o.OnCompleted();

                return Disposable.Empty;
            });

            var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, 0, true);

            Assert.False(fixture.IsSubscribed);
            Assert.Equal(42, fixture.Value);
            Assert.True(fixture.IsSubscribed);
        }

        [Fact]
        public void OAPHDeferSubscriptionShouldNotThrowIfDisposed()
        {
            var observable = Observable.Create<int>(o =>
            {
                o.OnNext(42);
                o.OnCompleted();

                return Disposable.Empty;
            });

            var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, 0, true);

            Assert.False(fixture.IsSubscribed);
            fixture.Dispose();
            var ex = Record.Exception(() => Assert.Equal(0, fixture.Value));
            Assert.Null(ex);
        }

        [Fact]
        public void OAPHShouldRethrowErrors()
        {
            var input = new Subject<int>();
            var sched = new TestScheduler();

            var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5, scheduler: sched);
            var errors = new List<Exception>();

            Assert.Equal(-5, fixture.Value);
            new[] { 1, 2, 3, 4 }.Run(x => input.OnNext(x));

            fixture.ThrownExceptions.Subscribe(errors.Add);

            sched.Start();

            Assert.Equal(4, fixture.Value);

            input.OnError(new Exception("Die!"));

            sched.Start();

            Assert.Equal(4, fixture.Value);
            Assert.Equal(1, errors.Count);
        }

        [Fact]
        public void NoThrownExceptionsSubscriberEqualsOAPHDeath()
        {
            new TestScheduler().With(sched =>
            {
                var input = new Subject<int>();
                var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5, scheduler: ImmediateScheduler.Instance);

                Assert.Equal(-5, fixture.Value);
                new[] { 1, 2, 3, 4 }.Run(x => input.OnNext(x));

                input.OnError(new Exception("Die!"));

                var failed = true;
                try
                {
                    sched.Start();
                }
                catch (Exception ex)
                {
                    failed = ex?.InnerException?.Message != "Die!";
                }

                Assert.False(failed);
                Assert.Equal(4, fixture.Value);
            });
        }

        [Fact]
        public void ToPropertyShouldFireBothChangingAndChanged()
        {
            var fixture = new OaphTestFixture();

            // NB: This is a hack to connect up the OAPH
            var dontcare = (fixture.FirstThreeLettersOfOneWord ?? string.Empty).Substring(0, 0);

            fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, beforeChange: true)
                .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var resultChanging).Subscribe();
            fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, beforeChange: false)
                .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var resultChanged).Subscribe();

            Assert.Empty(resultChanging);
            Assert.Empty(resultChanged);

            fixture.IsOnlyOneWord = "FooBar";
            Assert.Equal(1, resultChanging.Count);
            Assert.Equal(1, resultChanged.Count);
            Assert.Equal(string.Empty, resultChanging[0].Value);
            Assert.Equal("Foo", resultChanged[0].Value);

            fixture.IsOnlyOneWord = "Bazz";
            Assert.Equal(2, resultChanging.Count);
            Assert.Equal(2, resultChanged.Count);
            Assert.Equal("Foo", resultChanging[1].Value);
            Assert.Equal("Baz", resultChanged[1].Value);
        }

        [Fact]
        public void ToProperty_NameOf_ShouldFireBothChangingAndChanged()
        {
            var fixture = new OaphNameOfTestFixture();

            var changing = false;
            var changed = false;

            fixture.PropertyChanging += (sender, e) => changing = true;
            fixture.PropertyChanged += (sender, e) => changed = true;

            Assert.False(changing);
            Assert.False(changed);

            fixture.IsOnlyOneWord = "baz";

            Assert.True(changing);
            Assert.True(changed);
        }

        [Theory]
        [InlineData(new string[] { "FooBar", "Bazz" }, new string[] { "Foo", "Baz" }, new string[] { "Bar", "azz" })]
        public void ToProperty_NameOf_ValidValuesProduced(string[] testWords, string[] first3Letters, string[] last3Letters)
        {
            var fixture = new OaphNameOfTestFixture();

            fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, beforeChange: true).ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var firstThreeChanging).Subscribe();

            fixture.ObservableForProperty(x => x.LastThreeLettersOfOneWord, beforeChange: true).ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var lastThreeChanging).Subscribe();

            var changing = new[] { firstThreeChanging, lastThreeChanging };

            fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, beforeChange: false).ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var firstThreeChanged).Subscribe();

            fixture.ObservableForProperty(x => x.LastThreeLettersOfOneWord, beforeChange: false).ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var lastThreeChanged).Subscribe();

            var changed = new[] { firstThreeChanged, lastThreeChanged };

            Assert.True(changed.All(x => x.Count == 0));
            Assert.True(changing.All(x => x.Count == 0));

            for (var i = 0; i < testWords.Length; ++i)
            {
                fixture.IsOnlyOneWord = testWords[i];
                Assert.True(changed.All(x => x.Count == i + 1));
                Assert.True(changing.All(x => x.Count == i + 1));
                Assert.Equal(first3Letters[i], firstThreeChanged[i].Value);
                Assert.Equal(last3Letters[i], lastThreeChanged[i].Value);
                var firstChanging = i - 1 < 0 ? string.Empty : first3Letters[i - 1];
                var lastChanging = i - 1 < 0 ? string.Empty : last3Letters[i - i];
                Assert.Equal(firstChanging, firstThreeChanging[i].Value);
                Assert.Equal(lastChanging, lastThreeChanging[i].Value);
            }
        }

        [Fact]
        public void ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName()
        {
            new TestScheduler().With(sched =>
            {
                var fixture = new OAPHIndexerTestFixture();
                var propertiesChanged = new List<string>();

                fixture.PropertyChanged += (sender, args) =>
                {
                    propertiesChanged.Add(args.PropertyName);
                };

                fixture.Text = "awesome";

                Assert.Equal(new[] { "Text", "Item[]" }, propertiesChanged);
            });
        }
    }
}
