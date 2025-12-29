// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

using TUnit.Assertions.Enums;

namespace ReactiveUI.Tests;

public class ReactiveNotifyPropertyChangedMixinTest
{
    /// <summary>
    /// Gets or sets the dummy.
    /// </summary>
    public string? Dummy { get; set; }

    /// <summary>
    /// Verifies that any change in a deep expression list triggers the update sequence.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AnyChangeInExpressionListTriggersUpdate()
    {
        var obj = new ObjChain1();
        var obsUpdated = false;

        obj.ObservableForProperty(x => x.Model.Model.Model.SomeOtherParam)
           .Subscribe(_ => obsUpdated = true);

        obsUpdated = false;
        obj.Model.Model.Model.SomeOtherParam = 42;
        await Assert.That(obsUpdated).IsTrue();

        obsUpdated = false;
        obj.Model.Model.Model = new HostTestFixture();
        await Assert.That(obsUpdated).IsTrue();

        obsUpdated = false;
        obj.Model.Model = new ObjChain3 { Model = new HostTestFixture { SomeOtherParam = 10 } };
        await Assert.That(obsUpdated).IsTrue();

        obsUpdated = false;
        obj.Model = new ObjChain2();
        await Assert.That(obsUpdated).IsTrue();
    }

    /// <summary>
    /// Ensures multi-property expressions are correctly rewritten and resolved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MultiPropertyExpressionsShouldBeProperlyResolved()
    {
        var data = new Dictionary<Expression<Func<HostTestFixture, object>>, string[]>
        {
            { static x => x!.Child!.IsOnlyOneWord!.Length, ["Child", "IsOnlyOneWord", "Length"] },
            { static x => x.SomeOtherParam, ["SomeOtherParam"] },
            { static x => x.Child!.IsNotNullString!, ["Child", "IsNotNullString"] },
            { static x => x.Child!.Changed, ["Child", "Changed"] },
        };

        var dataTypes = new Dictionary<Expression<Func<HostTestFixture, object>>, string[]>
        {
            { static x => x.Child!.IsOnlyOneWord!.Length, [typeof(TestFixture).FullName!, typeof(string).FullName!, typeof(int).FullName!] },
            { static x => x.SomeOtherParam, [typeof(int).FullName!] },
            { static x => x.Child!.IsNotNullString!, [typeof(TestFixture).FullName!, typeof(string).FullName!] },
            {
                static x => x.Child!.Changed, [typeof(TestFixture).FullName!, typeof(IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>).FullName!]
            },
        };

        var results = data.Keys.Select(static x => new { input = x, output = Reflection.Rewrite(x.Body).GetExpressionChain() })
                          .ToArray();

        var resultTypes = dataTypes.Keys
                                   .Select(static x => new
                                   {
                                       input = x,
                                       output = Reflection.Rewrite(x.Body).GetExpressionChain()
                                   }).ToArray();

        foreach (var x in results)
        {
            var names = x.output.Select(static y =>
                                            y.GetMemberInfo()?.Name ??
                                            throw new InvalidOperationException("propertyName should not be null.")).ToArray();

            await Assert.That(names).IsEquivalentTo(data[x.input], CollectionOrdering.Matching);
        }

        foreach (var x in resultTypes)
        {
            var types = x.output.Select(static y => y.Type.FullName!).ToArray();
            await Assert.That(types).IsEquivalentTo(dataTypes[x.input], CollectionOrdering.Matching);
        }
    }

