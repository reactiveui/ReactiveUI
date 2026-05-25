// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;

namespace ReactiveUI.AOT.Tests;

/// <summary>
/// Provides a suite of tests to verify compatibility of ReactiveUI features with ahead-of-time (AOT) compilation
/// scenarios.
/// </summary>
public class AOTCompatibilityTests
{
    /// <summary>
    /// The number of times the test property is expected to change during observation.
    /// </summary>
    private const int ExpectedPropertyChangeCount = 2;

    /// <summary>
    /// The value emitted by the observable-backed command under test.
    /// </summary>
    private const int CommandResultValue = 42;

    /// <summary>
    /// Tests that ReactiveObjects can be created and property changes work in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveObject_PropertyChanges_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var propertyChanged = false;

        obj.PropertyChanged += (_, _) => propertyChanged = true;
        obj.TestProperty = "New Value";

        using (Assert.Multiple())
        {
            await Assert.That(propertyChanged).IsTrue();
            await Assert.That(obj.TestProperty).IsEqualTo("New Value");
        }
    }

    /// <summary>
    /// Tests that ReactiveCommands can be created and executed in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveCommand_Create_WorksInAOT()
    {
        var executed = false;
        var command = ReactiveCommand.Create(() => executed = true);

        command.Execute().Subscribe();

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Tests that ReactiveCommands with parameters work in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveCommand_CreateWithParameter_WorksInAOT()
    {
        string? result = null;
        var command = ReactiveCommand.Create<string>(param => result = param);

        command.Execute("test").Subscribe();

        await Assert.That(result).IsEqualTo("test");
    }

    /// <summary>
    /// Tests that ObservableAsPropertyHelper works in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableAsPropertyHelper_WorksInAOT()
    {
        var obj = new TestReactiveObject();

        // Test string-based property helper (should work in AOT)
        var helper = Observable.Return("computed value")
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        await Assert.That(helper.Value).IsEqualTo("computed value");
    }

    /// <summary>
    /// Tests that WhenAnyValue works with string property names in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with RequiresUnreferencedCodeAttribute may break when trimming",
        Justification = "Test deliberately exercises the string/expression-based reflection API to verify runtime behavior.")]
    public async Task WhenAnyValue_StringPropertyNames_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        string? observedValue = null;

        // Using string property names should work in AOT
        obj.WhenAnyValue<TestReactiveObject, string>(nameof(TestReactiveObject.TestProperty))
            .Subscribe(value => observedValue = value);

        obj.TestProperty = "test value";

        await Assert.That(observedValue).IsEqualTo("test value");
    }

    /// <summary>
    /// Tests that interaction requests work in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Interaction_WorksInAOT()
    {
        var interaction = new Interaction<string, bool>();
        var called = false;

        interaction.RegisterHandler(context =>
        {
            called = true;
            context.SetOutput(true);
        });

        var result = interaction.Handle("test").Wait();

        using (Assert.Multiple())
        {
            await Assert.That(called).IsTrue();
            await Assert.That(result).IsTrue();
        }
    }

    /// <summary>
    /// Tests that INPC property observation works in AOT.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task INPCPropertyObservation_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var changes = new List<string?>();

        obj.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

        obj.TestProperty = "value1";
        obj.TestProperty = "value2";

        await Assert.That(changes).Contains(nameof(TestReactiveObject.TestProperty));
        await Assert.That(changes.Count(x => x == nameof(TestReactiveObject.TestProperty))).IsEqualTo(ExpectedPropertyChangeCount);
    }

    /// <summary>
    /// Tests that ReactiveCommand.CreateFromObservable works in AOT scenarios.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveCommand_CreateFromObservable_WorksInAOT()
    {
        var command = ReactiveCommand.CreateFromObservable(() => Observable.Return(CommandResultValue));

        // Ensure the execution completes before asserting by blocking for the result
        var result = command.Execute().Wait();

        await Assert.That(result).IsEqualTo(CommandResultValue);
    }

    /// <summary>
    /// Tests that string-based property bindings work in AOT (preferred pattern).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task StringBasedPropertyBinding_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var helper = Observable.Return("test")
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        await Assert.That(helper.Value).IsEqualTo("test");
    }
}
