// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#if REACTIVE_SHIM
using ReactiveUI.Reactive.Builder;
#else
using ReactiveUI.Builder;
#endif
using Splat;
using Splat.Builder;

namespace ReactiveUI.Tests.Activation;

/// <summary>Tests for the activation extension members on <see cref="ViewForMixins"/>.</summary>
public class ViewForMixinsTests
{
    /// <summary>The <c>Func&lt;IEnumerable&lt;IDisposable&gt;&gt;</c> activation overload runs the block on activation and
    /// disposes its returned resources on deactivation.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelWhenActivatedFuncOverloadActivatesAndDeactivates()
    {
        var activations = 0;
        var deactivations = new StrongBox<int>();
        var viewModel = new ActivatableViewModelMock();

        viewModel.WhenActivated(() =>
        {
            activations++;
            return [Scope.Create(deactivations, static d => d.Value++)];
        });

        using (Assert.Multiple())
        {
            await Assert.That(activations).IsEqualTo(0);
            await Assert.That(deactivations.Value).IsEqualTo(0);
        }

        _ = viewModel.Activator.Activate();
        await Assert.That(activations).IsEqualTo(1);

        viewModel.Activator.Deactivate();
        await Assert.That(deactivations.Value).IsEqualTo(1);
    }

    /// <summary>The parameterless <c>WhenActivated()</c> overload activates the view through its registered fetcher.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewWhenActivatedNoArgOverloadActivatesViewModel()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        _ = locator.CreateReactiveUIBuilder()
            .WithCoreServices()
            .WithCustomRegistration(static builder =>
                builder.Register<IActivationForViewFetcher>(static () => new ActivatingViewFetcher())).BuildApp();

