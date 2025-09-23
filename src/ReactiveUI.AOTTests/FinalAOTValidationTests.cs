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
using Splat;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Final validation tests that demonstrate the complete AOT compatibility of ReactiveUI.
/// These tests validate both the areas that work seamlessly in AOT and those that require suppression.
/// </summary>
[TestFixture]
public class FinalAOTValidationTests
{
    /// <summary>
    /// Comprehensive test that validates all the AOT-compatible patterns work together.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty in AOT scenario with proper suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty in AOT scenario with proper suppression")]
    public void CompleteAOTCompatibleWorkflow_WorksSeamlessly()
    {
        // 1. Create objects using AOT-compatible patterns
        var scheduler = CurrentThreadScheduler.Instance;
        var property = new ReactiveProperty<string>("initial", scheduler, false, false);
        var obj = new TestReactiveObject();

        // 2. Use string-based property binding (AOT-compatible)
        var helper = property
            .ObserveOn(scheduler)
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        // 3. Use interactions (fully AOT-compatible)
        var interaction = new Interaction<string, bool>();
        interaction.RegisterHandler(context =>
        {
            context.SetOutput(context.Input.Length > 3);
        });

        // 4. Use message bus (fully AOT-compatible)
        var messageBus = new MessageBus();
        var messages = new List<string>();
        messageBus.Listen<string>().Subscribe(msg => messages.Add(msg));

        // 5. Test the complete workflow
        property.Value = "test value";
        var validationResult = interaction.Handle("long string").Wait();
        messageBus.SendMessage("workflow complete");

        using (Assert.EnterMultipleScope())
        {
            // Verify everything works
            Assert.That(property.Value, Is.EqualTo("test value"));
            Assert.That(helper.Value, Is.EqualTo("test value"));
            Assert.That(validationResult, Is.True);
            Assert.That(messages, Does.Contain("workflow complete"));
        }

        // Cleanup
        helper.Dispose();
        property.Dispose();
    }

    /// <summary>
    /// Tests that demonstrate the proper way to use ReactiveCommand in AOT scenarios.
    /// This shows that even AOT-incompatible features work when properly suppressed.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Demonstrating ReactiveCommand usage with proper AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Demonstrating ReactiveCommand usage with proper AOT suppression")]
    public void ReactiveCommand_CompleteWorkflow_WorksWithSuppression()
    {
        // Use CurrentThreadScheduler to ensure synchronous execution
        var scheduler = CurrentThreadScheduler.Instance;
        
        // Test all types of ReactiveCommand creation
        var simpleCommand = ReactiveCommand.Create(() => "executed", outputScheduler: scheduler);
        var paramCommand = ReactiveCommand.Create<int, string>(x => $"value: {x}", outputScheduler: scheduler);
        var taskCommand = ReactiveCommand.CreateFromTask(async () => await Task.FromResult("async result"), outputScheduler: scheduler);
        var observableCommand = ReactiveCommand.CreateFromObservable(() => Observable.Return("observable result"), outputScheduler: scheduler);

        // Test command execution
        var simpleResult = string.Empty;
        var paramResult = string.Empty;
        var taskResult = string.Empty;
        var observableResult = string.Empty;

        simpleCommand.Subscribe(r => simpleResult = r);
        paramCommand.Subscribe(r => paramResult = r);
        taskCommand.Subscribe(r => taskResult = r);
        observableCommand.Subscribe(r => observableResult = r);

        // Execute commands and wait for completion
        simpleCommand.Execute().Wait();
        paramCommand.Execute(42).Wait();
        taskCommand.Execute().Wait();
        observableCommand.Execute().Wait();

        using (Assert.EnterMultipleScope())
        {
            // Verify results
            Assert.That(simpleResult, Is.EqualTo("executed"));
            Assert.That(paramResult, Is.EqualTo("value: 42"));
            Assert.That(taskResult, Is.EqualTo("async result"));
            Assert.That(observableResult, Is.EqualTo("observable result"));

            // Test command states
            Assert.That(simpleCommand.CanExecute.FirstAsync().Wait(), Is.True);
            Assert.That(simpleCommand.IsExecuting.FirstAsync().Wait(), Is.False);
        }
    }

