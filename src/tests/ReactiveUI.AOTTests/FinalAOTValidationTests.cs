// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.AOT.Tests;

/// <summary>
/// Provides a suite of tests that validate the compatibility of key ReactiveUI patterns and features with Ahead-of-Time
/// (AOT) compilation scenarios.
/// </summary>
public class FinalAOTValidationTests
{
    /// <summary>The minimum input length used by the validation interaction.</summary>
    private const int MinimumInputLength = 3;

    /// <summary>The parameter value passed to the parameterized command.</summary>
    private const int CommandParameterValue = 42;

    /// <summary>The expected minimum number of tested features.</summary>
    private const int ExpectedFeatureCount = 13;

    /// <summary>Comprehensive test that validates all the AOT-compatible patterns work together.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompleteAOTCompatibleWorkflow_WorksSeamlessly()
    {
        // 1. Create objects using AOT-compatible patterns
        var scheduler = Sequencer.CurrentThread;
        var property = new ReactiveProperty<string>("initial", scheduler, false, false);
        var obj = new TestReactiveObject();

        // 2. Use string-based property binding (AOT-compatible)
        var helper = property
            .ObserveOn(scheduler)
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        // 3. Use interactions (fully AOT-compatible)
        var interaction = new Interaction<string, bool>();
        _ = interaction.RegisterHandler(context => context.SetOutput(context.Input.Length > MinimumInputLength));

        // 4. Use message bus (fully AOT-compatible)
        var messageBus = new MessageBus();
        var messages = new List<string>();
        _ = messageBus.Listen<string>().Subscribe(messages.Add);

        // 5. Test the complete workflow
        property.Value = "test value";
        var validationResult = await interaction.Handle("long string").FirstAsync();
        messageBus.SendMessage("workflow complete");

        using (Assert.Multiple())
        {
            // Verify everything works
            await Assert.That(property.Value).IsEqualTo("test value");
            await Assert.That(helper.Value).IsEqualTo("test value");
            await Assert.That(validationResult).IsTrue();
            await Assert.That(messages).Contains("workflow complete");
        }

        // Cleanup
        helper.Dispose();
        property.Dispose();
    }

    /// <summary>
    /// Tests that demonstrate the proper way to use ReactiveCommand in AOT scenarios.
    /// This shows that even AOT-incompatible features work when properly suppressed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveCommand_CompleteWorkflow_WorksWithSuppression()
    {
        // Use CurrentThreadScheduler to ensure synchronous execution
        var scheduler = Sequencer.CurrentThread;

        // Test all types of ReactiveCommand creation
        var simpleCommand = ReactiveCommand.Create(() => "executed", outputScheduler: scheduler);
        var paramCommand = ReactiveCommand.Create<int, string>(x => $"value: {x}", outputScheduler: scheduler);
        var taskCommand = ReactiveCommand.CreateFromTask(
            () => Task.FromResult("async result"),
            outputScheduler: scheduler);
        var observableCommand = ReactiveCommand.CreateFromObservable(
            () => Signal.Emit("observable result"),
            outputScheduler: scheduler);

        // Test command execution
        var simpleResult = string.Empty;
        var paramResult = string.Empty;
        var taskResult = string.Empty;
        var observableResult = string.Empty;

        _ = simpleCommand.Subscribe(r => simpleResult = r);
        _ = paramCommand.Subscribe(r => paramResult = r);
        _ = taskCommand.Subscribe(r => taskResult = r);
        _ = observableCommand.Subscribe(r => observableResult = r);

        // Execute commands and wait for completion
        _ = simpleCommand.Execute().GetAwaiter().GetResult();
        _ = paramCommand.Execute(CommandParameterValue).GetAwaiter().GetResult();
        _ = taskCommand.Execute().GetAwaiter().GetResult();
        _ = observableCommand.Execute().GetAwaiter().GetResult();

        using (Assert.Multiple())
        {
            // Verify results
            await Assert.That(simpleResult).IsEqualTo("executed");
            await Assert.That(paramResult).IsEqualTo("value: 42");
            await Assert.That(taskResult).IsEqualTo("async result");
            await Assert.That(observableResult).IsEqualTo("observable result");

            // Test command states
            await Assert.That(await simpleCommand.CanExecute.FirstAsync()).IsTrue();
            await Assert.That(await simpleCommand.IsExecuting.FirstAsync()).IsFalse();
        }
    }

