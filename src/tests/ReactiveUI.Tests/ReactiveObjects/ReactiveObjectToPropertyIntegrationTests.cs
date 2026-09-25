// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.ReactiveObjects;

/// <summary>
/// Exercises ReactiveUI.Binding's generated <c>ToProperty</c> against <see cref="ReactiveObject"/>, which raises
/// its notifications through ReactiveUI's own change-notification state.
/// </summary>
public class ReactiveObjectToPropertyIntegrationTests
{
    /// <summary>The value the source emits after the initial one.</summary>
    private const string UpdatedValue = "updated";

    /// <summary>A selector-named helper raises changing and changed on the reactive object, in that order.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SelectorToProperty_RaisesChangingThenChanged()
    {
        var source = new Signal<string?>();
        var fixture = new ToPropertyViewModel(source, Signal.Silent<string?>());
        var events = new List<string>();
        fixture.PropertyChanging += (_, e) => events.Add($"changing:{e.PropertyName}");
        fixture.PropertyChanged += (_, e) => events.Add($"changed:{e.PropertyName}");

        source.OnNext(UpdatedValue);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.Upper).IsEqualTo(UpdatedValue);
            await Assert.That(events).IsEquivalentTo(
                [
                    $"changing:{nameof(ToPropertyViewModel.Upper)}",
                    $"changed:{nameof(ToPropertyViewModel.Upper)}",
                ]);
        }
    }

    /// <summary>A nameof-named helper reaches the same notifications as a selector.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task NameOfToProperty_RaisesChanged()
    {
        var source = new Signal<string?>();
        var fixture = new ToPropertyViewModel(Signal.Silent<string?>(), source);
        var changed = new List<string?>();
        fixture.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        source.OnNext(UpdatedValue);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.Named).IsEqualTo(UpdatedValue);
            await Assert.That(changed).Contains(nameof(ToPropertyViewModel.Named));
        }
    }

    /// <summary>A helper whose property depends on another reactive property follows it through WhenAnyValue.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WhenAnyValueToProperty_FollowsSourceProperty()
    {
        var fixture = new DerivedViewModel { FirstName = "Ada", LastName = "Lovelace" };

        await Assert.That(fixture.FullName).IsEqualTo("Ada Lovelace");

        fixture.LastName = "Byron";

        await Assert.That(fixture.FullName).IsEqualTo("Ada Byron");
    }

    /// <summary>A deferred helper does not subscribe to its source until its value is first read.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task DeferredToProperty_SubscribesOnFirstRead()
    {
        var subscribed = false;
        var source = Signal.Create<string?>(o =>
        {
            subscribed = true;
            o.OnNext(UpdatedValue);
            return Scope.Empty;
        });

        var fixture = new DeferredViewModel(source);

        await Assert.That(subscribed).IsFalse();
        await Assert.That(fixture.Value).IsEqualTo(UpdatedValue);
        await Assert.That(subscribed).IsTrue();
    }

    /// <summary>A source error reaches the helper's ThrownExceptions observers.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task SourceError_ReachesThrownExceptions()
    {
        var source = new Signal<string?>();
        var fixture = new ToPropertyViewModel(source, Signal.Silent<string?>());
        var errors = new List<Exception>();
        using var subscription = fixture.UpperHelper.ThrownExceptions.Subscribe(errors.Add);

        source.OnError(new InvalidOperationException("boom"));

        await Assert.That(errors).Count().IsEqualTo(1);
    }

    /// <summary>A view model whose helpers are named by selector and by nameof.</summary>
    internal sealed class ToPropertyViewModel : ReactiveObject
    {
        /// <summary>The helper named by selector.</summary>
        private readonly ObservableAsPropertyHelper<string?> _upper;

        /// <summary>The helper named by nameof.</summary>
        private readonly ObservableAsPropertyHelper<string?> _named;

        /// <summary>Initializes a new instance of the <see cref="ToPropertyViewModel"/> class.</summary>
        /// <param name="upperSource">The values the selector-named helper follows.</param>
        /// <param name="namedSource">The values the nameof-named helper follows.</param>
        public ToPropertyViewModel(IObservable<string?> upperSource, IObservable<string?> namedSource)
        {
            _upper = upperSource.ToProperty(this, static x => x.Upper);
            _named = namedSource.ToProperty(this, nameof(Named));
        }

        /// <summary>Gets the value named by selector.</summary>
        public string? Upper => _upper.Value;

        /// <summary>Gets the value named by nameof.</summary>
        public string? Named => _named.Value;

        /// <summary>Gets the selector-named helper.</summary>
        public ObservableAsPropertyHelper<string?> UpperHelper => _upper;
    }

    /// <summary>A view model deriving one property from two others.</summary>
    internal sealed class DerivedViewModel : ReactiveObject
    {
        /// <summary>The derived full name.</summary>
        private readonly ObservableAsPropertyHelper<string> _fullName;

        /// <summary>Initializes a new instance of the <see cref="DerivedViewModel"/> class.</summary>
        public DerivedViewModel() =>
            _fullName = this.WhenAnyValue(static x => x.FirstName, static x => x.LastName, static (first, last) => $"{first} {last}")
                .ToProperty(this, static x => x.FullName);

        /// <summary>Gets or sets the first name.</summary>
        public string? FirstName
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the last name.</summary>
        public string? LastName
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets the full name.</summary>
        public string FullName => _fullName.Value;
    }

    /// <summary>A view model whose helper defers its subscription.</summary>
    internal sealed class DeferredViewModel : ReactiveObject
    {
        /// <summary>The deferred helper.</summary>
        private readonly ObservableAsPropertyHelper<string?> _value;

        /// <summary>Initializes a new instance of the <see cref="DeferredViewModel"/> class.</summary>
        /// <param name="source">The value source.</param>
        public DeferredViewModel(IObservable<string?> source) =>
            _value = source.ToProperty(this, static x => x.Value, deferSubscription: true);

        /// <summary>Gets the value.</summary>
        public string? Value => _value.Value;
    }
}
