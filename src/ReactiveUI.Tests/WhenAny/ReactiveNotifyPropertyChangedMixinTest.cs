// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests the reactive notify property changed.
/// </summary>
[TestFixture]
public class ReactiveNotifyPropertyChangedMixinTest
{
    /// <summary>
    /// Gets or sets the dummy.
    /// </summary>
    public string? Dummy { get; set; }

    /// <summary>
    /// Verifies that any change in a deep expression list triggers the update sequence.
    /// </summary>
    [Test]
    public void AnyChangeInExpressionListTriggersUpdate()
    {
        var obj = new ObjChain1();
        var obsUpdated = false;

        obj.ObservableForProperty(x => x.Model.Model.Model.SomeOtherParam)
           .Subscribe(_ => obsUpdated = true);

        obsUpdated = false;
        obj.Model.Model.Model.SomeOtherParam = 42;
        Assert.That(
                    obsUpdated,
                    Is.True);

        obsUpdated = false;
        obj.Model.Model.Model = new HostTestFixture();
        Assert.That(
                    obsUpdated,
                    Is.True);

        obsUpdated = false;
        obj.Model.Model = new ObjChain3 { Model = new HostTestFixture { SomeOtherParam = 10 } };
        Assert.That(
                    obsUpdated,
                    Is.True);

        obsUpdated = false;
        obj.Model = new ObjChain2();
        Assert.That(
                    obsUpdated,
                    Is.True);
    }

    /// <summary>
    /// Ensures multi-property expressions are correctly rewritten and resolved.
    /// </summary>
    [Test]
    public void MultiPropertyExpressionsShouldBeProperlyResolved()
    {
        var data = new Dictionary<Expression<Func<HostTestFixture, object>>, string[]>
        {
            { x => x!.Child!.IsOnlyOneWord!.Length, ["Child", "IsOnlyOneWord", "Length"] },
            { x => x.SomeOtherParam, ["SomeOtherParam"] },
            { x => x.Child!.IsNotNullString!, ["Child", "IsNotNullString"] },
            { x => x.Child!.Changed, ["Child", "Changed"] },
        };

        var dataTypes = new Dictionary<Expression<Func<HostTestFixture, object>>, Type[]>
        {
            { x => x.Child!.IsOnlyOneWord!.Length, [typeof(TestFixture), typeof(string), typeof(int)] },
            { x => x.SomeOtherParam, [typeof(int)] },
            { x => x.Child!.IsNotNullString!, [typeof(TestFixture), typeof(string)] },
            {
                x => x.Child!.Changed, [typeof(TestFixture), typeof(IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>)
                ]
            },
        };

        var results = data.Keys.Select(x => new { input = x, output = Reflection.Rewrite(x.Body).GetExpressionChain() })
                          .ToArray();

        var resultTypes = dataTypes.Keys
                                   .Select(x => new
                                   {
                                       input = x, output = Reflection.Rewrite(x.Body).GetExpressionChain()
                                   }).ToArray();

        foreach (var x in results)
        {
            var names = x.output.Select(y =>
                                            y.GetMemberInfo()?.Name ??
                                            throw new InvalidOperationException("propertyName should not be null."));

            Assert.That(
                        names,
                        Is.EqualTo(data[x.input]));
        }

        foreach (var x in resultTypes)
        {
            var types = x.output.Select(y => y.Type);
            Assert.That(
                        types,
                        Is.EqualTo(dataTypes[x.input]));
        }
    }

    /// <summary>
    /// Verifies child change notification behavior when the host property changes.
    /// </summary>
    [Test]
    public void OFPChangingTheHostPropertyShouldFireAChildChangeNotificationOnlyIfThePreviousChildIsDifferent() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            fixture.ObservableForProperty(x => x.Child!.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.Child.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(1));

