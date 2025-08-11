// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Splat;
using Xunit;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Comprehensive AOT compatibility tests for ReactiveUI that demonstrate
/// proper AOT attribute usage and provide guidance for developers.
/// </summary>
public class ComprehensiveAOTTests
{
    /// <summary>
    /// Tests that demonstrate AOT-compatible patterns that work well in AOT scenarios.
    /// These tests use string-based property names and explicit schedulers.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    public void AOTCompatiblePatterns_WorkCorrectly()
    {
        // Use string-based property names for ToProperty (AOT-compatible)
        var obj = new TestReactiveObject();
        var helper = Observable.Return("test")
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        Assert.Equal("test", helper.Value);

        // Direct property observation works well in AOT
        var changes = new List<string?>();
        obj.PropertyChanged += (s, e) => changes.Add(e.PropertyName);

        obj.TestProperty = "test1";
        obj.TestProperty = "test2";

        Assert.Contains(nameof(TestReactiveObject.TestProperty), changes);
        Assert.Equal(2, changes.Count(x => x == nameof(TestReactiveObject.TestProperty)));
    }

    /// <summary>
    /// Tests that interaction patterns work well in AOT.
    /// </summary>
    [Fact]
    public void Interactions_WorkInAOT()
    {
        var interaction = new Interaction<string, bool>();
        var result = false;

        interaction.RegisterHandler(context =>
        {
            context.SetOutput(context.Input == "test");
        });

        result = interaction.Handle("test").Wait();
        Assert.True(result);

        result = interaction.Handle("fail").Wait();
        Assert.False(result);
    }

    /// <summary>
    /// Tests that message bus functionality works in AOT.
    /// </summary>
    [Fact]
    public void MessageBus_WorksInAOT()
    {
        var messageBus = new MessageBus();
        var receivedMessages = new List<string>();

        messageBus.Listen<string>().Subscribe(msg => receivedMessages.Add(msg));

        messageBus.SendMessage("Message1");
        messageBus.SendMessage("Message2");

        Assert.Equal(2, receivedMessages.Count);
        Assert.Contains("Message1", receivedMessages);
        Assert.Contains("Message2", receivedMessages);
    }

    /// <summary>
    /// Tests that demonstrate patterns that require AOT warnings to be suppressed.
    /// These show how to properly use ReactiveCommand in AOT scenarios.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Demonstrating proper suppression of AOT warnings for ReactiveCommand")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Demonstrating proper suppression of AOT warnings for ReactiveCommand")]
    public void ReactiveCommand_WithProperSuppression_WorksInAOT()
    {
        var executed = false;
        var command = ReactiveCommand.Create(() => executed = true);

        var canExecute = true;
        command.CanExecute.Subscribe(canExec => canExecute = canExec);

        Assert.True(canExecute);

        command.Execute().Subscribe();
        Assert.True(executed);
    }

    /// <summary>
    /// Tests that demonstrate proper usage of ReactiveProperty with explicit scheduler.
    /// This avoids AOT warnings by providing an explicit scheduler instead of relying on RxApp.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty with explicit scheduler which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty with explicit scheduler which requires AOT suppression")]
    public void ReactiveProperty_WithExplicitScheduler_WorksInAOT()
    {
        // Use explicit scheduler to avoid RxApp dependency (AOT-friendly)
        var scheduler = CurrentThreadScheduler.Instance;
        var property = new ReactiveProperty<string>("initial", scheduler, false, false);

        var values = new List<string>();
        property.Subscribe(value => values.Add(value ?? string.Empty));

        property.Value = "changed";

        Assert.Contains("initial", values);
        Assert.Contains("changed", values);
        Assert.Equal("changed", property.Value);
    }

    /// <summary>
    /// Tests that ObservableAsPropertyHelper works correctly with string-based binding.
    /// </summary>
    [Fact]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    public void ObservableAsPropertyHelper_StringBased_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var scheduler = CurrentThreadScheduler.Instance;

        // String-based property binding is AOT-compatible
        var source = new BehaviorSubject<string>("initial");
        var helper = source
            .ObserveOn(scheduler)
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        Assert.Equal("initial", helper.Value);

        source.OnNext("updated");
        Assert.Equal("updated", helper.Value);

        helper.Dispose();
        source.Dispose();
    }

    /// <summary>
    /// Tests that demonstrate how to use dependency injection in AOT scenarios.
    /// </summary>
    [Fact]
    public void DependencyInjection_BasicUsage_WorksInAOT()
    {
        // Basic DI operations that work in AOT
        var resolver = Locator.CurrentMutable;

        // Register concrete types (AOT-friendly)
        resolver.Register<IScheduler>(() => CurrentThreadScheduler.Instance);

        // Resolve registered types
        var scheduler = Locator.Current.GetService<IScheduler>();

        Assert.NotNull(scheduler);
        Assert.IsType<CurrentThreadScheduler>(scheduler);
    }

    /// <summary>
    /// Tests demonstrating view model activation patterns in AOT.
    /// </summary>
    [Fact]
    public void ViewModelActivation_PatternWorks_InAOT()
    {
        var viewModel = new TestActivatableViewModel();
        var activationCount = 0;
        var deactivationCount = 0;

        viewModel.WhenActivated(disposables =>
        {
            activationCount++;

            // Register cleanup action
            Disposable.Create(() => deactivationCount++)
                .DisposeWith(disposables);
        });

        // Test activation/deactivation cycle
        viewModel.Activator.Activate();
        Assert.Equal(1, activationCount);
        Assert.Equal(0, deactivationCount);

        viewModel.Activator.Deactivate();
        Assert.Equal(1, activationCount);
        Assert.Equal(1, deactivationCount);

        // Test multiple activation cycles
        viewModel.Activator.Activate();
        viewModel.Activator.Deactivate();

        Assert.Equal(2, activationCount);
        Assert.Equal(2, deactivationCount);
    }
}
