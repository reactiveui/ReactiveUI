// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Activation;

#if REACTIVE_SHIM
using ICanActivateContract = ReactiveUI.Reactive.ICanActivate;
#else
using ICanActivateContract = ReactiveUI.ICanActivate;
#endif

/// <summary>Tests for the <see cref="CanActivateViewFetcher" />.</summary>
public class CanActivateViewFetcherTests
{
    /// <summary>Verifies an activate/deactivate/activate cycle emits the expected sequence.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_ActivateDeactivateCycle_EmitsCorrectSequence()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestCanActivateView();
        var results = new List<bool>();

        var activation = fetcher.GetActivationForView(view).ObserveOn(Sequencer.Immediate);
        using var subscription = activation.Subscribe(results.Add);

        const int ExpectedCount = 3;
        const int ThirdIndex = 2;
        view.Activate();
        view.Deactivate();
        view.Activate();

        await Assert.That(results.Count).IsEqualTo(ExpectedCount);
        await Assert.That(results[0]).IsTrue();
        await Assert.That(results[1]).IsFalse();
        await Assert.That(results[ThirdIndex]).IsTrue();
    }

    /// <summary>Verifies repeated activations each emit a value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_MultipleActivations_EmitsEachTime()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestCanActivateView();
        var results = new List<bool>();

        var activation = fetcher.GetActivationForView(view).ObserveOn(Sequencer.Immediate);
        using var subscription = activation.Subscribe(results.Add);

        const int ExpectedCount = 3;
        view.Activate();
        view.Activate();
        view.Activate();

        await Assert.That(results.Count).IsEqualTo(ExpectedCount);
        await Assert.That(results).IsEquivalentTo([true, true, true]);
    }

    /// <summary>Verifies repeated deactivations each emit a value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_MultipleDeactivations_EmitsEachTime()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestCanActivateView();
        var results = new List<bool>();

        var activation = fetcher.GetActivationForView(view).ObserveOn(Sequencer.Immediate);
        using var subscription = activation.Subscribe(results.Add);

        const int ExpectedCount = 3;
        view.Deactivate();
        view.Deactivate();
        view.Deactivate();

        await Assert.That(results.Count).IsEqualTo(ExpectedCount);
        await Assert.That(results).IsEquivalentTo([false, false, false]);
    }

    /// <summary>Verifies an activated view emits true.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_WithActivatedView_ReturnsTrue()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestCanActivateView();
        bool? result = null;

        var activation = fetcher.GetActivationForView(view).ObserveOn(Sequencer.Immediate);
        using var subscription = activation.Subscribe(x => result = x);

        view.Activate();

        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies a deactivated view emits false.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_WithDeactivatedView_ReturnsFalse()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestCanActivateView();
        bool? result = null;

        var activation = fetcher.GetActivationForView(view).ObserveOn(Sequencer.Immediate);
        using var subscription = activation.Subscribe(x => result = x);

        view.Deactivate();

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies a view that does not implement <see cref="ICanActivate" /> emits false.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_WithNonICanActivate_ReturnsFalse()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestNonActivatableView();
        bool? result = null;

        var activation = fetcher.GetActivationForView(view).ObserveOn(Sequencer.Immediate);
        using var subscription = activation.Subscribe(x => result = x);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies the affinity is 10 for the <see cref="ICanActivate" /> type.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_WithICanActivate_Returns10()
    {
        var fetcher = new CanActivateViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(ICanActivate));
        await Assert.That(affinity).IsEqualTo(BindingAffinity.ExactType);
    }

    /// <summary>Verifies the affinity is 10 for a type derived from <see cref="ICanActivate" />.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_WithICanActivateDerivative_Returns10()
    {
        var fetcher = new CanActivateViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(TestCanActivateView));
        await Assert.That(affinity).IsEqualTo(BindingAffinity.ExactType);
    }

    /// <summary>Verifies the affinity is 0 for a non-activatable view.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_WithNonActivatableView_Returns0()
    {
        var fetcher = new CanActivateViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(TestNonActivatableView));
        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>Verifies the affinity is 0 for a type that does not implement <see cref="ICanActivate" />.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_WithNonICanActivate_Returns0()
    {
        var fetcher = new CanActivateViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(string));
        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>A test view that implements <see cref="ICanActivate" /> and can be activated and deactivated on demand.</summary>
    private sealed class TestCanActivateView : ReactiveObject, IViewFor<TestViewModel>, ICanActivateContract
    {
        /// <summary>The subject used to signal that the view has been activated.</summary>
        private readonly Signal<RxVoid> _activated = new();

        /// <summary>The subject used to signal that the view has been deactivated.</summary>
        private readonly Signal<RxVoid> _deactivated = new();

        /// <summary>Gets an observable that signals when the view is activated.</summary>
        public IObservable<RxVoid> Activated => _activated;

        /// <summary>Gets an observable that signals when the view is deactivated.</summary>
        public IObservable<RxVoid> Deactivated => _deactivated;

        /// <summary>Gets or sets the view model.</summary>
        public TestViewModel? ViewModel
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <inheritdoc />
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }

        /// <summary>Signals that the view has been activated.</summary>
        public void Activate() => _activated.OnNext(RxVoid.Default);

        /// <summary>Signals that the view has been deactivated.</summary>
        public void Deactivate() => _deactivated.OnNext(RxVoid.Default);
    }

    /// <summary>A test view that does not implement <see cref="ICanActivate" />.</summary>
    private sealed class TestNonActivatableView : ReactiveObject, IViewFor<TestViewModel>
    {
        /// <summary>Gets or sets the view model.</summary>
        public TestViewModel? ViewModel
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <inheritdoc />
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>A simple view model used by the test views.</summary>
    [SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class TestViewModel : ReactiveObject;
}
