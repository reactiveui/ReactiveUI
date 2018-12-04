﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.Serialization;

using DynamicData;
using DynamicData.Binding;

using Xunit;

namespace ReactiveUI.Tests
{
    [DataContract]
    public class TestFixture : ReactiveObject
    {
        [IgnoreDataMember]
        private string _isNotNullString;

        [IgnoreDataMember]
        private string _isOnlyOneWord;

        private string _notSerialized;

        [IgnoreDataMember]
        private int? _nullableInt;

        [IgnoreDataMember]
        private string _pocoProperty;

        [IgnoreDataMember]
        private List<string> _stackOverflowTrigger;

        [IgnoreDataMember]
        private string _usesExprRaiseSet;

        public TestFixture()
        {
            TestCollection = new ObservableCollectionExtended<int>();
        }

        [DataMember]
        public string IsNotNullString
        {
            get => _isNotNullString;
            set => this.RaiseAndSetIfChanged(ref _isNotNullString, value);
        }

        [DataMember]
        public string IsOnlyOneWord
        {
            get => _isOnlyOneWord;
            set => this.RaiseAndSetIfChanged(ref _isOnlyOneWord, value);
        }

        public string NotSerialized
        {
            get => _notSerialized;
            set => this.RaiseAndSetIfChanged(ref _notSerialized, value);
        }

        [DataMember]
        public int? NullableInt
        {
            get => _nullableInt;
            set => this.RaiseAndSetIfChanged(ref _nullableInt, value);
        }

        [DataMember]
        public string PocoProperty
        {
            get => _pocoProperty;
            set => _pocoProperty = value;
        }

        [DataMember]
        public List<string> StackOverflowTrigger
        {
            get => _stackOverflowTrigger;
            set => this.RaiseAndSetIfChanged(ref _stackOverflowTrigger, value.ToList());
        }

        [DataMember]
        public ObservableCollectionExtended<int> TestCollection { get; protected set; }

        [DataMember]
        public string UsesExprRaiseSet
        {
            get => _usesExprRaiseSet;
            set => this.RaiseAndSetIfChanged(ref _usesExprRaiseSet, value);
        }
    }

    public class OaphTestFixture : TestFixture
    {
        [IgnoreDataMember]
        private readonly ObservableAsPropertyHelper<string> _firstThreeLettersOfOneWord;

        public OaphTestFixture()
        {
            this.WhenAnyValue(x => x.IsOnlyOneWord).Select(x => x ?? string.Empty).Select(x => x.Length >= 3 ? x.Substring(0, 3) : x).ToProperty(this, x => x.FirstThreeLettersOfOneWord, out _firstThreeLettersOfOneWord);
        }

        [IgnoreDataMember]
        public string FirstThreeLettersOfOneWord => _firstThreeLettersOfOneWord.Value;
    }

    public class OaphNameOfTestFixture : TestFixture
    {
        [IgnoreDataMember]
        private readonly ObservableAsPropertyHelper<string> _firstThreeLettersOfOneWord;

        [IgnoreDataMember]
        private readonly ObservableAsPropertyHelper<string> _lastThreeLettersOfOneWord;

        public OaphNameOfTestFixture()
        {
            this.WhenAnyValue(x => x.IsOnlyOneWord).Select(x => x ?? string.Empty).Select(x => x.Length >= 3 ? x.Substring(0, 3) : x).ToProperty(this, nameof(FirstThreeLettersOfOneWord), out _firstThreeLettersOfOneWord);

            _lastThreeLettersOfOneWord = this.WhenAnyValue(x => x.IsOnlyOneWord).Select(x => x ?? string.Empty).Select(x => x.Length >= 3 ? x.Substring(x.Length - 3, 3) : x).ToProperty(this, nameof(LastThreeLettersOfOneWord));
        }

        [IgnoreDataMember]
        public string FirstThreeLettersOfOneWord => _firstThreeLettersOfOneWord.Value;

        [IgnoreDataMember]
        public string LastThreeLettersOfOneWord => _lastThreeLettersOfOneWord.Value;
    }

    public class ReactiveObjectTest
    {
        [Fact]
        public void ChangingShouldAlwaysArriveBeforeChanged()
        {
            var before_set = "Foo";
            var after_set = "Bar";

            var fixture = new TestFixture
            {
                IsOnlyOneWord = before_set
            };

            var before_fired = false;
            fixture.Changing.Subscribe(
                                       x =>
                                       {
                                           // XXX: The content of these asserts don't actually get
                                           // propagated back, it only prevents before_fired from
                                           // being set - we have to enable 1st-chance exceptions
                                           // to see the real error
                                           Assert.Equal("IsOnlyOneWord", x.PropertyName);
                                           Assert.Equal(fixture.IsOnlyOneWord, before_set);
                                           before_fired = true;
                                       });

            var after_fired = false;
            fixture.Changed.Subscribe(
                                      x =>
                                      {
                                          Assert.Equal("IsOnlyOneWord", x.PropertyName);
                                          Assert.Equal(fixture.IsOnlyOneWord, after_set);
                                          after_fired = true;
                                      });

            fixture.IsOnlyOneWord = after_set;

            Assert.True(before_fired);
            Assert.True(after_fired);
        }

