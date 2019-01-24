﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Reactive.Testing;

using ReactiveUI.Legacy;
using ReactiveUI.Testing;

using Xunit;

namespace ReactiveUI.Tests
{
    public class ReactiveNotifyPropertyChangedMixinTest
    {
        [Fact]
        public void AnyChangeInExpressionListTriggersUpdate()
        {
            var obj = new ObjChain1();
            bool obsUpdated;

            obj.ObservableForProperty(x => x.Model.Model.Model.SomeOtherParam).Subscribe(_ => obsUpdated = true);

            obsUpdated = false;
            obj.Model.Model.Model.SomeOtherParam = 42;
            Assert.True(obsUpdated);

            obsUpdated = false;
            obj.Model.Model.Model = new HostTestFixture();
            Assert.True(obsUpdated);

            obsUpdated = false;
            obj.Model.Model = new ObjChain3
            {
                Model = new HostTestFixture
                {
                    SomeOtherParam = 10
                }
            };
            Assert.True(obsUpdated);

            obsUpdated = false;
            obj.Model = new ObjChain2();
            Assert.True(obsUpdated);
        }

        [Fact]
        public void MultiPropertyExpressionsShouldBeProperlyResolved()
        {
            var data = new Dictionary<Expression<Func<HostTestFixture, object>>, string[]>
            {
                { x => x.Child.IsOnlyOneWord.Length, new[] { "Child", "IsOnlyOneWord", "Length" } },
                { x => x.SomeOtherParam, new[] { "SomeOtherParam" } },
                { x => x.Child.IsNotNullString, new[] { "Child", "IsNotNullString" } },
                { x => x.Child.Changed, new[] { "Child", "Changed" } },
            };

            var dataTypes = new Dictionary<Expression<Func<HostTestFixture, object>>, Type[]>
            {
                { x => x.Child.IsOnlyOneWord.Length, new[] { typeof(TestFixture), typeof(string), typeof(int) } },
                { x => x.SomeOtherParam, new[] { typeof(int) } },
                { x => x.Child.IsNotNullString, new[] { typeof(TestFixture), typeof(string) } },
                { x => x.Child.Changed, new[] { typeof(TestFixture), typeof(IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>) } },
            };

            var results = data.Keys.Select(
                                           x => new
                                           {
                                               input = x,
                                               output = Reflection.Rewrite(x.Body).GetExpressionChain()
                                           }).ToArray();
            var resultTypes = dataTypes.Keys.Select(
                                                    x => new
                                                    {
                                                        input = x,
                                                        output = Reflection.Rewrite(x.Body).GetExpressionChain()
                                                    }).ToArray();

            foreach (var x in results)
            {
                data[x.input].AssertAreEqual(x.output.Select(y => y.GetMemberInfo().Name));
            }

            foreach (var x in resultTypes)
            {
                dataTypes[x.input].AssertAreEqual(x.output.Select(y => y.Type));
            }
        }

        [Fact]
        public void OFPChangingTheHostPropertyShouldFireAChildChangeNotificationOnlyIfThePreviousChildIsDifferent()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new HostTestFixture
                                         {
                                             Child = new TestFixture()
                                         };
                                         var changes = fixture.ObservableForProperty(x => x.Child.IsOnlyOneWord).CreateCollection(scheduler: ImmediateScheduler.Instance);

                                         fixture.Child.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(1, changes.Count);

