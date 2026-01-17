// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Activation;

public class CanActivateViewFetcherTests
{
    [Test]
    public async Task GetActivationForView_ActivateDeactivateCycle_EmitsCorrectSequence()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestCanActivateView();
        var results = new List<bool>();

        var activation = fetcher.GetActivationForView(view).ObserveOn(ImmediateScheduler.Instance);
        using var subscription = activation.Subscribe(results.Add);

        view.Activate();
        view.Deactivate();
        view.Activate();

        await Assert.That(results.Count).IsEqualTo(3);
        await Assert.That(results[0]).IsTrue();
        await Assert.That(results[1]).IsFalse();
        await Assert.That(results[2]).IsTrue();
    }

    [Test]
    public async Task GetActivationForView_MultipleActivations_EmitsEachTime()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestCanActivateView();
        var results = new List<bool>();

        var activation = fetcher.GetActivationForView(view).ObserveOn(ImmediateScheduler.Instance);
        using var subscription = activation.Subscribe(results.Add);

        view.Activate();
        view.Activate();
        view.Activate();

        await Assert.That(results.Count).IsEqualTo(3);
        await Assert.That(results).IsEquivalentTo([true, true, true]);
    }

    [Test]
    public async Task GetActivationForView_MultipleDeactivations_EmitsEachTime()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestCanActivateView();
        var results = new List<bool>();

        var activation = fetcher.GetActivationForView(view).ObserveOn(ImmediateScheduler.Instance);
        using var subscription = activation.Subscribe(results.Add);

        view.Deactivate();
        view.Deactivate();
        view.Deactivate();

        await Assert.That(results.Count).IsEqualTo(3);
        await Assert.That(results).IsEquivalentTo([false, false, false]);
    }

    [Test]
    public async Task GetActivationForView_WithActivatedView_ReturnsTrue()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestCanActivateView();
        bool? result = null;

        var activation = fetcher.GetActivationForView(view).ObserveOn(ImmediateScheduler.Instance);
        using var subscription = activation.Subscribe(x => result = x);

        view.Activate();

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task GetActivationForView_WithDeactivatedView_ReturnsFalse()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestCanActivateView();
        bool? result = null;

        var activation = fetcher.GetActivationForView(view).ObserveOn(ImmediateScheduler.Instance);
        using var subscription = activation.Subscribe(x => result = x);

        view.Deactivate();

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task GetActivationForView_WithNonICanActivate_ReturnsFalse()
    {
        var fetcher = new CanActivateViewFetcher();
        var view = new TestNonActivatableView();
        bool? result = null;

        var activation = fetcher.GetActivationForView(view).ObserveOn(ImmediateScheduler.Instance);
        using var subscription = activation.Subscribe(x => result = x);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task GetAffinityForView_WithICanActivate_Returns10()
    {
        var fetcher = new CanActivateViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(ICanActivate));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForView_WithICanActivateDerivative_Returns10()
    {
        var fetcher = new CanActivateViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(TestCanActivateView));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForView_WithNonActivatableView_Returns0()
    {
        var fetcher = new CanActivateViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(TestNonActivatableView));
        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task GetAffinityForView_WithNonICanActivate_Returns0()
    {
        var fetcher = new CanActivateViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(string));
        await Assert.That(affinity).IsEqualTo(0);
    }

    private class TestCanActivateView : ReactiveObject, IViewFor<TestViewModel>, ICanActivate
    {
        private readonly Subject<Unit> _activated = new();
        private readonly Subject<Unit> _deactivated = new();
        private TestViewModel? _viewModel;

        public IObservable<Unit> Activated => _activated;

        public IObservable<Unit> Deactivated => _deactivated;

        public TestViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }

        public void Activate() => _activated.OnNext(Unit.Default);

        public void Deactivate() => _deactivated.OnNext(Unit.Default);
    }

    private class TestNonActivatableView : ReactiveObject, IViewFor<TestViewModel>
    {
        private TestViewModel? _viewModel;

        public TestViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    private class TestViewModel : ReactiveObject
    {
    }
}
