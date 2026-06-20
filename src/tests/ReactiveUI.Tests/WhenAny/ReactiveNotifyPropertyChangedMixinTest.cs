// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using ReactiveUI.Tests.WhenAny.Mockups;
using TUnit.Assertions.Enums;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests for the reactive notify property changed mixin (WhenAny, WhenAnyValue, ObservableForProperty).</summary>
public partial class ReactiveNotifyPropertyChangedMixinTest
{
    /// <summary>The expected notification count after the second property change.</summary>
    private const int ExpectedCountAfterSecondChange = 2;

    /// <summary>The expected notification count after the third property change.</summary>
    private const int ExpectedCountAfterThirdChange = 3;

    /// <summary>The expected notification count after the fourth property change.</summary>
    private const int ExpectedCountAfterFourthChange = 4;

    /// <summary>Test value "Foo".</summary>
    private const string FooText = "Foo";

    /// <summary>Test value "Bar".</summary>
    private const string BarText = "Bar";

    /// <summary>Test value "Baz".</summary>
    private const string BazText = "Baz";

    /// <summary>Test value "Bamf".</summary>
    private const string BamfText = "Bamf";

    /// <summary>Test value "Initial".</summary>
    private const string InitialText = "Initial";

    /// <summary>Test value "Changed".</summary>
    private const string ChangedText = "Changed";

    /// <summary>Test value "Pre".</summary>
    private const string PreText = "Pre";

    /// <summary>Test value "Test".</summary>
    private const string TestText = "Test";

    /// <summary>Test value "abc".</summary>
    private const string AbcText = "abc";

    /// <summary>Test value "A".</summary>
    private const string AText = "A";

    /// <summary>Test value "B".</summary>
    private const string BText = "B";

    /// <summary>Test value "Value1".</summary>
    private const string Value1Text = "Value1";

    /// <summary>Test value "Value2".</summary>
    private const string Value2Text = "Value2";

    /// <summary>The "IsOnlyOneWord" property name used in expression-chain tests.</summary>
    private const string IsOnlyOneWordName = "IsOnlyOneWord";

    /// <summary>The "Child" property name used in expression-chain tests.</summary>
    private const string ChildName = "Child";

    /// <summary>The "Child.IsOnlyOneWord" nested property path used in expression-chain tests.</summary>
    private const string ChildIsOnlyOneWordName = "Child.IsOnlyOneWord";

    /// <summary>Test value "1".</summary>
    private const string OneText = "1";

    /// <summary>Test value "13".</summary>
    private const string OneThreeText = "13";

    /// <summary>Test value "135".</summary>
    private const string OneThreeFiveText = "135";

    /// <summary>Test value "1357".</summary>
    private const string OneThreeFiveSevenText = "1357";

    /// <summary>Gets or sets the dummy.</summary>
    public string? Dummy { get; set; }

    /// <summary>Verifies that any change in a deep expression list triggers the update sequence.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AnyChangeInExpressionListTriggersUpdate()
    {
        const int FirstParamValue = 42;
        const int SecondParamValue = 10;

        var obj = new ObjChain1();

        var obsUpdated = false;

        obj.ObservableForProperty(x => x.Model.Model.Model.SomeOtherParam).Subscribe(_ => obsUpdated = true);

        obsUpdated = false;

        obj.Model.Model.Model.SomeOtherParam = FirstParamValue;

        await Assert.That(obsUpdated).IsTrue();

        obsUpdated = false;

        obj.Model.Model.Model = new();

        await Assert.That(obsUpdated).IsTrue();

        obsUpdated = false;

        obj.Model.Model = new() { Model = new() { SomeOtherParam = SecondParamValue } };

        await Assert.That(obsUpdated).IsTrue();

        obsUpdated = false;

        obj.Model = new();

        await Assert.That(obsUpdated).IsTrue();
    }

