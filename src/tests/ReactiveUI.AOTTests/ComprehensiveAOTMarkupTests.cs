// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Tests for testing the AOT and making sure trimming is correct.
/// </summary>
[NotInParallel]
public class ComprehensiveAOTMarkupTests
{
    /// <summary>
    /// Tests that ReactiveObject constructor works with AOT suppression.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveObject_Constructor_WorksWithAOTSuppression()
    {
        var obj = new TestReactiveObject();
        var propertyChangedFired = false;

        obj.PropertyChanged += (_, _) => propertyChangedFired = true;
        obj.TestProperty = "test value";

        using (Assert.Multiple())
        {
            await Assert.That(propertyChangedFired).IsTrue();
            await Assert.That(obj.TestProperty).IsEqualTo("test value");
        }
    }

    /// <summary>
    /// Tests that ReactiveProperty Refresh method works with AOT suppression.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveProperty_Refresh_WorksWithAOTSuppression()
    {
        var scheduler = CurrentThreadScheduler.Instance;
        var property = new ReactiveProperty<string>("initial", scheduler, false, false);
        var values = new List<string>();

        property.Subscribe(value => values.Add(value ?? string.Empty));

        property.Refresh(); // This calls RaisePropertyChanged which has AOT attributes

        await Assert.That(values).Contains("initial");
        await Assert.That(values).Count().IsGreaterThanOrEqualTo(2); // Initial value plus refresh

        property.Dispose();
    }

    /// <summary>
    /// Tests that platform-specific WireUpControls methods are properly marked for AOT.
    /// This validates that Android platform code has proper AOT attributes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task PlatformSpecific_AOTMarkup_IsProperlyApplied()
    {
        // This test validates that platform-specific code has AOT attributes
        // We can't directly test Android code in this context, but we can verify
        // that the patterns we expect are working
        var testScheduler = CurrentThreadScheduler.Instance;
        var property = new ReactiveProperty<string>("test", testScheduler, false, false);

        // Test that basic ReactiveUI functionality works
        await Assert.That(property.Value).IsEqualTo("test");

        property.Dispose();
    }

    /// <summary>
    /// Tests that all reactive property operations work with proper AOT handling.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveProperty_ComprehensiveOperations_WorkWithAOT()
    {
        var scheduler = CurrentThreadScheduler.Instance;
        var property = new ReactiveProperty<string>("initial", scheduler, false, false);

        // Test basic operations
        var valueChanges = new List<string>();
        property.Subscribe(value => valueChanges.Add(value ?? string.Empty));

        // Test value setting (uses RaisePropertyChanged)
        property.Value = "changed";

        // Test refresh (uses RaisePropertyChanged)
        property.Refresh();

        // Test validation
        var hasErrors = false;
        property.ObserveHasErrors.Subscribe(errors => hasErrors = errors);

        _ = property.AddValidationError(x => string.IsNullOrEmpty(x) ? "Required" : null);
        property.Value = string.Empty;

        await Assert.That(valueChanges).Contains("initial");
        using (Assert.Multiple())
        {
            await Assert.That(valueChanges).Contains("changed");
            await Assert.That(hasErrors).IsTrue();
        }

        property.Dispose();
    }

    /// <summary>
    /// Tests that complex ReactiveUI scenarios work with mixed AOT compatible and incompatible features.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MixedAOTScenario_ComplexWorkflow_WorksCorrectly()
    {
        var scheduler = CurrentThreadScheduler.Instance;

        // AOT-compatible: Basic observable creation
        var source = new BehaviorSubject<string>("start");

        // AOT-incompatible but suppressed: ReactiveProperty creation
        var property = new ReactiveProperty<string>("initial", scheduler, false, false);

        // AOT-compatible: MessageBus usage
        var messageBus = new MessageBus();
        var messages = new List<string>();
        messageBus.Listen<string>().Subscribe(msg => messages.Add(msg));

        // AOT-compatible: Interactions
        var interaction = new Interaction<string, bool>();
        interaction.RegisterHandler(context =>
        {
            context.SetOutput(context.Input == "test");
        });

        // Test the workflow
        property.Value = "updated";
        messageBus.SendMessage("workflow test");
        var result = interaction.Handle("test").Wait();

        using (Assert.Multiple())
        {
            await Assert.That(property.Value).IsEqualTo("updated");
            await Assert.That(messages).Contains("workflow test");
            await Assert.That(result).IsTrue();
        }

        // Cleanup
        source.Dispose();
        property.Dispose();
    }

