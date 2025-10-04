// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.AOTTests;

/// <summary>
/// Tests to verify that ReactiveUI works correctly in AOT (Ahead-of-Time) compilation scenarios.
/// These tests ensure that the library doesn't rely on reflection in ways that break with AOT.
/// </summary>
[TestFixture]
public class AOTCompatibilityTests
{
    /// <summary>
    /// Tests that ReactiveObjects can be created and property changes work in AOT.
    /// </summary>
    [Test]
    public void ReactiveObject_PropertyChanges_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var propertyChanged = false;

        obj.PropertyChanged += (s, e) => propertyChanged = true;
        obj.TestProperty = "New Value";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(propertyChanged, Is.True);
            Assert.That(obj.TestProperty, Is.EqualTo("New Value"));
        }
    }

    /// <summary>
    /// Tests that ReactiveCommands can be created and executed in AOT.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing AOT-incompatible ReactiveCommand.Create method")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing AOT-incompatible ReactiveCommand.Create method")]
    public void ReactiveCommand_Create_WorksInAOT()
    {
        var executed = false;
        var command = ReactiveCommand.Create(() => executed = true);

        command.Execute().Subscribe();

        Assert.That(executed, Is.True);
    }

    /// <summary>
    /// Tests that ReactiveCommands with parameters work in AOT.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing AOT-incompatible ReactiveCommand.Create method")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing AOT-incompatible ReactiveCommand.Create method")]
    public void ReactiveCommand_CreateWithParameter_WorksInAOT()
    {
        string? result = null;
        var command = ReactiveCommand.Create<string>(param => result = param);

        command.Execute("test").Subscribe();

        Assert.That(result, Is.EqualTo("test"));
    }

    /// <summary>
    /// Tests that ObservableAsPropertyHelper works in AOT.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    public void ObservableAsPropertyHelper_WorksInAOT()
    {
        var obj = new TestReactiveObject();

        // Test string-based property helper (should work in AOT)
        var helper = Observable.Return("computed value")
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        Assert.That(helper.Value, Is.EqualTo("computed value"));
    }

    /// <summary>
    /// Tests that WhenAnyValue works with string property names in AOT.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing WhenAnyValue which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing WhenAnyValue which requires AOT suppression")]
    public void WhenAnyValue_StringPropertyNames_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        string? observedValue = null;

        // Using string property names should work in AOT
        obj.WhenAnyValue<TestReactiveObject, string>(nameof(TestReactiveObject.TestProperty))
           .Subscribe(value => observedValue = value);

        obj.TestProperty = "test value";

        Assert.That(observedValue, Is.EqualTo("test value"));
    }

    /// <summary>
    /// Tests that interaction requests work in AOT.
    /// </summary>
    [Test]
    public void Interaction_WorksInAOT()
    {
        var interaction = new Interaction<string, bool>();
        var called = false;

        interaction.RegisterHandler(context =>
        {
            called = true;
            context.SetOutput(true);
        });

        var result = interaction.Handle("test").Wait();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(called, Is.True);
            Assert.That(result, Is.True);
        }
    }

    /// <summary>
    /// Tests that INPC property observation works in AOT.
    /// </summary>
    [Test]
    public void INPCPropertyObservation_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var changes = new List<string?>();

        obj.PropertyChanged += (s, e) => changes.Add(e.PropertyName);

        obj.TestProperty = "value1";
        obj.TestProperty = "value2";

        Assert.That(changes, Does.Contain(nameof(TestReactiveObject.TestProperty)));
        Assert.That(changes.Count(x => x == nameof(TestReactiveObject.TestProperty)), Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that ReactiveCommand.CreateFromObservable works in AOT scenarios.
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing AOT-incompatible ReactiveCommand.CreateFromObservable method")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing AOT-incompatible ReactiveCommand.CreateFromObservable method")]
    public void ReactiveCommand_CreateFromObservable_WorksInAOT()
    {
        var command = ReactiveCommand.CreateFromObservable(() => Observable.Return(42));

        // Ensure the execution completes before asserting by blocking for the result
        var result = command.Execute().Wait();

        Assert.That(result, Is.EqualTo(42));
    }

    /// <summary>
    /// Tests that string-based property bindings work in AOT (preferred pattern).
    /// </summary>
    [Test]
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Testing ToProperty with string-based property names which requires AOT suppression")]
    public void StringBasedPropertyBinding_WorksInAOT()
    {
        var obj = new TestReactiveObject();
        var helper = Observable.Return("test")
            .ToProperty(obj, nameof(TestReactiveObject.ComputedProperty));

        Assert.That(helper.Value, Is.EqualTo("test"));
    }
}