    /// <summary>
    /// Verifies child change notification behavior when the host property changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OFPChangingTheHostPropertyShouldFireAChildChangeNotificationOnlyIfThePreviousChildIsDifferent() =>
        await new TestScheduler().With(static async scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            fixture.ObservableForProperty(static x => x.Child!.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.Child.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(1);

            fixture.Child.IsOnlyOneWord = "Bar";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            fixture.Child = new TestFixture { IsOnlyOneWord = "Bar" };
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);
        });

    /// <summary>
    /// Observes a named property and verifies notifications and values.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OFPNamedPropertyTest() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture();
            fixture.ObservableForProperty(x => x.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(1);

            fixture.IsOnlyOneWord = "Bar";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            fixture.IsOnlyOneWord = "Baz";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);

            fixture.IsOnlyOneWord = "Baz";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);

            using (Assert.Multiple())
            {
                await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();
                await Assert.That(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord")).IsTrue();
                await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo(["Foo", "Bar", "Baz"]);
            }
        });

    /// <summary>
    /// Observes a named property before change and verifies notifications.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OFPNamedPropertyTestBeforeChange() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture { IsOnlyOneWord = "Pre" };
            fixture.ObservableForProperty(
                                          x => x.IsOnlyOneWord,
                                          beforeChange: true)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            scheduler.Start();
            await Assert.That(changes).IsEmpty();

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(1);

            fixture.IsOnlyOneWord = "Bar";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            using (Assert.Multiple())
            {
                await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();
                await Assert.That(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord")).IsTrue();
                await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo(["Pre", "Foo"]);
            }
        });

    /// <summary>
    /// Observes a named property with no initial-skip and verifies notifications.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OFPNamedPropertyTestNoSkipInitial() =>
        await new TestScheduler().With(async scheduler =>
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
            await Assert.That(changes).Count().IsEqualTo(1);

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            using (Assert.Multiple())
            {
                await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();
                await Assert.That(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord")).IsTrue();
                await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo(["Pre", "Foo"]);
            }
        });

    /// <summary>
    /// Verifies that repeated values are de-duplicated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OFPNamedPropertyTestRepeats() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture();
            fixture.ObservableForProperty(x => x.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(1);

            fixture.IsOnlyOneWord = "Bar";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            fixture.IsOnlyOneWord = "Bar";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);

            using (Assert.Multiple())
            {
                await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();
                await Assert.That(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord")).IsTrue();
                await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo(["Foo", "Bar", "Foo"]);
            }
        });

    /// <summary>
    /// Verifies re-subscription behavior when replacing the host.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OFPReplacingTheHostShouldResubscribeTheObservable() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            fixture.ObservableForProperty(x => x.Child!.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.Child.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(1);

            fixture.Child.IsOnlyOneWord = "Bar";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            // From "Bar" to null (new TestFixture with null IsOnlyOneWord)
            fixture.Child = new TestFixture();
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);

            // Setting null again doesn't change
            fixture.Child.IsOnlyOneWord = null!;
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);

            fixture.Child.IsOnlyOneWord = "Baz";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(4);

            fixture.Child.IsOnlyOneWord = "Baz";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(4);

            using (Assert.Multiple())
            {
                await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();
                await Assert.That(changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord")).IsTrue();
                await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo(["Foo", "Bar", null, "Baz"]);
            }
        });

    /// <summary>
    /// Verifies re-subscription behavior when host becomes null and then is restored.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OFPReplacingTheHostWithNullThenSettingItBackShouldResubscribeTheObservable() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            var fixtureProp = fixture.ObservableForProperty(x => x.Child!.IsOnlyOneWord);
            fixtureProp
                .ToObservableChangeSet(ImmediateScheduler.Instance)
                .Bind(out var changes)
                .Subscribe();

            fixture.Child.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(1);

            fixture.Child.IsOnlyOneWord = "Bar";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            // Child becomes null
            fixture.Child = null!;
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            // From "Bar" to null (child restored but value is null)
            fixture.Child = new TestFixture();
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);

            using (Assert.Multiple())
            {
                await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();
                await Assert.That(changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord")).IsTrue();
                await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo(["Foo", "Bar", null]);
            }
        });

    /// <summary>
    /// Ensures ObservableForProperty works with non-reactive INPC objects.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OFPShouldWorkWithINPCObjectsToo() =>
        await new TestScheduler().With(static async scheduler =>
        {
            var fixture = new NonReactiveINPCObject { InpcProperty = null! };
            fixture.ObservableForProperty(static x => x.InpcProperty.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.InpcProperty = new TestFixture();
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(1);

            fixture.InpcProperty.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            fixture.InpcProperty.IsOnlyOneWord = "Bar";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);
        });

    /// <summary>
    /// Simple child property observation test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OFPSimpleChildPropertyTest() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            fixture.ObservableForProperty(x => x.Child!.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.Child.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(1);

            fixture.Child.IsOnlyOneWord = "Bar";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            fixture.Child.IsOnlyOneWord = "Baz";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);

            fixture.Child.IsOnlyOneWord = "Baz";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);

            using (Assert.Multiple())
            {
                await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();
                await Assert.That(changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord")).IsTrue();
                await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo(["Foo", "Bar", "Baz"]);
            }
        });

    /// <summary>
    /// Simple property observation test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OFPSimplePropertyTest() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture();
            fixture.ObservableForProperty(x => x.IsOnlyOneWord)
                   .ToObservableChangeSet(ImmediateScheduler.Instance)
                   .Bind(out var changes)
                   .Subscribe();

            fixture.IsOnlyOneWord = "Foo";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(1);

            fixture.IsOnlyOneWord = "Bar";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(2);

            fixture.IsOnlyOneWord = "Baz";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);

            fixture.IsOnlyOneWord = "Baz";
            scheduler.Start();
            await Assert.That(changes).Count().IsEqualTo(3);

            using (Assert.Multiple())
            {
                await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();
                await Assert.That(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord")).IsTrue();
                await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo(["Foo", "Bar", "Baz"]);
            }
        });

    /// <summary>
    /// Subscribing to WhenAny should push the current value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SubscriptionToWhenAnyShouldReturnCurrentValue()
    {
        var obj = new HostTestFixture();
        var observedValue = 1;
        obj.WhenAnyValue(x => x.SomeOtherParam).Subscribe(x => observedValue = x);

        obj.SomeOtherParam = 42;

        await Assert.That(observedValue).IsEqualTo(obj.SomeOtherParam);
    }

    /// <summary>
    /// WhenAny executes on the current synchronization context.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyShouldRunInContext()
    {
        var tid = Environment.CurrentManagedThreadId;

        await TaskPoolScheduler.Default.WithAsync(async _ =>
        {
            var whenAnyTid = 0;
            var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

            fixture.WhenAnyValue(x => x.IsNotNullString)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(__ => whenAnyTid = Environment.CurrentManagedThreadId);

            fixture.IsNotNullString = "Bar";

            await Assert.That(whenAnyTid).IsEqualTo(tid);
        });
    }

    /// <summary>
    /// WhenAny works with "normal" CLR properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyShouldWorkEvenWithNormalProperties()
    {
        var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

        var output = new List<IObservedChange<TestFixture, string?>?>();
        fixture.WhenAny(
                        static x => x.PocoProperty,
                        static x => x).Subscribe(output.Add);

        var output2 = new List<string?>();
        fixture.WhenAnyValue(static x => x.PocoProperty).Subscribe(output2.Add);

        var output3 = new List<IObservedChange<TestFixture, int?>?>();
        fixture.WhenAny(
                        static x => x.NullableInt,
                        static x => x).Subscribe(output3.Add);

        var output4 = new List<int?>();
        fixture.WhenAnyValue(static x => x.NullableInt).Subscribe(output4.Add);

        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(1);
            await Assert.That(output[0]!.Sender).IsEqualTo(fixture);
            await Assert.That(output[0]!.GetPropertyName()).IsEqualTo("PocoProperty");
            await Assert.That(output[0]!.Value).IsEqualTo("Bamf");

            await Assert.That(output2).Count().IsEqualTo(1);
            await Assert.That(output2[0]).IsEqualTo("Bamf");

            await Assert.That(output3).Count().IsEqualTo(1);
            await Assert.That(output3[0]!.Sender).IsEqualTo(fixture);
            await Assert.That(output3[0]!.GetPropertyName()).IsEqualTo("NullableInt");
            await Assert.That(output3[0]!.Value).IsNull();

            await Assert.That(output4).Count().IsEqualTo(1);
            await Assert.That(output4[0]).IsNull();
        }
    }

    /// <summary>
    /// The <c>Changed</c> stream contains valid sender and property name data.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangedShouldHaveValidData()
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

        using (Assert.Multiple())
        {
            await Assert.That(sender).IsEqualTo(fixture);
            await Assert.That(propertyName).IsEqualTo(nameof(fixture.UsesExprRaiseSet));
        }

        sender = null;
        propertyName = null;
        fixture.PocoProperty = "abc";

        using (Assert.Multiple())
        {
            await Assert.That(sender).IsNull();
            await Assert.That(propertyName).IsNull();
        }
    }

    /// <summary>
    /// The <c>Changing</c> stream contains valid sender and property name data.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangingShouldHaveValidData()
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

        using (Assert.Multiple())
        {
            await Assert.That(sender).IsEqualTo(fixture);
            await Assert.That(propertyName).IsEqualTo(nameof(fixture.UsesExprRaiseSet));
        }

        sender = null;
        propertyName = null;
        fixture.PocoProperty = "abc";

        using (Assert.Multiple())
        {
            await Assert.That(sender).IsNull();
            await Assert.That(propertyName).IsNull();
        }
    }

    /// <summary>
    /// Smoke test for <c>WhenAny</c> combining two properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnySmokeTest() =>
        await new TestScheduler().With(async scheduler =>
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
            using (Assert.Multiple())
            {
                await Assert.That(output1).Count().IsEqualTo(1);
                await Assert.That(output2).Count().IsEqualTo(1);
                await Assert.That(output1[0].Sender).IsEqualTo(fixture);
                await Assert.That(output2[0].Sender).IsEqualTo(fixture);
                await Assert.That(output1[0].Value).IsEqualTo(5);
                await Assert.That(output2[0].Value).IsEqualTo("Foo");
            }

            fixture.SomeOtherParam = 10;
            scheduler.Start();
            using (Assert.Multiple())
            {
                await Assert.That(output1).Count().IsEqualTo(2);
                await Assert.That(output2).Count().IsEqualTo(2);
                await Assert.That(output1[1].Sender).IsEqualTo(fixture);
                await Assert.That(output2[1].Sender).IsEqualTo(fixture);
                await Assert.That(output1[1].Value).IsEqualTo(10);
                await Assert.That(output2[1].Value).IsEqualTo("Foo");
            }

            fixture.Child.IsNotNullString = "Bar";
            scheduler.Start();
            using (Assert.Multiple())
            {
                await Assert.That(output1).Count().IsEqualTo(3);
                await Assert.That(output2).Count().IsEqualTo(3);
                await Assert.That(output1[2].Sender).IsEqualTo(fixture);
                await Assert.That(output2[2].Sender).IsEqualTo(fixture);
                await Assert.That(output1[2].Value).IsEqualTo(10);
                await Assert.That(output2[2].Value).IsEqualTo("Bar");
            }
        });

    /// <summary>
    /// WhenAnyValue supports normal CLR properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueShouldWorkEvenWithNormalProperties()
    {
        var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

        var output1 = new List<string?>();
        var output2 = new List<int?>();
        fixture.WhenAnyValue(static x => x.PocoProperty).Subscribe(output1.Add);
        fixture.WhenAnyValue(
                             static x => x.IsOnlyOneWord,
                             static x => x?.Length).Subscribe(output2.Add);

        using (Assert.Multiple())
        {
            await Assert.That(output1).Count().IsEqualTo(1);
            await Assert.That(output1[0]).IsEqualTo("Bamf");
            await Assert.That(output2).Count().IsEqualTo(1);
            await Assert.That(output2[0]).IsEqualTo(3);
        }
    }

    /// <summary>
    /// Smoke test for WhenAnyValue combining two properties with a projector.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueSmokeTest() =>
        await new TestScheduler().With(async scheduler =>
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
            using (Assert.Multiple())
            {
                await Assert.That(output1).Count().IsEqualTo(1);
                await Assert.That(output2).Count().IsEqualTo(1);
                await Assert.That(output1[0]).IsEqualTo(5);
                await Assert.That(output2[0]).IsEqualTo("Foo");
            }

            fixture.SomeOtherParam = 10;
            scheduler.Start();
            using (Assert.Multiple())
            {
                await Assert.That(output1).Count().IsEqualTo(2);
                await Assert.That(output2).Count().IsEqualTo(2);
                await Assert.That(output1[1]).IsEqualTo(10);
                await Assert.That(output2[1]).IsEqualTo("Foo");
            }

            fixture.Child.IsNotNullString = "Bar";
            scheduler.Start();
            using (Assert.Multiple())
            {
                await Assert.That(output1).Count().IsEqualTo(3);
                await Assert.That(output2).Count().IsEqualTo(3);
                await Assert.That(output1[2]).IsEqualTo(10);
                await Assert.That(output2[2]).IsEqualTo("Bar");
            }
        });

    /// <summary>
    /// Ensures intermediate objects are eligible for GC when property value changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObjectShouldBeGarbageCollectedWhenPropertyValueChanges()
    {
        static (ObjChain1, WeakReference) GetWeakReference1()
        {
            var obj = new ObjChain1();
            var weakRef = new WeakReference(obj.Model);
            obj.ObservableForProperty(static x => x.Model.Model.Model.SomeOtherParam).Subscribe();
            obj.Model = new ObjChain2();
            return (obj, weakRef);
        }

        static (ObjChain1, WeakReference) GetWeakReference2()
        {
            var obj = new ObjChain1();
            var weakRef = new WeakReference(obj.Model.Model);
            obj.ObservableForProperty(static x => x.Model.Model.Model.SomeOtherParam).Subscribe();
            obj.Model.Model = new ObjChain3();
            return (obj, weakRef);
        }

        static (ObjChain1, WeakReference) GetWeakReference3()
        {
            var obj = new ObjChain1();
            var weakRef = new WeakReference(obj.Model.Model.Model);
            obj.ObservableForProperty(static x => x.Model.Model.Model.SomeOtherParam).Subscribe();
            obj.Model.Model.Model = new HostTestFixture();
            return (obj, weakRef);
        }

        var (obj1, weakRef1) = GetWeakReference1();
        var (obj2, weakRef2) = GetWeakReference2();
        var (obj3, weakRef3) = GetWeakReference3();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        using (Assert.Multiple())
        {
            await Assert.That(weakRef1.IsAlive).IsFalse();
            await Assert.That(weakRef2.IsAlive).IsFalse();
            await Assert.That(weakRef3.IsAlive).IsFalse();
        }

        // Keep objs alive till after GC (prevent JIT optimization)
        GC.KeepAlive(obj1);
        GC.KeepAlive(obj2);
        GC.KeepAlive(obj3);
    }

    /// <summary>
    /// Throws when WhenAnyValue receives an unsupported Equal expression.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueUnsupportedExpressionType_Equal()
    {
        var fixture = new TestFixture();
        var exception =
            Assert.Throws<NotSupportedException>(() => fixture.WhenAnyValue(x => x.IsNotNullString == x.IsOnlyOneWord)
                                                              .Subscribe());

        await Assert.That(exception!.Message).IsEqualTo("Unsupported expression of type 'Equal' (x.IsNotNullString == x.IsOnlyOneWord). Did you meant to use expressions 'x.IsNotNullString' and 'x.IsOnlyOneWord'?");
    }

    /// <summary>
    /// Throws when WhenAnyValue receives an unsupported Constant expression.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueUnsupportedExpressionType_Constant()
    {
        var fixture = new TestFixture();
        var exception = Assert.Throws<NotSupportedException>(() => fixture.WhenAnyValue(_ => Dummy).Subscribe());

        await Assert.That(exception!.Message).IsEqualTo("Unsupported expression of type 'Constant'. Did you miss the member access prefix in the expression?");
    }

    /// <summary>
    /// Nullable pipeline works without extra decorators.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableTypesTestShouldntNeedDecorators()
    {
        var fixture = new WhenAnyTestFixture();
        IEnumerable<AccountUser?>? result = null;
        fixture.WhenAnyValue(x => x.AccountService.AccountUsersNullable)
               .Where(users => users.Count > 0)
               .Select(users => users.Values.Where(x => !string.IsNullOrWhiteSpace(x?.LastName)))
               .Subscribe(dict => result = dict);

        await Assert.That(result!.Count()).IsEqualTo(3);
    }

    /// <summary>
    /// Nullable tuple pipeline works without extra decorators.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableTypesTestShouldntNeedDecorators2()
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

        await Assert.That(result!.Count()).IsEqualTo(3);
    }

    /// <summary>
    /// Non-nullable pipeline works without extra decorators.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NonNullableTypesTestShouldntNeedDecorators()
    {
        var fixture = new WhenAnyTestFixture();
        IEnumerable<AccountUser>? result = null;
        fixture.WhenAnyValue(x => x.AccountService.AccountUsers)
               .Where(users => users.Count > 0)
               .Select(users => users.Values.Where(x => !string.IsNullOrWhiteSpace(x.LastName)))
               .Subscribe(dict => result = dict);

        await Assert.That(result!.Count()).IsEqualTo(3);
    }

    /// <summary>
    /// Non-nullable tuple pipeline works without extra decorators.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NonNullableTypesTestShouldntNeedDecorators2()
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

        await Assert.That(result!.Count()).IsEqualTo(3);
    }

    /// <summary>
    /// WhenAnyValue with one parameter returns the value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith1Paramerters()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = null;
        fixture.WhenAnyValue(x => x.Value1).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo("1");
    }

    /// <summary>
    /// WhenAnyValue with one parameter reflects sequential changes (nullable target set later).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith1ParamertersSequentialCheck()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = string.Empty;
        fixture.Value1 = null!;
        fixture.WhenAnyValue(x => x.Value1).Subscribe(value => result = value);

        await Assert.That(result).IsNull();

        fixture.Value1 = "A";
        await Assert.That(result).IsEqualTo("A");

        fixture.Value1 = "B";
        await Assert.That(result).IsEqualTo("B");

        fixture.Value1 = null!;
        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// WhenAnyValue with one parameter (already nullable) reflects sequential changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith1ParamertersSequentialCheckNullable()
    {
        var fixture = new WhenAnyTestFixture();
        string? result = string.Empty;
        fixture.WhenAnyValue(x => x.Value2).Subscribe(value => result = value);

        await Assert.That(result).IsNull();

        fixture.Value2 = "A";
        await Assert.That(result).IsEqualTo("A");

        fixture.Value2 = "B";
        await Assert.That(result).IsEqualTo("B");

        fixture.Value2 = null;
        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// WhenAnyValue with two parameters (tuple result).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith2ParamertersReturnsTuple()
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

        await Assert.That(result).IsEqualTo("1");
    }

    /// <summary>
    /// WhenAnyValue with two parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith2ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("1");
    }

    /// <summary>
    /// WhenAnyValue with three parameters (tuple result).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith3ParamertersReturnsTuple()
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

        await Assert.That(result).IsEqualTo("13");
    }

    /// <summary>
    /// WhenAnyValue with three parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith3ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("13");
    }

    /// <summary>
    /// WhenAnyValue with four parameters (tuple result).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith4ParamertersReturnsTuple()
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

        await Assert.That(result).IsEqualTo("13");
    }

    /// <summary>
    /// WhenAnyValue with four parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith4ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("13");
    }

    /// <summary>
    /// WhenAnyValue with five parameters (tuple result).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith5ParamertersReturnsTuple()
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

        await Assert.That(result).IsEqualTo("135");
    }

    /// <summary>
    /// WhenAnyValue with five parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith5ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("135");
    }

    /// <summary>
    /// WhenAnyValue with six parameters (tuple result).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith6ParamertersReturnsTuple()
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

        await Assert.That(result).IsEqualTo("135");
    }

    /// <summary>
    /// WhenAnyValue with six parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith6ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("135");
    }

    /// <summary>
    /// WhenAnyValue with seven parameters (tuple result).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith7ParamertersReturnsTuple()
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

        await Assert.That(result).IsEqualTo("1357");
    }

    /// <summary>
    /// WhenAnyValue with seven parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith7ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("1357");
    }

    /// <summary>
    /// WhenAnyValue with eight parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith8ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("1357");
    }

    /// <summary>
    /// WhenAnyValue with nine parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith9ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("13579");
    }

    /// <summary>
    /// WhenAnyValue with ten parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith10ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("13579");
    }

    /// <summary>
    /// WhenAnyValue with eleven parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith11ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("1357911");
    }

    /// <summary>
    /// WhenAnyValue with twelve parameters (values projector).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith12ParamertersReturnsValues()
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

        await Assert.That(result).IsEqualTo("1357911");
    }

    /// <summary>
    /// Verifies ToProperty projections for owner and owner name.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWithToProperty()
    {
        var fixture = new HostTestFixture();

        using (Assert.Multiple())
        {
            await Assert.That(fixture.Owner).IsNull();
            await Assert.That(fixture.OwnerName).IsNull();
        }

        fixture.Owner = new() { Name = "Fred" };
        using (Assert.Multiple())
        {
            await Assert.That(fixture.Owner).IsNotNull();
            await Assert.That(fixture.OwnerName).IsEqualTo("Fred");
        }

        fixture.Owner!.Name = "Wilma";
        await Assert.That(fixture.OwnerName).IsEqualTo("Wilma");

        fixture.Owner.Name = null;
        await Assert.That(fixture.OwnerName).IsNull();

        fixture.Owner.Name = "Barney";
        await Assert.That(fixture.OwnerName).IsEqualTo("Barney");

        fixture.Owner.Name = "Betty";
        await Assert.That(fixture.OwnerName).IsEqualTo("Betty");
    }

    /// <summary>
    /// Tests ObservableForProperty with selector.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_WithSelector_TransformsValues() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture { IsOnlyOneWord = "Test" };
            var results = new List<int>();

            fixture.ObservableForProperty(x => x.IsOnlyOneWord, value => value?.Length ?? 0)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(results.Add);

            fixture.IsOnlyOneWord = "Hello";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).IsEqualTo(5);

            fixture.IsOnlyOneWord = "Hi";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(2);
            await Assert.That(results[1]).IsEqualTo(2);
        });

    /// <summary>
    /// Tests ObservableForProperty with selector and beforeChange.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_WithSelectorAndBeforeChange_TransformsBeforeValues() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture { IsOnlyOneWord = "Initial" };
            var results = new List<int>();

            fixture.ObservableForProperty(x => x.IsOnlyOneWord, value => value?.Length ?? 0, beforeChange: true)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(results.Add);

            fixture.IsOnlyOneWord = "Changed";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).IsEqualTo(7); // Length of "Initial"

            fixture.IsOnlyOneWord = "New";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(2);
            await Assert.That(results[1]).IsEqualTo(7); // Length of "Changed"
        });

    /// <summary>
    /// Tests ObservableForProperty with selector throws for null selector.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_NullSelector_Throws()
    {
        var fixture = new TestFixture();

        await Assert.That(() => fixture.ObservableForProperty(x => x.IsOnlyOneWord, (Func<string?, int>)null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests SubscribeToExpressionChain basic functionality.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SubscribeToExpressionChain_BasicUsage_NotifiesOnChange() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
            var results = new List<string?>();

            fixture.SubscribeToExpressionChain<HostTestFixture, string?>(expression.Body)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(x => results.Add(x.Value));

            fixture.Child.IsOnlyOneWord = "First";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).IsEqualTo("First");

            fixture.Child.IsOnlyOneWord = "Second";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(2);
            await Assert.That(results[1]).IsEqualTo("Second");
        });

    /// <summary>
    /// Tests SubscribeToExpressionChain with beforeChange parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SubscribeToExpressionChain_WithBeforeChange_NotifiesBeforeChange() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture { IsOnlyOneWord = "Initial" } };
            Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
            var results = new List<string?>();

            fixture.SubscribeToExpressionChain<HostTestFixture, string?>(expression.Body, beforeChange: true)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(x => results.Add(x.Value));

            fixture.Child.IsOnlyOneWord = "Changed";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).IsEqualTo("Initial");
        });

    /// <summary>
    /// Tests SubscribeToExpressionChain with beforeChange and skipInitial.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SubscribeToExpressionChain_WithBeforeChangeAndSkipInitial_SkipsFirst() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture { IsOnlyOneWord = "Initial" } };
            Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
            var results = new List<string?>();

            fixture.SubscribeToExpressionChain<HostTestFixture, string?>(expression.Body, beforeChange: true, skipInitial: true)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(x => results.Add(x.Value));

            scheduler.Start();
            await Assert.That(results).IsEmpty();

            fixture.Child.IsOnlyOneWord = "Changed";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).IsEqualTo("Initial");
        });

    /// <summary>
    /// Tests SubscribeToExpressionChain with suppressWarnings parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SubscribeToExpressionChain_WithSuppressWarnings_DoesNotWarn() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
            var results = new List<string?>();

            fixture.SubscribeToExpressionChain<HostTestFixture, string?>(
                       expression.Body,
                       beforeChange: false,
                       skipInitial: true,
                       suppressWarnings: true)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(x => results.Add(x.Value));

            fixture.Child.IsOnlyOneWord = "Test";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).IsEqualTo("Test");
        });

    /// <summary>
    /// Tests SubscribeToExpressionChain with isDistinct parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SubscribeToExpressionChain_WithIsDistinct_Works() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new HostTestFixture { Child = new TestFixture() };
            Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
            var results = new List<string?>();

            fixture.SubscribeToExpressionChain<HostTestFixture, string?>(
                       expression.Body,
                       beforeChange: false,
                       skipInitial: true,
                       suppressWarnings: false,
                       isDistinct: true)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(x => results.Add(x.Value));

            fixture.Child.IsOnlyOneWord = "Value1";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);

            fixture.Child.IsOnlyOneWord = "Value2";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(2);
        });

    /// <summary>
    /// Tests ObservableForProperty string overload with property name.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_StringPropertyName_ObservesProperty() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture();
            var results = new List<string?>();

            fixture.ObservableForProperty<TestFixture, string?>(nameof(TestFixture.IsOnlyOneWord))
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(x => results.Add(x.Value));

            fixture.IsOnlyOneWord = "Value1";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).IsEqualTo("Value1");

            fixture.IsOnlyOneWord = "Value2";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(2);
            await Assert.That(results[1]).IsEqualTo("Value2");
        });

    /// <summary>
    /// Tests ObservableForProperty string overload with beforeChange.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_StringPropertyNameBeforeChange_ObservesBeforeChange() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture { IsOnlyOneWord = "Initial" };
            var results = new List<string?>();

            fixture.ObservableForProperty<TestFixture, string?>(nameof(TestFixture.IsOnlyOneWord), beforeChange: true)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(x => results.Add(x.Value));

            fixture.IsOnlyOneWord = "Changed";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).IsEqualTo("Initial");
        });

    /// <summary>
    /// Tests ObservableForProperty string overload without skipInitial.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_StringPropertyNameNoSkipInitial_EmitsInitialValue() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture { IsOnlyOneWord = "Initial" };
            var results = new List<string?>();

            fixture.ObservableForProperty<TestFixture, string?>(
                       nameof(TestFixture.IsOnlyOneWord),
                       beforeChange: false,
                       skipInitial: false)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(x => results.Add(x.Value));

            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).IsEqualTo("Initial");

            fixture.IsOnlyOneWord = "Changed";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(2);
            await Assert.That(results[1]).IsEqualTo("Changed");
        });

    /// <summary>
    /// Tests ObservableForProperty string overload with isDistinct parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_StringPropertyNameWithIsDistinct_Works() =>
        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture();
            var results = new List<string?>();

            fixture.ObservableForProperty<TestFixture, string?>(
                       nameof(TestFixture.IsOnlyOneWord),
                       beforeChange: false,
                       skipInitial: true,
                       isDistinct: true)
                   .ObserveOn(ImmediateScheduler.Instance)
                   .Subscribe(x => results.Add(x.Value));

            fixture.IsOnlyOneWord = "Value1";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(1);

            fixture.IsOnlyOneWord = "Value2";
            scheduler.Start();
            await Assert.That(results).Count().IsEqualTo(2);
        });

    /// <summary>
    /// Tests ObservableForProperty string overload throws for null property name.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_StringPropertyNameNull_Throws()
    {
        var fixture = new TestFixture();

        await Assert.That(() => fixture.ObservableForProperty<TestFixture, string?>((string)null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests ObservableForProperty string overload throws for null item.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_StringPropertyNameNullItem_Throws()
    {
        TestFixture? fixture = null;

        await Assert.That(() => fixture.ObservableForProperty<TestFixture, string?>(nameof(TestFixture.IsOnlyOneWord)))
            .Throws<ArgumentNullException>();
    }
}
