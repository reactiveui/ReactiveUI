// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Provides a suite of advanced tests to verify that key ReactiveUI features function correctly under Ahead-of-Time
/// (AOT) compilation scenarios.
/// </summary>
[NotInParallel] // These tests modify global state (e.g., Locator.Current)
public class AdvancedAOTTests
{
    /// <summary>
    /// Tests that routing functionality works in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing AOT-incompatible RoutingState which uses ReactiveCommand")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing AOT-incompatible RoutingState which uses ReactiveCommand")]
    public async Task RoutingState_Navigation_WorksInAOT()
    {
        var routingState = new RoutingState();
        var viewModel = new TestRoutableViewModel();

        // Test navigation
        routingState.Navigate.Execute(viewModel).Subscribe();

        await Assert.That(routingState.NavigationStack).Count().IsEqualTo(1);
        await Assert.That(routingState.NavigationStack[0]).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that property validation works in AOT scenarios.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty constructor that uses RxApp")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty constructor that uses RxApp")]
    public async Task PropertyValidation_WorksInAOT()
    {
        var property = new ReactiveProperty<string>(string.Empty, ImmediateScheduler.Instance, false, false);
        var hasErrors = false;

        property.ObserveValidationErrors()
            .Subscribe(error => hasErrors = !string.IsNullOrEmpty(error));

        _ = property.AddValidationError(x => string.IsNullOrEmpty(x) ? "Required" : null);
        property.Value = string.Empty;

        await Assert.That(hasErrors).IsTrue();
    }

    /// <summary>
    /// Tests that view model activation works in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty constructor that uses RxApp")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty constructor that uses RxApp")]
    public async Task ViewModelActivation_WorksInAOT()
    {
        var viewModel = new TestActivatableViewModel();
        var activated = false;
        var deactivated = false;

        viewModel.WhenActivated(disposables =>
        {
            activated = true;
            Disposable.Create(() => deactivated = true).DisposeWith(disposables);
        });

        viewModel.Activator.Activate();
        await Assert.That(activated).IsTrue();

        viewModel.Activator.Deactivate();
        await Assert.That(deactivated).IsTrue();
    }

    /// <summary>
    /// Tests that observable property helpers work correctly in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty which requires AOT suppression")]
    public async Task ObservableAsPropertyHelper_Lifecycle_WorksInAOT()
    {
        var testObject = new TestReactiveObject();
        var source = new BehaviorSubject<string>("initial");

        var helper = source.ToProperty(testObject, nameof(TestReactiveObject.ComputedProperty));

        await Assert.That(helper.Value).IsEqualTo("initial");

        source.OnNext("updated");
        await Assert.That(helper.Value).IsEqualTo("updated");

        source.OnCompleted();
        helper.Dispose();
    }

    /// <summary>
    /// Tests that dependency resolution works in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DependencyResolution_BasicOperations_WorkInAOT()
    {
        var resolver = Locator.CurrentMutable;

        // Test basic registration and resolution
        resolver.RegisterConstant("test value");
        var resolved = Locator.Current.GetService<string>();

        await Assert.That(resolved).IsEqualTo("test value");
    }

    /// <summary>
    /// Tests that message bus functionality works in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MessageBus_Operations_WorkInAOT()
    {
        var messageBus = new MessageBus();
        var received = false;
        const string testMessage = "test message";

        messageBus.Listen<string>().Subscribe(msg =>
        {
            received = msg == testMessage;
        });

        messageBus.SendMessage(testMessage);

        await Assert.That(received).IsTrue();
    }
}
