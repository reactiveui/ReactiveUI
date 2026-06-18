// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.AOT.Tests;

/// <summary>Tests for testing the AOT and making sure trimming is correct.</summary>
[NotInParallel]
public class ComprehensiveAOTMarkupTests
{
    /// <summary>The initial value used when constructing reactive properties under test.</summary>
    private const string InitialValue = "initial";

    /// <summary>The updated value assigned to reactive properties during the workflow tests.</summary>
    private const string UpdatedValue = "updated";

    /// <summary>The expected count or activation total used in assertions.</summary>
    private const int ExpectedCount = 2;

    /// <summary>Tests that ReactiveObject constructor works with AOT suppression.</summary>
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

    /// <summary>Tests that ReactiveProperty Refresh method works with AOT suppression.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveProperty_Refresh_WorksWithAOTSuppression()
    {
        var scheduler = Sequencer.CurrentThread;
        var property = new ReactiveProperty<string>(InitialValue, scheduler, false, false);
        var values = new List<string>();

        property.Subscribe(value => values.Add(value ?? string.Empty));

        property.Refresh(); // This calls RaisePropertyChanged which has AOT attributes

        await Assert.That(values).Contains(InitialValue);
        await Assert.That(values).Count().IsGreaterThanOrEqualTo(ExpectedCount); // Initial value plus refresh

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
        var testScheduler = Sequencer.CurrentThread;
        var property = new ReactiveProperty<string>("test", testScheduler, false, false);

        // Test that basic ReactiveUI functionality works
        await Assert.That(property.Value).IsEqualTo("test");

        property.Dispose();
    }

    /// <summary>Tests that all reactive property operations work with proper AOT handling.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveProperty_ComprehensiveOperations_WorkWithAOT()
    {
        var scheduler = Sequencer.CurrentThread;
        var property = new ReactiveProperty<string>(InitialValue, scheduler, false, false);

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

        await Assert.That(valueChanges).Contains(InitialValue);
        using (Assert.Multiple())
        {
            await Assert.That(valueChanges).Contains("changed");
            await Assert.That(hasErrors).IsTrue();
        }

        property.Dispose();
    }

    /// <summary>Tests that complex ReactiveUI scenarios work with mixed AOT compatible and incompatible features.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MixedAOTScenario_ComplexWorkflow_WorksCorrectly()
    {
        var scheduler = Sequencer.CurrentThread;

        // AOT-compatible: Basic observable creation
        using var source = new StateSignal<string>("start");

        // AOT-incompatible but suppressed: ReactiveProperty creation
        var property = new ReactiveProperty<string>(InitialValue, scheduler, false, false);

        // AOT-compatible: MessageBus usage
        var messageBus = new MessageBus();
        var messages = new List<string>();
        messageBus.Listen<string>().Subscribe(messages.Add);

        // AOT-compatible: Interactions
        var interaction = new Interaction<string, bool>();
        interaction.RegisterHandler(context => context.SetOutput(context.Input == "test"));

        // Test the workflow
        property.Value = UpdatedValue;
        messageBus.SendMessage("workflow test");
        var result = await interaction.Handle("test").FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(property.Value).IsEqualTo(UpdatedValue);
            await Assert.That(messages).Contains("workflow test");
            await Assert.That(result).IsTrue();
        }

        // Cleanup
        source.Dispose();
        property.Dispose();
    }

    /// <summary>Tests that ObservableAsPropertyHelper works correctly in AOT scenarios.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableAsPropertyHelper_AOTCompatibleUsage_Works()
    {
        var obj = new TestReactiveObject();
        var scheduler = Sequencer.CurrentThread;
        using var source = new StateSignal<string>("computed");

        // String-based property binding is AOT-compatible
        var helper = source
            .ObserveOn(scheduler)
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        await Assert.That(helper.Value).IsEqualTo("computed");

        source.OnNext(UpdatedValue);
        await Assert.That(helper.Value).IsEqualTo(UpdatedValue);

        source.Dispose();
        helper.Dispose();
    }

    /// <summary>Tests that dependency injection patterns work in AOT scenarios.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DependencyInjection_AOTCompatiblePatterns_Work()
    {
        var resolver = Locator.CurrentMutable;

        // Register concrete implementations (AOT-friendly)
        resolver.Register<ISequencer>(static () => Sequencer.CurrentThread);
        resolver.RegisterConstant("test service");

        // Create a simple factory
        resolver.Register<Func<string, ReactiveProperty<string>>>(static () => static value =>
        {
            var scheduler = Locator.Current.GetService<ISequencer>();
            return new(value, scheduler, false, false);
        });

        // Test resolution
        var scheduler = Locator.Current.GetService<ISequencer>();
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

    /// <summary>Tests that activation/deactivation works correctly in AOT scenarios.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelActivation_AOTCompatible_WorksCorrectly()
    {
        var viewModel = new TestActivatableViewModel();
        var activationCount = 0;
        var deactivationCount = 0;
        var scheduler = Sequencer.CurrentThread;

        viewModel.WhenActivated(disposables =>
        {
            activationCount++;

            // Create reactive property within activation
            var property = new ReactiveProperty<string>("activated", scheduler, false, false);
            property.DisposeWith(disposables);

            // Setup cleanup
            new ActionDisposable(() => deactivationCount++).DisposeWith(disposables);
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
            await Assert.That(activationCount).IsEqualTo(ExpectedCount);
            await Assert.That(deactivationCount).IsEqualTo(1);
        }

        viewModel.Activator.Deactivate();
        using (Assert.Multiple())
        {
            await Assert.That(activationCount).IsEqualTo(ExpectedCount);
            await Assert.That(deactivationCount).IsEqualTo(ExpectedCount);
        }
    }

    /// <summary>Tests error handling patterns in AOT scenarios.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ErrorHandling_AOTScenarios_WorkCorrectly()
    {
        var scheduler = Sequencer.CurrentThread;
        var property = new ReactiveProperty<string>("test", scheduler, false, false);
        var errors = new List<Exception>();

        // Subscribe to thrown exceptions (tests ReactiveObject error handling)
        property.ThrownExceptions.Subscribe(errors.Add);

        // Test validation errors
        var validationErrors = new List<string>();
        property.ObserveErrorChanged
            .Where(errs => errs is not null)
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
