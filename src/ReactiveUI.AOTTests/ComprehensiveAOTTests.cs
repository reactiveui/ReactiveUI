// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Splat;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Comprehensive AOT compatibility tests for ReactiveUI that demonstrate
/// proper AOT attribute usage and provide guidance for developers.
/// </summary>
[TestFixture]
public class ComprehensiveAOTTests
{
    /// <summary>
    /// Tests that demonstrate AOT-compatible patterns that work well in AOT scenarios.
    /// These tests use string-based property names and explicit schedulers.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    public void AOTCompatiblePatterns_WorkCorrectly()
    {
        // Use string-based property names for ToProperty (AOT-compatible)
        var obj = new TestReactiveObject();
        var helper = Observable.Return("test")
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        Assert.That(helper.Value, Is.EqualTo("test"));

        // Direct property observation works well in AOT
        var changes = new List<string?>();
        obj.PropertyChanged += (s, e) => changes.Add(e.PropertyName);

        obj.TestProperty = "test1";
        obj.TestProperty = "test2";

        Assert.That(changes, Does.Contain(nameof(TestReactiveObject.TestProperty)));
        Assert.That(changes.Count(x => x == nameof(TestReactiveObject.TestProperty)), Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that interaction patterns work well in AOT.
    /// </summary>
    [Test]
    public void Interactions_WorkInAOT()
    {
        var interaction = new Interaction<string, bool>();
        var result = false;

        interaction.RegisterHandler(context =>
        {
            context.SetOutput(context.Input == "test");
        });

        result = interaction.Handle("test").Wait();
        Assert.That(result, Is.True);

        result = interaction.Handle("fail").Wait();
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that message bus functionality works in AOT.
    /// </summary>
    [Test]
    public void MessageBus_WorksInAOT()
    {
        var messageBus = new MessageBus();
        var receivedMessages = new List<string>();

        messageBus.Listen<string>().Subscribe(msg => receivedMessages.Add(msg));

        messageBus.SendMessage("Message1");
        messageBus.SendMessage("Message2");

        Assert.That(receivedMessages, Has.Count.EqualTo(2));
        Assert.That(receivedMessages, Does.Contain("Message1"));
        Assert.That(receivedMessages, Does.Contain("Message2"));
    }

    /// <summary>
    /// Tests that demonstrate patterns that require AOT warnings to be suppressed.
    /// These show how to properly use ReactiveCommand in AOT scenarios.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Demonstrating proper suppression of AOT warnings for ReactiveCommand")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Demonstrating proper suppression of AOT warnings for ReactiveCommand")]
    public void ReactiveCommand_WithProperSuppression_WorksInAOT()
    {
        var executed = false;
        var command = ReactiveCommand.Create(() => executed = true);

        var canExecute = true;
        command.CanExecute.Subscribe(canExec => canExecute = canExec);

        Assert.That(canExecute, Is.True);

        command.Execute().Subscribe();
        Assert.That(executed, Is.True);
    }

    /// <summary>
    /// Tests that demonstrate proper usage of ReactiveProperty with explicit scheduler.
    /// This avoids AOT warnings by providing an explicit scheduler instead of relying on RxApp.
    /// </summary>
    [Test]
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

        Assert.That(values, Does.Contain("initial"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(values, Does.Contain("changed"));
            Assert.That(property.Value, Is.EqualTo("changed"));
        }
    }

    /// <summary>
    /// Tests that ObservableAsPropertyHelper works correctly with string-based binding.
    /// </summary>
    [Test]
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

        Assert.That(helper.Value, Is.EqualTo("initial"));

        source.OnNext("updated");
        Assert.That(helper.Value, Is.EqualTo("updated"));

        helper.Dispose();
        source.Dispose();
    }

    /// <summary>
    /// Tests that demonstrate how to use dependency injection in AOT scenarios.
    /// </summary>
    [Test]
    public void DependencyInjection_BasicUsage_WorksInAOT()
    {
        Splat.Builder.AppBuilder.ResetBuilderStateForTests();

        // Basic DI operations that work in AOT
        var resolver = Locator.CurrentMutable;

        // Register concrete types (AOT-friendly)
        resolver.Register<IScheduler>(() => CurrentThreadScheduler.Instance);

        // Resolve registered types
        var scheduler = Locator.Current.GetService<IScheduler>();

        Assert.That(scheduler, Is.Not.Null);
        Assert.That(scheduler, Is.TypeOf<CurrentThreadScheduler>());
    }

    /// <summary>
    /// Tests demonstrating view model activation patterns in AOT.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty constructor that uses RxApp")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty constructor that uses RxApp")]
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(activationCount, Is.EqualTo(1));
            Assert.That(deactivationCount, Is.Zero);
        }

        viewModel.Activator.Deactivate();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(activationCount, Is.EqualTo(1));
            Assert.That(deactivationCount, Is.EqualTo(1));
        }

        // Test multiple activation cycles
        viewModel.Activator.Activate();
        viewModel.Activator.Deactivate();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(activationCount, Is.EqualTo(2));
            Assert.That(deactivationCount, Is.EqualTo(2));
        }
    }
}