            fixture.Child.IsOnlyOneWord = "Bar";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            fixture.Child = new TestFixture { IsOnlyOneWord = "Bar" };
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));
        });

    /// <summary>
    /// Observes a named property and verifies notifications and values.
    /// </summary>
    [Test]
    public void OFPNamedPropertyTest() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new TestFixture();
            fixture.ObservableForProperty(x => x.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(1));

            fixture.IsOnlyOneWord = "Bar";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            fixture.IsOnlyOneWord = "Baz";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));

            fixture.IsOnlyOneWord = "Baz";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            changes.All(x => x.Sender == fixture),
                            Is.True);
                Assert.That(
                            changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"),
                            Is.True);
                Assert.That(
                            changes.Select(x => x.Value),
                            Is.EqualTo(["Foo", "Bar", "Baz"]));
            }
        });

    /// <summary>
    /// Observes a named property before change and verifies notifications.
    /// </summary>
    [Test]
    public void OFPNamedPropertyTestBeforeChange() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new TestFixture { IsOnlyOneWord = "Pre" };
            fixture.ObservableForProperty(
                                          x => x.IsOnlyOneWord,
                                          beforeChange: true)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            scheduler.Start();
            Assert.That(
                        changes,
                        Is.Empty);

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(1));

            fixture.IsOnlyOneWord = "Bar";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            changes.All(x => x.Sender == fixture),
                            Is.True);
                Assert.That(
                            changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"),
                            Is.True);
                Assert.That(
                            changes.Select(x => x.Value),
                            Is.EqualTo(["Pre", "Foo"]));
            }
        });

    /// <summary>
    /// Observes a named property with no initial-skip and verifies notifications.
    /// </summary>
    [Test]
    public void OFPNamedPropertyTestNoSkipInitial() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new TestFixture { IsOnlyOneWord = "Pre" };
            fixture.ObservableForProperty(
                                          x => x.IsOnlyOneWord,
                                          false,
                                          false)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(1));

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            changes.All(x => x.Sender == fixture),
                            Is.True);
                Assert.That(
                            changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"),
                            Is.True);
                Assert.That(
                            changes.Select(x => x.Value),
                            Is.EqualTo(["Pre", "Foo"]));
            }
        });

    /// <summary>
    /// Verifies that repeated values are de-duplicated.
    /// </summary>
    [Test]
    public void OFPNamedPropertyTestRepeats() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new TestFixture();
            fixture.ObservableForProperty(x => x.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(1));

            fixture.IsOnlyOneWord = "Bar";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            fixture.IsOnlyOneWord = "Bar";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            changes.All(x => x.Sender == fixture),
                            Is.True);
                Assert.That(
                            changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"),
                            Is.True);
                Assert.That(
                            changes.Select(x => x.Value),
                            Is.EqualTo(["Foo", "Bar", "Foo"]));
            }
        });

    /// <summary>
    /// Verifies re-subscription behavior when replacing the host.
    /// </summary>
    [Test]
    public void OFPReplacingTheHostShouldResubscribeTheObservable() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            fixture.ObservableForProperty(x => x.Child!.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.Child.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(1));

            fixture.Child.IsOnlyOneWord = "Bar";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            // From "Bar" to null (new TestFixture with null IsOnlyOneWord)
            fixture.Child = new TestFixture();
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));

            // Setting null again doesn't change
            fixture.Child.IsOnlyOneWord = null!;
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));

            fixture.Child.IsOnlyOneWord = "Baz";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(4));

            fixture.Child.IsOnlyOneWord = "Baz";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(4));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            changes.All(x => x.Sender == fixture),
                            Is.True);
                Assert.That(
                            changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord"),
                            Is.True);
                Assert.That(
                            changes.Select(x => x.Value),
                            Is.EqualTo(["Foo", "Bar", null, "Baz"]));
            }
        });

    /// <summary>
    /// Verifies re-subscription behavior when host becomes null and then is restored.
    /// </summary>
    [Test]
    public void OFPReplacingTheHostWithNullThenSettingItBackShouldResubscribeTheObservable() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            var fixtureProp = fixture.ObservableForProperty(x => x.Child!.IsOnlyOneWord);
            fixtureProp
                .ToObservableChangeSet(ImmediateScheduler.Instance)
                .Bind(out var changes)
                .Subscribe();

            fixtureProp.Subscribe(x => Console.WriteLine(x.Value));

            fixture.Child.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(1));

            fixture.Child.IsOnlyOneWord = "Bar";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            // Child becomes null
            fixture.Child = null!;
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            // From "Bar" to null (child restored but value is null)
            fixture.Child = new TestFixture();
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            changes.All(x => x.Sender == fixture),
                            Is.True);
                Assert.That(
                            changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord"),
                            Is.True);
                Assert.That(
                            changes.Select(x => x.Value),
                            Is.EqualTo(["Foo", "Bar", null]));
            }
        });

    /// <summary>
    /// Ensures ObservableForProperty works with non-reactive INPC objects.
    /// </summary>
    [Test]
    public void OFPShouldWorkWithINPCObjectsToo() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new NonReactiveINPCObject { InpcProperty = null! };
            fixture.ObservableForProperty(x => x.InpcProperty.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.InpcProperty = new TestFixture();
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(1));

            fixture.InpcProperty.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            fixture.InpcProperty.IsOnlyOneWord = "Bar";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));
        });

    /// <summary>
    /// Simple child property observation test.
    /// </summary>
    [Test]
    public void OFPSimpleChildPropertyTest() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            fixture.ObservableForProperty(x => x.Child!.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.Child.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(1));

            fixture.Child.IsOnlyOneWord = "Bar";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            fixture.Child.IsOnlyOneWord = "Baz";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));

            fixture.Child.IsOnlyOneWord = "Baz";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            changes.All(x => x.Sender == fixture),
                            Is.True);
                Assert.That(
                            changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord"),
                            Is.True);
                Assert.That(
                            changes.Select(x => x.Value),
                            Is.EqualTo(["Foo", "Bar", "Baz"]));
            }
        });

    /// <summary>
    /// Simple property observation test.
    /// </summary>
    [Test]
    public void OFPSimplePropertyTest() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new TestFixture();
            fixture.ObservableForProperty(x => x.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(1));

            fixture.IsOnlyOneWord = "Bar";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(2));

            fixture.IsOnlyOneWord = "Baz";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));

            fixture.IsOnlyOneWord = "Baz";
            scheduler.Start();
            Assert.That(
                        changes,
                        Has.Count.EqualTo(3));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            changes.All(x => x.Sender == fixture),
                            Is.True);
                Assert.That(
                            changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"),
                            Is.True);
                Assert.That(
                            changes.Select(x => x.Value),
                            Is.EqualTo(["Foo", "Bar", "Baz"]));
            }
        });

    /// <summary>
    /// Subscribing to WhenAny should push the current value.
    /// </summary>
    [Test]
    public void SubscriptionToWhenAnyShouldReturnCurrentValue()
    {
        var obj = new HostTestFixture();
        var observedValue = 1;
        obj.WhenAnyValue(x => x.SomeOtherParam).Subscribe(x => observedValue = x);

        obj.SomeOtherParam = 42;

        Assert.That(
                    observedValue,
                    Is.EqualTo(obj.SomeOtherParam));
    }

    /// <summary>
    /// WhenAny executes on the current synchronization context.
    /// </summary>
    [Test]
    public void WhenAnyShouldRunInContext()
    {
        var tid = Environment.CurrentManagedThreadId;

        TaskPoolScheduler.Default.With(_ =>
        {
            var whenAnyTid = 0;
            var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

            fixture.WhenAnyValue(x => x.IsNotNullString)
                   .Subscribe(__ => whenAnyTid = Environment.CurrentManagedThreadId);

            var timeout = 10;
            fixture.IsNotNullString = "Bar";
            while (--timeout > 0 && whenAnyTid == 0)
            {
                Thread.Sleep(250);
            }

            Assert.That(
                        whenAnyTid,
                        Is.EqualTo(tid));
        });
    }

    /// <summary>
    /// WhenAny works with "normal" CLR properties.
    /// </summary>
    [Test]
    public void WhenAnyShouldWorkEvenWithNormalProperties()
    {
        var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

        var output = new List<IObservedChange<TestFixture, string?>?>();
        fixture.WhenAny(
                        x => x.PocoProperty,
                        x => x).Subscribe(output.Add);

        var output2 = new List<string?>();
        fixture.WhenAnyValue(x => x.PocoProperty).Subscribe(output2.Add);

        var output3 = new List<IObservedChange<TestFixture, int?>?>();
        fixture.WhenAny(
                        x => x.NullableInt,
                        x => x).Subscribe(output3.Add);

        var output4 = new List<int?>();
        fixture.WhenAnyValue(x => x.NullableInt).Subscribe(output4.Add);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        output,
                        Has.Count.EqualTo(1));
            Assert.That(
                        output[0]!.Sender,
                        Is.EqualTo(fixture));
            Assert.That(
                        output[0]!.GetPropertyName(),
                        Is.EqualTo("PocoProperty"));
            Assert.That(
                        output[0]!.Value,
                        Is.EqualTo("Bamf"));

            Assert.That(
                        output2,
                        Has.Count.EqualTo(1));
            Assert.That(
                        output2[0],
                        Is.EqualTo("Bamf"));

            Assert.That(
                        output3,
                        Has.Count.EqualTo(1));
            Assert.That(
                        output3[0]!.Sender,
                        Is.EqualTo(fixture));
            Assert.That(
                        output3[0]!.GetPropertyName(),
                        Is.EqualTo("NullableInt"));
            Assert.That(
                        output3[0]!.Value,
                        Is.Null);

            Assert.That(
                        output4,
                        Has.Count.EqualTo(1));
            Assert.That(
                        output4[0],
                        Is.Null);
        }
    }

    /// <summary>
    /// The <c>Changed</c> stream contains valid sender and property name data.
    /// </summary>
    [Test]
    public void ChangedShouldHaveValidData()
    {
        var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

        object? sender = null;
        string? propertyName = null;

        fixture.Changed.ObserveOn(ImmediateScheduler.Instance).Subscribe(x =>
        {
            sender = x.Sender;
            propertyName = x.PropertyName;
        });

        fixture.UsesExprRaiseSet = "abc";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        sender,
                        Is.EqualTo(fixture));
            Assert.That(
                        propertyName,
                        Is.EqualTo(nameof(fixture.UsesExprRaiseSet)));
        }

        sender = null;
        propertyName = null;
        fixture.PocoProperty = "abc";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        sender,
                        Is.Null);
            Assert.That(
                        propertyName,
                        Is.Null);
        }
    }

    /// <summary>
    /// The <c>Changing</c> stream contains valid sender and property name data.
    /// </summary>
    [Test]
    public void ChangingShouldHaveValidData()
    {
        var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

        object? sender = null;
        string? propertyName = null;

        fixture.Changing.ObserveOn(ImmediateScheduler.Instance).Subscribe(x =>
        {
            sender = x.Sender;
            propertyName = x.PropertyName;
        });

        fixture.UsesExprRaiseSet = "abc";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        sender,
                        Is.EqualTo(fixture));
            Assert.That(
                        propertyName,
                        Is.EqualTo(nameof(fixture.UsesExprRaiseSet)));
        }

        sender = null;
        propertyName = null;
        fixture.PocoProperty = "abc";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        sender,
                        Is.Null);
            Assert.That(
                        propertyName,
                        Is.Null);
        }
    }

    /// <summary>
    /// Smoke test for <c>WhenAny</c> combining two properties.
    /// </summary>
    [Test]
    public void WhenAnySmokeTest() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture(), SomeOtherParam = 5 };
            fixture.Child.IsNotNullString = "Foo";

            var output1 = new List<IObservedChange<HostTestFixture, int>>();
            var output2 = new List<IObservedChange<HostTestFixture, string>>();
            fixture.WhenAny(
                            x => x.SomeOtherParam,
                            x => x.Child!.IsNotNullString,
                            (sop, nns) => new { sop, nns })
                   .Subscribe(x =>
                   {
                       output1.Add(x!.sop);
                       output2.Add(x.nns!);
                   });

            scheduler.Start();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            output1,
                            Has.Count.EqualTo(1));
                Assert.That(
                            output2,
                            Has.Count.EqualTo(1));
                Assert.That(
                            output1[0].Sender,
                            Is.EqualTo(fixture));
                Assert.That(
                            output2[0].Sender,
                            Is.EqualTo(fixture));
                Assert.That(
                            output1[0].Value,
                            Is.EqualTo(5));
                Assert.That(
                            output2[0].Value,
                            Is.EqualTo("Foo"));
            }

            fixture.SomeOtherParam = 10;
            scheduler.Start();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            output1,
                            Has.Count.EqualTo(2));
                Assert.That(
                            output2,
                            Has.Count.EqualTo(2));
                Assert.That(
                            output1[1].Sender,
                            Is.EqualTo(fixture));
                Assert.That(
                            output2[1].Sender,
                            Is.EqualTo(fixture));
                Assert.That(
                            output1[1].Value,
                            Is.EqualTo(10));
                Assert.That(
                            output2[1].Value,
                            Is.EqualTo("Foo"));
            }

            fixture.Child.IsNotNullString = "Bar";
            scheduler.Start();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            output1,
                            Has.Count.EqualTo(3));
                Assert.That(
                            output2,
                            Has.Count.EqualTo(3));
                Assert.That(
                            output1[2].Sender,
                            Is.EqualTo(fixture));
                Assert.That(
                            output2[2].Sender,
                            Is.EqualTo(fixture));
                Assert.That(
                            output1[2].Value,
                            Is.EqualTo(10));
                Assert.That(
                            output2[2].Value,
                            Is.EqualTo("Bar"));
            }
        });

    /// <summary>
    /// WhenAnyValue supports normal CLR properties.
    /// </summary>
    [Test]
    public void WhenAnyValueShouldWorkEvenWithNormalProperties()
    {
        var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

        var output1 = new List<string?>();
        var output2 = new List<int?>();
        fixture.WhenAnyValue(x => x.PocoProperty).Subscribe(output1.Add);
        fixture.WhenAnyValue(
                             x => x.IsOnlyOneWord,
                             x => x?.Length).Subscribe(output2.Add);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        output1,
                        Has.Count.EqualTo(1));
            Assert.That(
                        output1[0],
                        Is.EqualTo("Bamf"));
            Assert.That(
                        output2,
                        Has.Count.EqualTo(1));
            Assert.That(
                        output2[0],
                        Is.EqualTo(3));
        }
    }

    /// <summary>
    /// Smoke test for WhenAnyValue combining two properties with a projector.
    /// </summary>
    [Test]
    public void WhenAnyValueSmokeTest() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture(), SomeOtherParam = 5 };
            fixture.Child.IsNotNullString = "Foo";

            var output1 = new List<int>();
            var output2 = new List<string>();
            fixture.WhenAnyValue(
                                 x => x.SomeOtherParam,
                                 x => x.Child!.IsNotNullString,
                                 (sop, nns) => new { sop, nns })
                   .Subscribe(x =>
                   {
                       output1.Add(x!.sop);
                       output2.Add(x.nns!);
                   });

            scheduler.Start();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            output1,
                            Has.Count.EqualTo(1));
                Assert.That(
                            output2,
                            Has.Count.EqualTo(1));
                Assert.That(
                            output1[0],
                            Is.EqualTo(5));
                Assert.That(
                            output2[0],
                            Is.EqualTo("Foo"));
            }

            fixture.SomeOtherParam = 10;
            scheduler.Start();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            output1,
                            Has.Count.EqualTo(2));
                Assert.That(
                            output2,
                            Has.Count.EqualTo(2));
                Assert.That(
                            output1[1],
                            Is.EqualTo(10));
                Assert.That(
                            output2[1],
                            Is.EqualTo("Foo"));
            }

            fixture.Child.IsNotNullString = "Bar";
            scheduler.Start();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            output1,
                            Has.Count.EqualTo(3));
                Assert.That(
                            output2,
                            Has.Count.EqualTo(3));
                Assert.That(
                            output1[2],
                            Is.EqualTo(10));
                Assert.That(
                            output2[2],
                            Is.EqualTo("Bar"));
            }
        });

    /// <summary>
    /// Ensures intermediate objects are eligible for GC when property value changes.
    /// </summary>
    [Test]
    public void ObjectShouldBeGarbageCollectedWhenPropertyValueChanges()
    {
        static (ObjChain1, WeakReference) GetWeakReference1()
        {
            var obj = new ObjChain1();
            var weakRef = new WeakReference(obj.Model);
            obj.ObservableForProperty(x => x.Model.Model.Model.SomeOtherParam).Subscribe();
            obj.Model = new ObjChain2();
            return (obj, weakRef);
        }

        static (ObjChain1, WeakReference) GetWeakReference2()
        {
            var obj = new ObjChain1();
            var weakRef = new WeakReference(obj.Model.Model);
            obj.ObservableForProperty(x => x.Model.Model.Model.SomeOtherParam).Subscribe();
            obj.Model.Model = new ObjChain3();
            return (obj, weakRef);
        }

        static (ObjChain1, WeakReference) GetWeakReference3()
        {
            var obj = new ObjChain1();
            var weakRef = new WeakReference(obj.Model.Model.Model);
            obj.ObservableForProperty(x => x.Model.Model.Model.SomeOtherParam).Subscribe();
            obj.Model.Model.Model = new HostTestFixture();
            return (obj, weakRef);
        }

        var (obj1, weakRef1) = GetWeakReference1();
        var (obj2, weakRef2) = GetWeakReference2();
        var (obj3, weakRef3) = GetWeakReference3();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        weakRef1.IsAlive,
                        Is.False);
            Assert.That(
                        weakRef2.IsAlive,
                        Is.False);
            Assert.That(
                        weakRef3.IsAlive,
                        Is.False);
        }

        // Keep objs alive till after GC (prevent JIT optimization)
        GC.KeepAlive(obj1);
        GC.KeepAlive(obj2);
        GC.KeepAlive(obj3);
    }

    /// <summary>
    /// Throws when WhenAnyValue receives an unsupported Equal expression.
    /// </summary>
    [Test]
    public void WhenAnyValueUnsupportedExpressionType_Equal()
    {
        var fixture = new TestFixture();
        var exception =
            Assert.Throws<NotSupportedException>(() => fixture.WhenAnyValue(x => x.IsNotNullString == x.IsOnlyOneWord)
                                                              .Subscribe());

        Assert.That(
                    exception!.Message,
                    Is.EqualTo(
                               "Unsupported expression of type 'Equal' (x.IsNotNullString == x.IsOnlyOneWord). Did you meant to use expressions 'x.IsNotNullString' and 'x.IsOnlyOneWord'?"));
    }

    /// <summary>
    /// Throws when WhenAnyValue receives an unsupported Constant expression.
    /// </summary>
    [Test]
    public void WhenAnyValueUnsupportedExpressionType_Constant()
    {
        var fixture = new TestFixture();
        var exception = Assert.Throws<NotSupportedException>(() => fixture.WhenAnyValue(_ => Dummy).Subscribe());

        Assert.That(
                    exception!.Message,
                    Is.EqualTo(
                               "Unsupported expression of type 'Constant'. Did you miss the member access prefix in the expression?"));
    }

    /// <summary>
    /// Nullable pipeline works without extra decorators.
    /// </summary>
    [Test]
    public void NullableTypesTestShouldntNeedDecorators()
    {
        var fixture = new WhenAnyTestFixture();
        IEnumerable<AccountUser?>? result = null;
        fixture.WhenAnyValue(x => x.AccountService.AccountUsersNullable)
               .Where(users => users.Count > 0)
               .Select(users => users.Values.Where(x => !string.IsNullOrWhiteSpace(x?.LastName)))
               .Subscribe(dict => result = dict);

        Assert.That(
                    result!.Count(),
                    Is.EqualTo(3));
    }

    /// <summary>
    /// Nullable tuple pipeline works without extra decorators.
    /// </summary>
    [Test]
    public void NullableTypesTestShouldntNeedDecorators2()
    {
        var fixture = new WhenAnyTestFixture();
        IEnumerable<AccountUser?>? result = null;
        fixture.WhenAnyValue(
                             x => x.ProjectService.ProjectsNullable,
                             x => x.AccountService.AccountUsersNullable)
               .Where(tuple => tuple.Item1.Count > 0 && tuple.Item2?.Count > 0)
               .Select(tuple =>
               {
                   var (projects, users) = tuple;
                   return users?.Values.Where(x => !string.IsNullOrWhiteSpace(x?.LastName));
               })
               .Subscribe(dict => result = dict);

        Assert.That(
                    result!.Count(),
                    Is.EqualTo(3));
    }

    /// <summary>
    /// Non-nullable pipeline works without extra decorators.
    /// </summary>
    [Test]
    public void NonNullableTypesTestShouldntNeedDecorators()
    {
        var fixture = new WhenAnyTestFixture();
        IEnumerable<AccountUser>? result = null;
        fixture.WhenAnyValue(x => x.AccountService.AccountUsers)
               .Where(users => users.Count > 0)
               .Select(users => users.Values.Where(x => !string.IsNullOrWhiteSpace(x.LastName)))
               .Subscribe(dict => result = dict);

        Assert.That(
                    result!.Count(),
                    Is.EqualTo(3));
    }

    /// <summary>
    /// Non-nullable tuple pipeline works without extra decorators.
    /// </summary>
    [Test]
    public void NonNullableTypesTestShouldntNeedDecorators2()
    {
        var fixture = new WhenAnyTestFixture();
        IEnumerable<AccountUser>? result = null;
        fixture.WhenAnyValue(
                             x => x.ProjectService.Projects,
                             x => x.AccountService.AccountUsers)
               .Where(tuple => tuple.Item1?.Count > 0 && tuple.Item2?.Count > 0)
               .Select(tuple =>
               {
                   var (_, users) = tuple;
                   return users!.Values.Where(x => !string.IsNullOrWhiteSpace(x.LastName));
               })
               .Subscribe(dict => result = dict);

        Assert.That(
                    result!.Count(),
                    Is.EqualTo(3));
    }

    /// <summary>
    /// WhenAnyValue with one parameter returns the value.
    /// </summary>
    [Test]
    public void WhenAnyValueWith1Paramerters()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(x => x.Value1).Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("1"));
    }

    /// <summary>
    /// WhenAnyValue with one parameter reflects sequential changes (nullable target set later).
    /// </summary>
    [Test]
    public void WhenAnyValueWith1ParamertersSequentialCheck()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = string.Empty;
        fixture.Value1 = null!;
        fixture.WhenAnyValue(x => x.Value1).Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.Null);

        fixture.Value1 = "A";
        Assert.That(
                    result,
                    Is.EqualTo("A"));

        fixture.Value1 = "B";
        Assert.That(
                    result,
                    Is.EqualTo("B"));

        fixture.Value1 = null!;
        Assert.That(
                    result,
                    Is.Null);
    }

    /// <summary>
    /// WhenAnyValue with one parameter (already nullable) reflects sequential changes.
    /// </summary>
    [Test]
    public void WhenAnyValueWith1ParamertersSequentialCheckNullable()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = string.Empty;
        fixture.WhenAnyValue(x => x.Value2).Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.Null);

        fixture.Value2 = "A";
        Assert.That(
                    result,
                    Is.EqualTo("A"));

        fixture.Value2 = "B";
        Assert.That(
                    result,
                    Is.EqualTo("B"));

        fixture.Value2 = null;
        Assert.That(
                    result,
                    Is.Null);
    }

    /// <summary>
    /// WhenAnyValue with two parameters (tuple result).
    /// </summary>
    [Test]
    public void WhenAnyValueWith2ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2)
               .Select(tuple =>
               {
                   var (value1, value2) = tuple;
                   return value1 + value2;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("1"));
    }

    /// <summary>
    /// WhenAnyValue with two parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith2ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             (v1, v2) => (v1, v2))
               .Select(tuple =>
               {
                   var (value1, value2) = tuple;
                   return value1 + value2;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("1"));
    }

    /// <summary>
    /// WhenAnyValue with three parameters (tuple result).
    /// </summary>
    [Test]
    public void WhenAnyValueWith3ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3)
               .Select(tuple =>
               {
                   var (value1, value2, value3) = tuple;
                   return value1 + value2 + value3;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("13"));
    }

    /// <summary>
    /// WhenAnyValue with three parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith3ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             (v1, v2, v3) => (v1, v2, v3))
               .Select(tuple =>
               {
                   var (value1, value2, value3) = tuple;
                   return value1 + value2 + value3;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("13"));
    }

    /// <summary>
    /// WhenAnyValue with four parameters (tuple result).
    /// </summary>
    [Test]
    public void WhenAnyValueWith4ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4)
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4) = tuple;
                   return value1 + value2 + value3 + value4;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("13"));
    }

    /// <summary>
    /// WhenAnyValue with four parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith4ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             (v1, v2, v3, v4) => (v1, v2, v3, v4))
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4) = tuple;
                   return value1 + value2 + value3 + value4;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("13"));
    }

    /// <summary>
    /// WhenAnyValue with five parameters (tuple result).
    /// </summary>
    [Test]
    public void WhenAnyValueWith5ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5)
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5) = tuple;
                   return value1 + value2 + value3 + value4 + value5;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("135"));
    }

    /// <summary>
    /// WhenAnyValue with five parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith5ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5,
                             (v1, v2, v3, v4, v5) => (v1, v2, v3, v4, v5))
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5) = tuple;
                   return value1 + value2 + value3 + value4 + value5;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("135"));
    }

    /// <summary>
    /// WhenAnyValue with six parameters (tuple result).
    /// </summary>
    [Test]
    public void WhenAnyValueWith6ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5,
                             x => x.Value6)
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5, value6) = tuple;
                   return value1 + value2 + value3 + value4 + value5 + value6;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("135"));
    }

    /// <summary>
    /// WhenAnyValue with six parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith6ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5,
                             x => x.Value6,
                             (v1, v2, v3, v4, v5, v6) => (v1, v2, v3, v4, v5, v6))
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5, value6) = tuple;
                   return value1 + value2 + value3 + value4 + value5 + value6;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("135"));
    }

    /// <summary>
    /// WhenAnyValue with seven parameters (tuple result).
    /// </summary>
    [Test]
    public void WhenAnyValueWith7ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5,
                             x => x.Value6,
                             x => x.Value7)
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5, value6, value7) = tuple;
                   return value1 + value2 + value3 + value4 + value5 + value6 + value7;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("1357"));
    }

    /// <summary>
    /// WhenAnyValue with seven parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith7ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5,
                             x => x.Value6,
                             x => x.Value7,
                             (v1, v2, v3, v4, v5, v6, v7) => (v1, v2, v3, v4, v5, v6, v7))
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5, value6, value7) = tuple;
                   return value1 + value2 + value3 + value4 + value5 + value6 + value7;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("1357"));
    }

    /// <summary>
    /// WhenAnyValue with eight parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith8ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5,
                             x => x.Value6,
                             x => x.Value7,
                             x => x.Value8,
                             (v1, v2, v3, v4, v5, v6, v7, v8) => (v1, v2, v3, v4, v5, v6, v7, v8))
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5, value6, value7, value8) = tuple;
                   return value1 + value2 + value3 + value4 + value5 + value6 + value7 + value8;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("1357"));
    }

    /// <summary>
    /// WhenAnyValue with nine parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith9ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5,
                             x => x.Value6,
                             x => x.Value7,
                             x => x.Value8,
                             x => x.Value9,
                             (v1, v2, v3, v4, v5, v6, v7, v8, v9) => (v1, v2, v3, v4, v5, v6, v7, v8, v9))
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5, value6, value7, value8, value9) = tuple;
                   return value1 + value2 + value3 + value4 + value5 + value6 + value7 + value8 + value9;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("13579"));
    }

    /// <summary>
    /// WhenAnyValue with ten parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith10ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5,
                             x => x.Value6,
                             x => x.Value7,
                             x => x.Value8,
                             x => x.Value9,
                             x => x.Value10,
                             (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10))
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5, value6, value7, value8, value9, value10) = tuple;
                   return value1 + value2 + value3 + value4 + value5 + value6 + value7 + value8 + value9 + value10;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("13579"));
    }

    /// <summary>
    /// WhenAnyValue with eleven parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith11ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5,
                             x => x.Value6,
                             x => x.Value7,
                             x => x.Value8,
                             x => x.Value9,
                             x => x.Value10,
                             x => x.Value11,
                             (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) =>
                                 (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11))
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11) =
                       tuple;
                   return value1 + value2 + value3 + value4 + value5 + value6 + value7 + value8 + value9 + value10 +
                          value11;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("1357911"));
    }

    /// <summary>
    /// WhenAnyValue with twelve parameters (values projector).
    /// </summary>
    [Test]
    public void WhenAnyValueWith12ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(
                             x => x.Value1,
                             x => x.Value2,
                             x => x.Value3,
                             x => x.Value4,
                             x => x.Value5,
                             x => x.Value6,
                             x => x.Value7,
                             x => x.Value8,
                             x => x.Value9,
                             x => x.Value10,
                             x => x.Value11,
                             x => x.Value12,
                             (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) =>
                                 (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12))
               .Select(tuple =>
               {
                   var (value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11,
                       value12) = tuple;
                   return value1 + value2 + value3 + value4 + value5 + value6 + value7 + value8 + value9 + value10 +
                          value11 + value12;
               })
               .Subscribe(value => result = value);

        Assert.That(
                    result,
                    Is.EqualTo("1357911"));
    }

    /// <summary>
    /// Verifies ToProperty projections for owner and owner name.
    /// </summary>
    [Test]
    public void WhenAnyValueWithToProperty()
    {
        var fixture = new HostTestFixture();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        fixture.Owner,
                        Is.Null);
            Assert.That(
                        fixture.OwnerName,
                        Is.Null);
        }

        fixture.Owner = new() { Name = "Fred" };
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        fixture.Owner,
                        Is.Not.Null);
            Assert.That(
                        fixture.OwnerName,
                        Is.EqualTo("Fred"));
        }

        fixture.Owner!.Name = "Wilma";
        Assert.That(
                    fixture.OwnerName,
                    Is.EqualTo("Wilma"));

        fixture.Owner.Name = null;
        Assert.That(
                    fixture.OwnerName,
                    Is.Null);

        fixture.Owner.Name = "Barney";
        Assert.That(
                    fixture.OwnerName,
                    Is.EqualTo("Barney"));

        fixture.Owner.Name = "Betty";
        Assert.That(
                    fixture.OwnerName,
                    Is.EqualTo("Betty"));
    }
}
