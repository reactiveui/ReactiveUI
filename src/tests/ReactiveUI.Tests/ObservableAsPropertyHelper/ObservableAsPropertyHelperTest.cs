// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

public class ObservableAsPropertyHelperTest
{
    /// <summary>
    /// Tests that Observable As Property Helpers should fire change notifications.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphShouldFireChangeNotifications()
    {
        var input = new[] { 1, 2, 3, 3, 4 }.ToObservable();
        var output = new List<int>();

        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new ObservableAsPropertyHelper<int>(
                                                              input,
                                                              x => output.Add(x),
                                                              -5,
                                                              scheduler: scheduler);

            scheduler.Start();

            using (Assert.Multiple())
            {
                await Assert.That(fixture.Value).IsEqualTo(input.LastAsync().Wait());

                // Suppresses duplicate notifications (note single '3')
                await Assert.That(output).IsEquivalentTo([-5, 1, 2, 3, 4]);
            }
        });
    }

    /// <summary>
    /// Tests that Observable As Property Helpers should skip first value if it matches the initial value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphShouldSkipFirstValueIfItMatchesTheInitialValue()
    {
        var input = new[] { 1, 2, 3 }.ToObservable();
        var output = new List<int>();

        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new ObservableAsPropertyHelper<int>(
                                                              input,
                                                              x => output.Add(x),
                                                              1,
                                                              scheduler: scheduler);

            scheduler.Start();

            using (Assert.Multiple())
            {
                await Assert.That(fixture.Value).IsEqualTo(input.LastAsync().Wait());
                await Assert.That(output).IsEquivalentTo([1, 2, 3]);
            }
        });
    }

    /// <summary>
    /// Tests that OAPH should provide initial value immediately regardless of scheduler.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphShouldProvideInitialValueImmediatelyRegardlessOfScheduler()
    {
        var output = new List<int>();

        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new ObservableAsPropertyHelper<int>(
                                                              Observable<int>.Never,
                                                              x => output.Add(x),
                                                              32,
                                                              scheduler: scheduler);

            await Assert.That(fixture.Value).IsEqualTo(32);
        });
    }

    /// <summary>
    /// Tests that OAPH should provide latest value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphShouldProvideLatestValue() =>
        await new TestScheduler().With(async scheduler =>
        {
            var input = new Subject<int>();

            var fixture = new ObservableAsPropertyHelper<int>(
                                                              input,
                                                              _ => { },
                                                              -5,
                                                              scheduler: scheduler);

            await Assert.That(fixture.Value).IsEqualTo(-5);

            new[] { 1, 2, 3, 4 }.Run(x => input.OnNext(x));
            scheduler.Start();
            await Assert.That(fixture.Value).IsEqualTo(4);

            input.OnCompleted();
            scheduler.Start();
            await Assert.That(fixture.Value).IsEqualTo(4);
        });

    /// <summary>
    /// OAPH should subscribe immediately to source.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphShouldSubscribeImmediatelyToSource()
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

        using (Assert.Multiple())
        {
            await Assert.That(isSubscribed).IsTrue();
            await Assert.That(fixture.Value).IsEqualTo(42);
        }
    }

    /// <summary>
    /// Defer subscription parameter defers subscription to source.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphDeferSubscriptionParameterDefersSubscriptionToSource()
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

        using (Assert.Multiple())
        {
            await Assert.That(isSubscribed).IsFalse();
            await Assert.That(fixture.Value).IsEqualTo(42);
        }

        await Assert.That(isSubscribed).IsTrue();
    }

    /// <summary>
    /// Defer subscription: IsSubscribed is not true initially.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphDeferSubscriptionParameterIsSubscribedIsNotTrueInitially()
    {
        var observable = Observable.Create<int>(static o =>
        {
            o.OnNext(42);
            o.OnCompleted();
            return Disposable.Empty;
        });

        var fixture = new ObservableAsPropertyHelper<int>(observable, static _ => { }, 0, true);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsFalse();
            await Assert.That(fixture.Value).IsEqualTo(42);
            await Assert.That(fixture.IsSubscribed).IsTrue();
        }
    }

    /// <summary>
    /// Defer subscription should not throw if disposed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphDeferSubscriptionShouldNotThrowIfDisposed()
    {
        var observable = Observable.Create<int>(o =>
        {
            o.OnNext(42);
            o.OnCompleted();
            return Disposable.Empty;
        });

        var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, 0, true);

        await Assert.That(fixture.IsSubscribed).IsFalse();
        fixture.Dispose();

        await Assert.That(() =>
        {
            var value = fixture.Value;
            return Task.CompletedTask;
        }).ThrowsNothing();

        var value = fixture.Value;
        await Assert.That(value).IsEqualTo(0);
    }

    /// <summary>
    /// Verifies that deferring subscription with an initial value does not emit the initial value.
    /// </summary>
    /// <param name="initialValue">The initial value to test with.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(42)]
    public async Task OaphDeferSubscriptionWithInitialValueShouldNotEmitInitialValue(int initialValue)
    {
        var observable = Observable.Empty<int>();
        var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, initialValue, deferSubscription: true);

        await Assert.That(fixture.IsSubscribed).IsFalse();

        int? emittedValue = null;
        fixture.Source.Subscribe(val => emittedValue = val);

        using (Assert.Multiple())
        {
            await Assert.That(emittedValue).IsNull();
            await Assert.That(fixture.IsSubscribed).IsFalse();
        }
    }

    /// <summary>
    /// Defer subscription with initial function value should not emit initial value nor access function.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphDeferSubscriptionWithInitialFuncValueShouldNotEmitInitialValueNorAccessFunc()
    {
        var observable = Observable.Empty<int>();

        var fixture = new ObservableAsPropertyHelper<int>(
                                                          observable, _ => { }, getInitialValue: ThrowIfAccessed, deferSubscription: true);

        await Assert.That(fixture.IsSubscribed).IsFalse();

        int? emittedValue = null;
        fixture.Source.Subscribe(val => emittedValue = val);

        using (Assert.Multiple())
        {
            await Assert.That(emittedValue).IsNull();
            await Assert.That(fixture.IsSubscribed).IsFalse();
        }

        return;

        static int ThrowIfAccessed() => throw new Exception();
    }

    /// <summary>
    /// Ensures that defer subscription with an initial value emits the initial value upon subscription.
    /// </summary>
    /// <param name="initialValue">
    /// The initial value set before any subscription occurs.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(42)]
    public async Task OaphDeferSubscriptionWithInitialValueEmitInitialValueWhenSubscribed(int initialValue)
    {
        var observable = Observable.Empty<int>();
        var fixture = new ObservableAsPropertyHelper<int>(observable, static _ => { }, initialValue, deferSubscription: true);

        await Assert.That(fixture.IsSubscribed).IsFalse();

        var result = fixture.Value;

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsTrue();
            await Assert.That(result).IsEqualTo(initialValue);
        }
    }

    /// <summary>
    /// Verifies that deferred subscription with an initial value provided by a function emits the initial value
    /// only when subscribed and confirms the function is accessed at that point.
    /// Ensures that the subscription state and access status align with the expected behavior.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphDeferSubscriptionWithInitialFuncValueEmitInitialValueWhenSubscribed()
    {
        var observable = Observable.Empty<int>();
        var wasAccessed = false;

        var fixture = new ObservableAsPropertyHelper<int>(
                                                          observable, _ => { }, getInitialValue: GetInitialValue, deferSubscription: true);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsFalse();
            await Assert.That(wasAccessed).IsFalse();
        }

        var result = fixture.Value;

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsTrue();
            await Assert.That(wasAccessed).IsTrue();
            await Assert.That(result).IsEqualTo(42);
        }

        return;

        int GetInitialValue()
        {
            wasAccessed = true;
            return 42;
        }
    }

    /// <summary>
    /// Verifies that deferring subscription with an initial function value does not trigger OnChanged when subscribed.
    /// </summary>
    /// <param name="initialValue">The initial value to set before any subscription occurs.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(42)]
    public async Task OAPHDeferSubscriptionWithInitialFuncValueNotCallOnChangedWhenSubscribed(int initialValue)
    {
        var observable = Observable.Empty<int>();

        var wasOnChangingCalled = false;
        var wasOnChangedCalled = false;

        var fixture = new ObservableAsPropertyHelper<int>(
                                                          observable, OnChanged, OnChanging, () => initialValue, true);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsFalse();
            await Assert.That(wasOnChangingCalled).IsFalse();
            await Assert.That(wasOnChangedCalled).IsFalse();
        }

        var result = fixture.Value;

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsTrue();
            await Assert.That(wasOnChangingCalled).IsFalse();
            await Assert.That(wasOnChangedCalled).IsFalse();
            await Assert.That(result).IsEqualTo(initialValue);
        }

        return;

        void OnChanged(int unused) => wasOnChangedCalled = true;

        void OnChanging(int unused) => wasOnChangingCalled = true;
    }

    /// <summary>
    /// Ensures that defer subscription with an initial function value does not trigger the OnChanged callback
    /// when the source observable provides the same initial value.
    /// </summary>
    /// <param name="initialValue">The initial value provided to the ObservableAsPropertyHelper.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(42)]
    public async Task OAPHDeferSubscriptionWithInitialFuncValueNotCallOnChangedWhenSourceProvidesInitialValue(int initialValue)
    {
        var observable = new Subject<int>();
        var wasOnChangingCalled = false;
        var wasOnChangedCalled = false;

        var fixture = new ObservableAsPropertyHelper<int>(observable, OnChanged, OnChanging, () => initialValue, true);

        var result = fixture.Value;
        await Assert.That(result).IsEqualTo(initialValue);

        observable.OnNext(initialValue);

        using (Assert.Multiple())
        {
            await Assert.That(wasOnChangingCalled).IsFalse();
            await Assert.That(wasOnChangedCalled).IsFalse();
        }

        return;

        void OnChanged(int unused) => wasOnChangedCalled = true;

        void OnChanging(int unused) => wasOnChangingCalled = true;
    }

    /// <summary>
    /// Verifies that the initial value of an Observable As Property Helper is emitted correctly.
    /// </summary>
    /// <param name="initialValue">The initial value provided to the Observable As Property Helper.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(42)]
    public async Task OaphInitialValueShouldEmitInitialValue(int initialValue)
    {
        var observable = Observable.Empty<int>();
        var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, initialValue, deferSubscription: false);

        await Assert.That(fixture.IsSubscribed).IsTrue();

        int? emittedValue = null;
        fixture.Source.Subscribe(val => emittedValue = val);

        await Assert.That(emittedValue).IsEqualTo(initialValue);
    }

    /// <summary>
    /// OAPH should rethrow errors via ThrownExceptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphShouldRethrowErrors() =>
        await new TestScheduler().With(async scheduler =>
        {
            var input = new Subject<int>();
            var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5, scheduler: scheduler);
            var errors = new List<Exception>();

            await Assert.That(fixture.Value).IsEqualTo(-5);
            new[] { 1, 2, 3, 4 }.Run(x => input.OnNext(x));

            fixture.ThrownExceptions.Subscribe(errors.Add);

            scheduler.Start();
            await Assert.That(fixture.Value).IsEqualTo(4);

            input.OnError(new Exception("Die!"));
            scheduler.Start();

            using (Assert.Multiple())
            {
                await Assert.That(fixture.Value).IsEqualTo(4);
                await Assert.That(errors).Count().IsEqualTo(1);
            }
        });

    /// <summary>
    /// No thrown-exceptions subscriber equals OAPH death.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NoThrownExceptionsSubscriberEqualsOaphDeath() =>
        await new TestScheduler().With(async scheduler =>
        {
            var input = new Subject<int>();
            var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5, scheduler: ImmediateScheduler.Instance);

            await Assert.That(fixture.Value).IsEqualTo(-5);
            new[] { 1, 2, 3, 4 }.Run(x => input.OnNext(x));

            input.OnError(new Exception("Die!"));

            var failed = true;
            try
            {
                scheduler.Start();
            }
            catch (Exception ex)
            {
                failed = ex.InnerException?.Message != "Die!";
            }

            using (Assert.Multiple())
            {
                await Assert.That(failed).IsFalse();
                await Assert.That(fixture.Value).IsEqualTo(4);
            }
        });

    /// <summary>
    /// ToProperty should fire both Changing and Changed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToPropertyShouldFireBothChangingAndChanged()
    {
        var fixture = new OaphTestFixture();

        // NB: Hack to connect up the OAPH
        _ = (fixture.FirstThreeLettersOfOneWord ?? string.Empty).Substring(0, 0);

        fixture.ObservableForProperty(static x => x.FirstThreeLettersOfOneWord, beforeChange: true)
               .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var resultChanging).Subscribe();
        fixture.ObservableForProperty(static x => x.FirstThreeLettersOfOneWord, beforeChange: false)
               .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var resultChanged).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(resultChanging).IsEmpty();
            await Assert.That(resultChanged).IsEmpty();
        }

        fixture.IsOnlyOneWord = "FooBar";
        using (Assert.Multiple())
        {
            await Assert.That(resultChanging).Count().IsEqualTo(1);
            await Assert.That(resultChanged).Count().IsEqualTo(1);
            await Assert.That(resultChanging[0].Value).IsEqualTo(string.Empty);
            await Assert.That(resultChanged[0].Value).IsEqualTo("Foo");
        }

        fixture.IsOnlyOneWord = "Bazz";
        using (Assert.Multiple())
        {
            await Assert.That(resultChanging).Count().IsEqualTo(2);
            await Assert.That(resultChanged).Count().IsEqualTo(2);
            await Assert.That(resultChanging[1].Value).IsEqualTo("Foo");
            await Assert.That(resultChanged[1].Value).IsEqualTo("Baz");
        }
    }

    /// <summary>
    /// ToProperty(nameof) should raise standard notifications.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_NameOf_ShouldFireBothChangingAndChanged()
    {
        var fixture = new OaphNameOfTestFixture();

        var changing = false;
        var changed = false;

        fixture.PropertyChanging += (_, _) => changing = true;
        fixture.PropertyChanged += (_, _) => changed = true;

        using (Assert.Multiple())
        {
            await Assert.That(changing).IsFalse();
            await Assert.That(changed).IsFalse();
        }

        fixture.IsOnlyOneWord = "baz";

        using (Assert.Multiple())
        {
            await Assert.That(changing).IsTrue();
            await Assert.That(changed).IsTrue();
        }
    }

    /// <summary>
    /// Ensures that the ToProperty method, when used with nameof, produces valid output values
    /// for derived properties by comparing test data against expected values.
    /// </summary>
    /// <param name="testWords">An array of input strings to evaluate.</param>
    /// <param name="first3Letters">An array of expected first three letters for each input string in <paramref name="testWords"/>.</param>
    /// <param name="last3Letters">An array of expected last three letters for each input string in <paramref name="testWords"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(
                 new[] { "FooBar", "Bazz" },
                 new[] { "Foo", "Baz" },
                 new[] { "Bar", "azz" })]
    public async Task ToProperty_NameOf_ValidValuesProduced(string[] testWords, string[] first3Letters, string[] last3Letters)
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

        using (Assert.Multiple())
        {
            await Assert.That(changed.All(x => x.Count == 0)).IsTrue();
            await Assert.That(changing.All(x => x.Count == 0)).IsTrue();
        }

        for (var i = 0; i < testWords.Length; ++i)
        {
            fixture.IsOnlyOneWord = testWords[i];

            using (Assert.Multiple())
            {
                await Assert.That(changed.All(x => x.Count == i + 1)).IsTrue();
                await Assert.That(changing.All(x => x.Count == i + 1)).IsTrue();
                await Assert.That(firstThreeChanged[i].Value).IsEqualTo(first3Letters[i]);
                await Assert.That(lastThreeChanged[i].Value).IsEqualTo(last3Letters[i]);
            }

            var firstChanging = i < 1 ? string.Empty : first3Letters[i - 1];
            var lastChanging = i < 1 ? string.Empty : last3Letters[i - 1]; // fixed from i - i

            using (Assert.Multiple())
            {
                await Assert.That(firstThreeChanging[i].Value).IsEqualTo(firstChanging);
                await Assert.That(lastThreeChanging[i].Value).IsEqualTo(lastChanging);
            }
        }
    }

    /// <summary>
    /// ToProperty with indexer notifies expected property name.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName()
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

        await Assert.That(propertiesChanged).IsEquivalentTo(["Text", "Item[]"]);
    }

    /// <summary>
    /// Indexer: not supported path throws NotSupportedException.
    /// </summary>
    [Test]
    public void ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName1()
    {
        var propertiesChanged = new List<string>();

        new TestScheduler().With(scheduler =>
                                     Assert.Throws<NotSupportedException>(() =>
                                     {
                                         var fixture = new OAPHIndexerTestFixture(
                                          1,
                                          scheduler);

                                         fixture.PropertyChanged += (_, args) =>
                                         {
                                             if (args.PropertyName is not null)
                                             {
                                                 propertiesChanged.Add(args.PropertyName);
                                             }
                                         };

                                         fixture.Text = "awesome";
                                     }));
    }

    /// <summary>
    /// Indexer: invalid path throws ArgumentException.
    /// </summary>
    [Test]
    public void ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName2() =>
        new TestScheduler().With(scheduler =>
                                     Assert.Throws<ArgumentException>(() => _ = new OAPHIndexerTestFixture(2, scheduler)));

    /// <summary>
    /// Nullable types test shouldn't need decorators with ToProperty.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableTypesTestShouldNotNeedDecorators2_ToProperty()
    {
        var fixture = new WhenAnyTestFixture();
        fixture.WhenAnyValue(
                             static x => x.ProjectService.ProjectsNullable,
                             static x => x.AccountService.AccountUsersNullable)
               .Where(static tuple => tuple.Item1.Count > 0 && tuple.Item2.Count > 0)
               .Select(static tuple =>
               {
                   var (_, users) = tuple;
                   return users.Values.Count(static x => !string.IsNullOrWhiteSpace(x?.LastName));
               })
               .ToProperty(fixture, static x => x.AccountsFound, out var helper);

        fixture.AccountsFoundHelper = helper;

        await Assert.That(fixture.AccountsFound).IsEqualTo(3);
    }
}