                                         fixture.Child.IsOnlyOneWord = "Bar";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         fixture.Child = new TestFixture
                                         {
                                             IsOnlyOneWord = "Bar"
                                         };
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);
                                     });
        }

        [Fact]
        public void OFPNamedPropertyTest()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new TestFixture();
                                         var changes = fixture.ObservableForProperty(x => x.IsOnlyOneWord).CreateCollection(scheduler: ImmediateScheduler.Instance);

                                         fixture.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(1, changes.Count);

                                         fixture.IsOnlyOneWord = "Bar";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         fixture.IsOnlyOneWord = "Baz";
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);

                                         fixture.IsOnlyOneWord = "Baz";
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);

                                         Assert.True(changes.All(x => x.Sender == fixture));
                                         Assert.True(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"));
                                         changes.Select(x => x.Value).AssertAreEqual(new[] { "Foo", "Bar", "Baz" });
                                     });
        }

        [Fact]
        public void OFPNamedPropertyTestBeforeChange()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new TestFixture
                                         {
                                             IsOnlyOneWord = "Pre"
                                         };
                                         var changes = fixture.ObservableForProperty(x => x.IsOnlyOneWord, beforeChange: true).CreateCollection(scheduler: ImmediateScheduler.Instance);

                                         sched.Start();
                                         Assert.Equal(0, changes.Count);

                                         fixture.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(1, changes.Count);

                                         fixture.IsOnlyOneWord = "Bar";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         Assert.True(changes.All(x => x.Sender == fixture));
                                         Assert.True(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"));
                                         changes.Select(x => x.Value).AssertAreEqual(new[] { "Pre", "Foo" });
                                     });
        }

        [Fact]
        public void OFPNamedPropertyTestNoSkipInitial()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new TestFixture
                                         {
                                             IsOnlyOneWord = "Pre"
                                         };
                                         var changes = fixture.ObservableForProperty(x => x.IsOnlyOneWord, skipInitial: false).CreateCollection(scheduler: ImmediateScheduler.Instance);

                                         sched.Start();
                                         Assert.Equal(1, changes.Count);

                                         fixture.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         Assert.True(changes.All(x => x.Sender == fixture));
                                         Assert.True(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"));
                                         changes.Select(x => x.Value).AssertAreEqual(new[] { "Pre", "Foo" });
                                     });
        }

        [Fact]
        public void OFPNamedPropertyTestRepeats()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new TestFixture();
                                         var changes = fixture.ObservableForProperty(x => x.IsOnlyOneWord).CreateCollection(scheduler: ImmediateScheduler.Instance);

                                         fixture.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(1, changes.Count);

                                         fixture.IsOnlyOneWord = "Bar";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         fixture.IsOnlyOneWord = "Bar";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         fixture.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);

                                         Assert.True(changes.All(x => x.Sender == fixture));
                                         Assert.True(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"));
                                         changes.Select(x => x.Value).AssertAreEqual(new[] { "Foo", "Bar", "Foo" });
                                     });
        }

        [Fact]
        public void OFPReplacingTheHostShouldResubscribeTheObservable()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new HostTestFixture
                                         {
                                             Child = new TestFixture()
                                         };
                                         var changes = fixture.ObservableForProperty(x => x.Child.IsOnlyOneWord).CreateCollection(scheduler: ImmediateScheduler.Instance);

                                         fixture.Child.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(1, changes.Count);

                                         fixture.Child.IsOnlyOneWord = "Bar";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         // Tricky! This is a change too, because from the perspective
                                         // of the binding, we've went from "Bar" to null
                                         fixture.Child = new TestFixture();
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);

                                         // Here we've set the value but it shouldn't change
                                         fixture.Child.IsOnlyOneWord = null;
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);

                                         fixture.Child.IsOnlyOneWord = "Baz";
                                         sched.Start();
                                         Assert.Equal(4, changes.Count);

                                         fixture.Child.IsOnlyOneWord = "Baz";
                                         sched.Start();
                                         Assert.Equal(4, changes.Count);

                                         Assert.True(changes.All(x => x.Sender == fixture));
                                         Assert.True(changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord"));
                                         changes.Select(x => x.Value).AssertAreEqual(new[] { "Foo", "Bar", null, "Baz" });
                                     });
        }

        [Fact]
        public void OFPReplacingTheHostWithNullThenSettingItBackShouldResubscribeTheObservable()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new HostTestFixture
                                         {
                                             Child = new TestFixture()
                                         };
                                         var changes = fixture.ObservableForProperty(x => x.Child.IsOnlyOneWord).CreateCollection(scheduler: ImmediateScheduler.Instance);

                                         fixture.Child.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(1, changes.Count);

                                         fixture.Child.IsOnlyOneWord = "Bar";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         // Oops, now the child is Null, we may now blow up
                                         fixture.Child = null;
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         // Tricky! This is a change too, because from the perspective
                                         // of the binding, we've went from "Bar" to null
                                         fixture.Child = new TestFixture();
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);

                                         Assert.True(changes.All(x => x.Sender == fixture));
                                         Assert.True(changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord"));
                                         changes.Select(x => x.Value).AssertAreEqual(new[] { "Foo", "Bar", null });
                                     });
        }

        [Fact]
        public void OFPShouldWorkWithINPCObjectsToo()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new NonReactiveINPCObject
                                         {
                                             InpcProperty = null
                                         };

                                         var changes = fixture.ObservableForProperty(x => x.InpcProperty.IsOnlyOneWord).CreateCollection(scheduler: ImmediateScheduler.Instance);

                                         fixture.InpcProperty = new TestFixture();
                                         sched.Start();
                                         Assert.Equal(1, changes.Count);

                                         fixture.InpcProperty.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         fixture.InpcProperty.IsOnlyOneWord = "Bar";
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);
                                     });
        }

        [Fact]
        public void OFPSimpleChildPropertyTest()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new HostTestFixture
                                         {
                                             Child = new TestFixture()
                                         };
                                         var changes = fixture.ObservableForProperty(x => x.Child.IsOnlyOneWord).CreateCollection(scheduler: ImmediateScheduler.Instance);

                                         fixture.Child.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(1, changes.Count);

                                         fixture.Child.IsOnlyOneWord = "Bar";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         fixture.Child.IsOnlyOneWord = "Baz";
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);

                                         fixture.Child.IsOnlyOneWord = "Baz";
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);

                                         Assert.True(changes.All(x => x.Sender == fixture));
                                         Assert.True(changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord"));
                                         changes.Select(x => x.Value).AssertAreEqual(new[] { "Foo", "Bar", "Baz" });
                                     });
        }

        [Fact]
        public void OFPSimplePropertyTest()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new TestFixture();
                                         var changes = fixture.ObservableForProperty(x => x.IsOnlyOneWord).CreateCollection(scheduler: ImmediateScheduler.Instance);

                                         fixture.IsOnlyOneWord = "Foo";
                                         sched.Start();
                                         Assert.Equal(1, changes.Count);

                                         fixture.IsOnlyOneWord = "Bar";
                                         sched.Start();
                                         Assert.Equal(2, changes.Count);

                                         fixture.IsOnlyOneWord = "Baz";
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);

                                         fixture.IsOnlyOneWord = "Baz";
                                         sched.Start();
                                         Assert.Equal(3, changes.Count);

                                         Assert.True(changes.All(x => x.Sender == fixture));
                                         Assert.True(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"));
                                         changes.Select(x => x.Value).AssertAreEqual(new[] { "Foo", "Bar", "Baz" });
                                     });
        }

        [Fact]
        public void SubscriptionToWhenAnyShouldReturnCurrentValue()
        {
            var obj = new HostTestFixture();
            var observedValue = 1;
            obj.WhenAnyValue(x => x.SomeOtherParam).Subscribe(x => observedValue = x);

            obj.SomeOtherParam = 42;

            Assert.True(observedValue == obj.SomeOtherParam);
        }

        [Fact]
        public void WhenAnyShouldRunInContext()
        {
            var tid = Thread.CurrentThread.ManagedThreadId;

            TaskPoolScheduler.Default.With(
                                           sched =>
                                           {
                                               var whenAnyTid = 0;
                                               var fixture = new TestFixture
                                               {
                                                   IsNotNullString = "Foo",
                                                   IsOnlyOneWord = "Baz",
                                                   PocoProperty = "Bamf"
                                               };

                                               fixture.WhenAnyValue(x => x.IsNotNullString).Subscribe(x => { whenAnyTid = Thread.CurrentThread.ManagedThreadId; });

                                               var timeout = 10;
                                               fixture.IsNotNullString = "Bar";
                                               while (--timeout > 0 && whenAnyTid == 0)
                                               {
                                                   Thread.Sleep(250);
                                               }

                                               Assert.Equal(tid, whenAnyTid);
                                           });
        }

        [Fact]
        public void WhenAnyShouldWorkEvenWithNormalProperties()
        {
            var fixture = new TestFixture
            {
                IsNotNullString = "Foo",
                IsOnlyOneWord = "Baz",
                PocoProperty = "Bamf"
            };

            var output = new List<IObservedChange<TestFixture, string>>();
            fixture.WhenAny(x => x.PocoProperty, x => x).Subscribe(output.Add);
            var output2 = new List<string>();
            fixture.WhenAnyValue(x => x.PocoProperty).Subscribe(output2.Add);
            var output3 = new List<IObservedChange<TestFixture, int?>>();
            fixture.WhenAny(x => x.NullableInt, x => x).Subscribe(output3.Add);

            var output4 = new List<int?>();
            fixture.WhenAnyValue(x => x.NullableInt).Subscribe(output4.Add);

            Assert.Equal(1, output.Count);
            Assert.Equal(fixture, output[0].Sender);
            Assert.Equal("PocoProperty", output[0].GetPropertyName());
            Assert.Equal("Bamf", output[0].Value);

            Assert.Equal(1, output2.Count);
            Assert.Equal("Bamf", output2[0]);

            Assert.Equal(1, output3.Count);
            Assert.Equal(fixture, output3[0].Sender);
            Assert.Equal("NullableInt", output3[0].GetPropertyName());
            Assert.Equal(null, output3[0].Value);

            Assert.Equal(1, output4.Count);
            Assert.Equal(null, output4[0]);
        }

        [Fact]
        public void ChangedShouldHaveValidData()
        {
            var fixture = new TestFixture
            {
                IsNotNullString = "Foo",
                IsOnlyOneWord = "Baz",
                PocoProperty = "Bamf"
            };

            object sender = null;
            string propertyName = null;
            fixture.Changed.ObserveOn(ImmediateScheduler.Instance).Subscribe(
                x =>
                {
                    sender = x.Sender;
                    propertyName = x.PropertyName;
                });

            fixture.UsesExprRaiseSet = "abc";

            Assert.Equal(fixture, sender);
            Assert.Equal(nameof(fixture.UsesExprRaiseSet), propertyName);

            sender = null;
            propertyName = null;
            fixture.PocoProperty = "abc";

            Assert.Equal(null, sender);
            Assert.Equal(null, propertyName);
        }

        [Fact]
        public void ChangingShouldHaveValidData()
        {
            var fixture = new TestFixture
            {
                IsNotNullString = "Foo",
                IsOnlyOneWord = "Baz",
                PocoProperty = "Bamf"
            };

            object sender = null;
            string propertyName = null;
            fixture.Changing.ObserveOn(ImmediateScheduler.Instance).Subscribe(
                x =>
                {
                    sender = x.Sender;
                    propertyName = x.PropertyName;
                });

            fixture.UsesExprRaiseSet = "abc";

            Assert.Equal(fixture, sender);
            Assert.Equal(nameof(fixture.UsesExprRaiseSet), propertyName);

            sender = null;
            propertyName = null;
            fixture.PocoProperty = "abc";

            Assert.Equal(null, sender);
            Assert.Equal(null, propertyName);
        }

        [Fact]
        public void WhenAnySmokeTest()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new HostTestFixture
                                         {
                                             Child = new TestFixture()
                                         };
                                         fixture.SomeOtherParam = 5;
                                         fixture.Child.IsNotNullString = "Foo";

                                         var output1 = new List<IObservedChange<HostTestFixture, int>>();
                                         var output2 = new List<IObservedChange<HostTestFixture, string>>();
                                         fixture.WhenAny(
                                                         x => x.SomeOtherParam,
                                                         x => x.Child.IsNotNullString,
                                                         (sop, nns) => new
                                                         {
                                                             sop,
                                                             nns
                                                         }).Subscribe(
                                                                      x =>
                                                                      {
                                                                          output1.Add(x.sop);
                                                                          output2.Add(x.nns);
                                                                      });

                                         sched.Start();
                                         Assert.Equal(1, output1.Count);
                                         Assert.Equal(1, output2.Count);
                                         Assert.Equal(fixture, output1[0].Sender);
                                         Assert.Equal(fixture, output2[0].Sender);
                                         Assert.Equal(5, output1[0].Value);
                                         Assert.Equal("Foo", output2[0].Value);

                                         fixture.SomeOtherParam = 10;
                                         sched.Start();
                                         Assert.Equal(2, output1.Count);
                                         Assert.Equal(2, output2.Count);
                                         Assert.Equal(fixture, output1[1].Sender);
                                         Assert.Equal(fixture, output2[1].Sender);
                                         Assert.Equal(10, output1[1].Value);
                                         Assert.Equal("Foo", output2[1].Value);

                                         fixture.Child.IsNotNullString = "Bar";
                                         sched.Start();
                                         Assert.Equal(3, output1.Count);
                                         Assert.Equal(3, output2.Count);
                                         Assert.Equal(fixture, output1[2].Sender);
                                         Assert.Equal(fixture, output2[2].Sender);
                                         Assert.Equal(10, output1[2].Value);
                                         Assert.Equal("Bar", output2[2].Value);
                                     });
        }

        [Fact]
        public void WhenAnyValueShouldWorkEvenWithNormalProperties()
        {
            var fixture = new TestFixture
            {
                IsNotNullString = "Foo",
                IsOnlyOneWord = "Baz",
                PocoProperty = "Bamf"
            };

            var output1 = new List<string>();
            var output2 = new List<int>();
            fixture.WhenAnyValue(x => x.PocoProperty).Subscribe(output1.Add);
            fixture.WhenAnyValue(x => x.IsOnlyOneWord, x => x.Length).Subscribe(output2.Add);

            Assert.Equal(1, output1.Count);
            Assert.Equal("Bamf", output1[0]);
            Assert.Equal(1, output2.Count);
            Assert.Equal(3, output2[0]);
        }

        [Fact]
        public void WhenAnyValueSmokeTest()
        {
            new TestScheduler().With(
                                     sched =>
                                     {
                                         var fixture = new HostTestFixture
                                         {
                                             Child = new TestFixture()
                                         };
                                         fixture.SomeOtherParam = 5;
                                         fixture.Child.IsNotNullString = "Foo";

                                         var output1 = new List<int>();
                                         var output2 = new List<string>();
                                         fixture.WhenAnyValue(
                                                              x => x.SomeOtherParam,
                                                              x => x.Child.IsNotNullString,
                                                              (sop, nns) => new
                                                              {
                                                                  sop,
                                                                  nns
                                                              }).Subscribe(
                                                                           x =>
                                                                           {
                                                                               output1.Add(x.sop);
                                                                               output2.Add(x.nns);
                                                                           });

                                         sched.Start();
                                         Assert.Equal(1, output1.Count);
                                         Assert.Equal(1, output2.Count);
                                         Assert.Equal(5, output1[0]);
                                         Assert.Equal("Foo", output2[0]);

                                         fixture.SomeOtherParam = 10;
                                         sched.Start();
                                         Assert.Equal(2, output1.Count);
                                         Assert.Equal(2, output2.Count);
                                         Assert.Equal(10, output1[1]);
                                         Assert.Equal("Foo", output2[1]);

                                         fixture.Child.IsNotNullString = "Bar";
                                         sched.Start();
                                         Assert.Equal(3, output1.Count);
                                         Assert.Equal(3, output2.Count);
                                         Assert.Equal(10, output1[2]);
                                         Assert.Equal("Bar", output2[2]);
                                     });
        }
    }
}
