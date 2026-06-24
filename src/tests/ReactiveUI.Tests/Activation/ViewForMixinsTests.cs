// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
        var deactivations = 0;
        var viewModel = new ActivatableViewModelMock();

        viewModel.WhenActivated(() =>
        {
            activations++;
            return [Scope.Create(() => deactivations++)];
        });

        using (Assert.Multiple())
        {
            await Assert.That(activations).IsEqualTo(0);
            await Assert.That(deactivations).IsEqualTo(0);
        }

        _ = viewModel.Activator.Activate();
        await Assert.That(activations).IsEqualTo(1);

        viewModel.Activator.Deactivate();
        await Assert.That(deactivations).IsEqualTo(1);
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
            .WithCustomRegistration(builder =>
                builder.Register<IActivationForViewFetcher>(() => new ActivatingViewFetcher())).BuildApp();

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

    /// <summary>A minimal activatable view model used to drive the activation lifecycle.</summary>
    private sealed class ActivatableViewModelMock : ReactiveObject, IActivatableViewModel
    {
        /// <inheritdoc/>
        public ViewModelActivator Activator { get; } = new();
    }
}
