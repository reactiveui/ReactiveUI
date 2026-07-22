// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.ObservableAsPropertyHelper.Mocks;
using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.ObservableAsPropertyHelper;

/// <summary>Value/error propagation and scheduling tests for <see cref="ObservableAsPropertyHelper{T}"/>.</summary>
public partial class ObservableAsPropertyHelperTest
{
    /// <summary>OAPH should subscribe immediately to source.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphShouldSubscribeImmediatelyToSource()
    {
        var isSubscribed = false;

        var observable = Signal.Create<int>(o =>
        {
            isSubscribed = true;
            o.OnNext(EmittedValue);
            o.OnCompleted();
            return Scope.Empty;
        });

        var fixture = new ObservableAsPropertyHelper<int>(observable, static _ => { }, 0);

        using (Assert.Multiple())
        {
            await Assert.That(isSubscribed).IsTrue();
            await Assert.That(fixture.Value).IsEqualTo(EmittedValue);
        }
    }

    /// <summary>ToProperty with indexer notifies expected property name.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName()
    {
        var fixture = new OaphIndexerTestFixture(0, Sequencer.Immediate);
        var propertiesChanged = new List<string>();

        fixture.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName is null)
            {
                return;
            }

            propertiesChanged.Add(args.PropertyName);
        };

        fixture.Text = "awesome";

        await Assert.That(propertiesChanged).IsEquivalentTo(["Text", "Item[]"]);
    }

    /// <summary>Indexer: not supported path throws NotSupportedException.</summary>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public void ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName1()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var propertiesChanged = new List<string>();

        _ = Assert.Throws<NotSupportedException>(() =>
        {
            var fixture = new OaphIndexerTestFixture(
                1,
                scheduler);

            fixture.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName is null)
                {
                    return;
                }

                propertiesChanged.Add(args.PropertyName);
            };

            fixture.Text = "awesome";
        });
    }

    /// <summary>Indexer: invalid path throws ArgumentException.</summary>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public void ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName2()
    {
        const int InvalidPathMode = 2;
        var scheduler = TestContext.Current!.GetScheduler();
        _ = Assert.Throws<ArgumentException>(() => _ = new OaphIndexerTestFixture(InvalidPathMode, scheduler));
    }

    /// <summary>ToProperty(nameof) should raise standard notifications.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
    ///     Ensures that the ToProperty method, when used with nameof, produces valid output values
    ///     for derived properties by comparing test data against expected values.
    /// </summary>
    /// <param name="testWords">An array of input strings to evaluate.</param>
    /// <param name="first3Letters">
    ///     An array of expected first three letters for each input string in
    ///     <paramref name="testWords" />.
    /// </param>
    /// <param name="last3Letters">
    ///     An array of expected last three letters for each input string in
    ///     <paramref name="testWords" />.
    /// </param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(
        new[] { "FooBar", "Bazz" },
        new[] { "Foo", "Baz" },
        new[] { "Bar", "azz" })]
    public async Task ToProperty_NameOf_ValidValuesProduced(
        string[] testWords,
        string[] first3Letters,
        string[] last3Letters)
    {
        ArgumentExceptionHelper.ThrowIfNull(testWords);
        ArgumentExceptionHelper.ThrowIfNull(first3Letters);
        ArgumentExceptionHelper.ThrowIfNull(last3Letters);

        var fixture = new OaphNameOfTestFixture();

        var firstThreeChanging = fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, true).Collect();
        var lastThreeChanging = fixture.ObservableForProperty(x => x.LastThreeLettersOfOneWord, true).Collect();
        var changing = new[] { firstThreeChanging!, lastThreeChanging };

        var firstThreeChanged = fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, false).Collect();
        var lastThreeChanged = fixture.ObservableForProperty(x => x.LastThreeLettersOfOneWord, false).Collect();
        var changed = new[] { firstThreeChanged!, lastThreeChanged };

        using (Assert.Multiple())
        {
            await Assert.That(Array.TrueForAll(changed, static x => x.Count == 0)).IsTrue();
            await Assert.That(Array.TrueForAll(changing, static x => x.Count == 0)).IsTrue();
        }

        for (var i = 0; i < testWords.Length; ++i)
        {
            fixture.IsOnlyOneWord = testWords[i];

            using (Assert.Multiple())
            {
                await Assert.That(Array.TrueForAll(changed, x => x.Count == i + 1)).IsTrue();
                await Assert.That(Array.TrueForAll(changing, x => x.Count == i + 1)).IsTrue();
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

    /// <summary>ToProperty should fire both Changing and Changed.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToPropertyShouldFireBothChangingAndChanged()
    {
        var fixture = new OaphTestFixture();

        // NB: Hack to connect up the OAPH
        _ = (fixture.FirstThreeLettersOfOneWord ?? string.Empty).Substring(0, 0);

        var resultChanging = fixture.ObservableForProperty(static x => x.FirstThreeLettersOfOneWord, true).Collect();
        var resultChanged = fixture.ObservableForProperty(static x => x.FirstThreeLettersOfOneWord, false).Collect();

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

        const int ExpectedCountAfterSecondChange = 2;
        fixture.IsOnlyOneWord = "Bazz";
        using (Assert.Multiple())
        {
            await Assert.That(resultChanging).Count().IsEqualTo(ExpectedCountAfterSecondChange);
            await Assert.That(resultChanged).Count().IsEqualTo(ExpectedCountAfterSecondChange);
            await Assert.That(resultChanging[1].Value).IsEqualTo("Foo");
            await Assert.That(resultChanged[1].Value).IsEqualTo("Baz");
        }
    }

    /// <summary>The two-argument constructor seeds the default value.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task TwoArgConstructorUsesDefaultValue()
    {
        using var fixture = new ObservableAsPropertyHelper<int>(Signal.Silent<int>(), static _ => { });
        await Assert.That(fixture.Value).IsEqualTo(0);
    }

    /// <summary>The value-plus-defer-plus-scheduler constructor is usable.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task InitialValueDeferSchedulerConstructor()
    {
        const int seed = 7;
        using var fixture = new ObservableAsPropertyHelper<int>(Signal.Silent<int>(), static _ => { }, seed, false, Sequencer.Immediate);
        await Assert.That(fixture.Value).IsEqualTo(seed);
    }

    /// <summary>The onChanging-only constructor seeds the default value via the default factory.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task OnChangingOnlyConstructorUsesDefaultValue()
    {
        using var fixture = new ObservableAsPropertyHelper<int>(Signal.Silent<int>(), static _ => { }, static _ => { });
        await Assert.That(fixture.Value).IsEqualTo(0);
    }

    /// <summary>The onChanging-plus-initial-value constructor seeds the supplied value.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task OnChangingInitialValueConstructor()
    {
        const int seed = 11;
        using var fixture = new ObservableAsPropertyHelper<int>(Signal.Silent<int>(), static _ => { }, static _ => { }, seed);
        await Assert.That(fixture.Value).IsEqualTo(seed);
    }

    /// <summary>The onChanging-plus-initial-value-plus-defer constructor seeds the supplied value.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task OnChangingInitialValueDeferConstructor()
    {
        const int seed = 13;
        using var fixture = new ObservableAsPropertyHelper<int>(Signal.Silent<int>(), static _ => { }, static _ => { }, seed, false);
        await Assert.That(fixture.Value).IsEqualTo(seed);
    }

    /// <summary>The Func-based initial value constructor evaluates the factory.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task FuncInitialValueConstructor()
    {
        const int seed = 17;
        using var fixture = new ObservableAsPropertyHelper<int>(Signal.Silent<int>(), static _ => { }, static _ => { }, static () => seed, false);
        await Assert.That(fixture.Value).IsEqualTo(seed);
    }

    /// <summary>The default factory methods build usable helpers.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultFactoryMethods()
    {
        const int valueSeed = 3;
        const int schedulerSeed = 5;
        using var withoutValue = ObservableAsPropertyHelper<int>.Default();
        using var withValue = ObservableAsPropertyHelper<int>.Default(valueSeed);
        using var withValueAndScheduler = ObservableAsPropertyHelper<int>.Default(schedulerSeed, Sequencer.Immediate);
        await Assert.That(withoutValue.Value).IsEqualTo(0);
        await Assert.That(withValue.Value).IsEqualTo(valueSeed);
        await Assert.That(withValueAndScheduler.Value).IsEqualTo(schedulerSeed);
    }

    /// <summary>Disposing twice is a no-op.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposeIsIdempotent()
    {
        var fixture = new ObservableAsPropertyHelper<int>(Signal.Silent<int>(), static _ => { }, 1);
        fixture.Dispose();
        fixture.Dispose();
        await Assert.That(fixture.IsSubscribed).IsTrue();
    }

    /// <summary>A deferred source error is routed to a ThrownExceptions subscriber.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrownExceptionsDeliversToSubscriber()
    {
        var error = new InvalidOperationException("boom");

        // Defer subscription so the source (which errors on subscribe) is only touched once we read Value,
        // by which point our ThrownExceptions observer is attached.
        using var fixture = new ObservableAsPropertyHelper<int>(new ErrorObservable<int>(error), static _ => { }, 0, true, Sequencer.Immediate);

        Exception? received = null;
        var subscription = fixture.ThrownExceptions.Subscribe(ex => received = ex);
        _ = fixture.Value;

        await Assert.That(received).IsSameReferenceAs(error);
        subscription.Dispose();
    }

    /// <summary>The constructor overload with an initial-value factory seeds the value and delivers later source values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InitialValueFactoryConstructorSeedsAndDelivers()
    {
        const int Seed = 7;
        var source = new Signal<int>();

        using var fixture = new ObservableAsPropertyHelper<int>(source, static _ => { }, onChanging: null, getInitialValue: static () => Seed);

        await Assert.That(fixture.Value).IsEqualTo(Seed);

        source.OnNext(EmittedValue);
        await Assert.That(fixture.Value).IsEqualTo(EmittedValue);
    }

    /// <summary>A null initial-value factory falls back to <c>default(T)</c> for the seeded value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullInitialValueFactoryFallsBackToDefault()
    {
        var source = new Signal<int>();

        using var fixture = new ObservableAsPropertyHelper<int>(source, static _ => { }, onChanging: null, getInitialValue: null);

        await Assert.That(fixture.Value).IsEqualTo(0);
    }

    /// <summary>When there is no thrown-exceptions subscriber, a source error is routed to the default exception handler.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task SourceErrorWithoutSubscriberGoesToDefaultExceptionHandler()
    {
        RxState.ResetForTesting();
        var handler = new CapturingExceptionObserver();
        RxState.InitializeExceptionHandler(handler);
        try
        {
            var error = new InvalidOperationException("Die!");
            using var fixture = new ObservableAsPropertyHelper<int>(new ErrorObservable<int>(error), static _ => { }, 0, true, Sequencer.Immediate);

            _ = fixture.Value;

            await Assert.That(handler.Captured).IsSameReferenceAs(error);
        }
        finally
        {
            RxState.ResetForTesting();
        }
    }

    /// <summary>A source value arriving after disposal is ignored rather than delivered.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnSourceNextAfterDisposeIsIgnored()
    {
        var source = new Signal<int>();
        var observed = new List<int>();
        var fixture = new ObservableAsPropertyHelper<int>(source, observed.Add, 0, false, Sequencer.Immediate);
        observed.Clear();

        fixture.Dispose();
        fixture.OnSourceNext(EmittedValue);

        await Assert.That(observed).IsEmpty();
    }

    /// <summary>Activating after disposal subscribes then immediately disposes the source subscription.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ActivateOnFirstAccessAfterDisposeDisposesSubscription()
    {
        var source = new TrackingObservable();
        var fixture = new ObservableAsPropertyHelper<int>(source, static _ => { }, 0, true, Sequencer.Immediate);
        fixture.Dispose();

        fixture.ActivateOnFirstAccess();

        await Assert.That(source.LastSubscriptionDisposed).IsTrue();
    }

    /// <summary>An exception observer that records the most recent exception it receives.</summary>
    private sealed class CapturingExceptionObserver : IObserver<Exception>
    {
        /// <summary>Gets the most recently captured exception.</summary>
        public Exception? Captured { get; private set; }

        /// <inheritdoc/>
        public void OnNext(Exception value) => Captured = value;

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }
    }

    /// <summary>An observable that records whether the subscription it handed out has been disposed.</summary>
    private sealed class TrackingObservable : IObservable<int>
    {
        /// <summary>Gets a value indicating whether the most recent subscription was disposed.</summary>
        public bool LastSubscriptionDisposed { get; private set; }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<int> observer) =>
            Scope.Create(this, static self => self.LastSubscriptionDisposed = true);
    }

    /// <summary>An observable that immediately errors any subscriber.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="error">The error to deliver on subscribe.</param>
    private sealed class ErrorObservable<T>(Exception error) : IObservable<T?>
    {
        /// <summary>Errors the observer immediately.</summary>
        /// <param name="observer">The observer.</param>
        /// <returns>An empty disposable.</returns>
        public IDisposable Subscribe(IObserver<T?> observer)
        {
            observer.OnError(error);
            return EmptyDisposable.Instance;
        }
    }
}
