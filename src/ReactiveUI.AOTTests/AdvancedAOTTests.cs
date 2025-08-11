// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Splat;
using Xunit;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Additional AOT compatibility tests for more advanced scenarios.
/// </summary>
public class AdvancedAOTTests
{
    /// <summary>
    /// Tests that routing functionality works in AOT.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing AOT-incompatible RoutingState which uses ReactiveCommand")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing AOT-incompatible RoutingState which uses ReactiveCommand")]
    public void RoutingState_Navigation_WorksInAOT()
    {
        var routingState = new RoutingState();
        var viewModel = new TestRoutableViewModel();

        // Test navigation
        routingState.Navigate.Execute(viewModel).Subscribe();

        Assert.Single(routingState.NavigationStack);
        Assert.Equal(viewModel, routingState.NavigationStack[0]);
    }

    /// <summary>
    /// Tests that property validation works in AOT scenarios.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty constructor that uses RxApp")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty constructor that uses RxApp")]
    public void PropertyValidation_WorksInAOT()
    {
        var property = new ReactiveProperty<string>(string.Empty);
        var hasErrors = false;

        property.ObserveValidationErrors()
            .Subscribe(error => hasErrors = !string.IsNullOrEmpty(error));

        property.AddValidationError(x => string.IsNullOrEmpty(x) ? "Required" : null);
        property.Value = string.Empty;

        Assert.True(hasErrors);
    }

    /// <summary>
    /// Tests that view model activation works in AOT.
    /// </summary>
    [Fact]
    public void ViewModelActivation_WorksInAOT()
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
        Assert.True(activated);

        viewModel.Activator.Deactivate();
        Assert.True(deactivated);
    }

    /// <summary>
    /// Tests that observable property helpers work correctly in AOT.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty which requires AOT suppression")]
    public void ObservableAsPropertyHelper_Lifecycle_WorksInAOT()
    {
        var testObject = new TestReactiveObject();
        var source = new BehaviorSubject<string>("initial");

        var helper = source.ToProperty(testObject, nameof(TestReactiveObject.ComputedProperty));

        Assert.Equal("initial", helper.Value);

        source.OnNext("updated");
        Assert.Equal("updated", helper.Value);

        source.OnCompleted();
        helper.Dispose();
    }

    /// <summary>
    /// Tests that dependency resolution works in AOT.
    /// </summary>
    [Fact]
    public void DependencyResolution_BasicOperations_WorkInAOT()
    {
        var resolver = Locator.CurrentMutable;

        // Test basic registration and resolution
        resolver.RegisterConstant<string>("test value");
        var resolved = Locator.Current.GetService<string>();

        Assert.Equal("test value", resolved);
    }

    /// <summary>
    /// Tests that message bus functionality works in AOT.
    /// </summary>
    [Fact]
    public void MessageBus_Operations_WorkInAOT()
    {
        var messageBus = new MessageBus();
        var received = false;
        var testMessage = "test message";

        messageBus.Listen<string>().Subscribe(msg =>
        {
            received = msg == testMessage;
        });

        messageBus.SendMessage(testMessage);

        Assert.True(received);
    }
}