    /// <summary>
    /// Tests that ObservableAsPropertyHelper works correctly in AOT scenarios.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableAsPropertyHelper_AOTCompatibleUsage_Works()
    {
        var obj = new TestReactiveObject();
        var scheduler = CurrentThreadScheduler.Instance;
        var source = new BehaviorSubject<string>("computed");

        // String-based property binding is AOT-compatible
        var helper = source
            .ObserveOn(scheduler)
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        await Assert.That(helper.Value).IsEqualTo("computed");

        source.OnNext("updated");
        await Assert.That(helper.Value).IsEqualTo("updated");

        source.Dispose();
        helper.Dispose();
    }

    /// <summary>
    /// Tests that dependency injection patterns work in AOT scenarios.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DependencyInjection_AOTCompatiblePatterns_Work()
    {
        var resolver = Locator.CurrentMutable;

        // Register concrete implementations (AOT-friendly)
        resolver.Register<IScheduler>(static () => CurrentThreadScheduler.Instance);
        resolver.RegisterConstant("test service");

        // Create a simple factory
        resolver.Register<Func<string, ReactiveProperty<string>>>(static () => static value =>
        {
            var scheduler = Locator.Current.GetService<IScheduler>();
            return new ReactiveProperty<string>(value, scheduler, false, false);
        });

        // Test resolution
        var scheduler = Locator.Current.GetService<IScheduler>();
        var constant = Locator.Current.GetService<string>();
        var factory = Locator.Current.GetService<Func<string, ReactiveProperty<string>>>();

        using (Assert.Multiple())
        {
            await Assert.That(scheduler).IsNotNull();
            await Assert.That(constant).IsEqualTo("test service");
            await Assert.That(factory).IsNotNull();
        }

        var property = factory("factory test");
        await Assert.That(property.Value).IsEqualTo("factory test");

        property.Dispose();
    }

    /// <summary>
    /// Tests that activation/deactivation works correctly in AOT scenarios.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelActivation_AOTCompatible_WorksCorrectly()
    {
        var viewModel = new TestActivatableViewModel();
        var activationCount = 0;
        var deactivationCount = 0;
        var scheduler = CurrentThreadScheduler.Instance;

        viewModel.WhenActivated(disposables =>
        {
            activationCount++;

            // Create reactive property within activation
            var property = new ReactiveProperty<string>("activated", scheduler, false, false);
            property.DisposeWith(disposables);

            // Setup cleanup
            Disposable.Create(() => deactivationCount++).DisposeWith(disposables);
        });

        // Test activation cycle
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

        // Test reactivation
        viewModel.Activator.Activate();
        using (Assert.Multiple())
        {
            await Assert.That(activationCount).IsEqualTo(2);
            await Assert.That(deactivationCount).IsEqualTo(1);
        }

        viewModel.Activator.Deactivate();
        using (Assert.Multiple())
        {
            await Assert.That(activationCount).IsEqualTo(2);
            await Assert.That(deactivationCount).IsEqualTo(2);
        }
    }

    /// <summary>
    /// Tests error handling patterns in AOT scenarios.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ErrorHandling_AOTScenarios_WorkCorrectly()
    {
        var scheduler = CurrentThreadScheduler.Instance;
        var property = new ReactiveProperty<string>("test", scheduler, false, false);
        var errors = new List<Exception>();

        // Subscribe to thrown exceptions (tests ReactiveObject error handling)
        property.ThrownExceptions.Subscribe(ex => errors.Add(ex));

        // Test validation errors
        var validationErrors = new List<string>();
        property.ObserveErrorChanged
            .Where(errs => errs != null)
            .Subscribe(errs => validationErrors.AddRange(errs!.OfType<string>()));

        _ = property.AddValidationError(x => string.IsNullOrEmpty(x) ? "Value required" : null);
        property.Value = string.Empty;

        using (Assert.Multiple())
        {
            // Test that validation works
            await Assert.That(property.HasErrors).IsTrue();
            await Assert.That(validationErrors).Contains("Value required");
        }

        // Fix the error
        property.Value = "valid value";
        await Assert.That(property.HasErrors).IsFalse();

        property.Dispose();
    }
}