    /// <summary>The <c>Changed</c> stream contains valid sender and property name data.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangedShouldHaveValidData()
    {
        var fixture = new TestFixture { IsNotNullString = FooText, IsOnlyOneWord = BazText, PocoProperty = BamfText };

        object? sender = null;

        string? propertyName = null;

        fixture.Changed.ObserveOn(Sequencer.Immediate).Subscribe(x =>
        {
            sender = x.Sender;

            propertyName = x.PropertyName;
        });

        fixture.UsesExprRaiseSet = AbcText;

        using (Assert.Multiple())
        {
            await Assert.That(sender).IsEqualTo(fixture);

            await Assert.That(propertyName).IsEqualTo(nameof(fixture.UsesExprRaiseSet));
        }

        sender = null;

        propertyName = null;

        fixture.PocoProperty = AbcText;

        using (Assert.Multiple())
        {
            await Assert.That(sender).IsNull();

            await Assert.That(propertyName).IsNull();
        }
    }

    /// <summary>The <c>Changing</c> stream contains valid sender and property name data.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangingShouldHaveValidData()
    {
        var fixture = new TestFixture { IsNotNullString = FooText, IsOnlyOneWord = BazText, PocoProperty = BamfText };

        object? sender = null;

        string? propertyName = null;

        fixture.Changing.ObserveOn(Sequencer.Immediate).Subscribe(x =>
        {
            sender = x.Sender;

            propertyName = x.PropertyName;
        });

        fixture.UsesExprRaiseSet = AbcText;

        using (Assert.Multiple())
        {
            await Assert.That(sender).IsEqualTo(fixture);

            await Assert.That(propertyName).IsEqualTo(nameof(fixture.UsesExprRaiseSet));
        }

        sender = null;

        propertyName = null;

        fixture.PocoProperty = AbcText;

        using (Assert.Multiple())
        {
            await Assert.That(sender).IsNull();

            await Assert.That(propertyName).IsNull();
        }
    }