        [Fact]
        public void DeferringNotificationsDontShowUpUntilUndeferred()
        {
            var fixture = new TestFixture();
            fixture.Changed.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var output).Subscribe();

            Assert.Equal(0, output.Count);
            fixture.NullableInt = 4;
            Assert.Equal(1, output.Count);

            var stopDelaying = fixture.DelayChangeNotifications();

            fixture.NullableInt = 5;
            Assert.Equal(1, output.Count);

            fixture.IsNotNullString = "Bar";
            Assert.Equal(1, output.Count);

            fixture.NullableInt = 6;
            Assert.Equal(1, output.Count);

            fixture.IsNotNullString = "Baz";
            Assert.Equal(1, output.Count);

            var stopDelayingMore = fixture.DelayChangeNotifications();

            fixture.IsNotNullString = "Bamf";
            Assert.Equal(1, output.Count);

            stopDelaying.Dispose();

            fixture.IsNotNullString = "Blargh";
            Assert.Equal(1, output.Count);

            // NB: Because we debounce queued up notifications, we should only
            // see a notification from the latest NullableInt and the latest
            // IsNotNullableString
            stopDelayingMore.Dispose();
            Assert.Equal(3, output.Count);
        }

        [Fact]
        public void ExceptionsThrownInSubscribersShouldMarshalToThrownExceptions()
        {
            var fixture = new TestFixture
            {
                IsOnlyOneWord = "Foo"
            };

            fixture.Changed.Subscribe(x => { throw new Exception("Die!"); });
            fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var exceptionList).Subscribe();

            fixture.IsOnlyOneWord = "Bar";
            Assert.Equal(1, exceptionList.Count);
        }

        [Fact]
        public void ObservableForPropertyUsingExpression()
        {
            var fixture = new TestFixture
            {
                IsNotNullString = "Foo",
                IsOnlyOneWord = "Baz"
            };
            var output = new List<IObservedChange<TestFixture, string>>();
            fixture.ObservableForProperty(x => x.IsNotNullString).Subscribe(x => { output.Add(x); });

            fixture.IsNotNullString = "Bar";
            fixture.IsNotNullString = "Baz";
            fixture.IsNotNullString = "Baz";

            fixture.IsOnlyOneWord = "Bamf";

            Assert.Equal(2, output.Count);

            Assert.Equal(fixture, output[0].Sender);
            Assert.Equal("IsNotNullString", output[0].GetPropertyName());
            Assert.Equal("Bar", output[0].Value);

            Assert.Equal(fixture, output[1].Sender);
            Assert.Equal("IsNotNullString", output[1].GetPropertyName());
            Assert.Equal("Baz", output[1].Value);
        }

        [Fact]
        public void RaiseAndSetUsingExpression()
        {
            var fixture = new TestFixture
            {
                IsNotNullString = "Foo",
                IsOnlyOneWord = "Baz"
            };
            var output = new List<string>();
            fixture.Changed.Subscribe(x => output.Add(x.PropertyName));

            fixture.UsesExprRaiseSet = "Foo";
            fixture.UsesExprRaiseSet = "Foo"; // This one shouldn't raise a change notification

            Assert.Equal("Foo", fixture.UsesExprRaiseSet);
            Assert.Equal(1, output.Count);
            Assert.Equal("UsesExprRaiseSet", output[0]);
        }

        [Fact]
        public void ReactiveObjectShouldntSerializeAnythingExtra()
        {
            var fixture = new TestFixture
            {
                IsNotNullString = "Foo",
                IsOnlyOneWord = "Baz"
            };
            string json = JSONHelper.Serialize(fixture);

            // Should look something like:
            // {"IsNotNullString":"Foo","IsOnlyOneWord":"Baz","NullableInt":null,"PocoProperty":null,"StackOverflowTrigger":null,"TestCollection":[],"UsesExprRaiseSet":null}
            Assert.True(json.Count(x => x == ',') == 6);
            Assert.True(json.Count(x => x == ':') == 7);
            Assert.True(json.Count(x => x == '"') == 18);
        }

        [Fact]
        public void ReactiveObjectSmokeTest()
        {
            var output_changing = new List<string>();
            var output = new List<string>();
            var fixture = new TestFixture();

            fixture.Changing.Subscribe(x => output_changing.Add(x.PropertyName));
            fixture.Changed.Subscribe(x => output.Add(x.PropertyName));

            fixture.IsNotNullString = "Foo Bar Baz";
            fixture.IsOnlyOneWord = "Foo";
            fixture.IsOnlyOneWord = "Bar";
            fixture.IsNotNullString = null; // Sorry.
            fixture.IsNotNullString = null;

            var results = new[] { "IsNotNullString", "IsOnlyOneWord", "IsOnlyOneWord", "IsNotNullString" };

            Assert.Equal(results.Length, output.Count);

            output.AssertAreEqual(output_changing);
            results.AssertAreEqual(output);
        }
    }
}