        using (locator.WithResolver())
        {
            var viewModel = new ActivatingViewModel();
            var fixture = new ActivatingView { ViewModel = viewModel };

            using var activation = fixture.WhenActivated();

            fixture.Loaded.OnNext(RxVoid.Default);
            await Assert.That(viewModel.IsActiveCount).IsEqualTo(1);

            fixture.Unloaded.OnNext(RxVoid.Default);
            await Assert.That(viewModel.IsActiveCount).IsEqualTo(0);
        }
    }

    /// <summary>The <c>Func&lt;IEnumerable&lt;IDisposable&gt;&gt;</c> single-argument view overload runs the block on activation
    /// and disposes its returned resources on deactivation.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewWhenActivatedFuncOverloadActivatesAndDeactivates()
    {
        using (WithActivatingViewFetcher())
        {
            var view = new ActivatingView();
            var activations = 0;
            var deactivations = new StrongBox<int>();

            using var activation = view.WhenActivated(() =>
            {
                activations++;
                return [Scope.Create(deactivations, static d => d.Value++)];
            });

            view.Loaded.OnNext(RxVoid.Default);
            await Assert.That(activations).IsEqualTo(1);

            view.Unloaded.OnNext(RxVoid.Default);
            await Assert.That(deactivations.Value).IsEqualTo(1);
        }
    }

    /// <summary>The <c>Action&lt;MultipleDisposable&gt;</c> view-model overload runs the block on activation and disposes the
    /// composite on deactivation.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelWhenActivatedMultipleDisposableOverloadActivatesAndDeactivates()
    {
        var activations = 0;
        var deactivations = new StrongBox<int>();
        var viewModel = new ActivatableViewModelMock();

        viewModel.WhenActivated(d =>
        {
            activations++;
            d.Add(Scope.Create(deactivations, static d => d.Value++));
        });

        _ = viewModel.Activator.Activate();
        await Assert.That(activations).IsEqualTo(1);

        viewModel.Activator.Deactivate();
        await Assert.That(deactivations.Value).IsEqualTo(1);
    }

    /// <summary>The trim-safe <c>Func</c> overload activates the view block and forwards <see cref="IActivatableViewModel"/>
    /// activation using the caller-supplied ViewModel-change observable, with no reflection.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewWhenActivatedWithViewModelChangedForwardsViewModelActivation()
    {
        using (WithActivatingViewFetcher())
        {
            var viewModel = new ActivatingViewModel();
            using var viewModelChanged = new BehaviorSignal<object?>(viewModel);
            var view = new ActivatingView();
            var activations = 0;
            var deactivations = new StrongBox<int>();

            using var activation = view.WhenActivated(
                () =>
                {
                    activations++;
                    return [Scope.Create(deactivations, static d => d.Value++)];
                },
                viewModelChanged);

            using (Assert.Multiple())
            {
                await Assert.That(activations).IsEqualTo(0);
                await Assert.That(viewModel.IsActiveCount).IsEqualTo(0);
            }

            view.Loaded.OnNext(RxVoid.Default);
            using (Assert.Multiple())
            {
                await Assert.That(activations).IsEqualTo(1);
                await Assert.That(viewModel.IsActiveCount).IsEqualTo(1);
            }

            view.Unloaded.OnNext(RxVoid.Default);
            using (Assert.Multiple())
            {
                await Assert.That(deactivations.Value).IsEqualTo(1);
                await Assert.That(viewModel.IsActiveCount).IsEqualTo(0);
            }
        }
    }

    /// <summary>The trim-safe <c>Action&lt;Action&lt;IDisposable&gt;&gt;</c> overload activates the view block and forwards
    /// ViewModel activation using the supplied observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewWhenActivatedWithViewModelChangedActionOverloadActivatesView()
    {
        using (WithActivatingViewFetcher())
        {
            var viewModel = new ActivatingViewModel();
            using var viewModelChanged = new BehaviorSignal<object?>(viewModel);
            var view = new ActivatingView();
            var activations = 0;
            var deactivations = new StrongBox<int>();

            using var activation = view.WhenActivated(
                d =>
                {
                    activations++;
                    d(Scope.Create(deactivations, static d => d.Value++));
                },
                viewModelChanged);

            view.Loaded.OnNext(RxVoid.Default);
            using (Assert.Multiple())
            {
                await Assert.That(activations).IsEqualTo(1);
                await Assert.That(viewModel.IsActiveCount).IsEqualTo(1);
            }

            view.Unloaded.OnNext(RxVoid.Default);
            using (Assert.Multiple())
            {
                await Assert.That(deactivations.Value).IsEqualTo(1);
                await Assert.That(viewModel.IsActiveCount).IsEqualTo(0);
            }
        }
    }

    /// <summary>The trim-safe <c>Action&lt;MultipleDisposable&gt;</c> overload activates the view block and forwards ViewModel
    /// activation using the supplied observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewWhenActivatedWithViewModelChangedMultipleDisposableOverloadActivatesView()
    {
        using (WithActivatingViewFetcher())
        {
            var viewModel = new ActivatingViewModel();
            using var viewModelChanged = new BehaviorSignal<object?>(viewModel);
            var view = new ActivatingView();
            var activations = 0;
            var deactivations = new StrongBox<int>();

            using var activation = view.WhenActivated(
                d =>
                {
                    activations++;
                    d.Add(Scope.Create(deactivations, static d => d.Value++));
                },
                viewModelChanged);

            view.Loaded.OnNext(RxVoid.Default);
            using (Assert.Multiple())
            {
                await Assert.That(activations).IsEqualTo(1);
                await Assert.That(viewModel.IsActiveCount).IsEqualTo(1);
            }

            view.Unloaded.OnNext(RxVoid.Default);
            using (Assert.Multiple())
            {
                await Assert.That(deactivations.Value).IsEqualTo(1);
                await Assert.That(viewModel.IsActiveCount).IsEqualTo(0);
            }
        }
    }

    /// <summary>The trim-safe no-view-block overload forwards ViewModel activation using the supplied observable and reacts to
    /// later ViewModel changes while the view is active.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewWhenActivatedWithViewModelChangedNoBlockOverloadForwardsViewModelActivation()
    {
        using (WithActivatingViewFetcher())
        {
            var viewModel = new ActivatingViewModel();
            using var viewModelChanged = new BehaviorSignal<object?>(viewModel);
            var view = new ActivatingView();

            using var activation = view.WhenActivated(viewModelChanged);

            await Assert.That(viewModel.IsActiveCount).IsEqualTo(0);

            view.Loaded.OnNext(RxVoid.Default);
            await Assert.That(viewModel.IsActiveCount).IsEqualTo(1);

            // A non-activatable ViewModel emitted while active deactivates the previous one.
            viewModelChanged.OnNext(null);
            await Assert.That(viewModel.IsActiveCount).IsEqualTo(0);

            // Re-emitting the activatable ViewModel while active re-activates it.
            viewModelChanged.OnNext(viewModel);
            await Assert.That(viewModel.IsActiveCount).IsEqualTo(1);

            view.Unloaded.OnNext(RxVoid.Default);
            await Assert.That(viewModel.IsActiveCount).IsEqualTo(0);
        }
    }

    /// <summary>Activation fetcher resolution skips <see langword="null"/> registered fetchers and uses the highest-affinity one.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenActivatedSkipsNullActivationFetchers()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        _ = locator.CreateReactiveUIBuilder()
            .WithCoreServices()
            .WithCustomRegistration(static builder =>
            {
                builder.Register<IActivationForViewFetcher>(static () => null!);
                builder.Register<IActivationForViewFetcher>(static () => new ActivatingViewFetcher());
            }).BuildApp();

        using (locator.WithResolver())
        {
            var viewModel = new ActivatingViewModel();
            var view = new ActivatingView { ViewModel = viewModel };

            view.Loaded.OnNext(RxVoid.Default);
            await Assert.That(viewModel.IsActiveCount).IsEqualTo(1);

            view.Unloaded.OnNext(RxVoid.Default);
            await Assert.That(viewModel.IsActiveCount).IsEqualTo(0);
        }
    }

    /// <summary>Builds a dependency resolver scope with a registered <see cref="ActivatingViewFetcher"/>.</summary>
    /// <returns>A disposable that restores the previous resolver when disposed.</returns>
    private static IDisposable WithActivatingViewFetcher()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        _ = locator.CreateReactiveUIBuilder()
            .WithCoreServices()
            .WithCustomRegistration(static builder =>
                builder.Register<IActivationForViewFetcher>(static () => new ActivatingViewFetcher())).BuildApp();

        return locator.WithResolver();
    }

    /// <summary>A minimal activatable view model used to drive the activation lifecycle.</summary>
    [SuppressMessage(
        "Usage",
        "SST2315:Type owns a disposable but is not IDisposable",
        Justification = "test fixture; the owned disposable lives for the test-process lifetime and is released at process exit.")]
    private sealed class ActivatableViewModelMock : ReactiveObject, IActivatableViewModel
    {
        /// <inheritdoc/>
        public ViewModelActivator Activator { get; } = new();
    }
}