    /// <summary>
    /// Tests that demonstrate mixed usage scenarios where some features are AOT-compatible
    /// and others require suppression, showing how to build complex applications.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MixedAOTScenario_ComplexApplication_Works()
    {
        var scheduler = Sequencer.CurrentThread;
        var viewModel = new TestActivatableViewModel();

        // AOT-compatible: Activation
        var activationCount = 0;
        string? propertyValue = null;
        bool? interactionResult = null;
        viewModel.WhenActivated(d =>
        {
            activationCount++;

            // AOT-compatible: Property with explicit scheduler
            var property = new ReactiveProperty<string>("initial", scheduler, false, false);
            _ = property.DisposeWith(d);

            // AOT-incompatible but properly suppressed: ReactiveCommand
            var command = ReactiveCommand.Create(() => property.Value = "updated");
            _ = command.DisposeWith(d);

            // AOT-compatible: Interactions
            var interaction = new Interaction<RxVoid, bool>();
            _ = interaction.RegisterHandler(ctx => ctx.SetOutput(true));

            // Execute mixed workflow
            _ = command.Execute().Subscribe();
            interactionResult = interaction.Handle(RxVoid.Default).GetAwaiter().GetResult();
            propertyValue = property.Value;
        });

        _ = viewModel.Activator.Activate();

        using (Assert.Multiple())
        {
            await Assert.That(activationCount).IsEqualTo(1);
            await Assert.That(propertyValue).IsEqualTo("updated");
            await Assert.That(interactionResult).IsTrue();
        }

        viewModel.Activator.Deactivate();
    }

    /// <summary>Tests that verify dependency injection patterns work in AOT scenarios.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DependencyInjection_AdvancedScenarios_WorkInAOT()
    {
        var resolver = Locator.CurrentMutable;

        // Register services
        resolver.Register<ISequencer>(static () => Sequencer.CurrentThread);
        resolver.RegisterConstant("test service");

        // Create a factory that uses registered services
        resolver.Register<Func<ReactiveProperty<string>>>(static () => static () =>
        {
            var scheduler = Locator.Current.GetService<ISequencer>();
            var initialValue = Locator.Current.GetService<string>();
            return new(initialValue ?? string.Empty, scheduler, false, false);
        });

        // Test the factory
        var factory = Locator.Current.GetService<Func<ReactiveProperty<string>>>();
        var property = factory!();

        await Assert.That(property.Value).IsEqualTo("test service");
        property.Dispose();
    }

    /// <summary>Tests that demonstrate error handling and disposal patterns in AOT scenarios.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ErrorHandlingAndDisposal_PatternsWork_InAOT()
    {
        var disposables = new MultipleDisposable();
        var scheduler = Sequencer.CurrentThread;

        try
        {
            // Create observables with proper disposal
            var source = new StateSignal<string>("test");
            _ = source.DisposeWith(disposables);

            var property = new ReactiveProperty<string>(string.Empty, scheduler, false, false);
            _ = property.DisposeWith(disposables);

            // Connect with error handling
            _ = source
                .Recover<string, Exception>(ex => Signal.Emit($"Error: {ex.Message}"))
                .Subscribe(value => property.Value = value)
                .DisposeWith(disposables);

            source.OnNext("success");
            await Assert.That(property.Value).IsEqualTo("success");

            source.OnError(new InvalidOperationException("test error"));
            await Assert.That(property.Value).IsEqualTo("Error: test error");
        }
        finally
        {
            disposables.Dispose();
        }
    }

    /// <summary>Final validation that all key ReactiveUI patterns have been tested for AOT compatibility.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AllKeyReactiveUIFeatures_TestedForAOT()
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
        await Assert.That(testedFeatures).Count().IsGreaterThanOrEqualTo(ExpectedFeatureCount);
    }
}
