// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.AOT.Tests;

/// <summary>
/// Verifies the trim- and AOT-safe <c>IActivatableView.WhenActivated(block, IObservable&lt;object?&gt;)</c> overloads.
/// These are callable from this <c>PublishAot</c>/<c>TrimMode=full</c> assembly without a
/// <c>[RequiresUnreferencedCode]</c> annotation or any suppression, because the caller supplies the ViewModel-change
/// signal directly instead of the framework discovering it by reflection.
/// </summary>
public class ViewActivationAOTTests
{
    /// <summary>The view overload activates the view block and forwards ViewModel activation using the supplied signal, without reflection.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewWhenActivatedWithViewModelChangedForwardsActivationInAot()
    {
        Locator.CurrentMutable.Register<IActivationForViewFetcher>(static () => new TestActivationForViewFetcher());

        var viewModel = new TestActivatableViewModel();
        var viewModelActivations = 0;
        viewModel.WhenActivated(disposables =>
        {
            viewModelActivations++;
            _ = new ActionDisposable(() => viewModelActivations--).DisposeWith(disposables);
        });

        using var viewModelChanged = new BehaviorSignal<object?>(viewModel);
        var view = new TestActivatableView();
        var viewActivations = 0;
        var viewDeactivations = 0;

        using var activation = view.WhenActivated(
            disposables =>
            {
                viewActivations++;
                _ = new ActionDisposable(() => viewDeactivations++).DisposeWith(disposables);
            },
            viewModelChanged);

        using (Assert.Multiple())
        {
            await Assert.That(viewActivations).IsEqualTo(0);
            await Assert.That(viewModelActivations).IsEqualTo(0);
        }

        view.Loaded.OnNext(RxVoid.Default);
        using (Assert.Multiple())
        {
            await Assert.That(viewActivations).IsEqualTo(1);
            await Assert.That(viewModelActivations).IsEqualTo(1);
        }

        view.Unloaded.OnNext(RxVoid.Default);
        using (Assert.Multiple())
        {
            await Assert.That(viewDeactivations).IsEqualTo(1);
            await Assert.That(viewModelActivations).IsEqualTo(0);
        }
    }

    /// <summary>A minimal activatable view driven by explicit loaded/unloaded signals.</summary>
    private sealed class TestActivatableView : ReactiveObject, IViewFor<TestActivatableViewModel>, ICanActivate
    {
        /// <summary>Gets the signal raised when the view is loaded.</summary>
        public Signal<RxVoid> Loaded { get; } = new();

        /// <summary>Gets the signal raised when the view is unloaded.</summary>
        public Signal<RxVoid> Unloaded { get; } = new();

        /// <inheritdoc/>
        public IObservable<RxVoid> Activated => Loaded;

        /// <inheritdoc/>
        public IObservable<RxVoid> Deactivated => Unloaded;

        /// <inheritdoc/>
        public TestActivatableViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestActivatableViewModel?)value;
        }
    }

    /// <summary>Resolves activation for <see cref="TestActivatableView"/> from its loaded/unloaded signals.</summary>
    private sealed class TestActivationForViewFetcher : IActivationForViewFetcher
    {
        /// <summary>The affinity returned for the recognised view type.</summary>
        private const int ViewAffinity = 100;

        /// <inheritdoc/>
        public int GetAffinityForView(Type view) => view == typeof(TestActivatableView) ? ViewAffinity : 0;

        /// <inheritdoc/>
        public IObservable<bool> GetActivationForView(IActivatableView view) =>
            view is TestActivatableView v
                ? Signal.Blend(v.Loaded.Select(static _ => true), v.Unloaded.Select(static _ => false))
                : throw new ArgumentException("Unexpected view type.", nameof(view));
    }
}
