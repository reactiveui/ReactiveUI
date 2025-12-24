// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Provides a suite of tests that verify AOT-compatible patterns and usage scenarios for reactive programming
/// components. These tests demonstrate approaches for property binding, command execution, message bus communication,
/// dependency injection, and view model activation that work reliably in ahead-of-time (AOT) compilation environments.
/// </summary>
public class ComprehensiveAOTTests
{
    /// <summary>
    /// Tests that demonstrate AOT-compatible patterns that work well in AOT scenarios.
    /// These tests use string-based property names and explicit schedulers.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    public async Task AOTCompatiblePatterns_WorkCorrectly()
    {
        // Use string-based property names for ToProperty (AOT-compatible)
        var obj = new TestReactiveObject();
        var helper = Observable.Return("test")
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        await Assert.That(helper.Value).IsEqualTo("test");

        // Direct property observation works well in AOT
        var changes = new List<string?>();
        obj.PropertyChanged += (s, e) => changes.Add(e.PropertyName);

        obj.TestProperty = "test1";
        obj.TestProperty = "test2";

        await Assert.That(changes).Contains(nameof(TestReactiveObject.TestProperty));
        await Assert.That(changes.Count(x => x == nameof(TestReactiveObject.TestProperty))).IsEqualTo(2);
    }

    /// <summary>
    /// Tests that interaction patterns work well in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Interactions_WorkInAOT()
    {
        var interaction = new Interaction<string, bool>();
        var result = false;

        interaction.RegisterHandler(static context =>
        {
            context.SetOutput(context.Input == "test");
        });

        result = interaction.Handle("test").Wait();
        await Assert.That(result).IsTrue();

        result = interaction.Handle("fail").Wait();
        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Tests that message bus functionality works in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MessageBus_WorksInAOT()
    {
        var messageBus = new MessageBus();
        var receivedMessages = new List<string>();

        messageBus.Listen<string>().Subscribe(msg => receivedMessages.Add(msg));

        messageBus.SendMessage("Message1");
        messageBus.SendMessage("Message2");

        await Assert.That(receivedMessages).Count().IsEqualTo(2);
        await Assert.That(receivedMessages).Contains("Message1");
        await Assert.That(receivedMessages).Contains("Message2");
    }

    /// <summary>
    /// Tests that demonstrate patterns that require AOT warnings to be suppressed.
    /// These show how to properly use ReactiveCommand in AOT scenarios.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Demonstrating proper suppression of AOT warnings for ReactiveCommand")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Demonstrating proper suppression of AOT warnings for ReactiveCommand")]
    public async Task ReactiveCommand_WithProperSuppression_WorksInAOT()
    {
        var executed = false;
        var command = ReactiveCommand.Create(() => executed = true);

        var canExecute = true;
        command.CanExecute.Subscribe(canExec => canExecute = canExec);

        await Assert.That(canExecute).IsTrue();

        command.Execute().Subscribe();
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Tests that demonstrate proper usage of ReactiveProperty with explicit scheduler.
    /// This avoids AOT warnings by providing an explicit scheduler instead of relying on RxApp.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty with explicit scheduler which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty with explicit scheduler which requires AOT suppression")]
    public async Task ReactiveProperty_WithExplicitScheduler_WorksInAOT()
    {
        // Use explicit scheduler to avoid RxApp dependency (AOT-friendly)
        var scheduler = CurrentThreadScheduler.Instance;
        var property = new ReactiveProperty<string>("initial", scheduler, false, false);

        var values = new List<string>();
        property.Subscribe(value => values.Add(value ?? string.Empty));

        property.Value = "changed";

        await Assert.That(values).Contains("initial");
        using (Assert.Multiple())
        {
            await Assert.That(values).Contains("changed");
            await Assert.That(property.Value).IsEqualTo("changed");
        }
    }

    /// <summary>
    /// Tests that ObservableAsPropertyHelper works correctly with string-based binding.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    public async Task ObservableAsPropertyHelper_StringBased_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var scheduler = CurrentThreadScheduler.Instance;

        // String-based property binding is AOT-compatible
        var source = new BehaviorSubject<string>("initial");
        var helper = source
            .ObserveOn(scheduler)
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        await Assert.That(helper.Value).IsEqualTo("initial");

        source.OnNext("updated");
        await Assert.That(helper.Value).IsEqualTo("updated");

        helper.Dispose();
        source.Dispose();
    }

    /// <summary>
    /// Tests that demonstrate how to use dependency injection in AOT scenarios.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DependencyInjection_BasicUsage_WorksInAOT()
    {
        Splat.Builder.AppBuilder.ResetBuilderStateForTests();

        // Basic DI operations that work in AOT
        var resolver = Locator.CurrentMutable;

        // Register concrete types (AOT-friendly)
        resolver.Register<IScheduler>(static () => CurrentThreadScheduler.Instance);

        // Resolve registered types
        var scheduler = Locator.Current.GetService<IScheduler>();

        await Assert.That(scheduler).IsNotNull();
        await Assert.That(scheduler).IsTypeOf<CurrentThreadScheduler>();
    }

    /// <summary>
    /// Tests demonstrating view model activation patterns in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty constructor that uses RxApp")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty constructor that uses RxApp")]
    public async Task ViewModelActivation_PatternWorks_InAOT()
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
        using (Assert.Multiple())
        {
            await Assert.That(activationCount).IsEqualTo(1);
            await Assert.That(deactivationCount).IsEqualTo(0);
        }

        viewModel.Activator.Deactivate();
        using (Assert.Multiple())
        {
            await Assert.That(activationCount).IsEqualTo(1);
            await Assert.That(deactivationCount).IsEqualTo(1);
        }

        // Test multiple activation cycles
        viewModel.Activator.Activate();
        viewModel.Activator.Deactivate();

        using (Assert.Multiple())
        {
            await Assert.That(activationCount).IsEqualTo(2);
            await Assert.That(deactivationCount).IsEqualTo(2);
        }
    }
}