    /// <summary>Ensures multi-property expressions are correctly rewritten and resolved.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task MultiPropertyExpressionsShouldBeProperlyResolved()
    {
        var data = new Dictionary<Expression<Func<HostTestFixture, object>>, string[]>
        {
            { static x => x.Child!.IsOnlyOneWord!.Length, [ChildName, IsOnlyOneWordName, "Length"] },
            { static x => x.SomeOtherParam, ["SomeOtherParam"] },
            { static x => x.Child!.IsNotNullString!, [ChildName, "IsNotNullString"] },
            { static x => x.Child!.Changed, [ChildName, ChangedText] }
        };

        var dataTypes = new Dictionary<Expression<Func<HostTestFixture, object>>, string[]>
        {
            {
                static x =>
                    x.Child!.IsOnlyOneWord!.Length,
                [typeof(TestFixture).FullName!, typeof(string).FullName!, typeof(int).FullName!]
            },
            { static x => x.SomeOtherParam, [typeof(int).FullName!] },
            { static x => x.Child!.IsNotNullString!, [typeof(TestFixture).FullName!, typeof(string).FullName!] },
            {
                static x =>
                    x.Child!.Changed,
                [
                    typeof(TestFixture).FullName!,
                    typeof(IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>).FullName!
                ]
            }
        };

        var results = data.Keys.Select(static x => new { input = x, output = Reflection.Rewrite(x.Body).GetExpressionChain() }).ToArray();

        var resultTypes = dataTypes.Keys.Select(static x =>
                    new { input = x, output = Reflection.Rewrite(x.Body).GetExpressionChain() }).ToArray();

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

    /// <summary>Non-nullable pipeline works without extra decorators.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NonNullableTypesTestShouldntNeedDecorators()
    {
        const int ExpectedCount = 3;

        var fixture = new WhenAnyTestFixture();

        IEnumerable<AccountUser>? result = null;

        fixture.WhenAnyValue(x => x.AccountService.AccountUsers)
            .Where(users => users.Count > 0)
            .Select(users => users.Values.Where(x => !string.IsNullOrWhiteSpace(x.LastName)))
            .Subscribe(dict => result = dict);

        await Assert.That(result!.Count()).IsEqualTo(ExpectedCount);
    }

    /// <summary>Non-nullable tuple pipeline works without extra decorators.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NonNullableTypesTestShouldntNeedDecorators2()
    {
        const int ExpectedCount = 3;

        var fixture = new WhenAnyTestFixture();

        IEnumerable<AccountUser>? result = null;

        fixture.WhenAnyValue(
            x => x.ProjectService.Projects,
            x => x.AccountService.AccountUsers).Where(tuple => tuple.Value1?.Count > 0 && tuple.Value2?.Count > 0).Select(tuple =>
                {
                    var (_, users) = tuple;

                    return users.Values.Where(x => !string.IsNullOrWhiteSpace(x.LastName));
                }).Subscribe(dict => result = dict);

        await Assert.That(result!.Count()).IsEqualTo(ExpectedCount);
    }

    /// <summary>Nullable pipeline works without extra decorators.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableTypesTestShouldntNeedDecorators()
    {
        const int ExpectedCount = 3;

        var fixture = new WhenAnyTestFixture();

        IEnumerable<AccountUser?>? result = null;

        fixture.WhenAnyValue(x => x.AccountService.AccountUsersNullable)
            .Where(users => users.Count > 0)
            .Select(users => users.Values.Where(x => !string.IsNullOrWhiteSpace(x?.LastName)))
            .Subscribe(dict => result = dict);

        await Assert.That(result!.Count()).IsEqualTo(ExpectedCount);
    }

    /// <summary>Nullable tuple pipeline works without extra decorators.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableTypesTestShouldntNeedDecorators2()
    {
        const int ExpectedCount = 3;

        var fixture = new WhenAnyTestFixture();

        IEnumerable<AccountUser?>? result = null;

        fixture.WhenAnyValue(
            x => x.ProjectService.ProjectsNullable,
            x => x.AccountService.AccountUsersNullable).Where(tuple => tuple.Value1.Count > 0 && tuple.Value2?.Count > 0).Select(tuple =>
                {
                    var (_, users) = tuple;

                    return users?.Values.Where(x => !string.IsNullOrWhiteSpace(x?.LastName));
                }).Subscribe(dict => result = dict);

        await Assert.That(result!.Count()).IsEqualTo(ExpectedCount);
    }

    /// <summary>Ensures intermediate objects are eligible for GC when property value changes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObjectShouldBeGarbageCollectedWhenPropertyValueChanges()
    {
        static (ObjChain1, WeakReference) GetWeakReference1()
        {
            var obj = new ObjChain1();

            var weakRef = new WeakReference(obj.Model);

            obj.ObservableForProperty(static x => x.Model.Model.Model.SomeOtherParam).Subscribe();

            obj.Model = new();

            return (obj, weakRef);
        }

        static (ObjChain1, WeakReference) GetWeakReference2()
        {
            var obj = new ObjChain1();

            var weakRef = new WeakReference(obj.Model.Model);

            obj.ObservableForProperty(static x => x.Model.Model.Model.SomeOtherParam).Subscribe();

            obj.Model.Model = new();

            return (obj, weakRef);
        }

        static (ObjChain1, WeakReference) GetWeakReference3()
        {
            var obj = new ObjChain1();

            var weakRef = new WeakReference(obj.Model.Model.Model);

            obj.ObservableForProperty(static x => x.Model.Model.Model.SomeOtherParam).Subscribe();

            obj.Model.Model.Model = new();

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

    /// <summary>Subscribing to WhenAny should push the current value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SubscriptionToWhenAnyShouldReturnCurrentValue()
    {
        const int ExpectedValue = 42;

        var obj = new HostTestFixture();

        var observedValue = 1;

        obj.WhenAnyValue(x => x.SomeOtherParam).Subscribe(x => observedValue = x);

        obj.SomeOtherParam = ExpectedValue;

        await Assert.That(observedValue).IsEqualTo(obj.SomeOtherParam);
    }

    /// <summary>WhenAny executes on the current synchronization context.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyShouldRunInContext()
    {
        var tid = Environment.CurrentManagedThreadId;
        var whenAnyTid = 0;

        var fixture = new TestFixture { IsNotNullString = FooText, IsOnlyOneWord = BazText, PocoProperty = BamfText };

        fixture.WhenAnyValue(x => x.IsNotNullString).ObserveOn(Sequencer.Immediate).Subscribe(__ => whenAnyTid = Environment.CurrentManagedThreadId);

        fixture.IsNotNullString = BarText;

        await Assert.That(whenAnyTid).IsEqualTo(tid);
    }

    /// <summary>WhenAny works with "normal" CLR properties.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyShouldWorkEvenWithNormalProperties()
    {
        var fixture = new TestFixture { IsNotNullString = FooText, IsOnlyOneWord = BazText, PocoProperty = BamfText };

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

            await Assert.That(output[0]!.Value).IsEqualTo(BamfText);

            await Assert.That(output2).Count().IsEqualTo(1);

            await Assert.That(output2[0]).IsEqualTo(BamfText);

            await Assert.That(output3).Count().IsEqualTo(1);

            await Assert.That(output3[0]!.Sender).IsEqualTo(fixture);

            await Assert.That(output3[0]!.GetPropertyName()).IsEqualTo("NullableInt");

            await Assert.That(output3[0]!.Value).IsNull();

            await Assert.That(output4).Count().IsEqualTo(1);

            await Assert.That(output4[0]).IsNull();
        }
    }

    /// <summary>Smoke test for <c>WhenAny</c> combining two properties.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task WhenAnySmokeTest()
    {
        const int InitialParam = 5;
        const int UpdatedParam = 10;
        const int ThirdEmissionIndex = 2;

        var fixture = new HostTestFixture { Child = new(), SomeOtherParam = InitialParam };

        fixture.Child.IsNotNullString = FooText;

        var output1 = new List<IObservedChange<HostTestFixture, int>>();

        var output2 = new List<IObservedChange<HostTestFixture, string>>();

        fixture.WhenAny(
            x => x.SomeOtherParam,
            x => x.Child!.IsNotNullString,
            (sop, nns) => new { sop, nns }).Subscribe(x =>
            {
                output1.Add(x.sop);

                output2.Add(x.nns!);
            });

        // ImmediateScheduler executes synchronously
        using (Assert.Multiple())
        {
            await Assert.That(output1).Count().IsEqualTo(1);

            await Assert.That(output2).Count().IsEqualTo(1);

            await Assert.That(output1[0].Sender).IsEqualTo(fixture);

            await Assert.That(output2[0].Sender).IsEqualTo(fixture);

            await Assert.That(output1[0].Value).IsEqualTo(InitialParam);

            await Assert.That(output2[0].Value).IsEqualTo(FooText);
        }

        fixture.SomeOtherParam = UpdatedParam;

        // ImmediateScheduler executes synchronously
        using (Assert.Multiple())
        {
            await Assert.That(output1).Count().IsEqualTo(ExpectedCountAfterSecondChange);

            await Assert.That(output2).Count().IsEqualTo(ExpectedCountAfterSecondChange);

            await Assert.That(output1[1].Sender).IsEqualTo(fixture);

            await Assert.That(output2[1].Sender).IsEqualTo(fixture);

            await Assert.That(output1[1].Value).IsEqualTo(UpdatedParam);

            await Assert.That(output2[1].Value).IsEqualTo(FooText);
        }

        fixture.Child.IsNotNullString = BarText;

        // ImmediateScheduler executes synchronously
        using (Assert.Multiple())
        {
            await Assert.That(output1).Count().IsEqualTo(ExpectedCountAfterThirdChange);

            await Assert.That(output2).Count().IsEqualTo(ExpectedCountAfterThirdChange);

            await Assert.That(output1[ThirdEmissionIndex].Sender).IsEqualTo(fixture);

            await Assert.That(output2[ThirdEmissionIndex].Sender).IsEqualTo(fixture);

            await Assert.That(output1[ThirdEmissionIndex].Value).IsEqualTo(UpdatedParam);

            await Assert.That(output2[ThirdEmissionIndex].Value).IsEqualTo(BarText);
        }
    }

    /// <summary>WhenAnyValue supports normal CLR properties.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueShouldWorkEvenWithNormalProperties()
    {
        const int BazLength = 3;

        var fixture = new TestFixture { IsNotNullString = FooText, IsOnlyOneWord = BazText, PocoProperty = BamfText };

        var output1 = new List<string?>();

        var output2 = new List<int?>();

        fixture.WhenAnyValue(static x => x.PocoProperty).Subscribe(output1.Add);

        fixture.WhenAnyValue(
            static x => x.IsOnlyOneWord,
            static x => x?.Length).Subscribe(output2.Add);

        using (Assert.Multiple())
        {
            await Assert.That(output1).Count().IsEqualTo(1);

            await Assert.That(output1[0]).IsEqualTo(BamfText);

            await Assert.That(output2).Count().IsEqualTo(1);

            await Assert.That(output2[0]).IsEqualTo(BazLength);
        }
    }

    /// <summary>Smoke test for WhenAnyValue combining two properties with a projector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task WhenAnyValueSmokeTest()
    {
        const int InitialParam = 5;
        const int UpdatedParam = 10;
        const int ThirdEmissionIndex = 2;

        var fixture = new HostTestFixture { Child = new(), SomeOtherParam = InitialParam };

        fixture.Child.IsNotNullString = FooText;

        var output1 = new List<int>();

        var output2 = new List<string>();

        fixture.WhenAnyValue(
            x => x.SomeOtherParam,
            x => x.Child!.IsNotNullString,
            (sop, nns) => new { sop, nns }).Subscribe(x =>
            {
                output1.Add(x.sop);

                output2.Add(x.nns!);
            });

        // ImmediateScheduler executes synchronously
        using (Assert.Multiple())
        {
            await Assert.That(output1).Count().IsEqualTo(1);

            await Assert.That(output2).Count().IsEqualTo(1);

            await Assert.That(output1[0]).IsEqualTo(InitialParam);

            await Assert.That(output2[0]).IsEqualTo(FooText);
        }

        fixture.SomeOtherParam = UpdatedParam;

        // ImmediateScheduler executes synchronously
        using (Assert.Multiple())
        {
            await Assert.That(output1).Count().IsEqualTo(ExpectedCountAfterSecondChange);

            await Assert.That(output2).Count().IsEqualTo(ExpectedCountAfterSecondChange);

            await Assert.That(output1[1]).IsEqualTo(UpdatedParam);

            await Assert.That(output2[1]).IsEqualTo(FooText);
        }

        fixture.Child.IsNotNullString = BarText;

        // ImmediateScheduler executes synchronously
        using (Assert.Multiple())
        {
            await Assert.That(output1).Count().IsEqualTo(ExpectedCountAfterThirdChange);

            await Assert.That(output2).Count().IsEqualTo(ExpectedCountAfterThirdChange);

            await Assert.That(output1[ThirdEmissionIndex]).IsEqualTo(UpdatedParam);

            await Assert.That(output2[ThirdEmissionIndex]).IsEqualTo(BarText);
        }
    }

    /// <summary>Throws when WhenAnyValue receives an unsupported Constant expression.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueUnsupportedExpressionType_Constant()
    {
        var fixture = new TestFixture();

        var exception = Assert.Throws<NotSupportedException>(() => fixture.WhenAnyValue(_ => Dummy).Subscribe());

        await Assert.That(exception.Message).IsEqualTo("Unsupported expression of type 'Constant'. Did you miss the member access prefix in the expression?");
    }

    /// <summary>Throws when WhenAnyValue receives an unsupported Equal expression.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueUnsupportedExpressionType_Equal()
    {
        var fixture = new TestFixture();

        var exception =
            Assert.Throws<NotSupportedException>(() => fixture.WhenAnyValue(x => x.IsNotNullString == x.IsOnlyOneWord).Subscribe());

        await Assert.That(exception.Message)
            .IsEqualTo(
                "Unsupported expression of type 'Equal' (x.IsNotNullString == x.IsOnlyOneWord). Did you meant to use expressions 'x.IsNotNullString' and 'x.IsOnlyOneWord'?");
    }

    /// <summary>Verifies ToProperty projections for owner and owner name.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
}
