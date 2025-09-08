// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the observable as property helper.
/// </summary>
[TestFixture]
public class ObservableAsPropertyHelperTest
{
    /// <summary>
    /// Tests that Observable As Property Helpers should fire change notifications.
    /// </summary>
    [Test]
    public void OAPHShouldFireChangeNotifications()
    {
        var input = new[] { 1, 2, 3, 3, 4 }.ToObservable();
        var output = new List<int>();

        new TestScheduler().With(scheduler =>
        {
            var fixture = new ObservableAsPropertyHelper<int>(
                                                              input,
                                                              x => output.Add(x),
                                                              -5,
                                                              scheduler: scheduler);

            scheduler.Start();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.Value, Is.EqualTo(input.LastAsync().Wait()));

                // Suppresses duplicate notifications (note single '3')
                Assert.That(output, Is.EqualTo([-5, 1, 2, 3, 4]));
            }
        });
    }

    /// <summary>
    /// Tests that Observable As Property Helpers should skip first value if it matches the initial value.
    /// </summary>
    [Test]
    public void OAPHShouldSkipFirstValueIfItMatchesTheInitialValue()
    {
        var input = new[] { 1, 2, 3 }.ToObservable();
        var output = new List<int>();

        new TestScheduler().With(scheduler =>
        {
            var fixture = new ObservableAsPropertyHelper<int>(
                                                              input,
                                                              x => output.Add(x),
                                                              1,
                                                              scheduler: scheduler);

            scheduler.Start();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.Value, Is.EqualTo(input.LastAsync().Wait()));
                Assert.That(output, Is.EqualTo([1, 2, 3]));
            }
        });
    }

    /// <summary>
    /// Tests that OAPH should provide initial value immediately regardless of scheduler.
    /// </summary>
    [Test]
    public void OAPHShouldProvideInitialValueImmediatelyRegardlessOfScheduler()
    {
        var output = new List<int>();

        new TestScheduler().With(scheduler =>
        {
            var fixture = new ObservableAsPropertyHelper<int>(
                                                              Observable<int>.Never,
                                                              x => output.Add(x),
                                                              32,
                                                              scheduler: scheduler);

            Assert.That(fixture.Value, Is.EqualTo(32));
        });
    }

    /// <summary>
    /// Tests that OAPH should provide latest value.
    /// </summary>
    [Test]
    public void OAPHShouldProvideLatestValue() =>
        new TestScheduler().With(scheduler =>
        {
            var input = new Subject<int>();

            var fixture = new ObservableAsPropertyHelper<int>(
                                                              input,
                                                              _ => { },
                                                              -5,
                                                              scheduler: scheduler);

            Assert.That(fixture.Value, Is.EqualTo(-5));

            new[] { 1, 2, 3, 4 }.Run(x => input.OnNext(x));
            scheduler.Start();
            Assert.That(fixture.Value, Is.EqualTo(4));

            input.OnCompleted();
            scheduler.Start();
            Assert.That(fixture.Value, Is.EqualTo(4));
        });

    /// <summary>
    /// OAPH should subscribe immediately to source.
    /// </summary>
    [Test]
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(isSubscribed, Is.True);
            Assert.That(fixture.Value, Is.EqualTo(42));
        }
    }

    /// <summary>
    /// Defer subscription parameter defers subscription to source.
    /// </summary>
    [Test]
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(isSubscribed, Is.False);
            Assert.That(fixture.Value, Is.EqualTo(42));
        }

        Assert.That(isSubscribed, Is.True);
    }

    /// <summary>
    /// Defer subscription: IsSubscribed is not true initially.
    /// </summary>
    [Test]
    public void OAPHDeferSubscriptionParameterIsSubscribedIsNotTrueInitially()
    {
        var observable = Observable.Create<int>(o =>
        {
            o.OnNext(42);
            o.OnCompleted();
            return Disposable.Empty;
        });

        var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, 0, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsSubscribed, Is.False);
            Assert.That(fixture.Value, Is.EqualTo(42));
            Assert.That(fixture.IsSubscribed, Is.True);
        }
    }

    /// <summary>
    /// Defer subscription should not throw if disposed.
    /// </summary>
    [Test]
    public void OAPHDeferSubscriptionShouldNotThrowIfDisposed()
    {
        var observable = Observable.Create<int>(o =>
        {
            o.OnNext(42);
            o.OnCompleted();
            return Disposable.Empty;
        });

        var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, 0, true);

        Assert.That(fixture.IsSubscribed, Is.False);
        fixture.Dispose();

        Assert.DoesNotThrow(() =>
        {
            var value = fixture.Value;
            Assert.That(value, Is.Zero);
        });
    }

    /// <summary>
    /// Verifies that deferring subscription with an initial value does not emit the initial value.
    /// </summary>
    /// <param name="initialValue">The initial value to test with.</param>
    [TestCase(0)]
    [TestCase(42)]
    public void OAPHDeferSubscriptionWithInitialValueShouldNotEmitInitialValue(int initialValue)
    {
        var observable = Observable.Empty<int>();
        var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, initialValue, deferSubscription: true);

        Assert.That(fixture.IsSubscribed, Is.False);

        int? emittedValue = null;
        fixture.Source.Subscribe(val => emittedValue = val);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(emittedValue, Is.Null);
            Assert.That(fixture.IsSubscribed, Is.False);
        }
    }

    /// <summary>
    /// Defer subscription with initial function value should not emit initial value nor access function.
    /// </summary>
    [Test]
    public void OAPHDeferSubscriptionWithInitialFuncValueShouldNotEmitInitialValueNorAccessFunc()
    {
        var observable = Observable.Empty<int>();
        static int ThrowIfAccessed() => throw new Exception();

        var fixture = new ObservableAsPropertyHelper<int>(
                                                          observable, _ => { }, getInitialValue: ThrowIfAccessed, deferSubscription: true);

        Assert.That(fixture.IsSubscribed, Is.False);

        int? emittedValue = null;
        fixture.Source.Subscribe(val => emittedValue = val);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(emittedValue, Is.Null);
            Assert.That(fixture.IsSubscribed, Is.False);
        }
    }

    /// <summary>
    /// Ensures that defer subscription with an initial value emits the initial value upon subscription.
    /// </summary>
    /// <param name="initialValue">
    /// The initial value set before any subscription occurs.
    /// </param>
    [TestCase(0)]
    [TestCase(42)]
    public void OAPHDeferSubscriptionWithInitialValueEmitInitialValueWhenSubscribed(int initialValue)
    {
        var observable = Observable.Empty<int>();
        var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, initialValue, deferSubscription: true);

        Assert.That(fixture.IsSubscribed, Is.False);

        var result = fixture.Value;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsSubscribed, Is.True);
            Assert.That(result, Is.EqualTo(initialValue));
        }
    }

    /// <summary>
    /// Verifies that deferred subscription with an initial value provided by a function emits the initial value
    /// only when subscribed and confirms the function is accessed at that point.
    /// Ensures that the subscription state and access status align with the expected behavior.
    /// </summary>
    [Test]
    public void OAPHDeferSubscriptionWithInitialFuncValueEmitInitialValueWhenSubscribed()
    {
        var observable = Observable.Empty<int>();
        var wasAccessed = false;

        int GetInitialValue()
        {
            wasAccessed = true;
            return 42;
        }

        var fixture = new ObservableAsPropertyHelper<int>(
                                                          observable, _ => { }, getInitialValue: GetInitialValue, deferSubscription: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsSubscribed, Is.False);
            Assert.That(wasAccessed, Is.False);
        }

        var result = fixture.Value;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsSubscribed, Is.True);
            Assert.That(wasAccessed, Is.True);
            Assert.That(result, Is.EqualTo(42));
        }
    }

    /// <summary>
    /// Verifies that deferring subscription with an initial function value does not trigger OnChanged when subscribed.
    /// </summary>
    /// <param name="initialValue">The initial value to set before any subscription occurs.</param>
    [TestCase(0)]
    [TestCase(42)]
    public void OAPHDeferSubscriptionWithInitialFuncValueNotCallOnChangedWhenSubscribed(int initialValue)
    {
        var observable = Observable.Empty<int>();

        var wasOnChangingCalled = false;
        Action<int> onChanging = _ => wasOnChangingCalled = true;
        var wasOnChangedCalled = false;
        Action<int> onChanged = _ => wasOnChangedCalled = true;

        var fixture = new ObservableAsPropertyHelper<int>(
                                                          observable, onChanged, onChanging, () => initialValue, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsSubscribed, Is.False);
            Assert.That(wasOnChangingCalled, Is.False);
            Assert.That(wasOnChangedCalled, Is.False);
        }

        var result = fixture.Value;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsSubscribed, Is.True);
            Assert.That(wasOnChangingCalled, Is.False);
            Assert.That(wasOnChangedCalled, Is.False);
            Assert.That(result, Is.EqualTo(initialValue));
        }
    }

    /// <summary>
    /// Ensures that defer subscription with an initial function value does not trigger the OnChanged callback
    /// when the source observable provides the same initial value.
    /// </summary>
    /// <param name="initialValue">The initial value provided to the ObservableAsPropertyHelper.</param>
    [TestCase(0)]
    [TestCase(42)]
    public void OAPHDeferSubscriptionWithInitialFuncValueNotCallOnChangedWhenSourceProvidesInitialValue(int initialValue)
    {
        var observable = new Subject<int>();
        var wasOnChangingCalled = false;
        Action<int> onChanging = _ => wasOnChangingCalled = true;
        var wasOnChangedCalled = false;
        Action<int> onChanged = _ => wasOnChangedCalled = true;

        var fixture = new ObservableAsPropertyHelper<int>(observable, onChanged, onChanging, () => initialValue, true);

        var result = fixture.Value;
        Assert.That(result, Is.EqualTo(initialValue));

        observable.OnNext(initialValue);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(wasOnChangingCalled, Is.False);
            Assert.That(wasOnChangedCalled, Is.False);
        }
    }

    /// <summary>
    /// Verifies that the initial value of an Observable As Property Helper is emitted correctly.
    /// </summary>
    /// <param name="initialValue">The initial value provided to the Observable As Property Helper.</param>
    [TestCase(0)]
    [TestCase(42)]
    public void OAPHInitialValueShouldEmitInitialValue(int initialValue)
    {
        var observable = Observable.Empty<int>();
        var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, initialValue, deferSubscription: false);

        Assert.That(fixture.IsSubscribed, Is.True);

        int? emittedValue = null;
        fixture.Source.Subscribe(val => emittedValue = val);

        Assert.That(emittedValue, Is.EqualTo(initialValue));
    }

    /// <summary>
    /// OAPH should rethrow errors via ThrownExceptions.
    /// </summary>
    [Test]
    public void OAPHShouldRethrowErrors() =>
        new TestScheduler().With(scheduler =>
        {
            var input = new Subject<int>();
            var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5, scheduler: scheduler);
            var errors = new List<Exception>();

            Assert.That(fixture.Value, Is.EqualTo(-5));
            new[] { 1, 2, 3, 4 }.Run(x => input.OnNext(x));

            fixture.ThrownExceptions.Subscribe(errors.Add);

            scheduler.Start();
            Assert.That(fixture.Value, Is.EqualTo(4));

            input.OnError(new Exception("Die!"));
            scheduler.Start();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.Value, Is.EqualTo(4));
                Assert.That(errors, Has.Count.EqualTo(1));
            }
        });

    /// <summary>
    /// No thrown-exceptions subscriber equals OAPH death.
    /// </summary>
    [Test]
    public void NoThrownExceptionsSubscriberEqualsOAPHDeath() =>
        new TestScheduler().With(scheduler =>
        {
            var input = new Subject<int>();
            var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5, scheduler: ImmediateScheduler.Instance);

            Assert.That(fixture.Value, Is.EqualTo(-5));
            new[] { 1, 2, 3, 4 }.Run(x => input.OnNext(x));

            input.OnError(new Exception("Die!"));

            var failed = true;
            try
            {
                scheduler.Start();
            }
            catch (Exception ex)
            {
                failed = ex?.InnerException?.Message != "Die!";
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(failed, Is.False);
                Assert.That(fixture.Value, Is.EqualTo(4));
            }
        });

    /// <summary>
    /// ToProperty should fire both Changing and Changed.
    /// </summary>
    [Test]
    public void ToPropertyShouldFireBothChangingAndChanged()
    {
        var fixture = new OaphTestFixture();

        // NB: Hack to connect up the OAPH
        _ = (fixture.FirstThreeLettersOfOneWord ?? string.Empty).Substring(0, 0);

        fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, beforeChange: true)
               .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var resultChanging).Subscribe();
        fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, beforeChange: false)
               .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var resultChanged).Subscribe();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultChanging, Is.Empty);
            Assert.That(resultChanged, Is.Empty);
        }

        fixture.IsOnlyOneWord = "FooBar";
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultChanging, Has.Count.EqualTo(1));
            Assert.That(resultChanged, Has.Count.EqualTo(1));
            Assert.That(resultChanging[0].Value, Is.EqualTo(string.Empty));
            Assert.That(resultChanged[0].Value, Is.EqualTo("Foo"));
        }

        fixture.IsOnlyOneWord = "Bazz";
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultChanging, Has.Count.EqualTo(2));
            Assert.That(resultChanged, Has.Count.EqualTo(2));
            Assert.That(resultChanging[1].Value, Is.EqualTo("Foo"));
            Assert.That(resultChanged[1].Value, Is.EqualTo("Baz"));
        }
    }

    /// <summary>
    /// ToProperty(nameof) should raise standard notifications.
    /// </summary>
    [Test]
    public void ToProperty_NameOf_ShouldFireBothChangingAndChanged()
    {
        var fixture = new OaphNameOfTestFixture();

        var changing = false;
        var changed = false;

        fixture.PropertyChanging += (_, __) => changing = true;
        fixture.PropertyChanged += (_, __) => changed = true;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(changing, Is.False);
            Assert.That(changed, Is.False);
        }

        fixture.IsOnlyOneWord = "baz";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(changing, Is.True);
            Assert.That(changed, Is.True);
        }
    }

    /// <summary>
    /// Ensures that the ToProperty method, when used with nameof, produces valid output values
    /// for derived properties by comparing test data against expected values.
    /// </summary>
    /// <param name="testWords">An array of input strings to evaluate.</param>
    /// <param name="first3Letters">An array of expected first three letters for each input string in <paramref name="testWords"/>.</param>
    /// <param name="last3Letters">An array of expected last three letters for each input string in <paramref name="testWords"/>.</param>
    [TestCase(
                 new[] { "FooBar", "Bazz" },
                 new[] { "Foo", "Baz" },
                 new[] { "Bar", "azz" })]
    public void ToProperty_NameOf_ValidValuesProduced(string[] testWords, string[] first3Letters, string[] last3Letters)
    {
        if (testWords is null)
        {
            throw new ArgumentNullException(nameof(testWords));
        }

        if (first3Letters is null)
        {
            throw new ArgumentNullException(nameof(first3Letters));
        }

        if (last3Letters is null)
        {
            throw new ArgumentNullException(nameof(last3Letters));
        }

        var fixture = new OaphNameOfTestFixture();

        fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, beforeChange: true)
               .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var firstThreeChanging).Subscribe();
        fixture.ObservableForProperty(x => x.LastThreeLettersOfOneWord, beforeChange: true)
               .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var lastThreeChanging).Subscribe();
        var changing = new[] { firstThreeChanging!, lastThreeChanging };

        fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, beforeChange: false)
               .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var firstThreeChanged).Subscribe();
        fixture.ObservableForProperty(x => x.LastThreeLettersOfOneWord, beforeChange: false)
               .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var lastThreeChanged).Subscribe();
        var changed = new[] { firstThreeChanged!, lastThreeChanged };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(changed.All(x => x.Count == 0), Is.True);
            Assert.That(changing.All(x => x.Count == 0), Is.True);
        }

        for (var i = 0; i < testWords.Length; ++i)
        {
            fixture.IsOnlyOneWord = testWords[i];

            using (Assert.EnterMultipleScope())
            {
                Assert.That(changed.All(x => x.Count == i + 1), Is.True);
                Assert.That(changing.All(x => x.Count == i + 1), Is.True);
                Assert.That(firstThreeChanged[i].Value, Is.EqualTo(first3Letters[i]));
                Assert.That(lastThreeChanged[i].Value, Is.EqualTo(last3Letters[i]));
            }

            var firstChanging = i < 1 ? string.Empty : first3Letters[i - 1];
            var lastChanging = i < 1 ? string.Empty : last3Letters[i - 1]; // fixed from i - i

            using (Assert.EnterMultipleScope())
            {
                Assert.That(firstThreeChanging[i].Value, Is.EqualTo(firstChanging));
                Assert.That(lastThreeChanging[i].Value, Is.EqualTo(lastChanging));
            }
        }
    }

    /// <summary>
    /// ToProperty with indexer notifies expected property name.
    /// </summary>
    [Test]
    public void ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName()
    {
        var fixture = new OAPHIndexerTestFixture(0, ImmediateScheduler.Instance);
        var propertiesChanged = new List<string>();

        fixture.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName is not null)
            {
                propertiesChanged.Add(args.PropertyName);
            }
        };

        fixture.Text = "awesome";

        Assert.That(propertiesChanged, Is.EqualTo(["Text", "Item[]"]));
    }

    /// <summary>
    /// Indexer: not supported path throws NotSupportedException.
    /// </summary>
    [Test]
    public void ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName1() =>
        new TestScheduler().With(scheduler =>
                                     Assert.Throws<NotSupportedException>(() =>
                                     {
                                         var fixture = new OAPHIndexerTestFixture(1, scheduler);
                                         var propertiesChanged = new List<string>();

                                         fixture.PropertyChanged += (_, args) =>
                                         {
                                             if (args.PropertyName is not null)
                                             {
                                                 propertiesChanged.Add(args.PropertyName);
                                             }
                                         };

                                         fixture.Text = "awesome";
                                     }));

    /// <summary>
    /// Indexer: invalid path throws ArgumentException.
    /// </summary>
    [Test]
    public void ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName2() =>
        new TestScheduler().With(scheduler =>
                                     Assert.Throws<ArgumentException>(() =>
                                     {
                                         _ = new OAPHIndexerTestFixture(2, scheduler);
                                     }));

    /// <summary>
    /// Nullable types test shouldn't need decorators with ToProperty.
    /// </summary>
    [Test]
    public void NullableTypesTestShouldntNeedDecorators2_ToProperty()
    {
        var fixture = new WhenAnyTestFixture();
        fixture.WhenAnyValue(
                             x => x.ProjectService.ProjectsNullable,
                             x => x.AccountService.AccountUsersNullable)
               .Where(tuple => tuple.Item1.Count > 0 && tuple.Item2.Count > 0)
               .Select(tuple =>
               {
                   var (projects, users) = tuple;
                   return users?.Values.Count(x => !string.IsNullOrWhiteSpace(x?.LastName));
               })
               .ToProperty(fixture, x => x.AccountsFound, out fixture._accountsFound);

        Assert.That(fixture.AccountsFound, Is.EqualTo(3));
    }
}
