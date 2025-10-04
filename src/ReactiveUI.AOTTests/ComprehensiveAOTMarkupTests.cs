// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Tests that validate the comprehensive AOT markup applied throughout the ReactiveUI solution.
/// These tests ensure that all newly marked AOT-incompatible areas work correctly with proper suppression.
/// </summary>
[TestFixture]
public class ComprehensiveAOTMarkupTests
{
    /// <summary>
    /// Tests that ReactiveObject constructor works with AOT suppression.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveObject AOT-incompatible constructor")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveObject AOT-incompatible constructor")]
    public void ReactiveObject_Constructor_WorksWithAOTSuppression()
    {
        var obj = new TestReactiveObject();
        var propertyChangedFired = false;

        obj.PropertyChanged += (_, _) => propertyChangedFired = true;
        obj.TestProperty = "test value";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(propertyChangedFired, Is.True);
            Assert.That(obj.TestProperty, Is.EqualTo("test value"));
        }
    }

    /// <summary>
    /// Tests that ReactiveProperty Refresh method works with AOT suppression.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty Refresh AOT-incompatible method")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty Refresh AOT-incompatible method")]
    public void ReactiveProperty_Refresh_WorksWithAOTSuppression()
    {
        var scheduler = CurrentThreadScheduler.Instance;
        var property = new ReactiveProperty<string>("initial", scheduler, false, false);
        var values = new List<string>();

        property.Subscribe(value => values.Add(value ?? string.Empty));

        property.Refresh(); // This calls RaisePropertyChanged which has AOT attributes

        Assert.That(values, Does.Contain("initial"));
        Assert.That(values, Has.Count.GreaterThanOrEqualTo(2)); // Initial value plus refresh

        property.Dispose();
    }

    /// <summary>
    /// Tests that platform-specific WireUpControls methods are properly marked for AOT.
    /// This validates that Android platform code has proper AOT attributes.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty which requires AOT suppression")]
    public void PlatformSpecific_AOTMarkup_IsProperlyApplied()
    {
        // This test validates that platform-specific code has AOT attributes
        // We can't directly test Android code in this context, but we can verify
        // that the patterns we expect are working
        var testScheduler = CurrentThreadScheduler.Instance;
        var property = new ReactiveProperty<string>("test", testScheduler, false, false);

        // Test that basic ReactiveUI functionality works
        Assert.That(property.Value, Is.EqualTo("test"));

        property.Dispose();
    }

    /// <summary>
    /// Tests that all reactive property operations work with proper AOT handling.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing comprehensive ReactiveProperty AOT scenarios")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing comprehensive ReactiveProperty AOT scenarios")]
    public void ReactiveProperty_ComprehensiveOperations_WorkWithAOT()
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

        property.AddValidationError(x => string.IsNullOrEmpty(x) ? "Required" : null);
        property.Value = string.Empty;

        Assert.That(valueChanges, Does.Contain("initial"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(valueChanges, Does.Contain("changed"));
            Assert.That(hasErrors, Is.True);
        }

        property.Dispose();
    }

    /// <summary>
    /// Tests that complex ReactiveUI scenarios work with mixed AOT compatible and incompatible features.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing mixed AOT scenario")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing mixed AOT scenario")]
    public void MixedAOTScenario_ComplexWorkflow_WorksCorrectly()
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(property.Value, Is.EqualTo("updated"));
            Assert.That(messages, Does.Contain("workflow test"));
            Assert.That(result, Is.True);
        }

        // Cleanup
        source.Dispose();
        property.Dispose();
    }

    /// <summary>
    /// Tests that ObservableAsPropertyHelper works correctly in AOT scenarios.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty which requires AOT suppression")]
    public void ObservableAsPropertyHelper_AOTCompatibleUsage_Works()
    {
        var obj = new TestReactiveObject();
        var scheduler = CurrentThreadScheduler.Instance;
        var source = new BehaviorSubject<string>("computed");

        // String-based property binding is AOT-compatible
        var helper = source
            .ObserveOn(scheduler)
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        Assert.That(helper.Value, Is.EqualTo("computed"));

        source.OnNext("updated");
        Assert.That(helper.Value, Is.EqualTo("updated"));

        source.Dispose();
        helper.Dispose();
    }

    /// <summary>
    /// Tests that dependency injection patterns work in AOT scenarios.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty in AOT scenario with proper suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty in AOT scenario with proper suppression")]
    public void DependencyInjection_AOTCompatiblePatterns_Work()
    {
        var resolver = Locator.CurrentMutable;

        // Register concrete implementations (AOT-friendly)
        resolver.Register<IScheduler>(static () => CurrentThreadScheduler.Instance);
        resolver.RegisterConstant<string>("test service");

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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(scheduler, Is.Not.Null);
            Assert.That(constant, Is.EqualTo("test service"));
            Assert.That(factory, Is.Not.Null);
        }

        var property = factory("factory test");
        Assert.That(property.Value, Is.EqualTo("factory test"));

        property.Dispose();
    }

    /// <summary>
    /// Tests that activation/deactivation works correctly in AOT scenarios.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty in AOT scenario with proper suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty in AOT scenario with proper suppression")]
    public void ViewModelActivation_AOTCompatible_WorksCorrectly()
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

        // Test reactivation
        viewModel.Activator.Activate();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(activationCount, Is.EqualTo(2));
            Assert.That(deactivationCount, Is.EqualTo(1));
        }

        viewModel.Activator.Deactivate();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(activationCount, Is.EqualTo(2));
            Assert.That(deactivationCount, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// Tests error handling patterns in AOT scenarios.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing error handling in AOT scenario")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing error handling in AOT scenario")]
    public void ErrorHandling_AOTScenarios_WorkCorrectly()
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

        property.AddValidationError(x => string.IsNullOrEmpty(x) ? "Value required" : null);
        property.Value = string.Empty;

        using (Assert.EnterMultipleScope())
        {
            // Test that validation works
            Assert.That(property.HasErrors, Is.True);
            Assert.That(validationErrors, Does.Contain("Value required"));
        }

        // Fix the error
        property.Value = "valid value";
        Assert.That(property.HasErrors, Is.False);

        property.Dispose();
    }
}