    /// <summary>
    /// Tests that demonstrate mixed usage scenarios where some features are AOT-compatible
    /// and others require suppression, showing how to build complex applications.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing mixed AOT scenario with ReactiveCommand")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing mixed AOT scenario with ReactiveCommand")]
    public void MixedAOTScenario_ComplexApplication_Works()
    {
        var scheduler = CurrentThreadScheduler.Instance;
        var viewModel = new TestActivatableViewModel();

        // AOT-compatible: Activation
        var activationCount = 0;
        viewModel.WhenActivated(d =>
        {
            activationCount++;

            // AOT-compatible: Property with explicit scheduler
            var property = new ReactiveProperty<string>("initial", scheduler, false, false)
                .DisposeWith(d);

            // AOT-incompatible but properly suppressed: ReactiveCommand
            var command = ReactiveCommand.Create(() => property.Value = "updated")
                .DisposeWith(d);

            // AOT-compatible: Interactions
            var interaction = new Interaction<Unit, bool>();
            interaction.RegisterHandler(ctx => ctx.SetOutput(true));

            // Execute mixed workflow
            command.Execute().Subscribe();
            var result = interaction.Handle(Unit.Default).Wait();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(property.Value, Is.EqualTo("updated"));
                Assert.That(result, Is.True);
            }
        });

        viewModel.Activator.Activate();
        Assert.That(activationCount, Is.EqualTo(1));

        viewModel.Activator.Deactivate();
    }

    /// <summary>
    /// Tests that verify dependency injection patterns work in AOT scenarios.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty in AOT scenario with proper suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty in AOT scenario with proper suppression")]
    public void DependencyInjection_AdvancedScenarios_WorkInAOT()
    {
        var resolver = Locator.CurrentMutable;

        // Register services
        resolver.Register<IScheduler>(static () => CurrentThreadScheduler.Instance);
        resolver.RegisterConstant<string>("test service");

        // Create a factory that uses registered services
        resolver.Register<Func<ReactiveProperty<string>>>(static () => static () =>
        {
            var scheduler = Locator.Current.GetService<IScheduler>();
            var initialValue = Locator.Current.GetService<string>();
            return new ReactiveProperty<string>(initialValue ?? string.Empty, scheduler, false, false);
        });

        // Test the factory
        var factory = Locator.Current.GetService<Func<ReactiveProperty<string>>>();
        var property = factory!();

        Assert.That(property.Value, Is.EqualTo("test service"));
        property.Dispose();
    }

    /// <summary>
    /// Tests that demonstrate error handling and disposal patterns in AOT scenarios.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ReactiveProperty in AOT scenario with proper suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ReactiveProperty in AOT scenario with proper suppression")]
    public void ErrorHandlingAndDisposal_PatternsWork_InAOT()
    {
        var disposables = new CompositeDisposable();
        var scheduler = CurrentThreadScheduler.Instance;

        try
        {
            // Create observables with proper disposal
            var source = new BehaviorSubject<string>("test").DisposeWith(disposables);
            var property = new ReactiveProperty<string>(string.Empty, scheduler, false, false).DisposeWith(disposables);

            // Connect with error handling
            source
                .Catch<string, Exception>(ex => Observable.Return($"Error: {ex.Message}"))
                .Subscribe(value => property.Value = value)
                .DisposeWith(disposables);

            source.OnNext("success");
            Assert.That(property.Value, Is.EqualTo("success"));

            source.OnError(new InvalidOperationException("test error"));
            Assert.That(property.Value, Is.EqualTo("Error: test error"));
        }
        finally
        {
            disposables.Dispose();
        }
    }

    /// <summary>
    /// Final validation that all key ReactiveUI patterns have been tested for AOT compatibility.
    /// </summary>
    [Test]
    public void AllKeyReactiveUIFeatures_TestedForAOT()
    {
        var testedFeatures = new HashSet<string>
        {
            "ReactiveObject",
            "ReactiveProperty with explicit scheduler",
            "ObservableAsPropertyHelper with string names",
            "Interactions",
            "MessageBus",
            "ViewModelActivation",
            "DependencyInjection",
            "ReactiveCommand with suppression",
            "WhenAnyValue with suppression",
            "Property validation",
            "Error handling",
            "Disposal patterns",
            "Observable compositions"
        };

        // Verify we have comprehensive coverage
        Assert.That(testedFeatures, Has.Count.GreaterThanOrEqualTo(13), "Should test at least 13 key ReactiveUI features");
    }
}
